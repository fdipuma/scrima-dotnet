using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Scrima.Core.Exceptions;
using Scrima.Core.Query;
using Scrima.Core.Query.Expressions;

namespace Scrima.Queryable
{
    public static partial class ScrimaExtensions
    {
        /// <summary>
        /// Filters a sequence of values based on the provided <see cref="FilterQueryOption"/>
        /// </summary>
        /// <param name="source">The queryable source</param>
        /// <param name="filterQueryOption">The filter options to apply</param>
        /// <returns>The filtered data source</returns>
        /// <exception cref="ArgumentNullException">source or filter are null</exception>
        public static IQueryable<TSource> Where<TSource>(this IQueryable<TSource> source, FilterQueryOption filterQueryOption)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (filterQueryOption == null) throw new ArgumentNullException(nameof(filterQueryOption));

            if (filterQueryOption.Expression is null) return source;

            var parameter = Expression.Parameter(source.ElementType, "o");

            var filterClause = CreateExpression(filterQueryOption.Expression, parameter);

            var lambda = Expression.Lambda<Func<TSource, bool>>(filterClause, parameter);

            return source.Where(lambda);
        }

        private static Expression CreateExpression(QueryNode queryNode, Expression baseExpression)
        {
            return queryNode switch
            {
                ValueNode valueNode =>
                    CreateValueExpression(valueNode, baseExpression),

                FunctionCallNode functionCall =>
                    CreateFunctionCallExpression(functionCall.Name, functionCall.Parameters.CreateExpressions(baseExpression)),

                UnaryOperatorNode {OperatorKind: UnaryOperatorKind.Not} unaryOperator =>
                    Expression.Not(CreateExpression(unaryOperator.Operand, baseExpression)),

                BinaryOperatorNode binaryOperator => 
                    CreateBinaryOperatorExpression(binaryOperator, baseExpression),

                _ => throw new QueryOptionsValidationException($"Invalid query {queryNode.Kind}")
            };
        }

        private static IList<Expression> CreateExpressions(this IEnumerable<QueryNode> nodes, Expression baseExpression)
            => nodes.Select(n => CreateExpression(n, baseExpression)).ToList();

        /// <summary>
        /// Creates an expression from a binary operation like: left OPERATOR right
        /// </summary>
        /// <param name="binaryOperator">The binary operator node</param>
        /// <param name="baseExpression">Base expression</param>
        /// <returns></returns>
        private static Expression CreateBinaryOperatorExpression(BinaryOperatorNode binaryOperator, Expression baseExpression)
        {
            var left = CreateExpression(binaryOperator.Left, baseExpression);
            var right = CreateExpression(binaryOperator.Right, baseExpression);
            var kind = binaryOperator.OperatorKind;
            
            // bitwise operations
            if (kind == BinaryOperatorKind.Or)
                return Expression.Or(left, right);

            if (kind == BinaryOperatorKind.And)
                return Expression.And(left, right);

            // enum.HasFlag()
            if (kind == BinaryOperatorKind.Has)
                return Expression.Call(null, Methods.HasFlag, left, Expression.Convert(right, typeof(Enum)));
            
            // collection contains
            if (kind == BinaryOperatorKind.In)
                return CreateFunctionCallExpression("contains", new List<Expression> {right, left});
            
            var expressionType = kind switch
            {
                // comparison
                BinaryOperatorKind.Equal => ExpressionType.Equal,
                BinaryOperatorKind.NotEqual => ExpressionType.NotEqual,
                BinaryOperatorKind.GreaterThan => ExpressionType.GreaterThan,
                BinaryOperatorKind.GreaterThanOrEqual => ExpressionType.GreaterThanOrEqual,
                BinaryOperatorKind.LessThan => ExpressionType.LessThan,
                BinaryOperatorKind.LessThanOrEqual => ExpressionType.LessThanOrEqual,
                
                // additive
                BinaryOperatorKind.Add => ExpressionType.Add,
                BinaryOperatorKind.Subtract => ExpressionType.Subtract,
                
                // multiplicative
                BinaryOperatorKind.Multiply => ExpressionType.Multiply,
                BinaryOperatorKind.Divide => ExpressionType.Divide,
                BinaryOperatorKind.Modulo => ExpressionType.Modulo,
                _ => throw new QueryOptionsValidationException($"Invalid operator {kind}")
            };

            // let's "promote" expression so we have two expression of a compatible
            // type in both sides of the binary expression
            var promotedLeftExpression = ExpressionHelper.Promote(left, right);
            var promotedRightExpression = ExpressionHelper.Promote(right, left);
            
            return Expression.MakeBinary(expressionType, promotedLeftExpression, promotedRightExpression);
        }

        /// <summary>
        /// Create an expression containing a value (either a literal or a property access)
        /// </summary>
        /// <exception cref="QueryOptionsValidationException">Property is invalid or value node is not supported</exception>
        private static Expression CreateValueExpression(ValueNode valueNode, Expression parameterExpression)
        {
            switch (valueNode)
            {
                case ConstantNode {Value: null}:
                    return Expression.Constant(null);

                case ConstantNode {Value: { }} constant:
                    return Expression.Constant(constant.Value, constant.EdmType.ClrType);
                
                case ArrayNode { Value: { }} array:
                    return Expression.Constant(array.Value, array.ArrayClrType);
                    
                case PropertyAccessNode propertyAccess:
                    var expression = parameterExpression;
                    
                    foreach (var edmProperty in propertyAccess.Properties)
                    {
                        var property = expression.Type.GetProperty(edmProperty.Name);

                        if (property is null)
                        {
                            throw new QueryOptionsValidationException($"Invalid property name {edmProperty.Name}");
                        }

                        expression = Expression.MakeMemberAccess(expression, property);   
                    }

                    return expression;
                default:
                    throw new QueryOptionsValidationException($"Invalid query value {valueNode.Kind}");
            }
        }

        private static Expression CreateFunctionCallExpression(string functionName, IList<Expression> arguments)
        {
            void ValidateParameterCount(params int[] count)
            {
                if (count.Length == 1 && arguments.Count != count[0])
                {
                    throw new QueryOptionsValidationException($"{functionName} needs {count[0]} parameters");
                }

                if (!count.Contains(arguments.Count))
                {
                    var counts = string.Join(", ", count.Take(count.Length - 1)) + " or " + count.Last();
                    throw new QueryOptionsValidationException($"{functionName} needs {counts} parameters");
                }
            }

            Expression InvalidParameterTypes(string supportedTypes)
            {
                throw new QueryOptionsValidationException(
                    $"Unsupported parameters provided to function '{functionName}', supported types: {supportedTypes}");
            }

            Type itemType;
            
            switch (functionName)
            {
                // string functions
                case "concat":
                    if (arguments.Count < 1)
                    {
                        throw new QueryOptionsValidationException($"{functionName} needs at least 1 parameter");
                    }

                    if (ReflectionHelper.IsEnumerable(arguments[0].Type, out itemType))
                    {
                        var result = arguments[0];
                        var concatMethod = Methods.EnumerableConcat.MakeGenericMethod(itemType);
                        // ReSharper disable once LoopCanBeConvertedToQuery this one is clearer
                        foreach (var arg in arguments.Skip(1))
                        {
                            result = Expression.Call(null, concatMethod, result, arg);
                        }

                        return result;
                    }
                    else if (arguments[0].Type == StringType)
                    {
                        return Expression.Call(null, Methods.StringConcat, Expression.NewArrayInit(typeof(object), arguments));
                    }
                    else
                    {
                        return InvalidParameterTypes("strings, enumerables");
                    }

                case "contains":
                    ValidateParameterCount(2);

                    if (arguments[0].Type == StringType)
                    {
                        return Expression.Call(arguments[0], Methods.StringContains, arguments[1]);
                    }
                    else if (ReflectionHelper.IsEnumerable(arguments[0].Type, out itemType))
                    {
                        return Expression.Call(null, Methods.EnumerableContains.MakeGenericMethod(itemType),
                            arguments[0],
                            arguments[1]);
                    }
                    else
                    {
                        return InvalidParameterTypes("strings, enumerables");
                    }

                case "endswith":
                    ValidateParameterCount(2);
                    return Expression.Call(arguments[0], Methods.StringEndsWith, arguments[1]);

                case "indexof":
                    ValidateParameterCount(2);
                    return Expression.Call(arguments[0], Methods.StringIndexOf, arguments[1]);

                case "length":
                    ValidateParameterCount(1);

                    if (arguments[0].Type == StringType)
                    {
                        return Expression.MakeMemberAccess(arguments[0], Methods.StringLength);
                    }
                    else if (ReflectionHelper.IsEnumerable(arguments[0].Type, out itemType))
                    {
                        return Expression.Call(null, Methods.EnumerableCount.MakeGenericMethod(itemType), arguments[0]);
                    }
                    else
                    {
                        return InvalidParameterTypes("strings, enumerables");
                    }
                
                case "startswith":
                    ValidateParameterCount(2);
                    return Expression.Call(arguments[0], Methods.StringStartsWith, arguments[1]);

                case "substring":
                    ValidateParameterCount(2, 3);

                    if (arguments[0].Type == StringType)
                    {
                        if (arguments.Count == 2)
                        {
                            return Expression.Call(arguments[0], Methods.StringSubstringOneParam,
                                arguments[1]);
                        }

                        return Expression.Call(arguments[0], Methods.StringSubstringTwoParam,
                            arguments[1],
                            arguments[2]);
                    }
                    else if (ReflectionHelper.IsEnumerable(arguments[0].Type, out itemType))
                    {
                        if (arguments.Count == 2)
                        {
                            return Expression.Call(null, Methods.EnumerableSkip.MakeGenericMethod(itemType),
                                arguments[0],
                                arguments[1]);
                        }

                        var skip = Expression.Call(null, Methods.EnumerableSkip.MakeGenericMethod(itemType),
                            arguments[0],
                            arguments[1]);

                        return Expression.Call(null, Methods.EnumerableTake.MakeGenericMethod(itemType),
                            skip,
                            arguments[2]);
                    }
                    else
                    {
                        return InvalidParameterTypes("strings, enumerables");
                    }

                case "tolower":
                    ValidateParameterCount(1);

                    if (arguments[0].Type == StringType)
                    {
                        return Expression.Call(arguments[0], Methods.StringToLowerInvariant);
                    }
                    else
                    {
                        return InvalidParameterTypes("strings");
                    }
                case "toupper":
                    ValidateParameterCount(1);

                    if (arguments[0].Type == StringType)
                    {
                        return Expression.Call(arguments[0], Methods.StringToUpperInvariant);
                    }
                    else
                    {
                        return InvalidParameterTypes("strings");
                    }
                case "trim":
                    ValidateParameterCount(1);

                    if (arguments[0].Type == StringType)
                    {
                        return Expression.Call(arguments[0], Methods.StringTrim);
                    }
                    else
                    {
                        return InvalidParameterTypes("strings");
                    }

                // date and time functions
                case "date":
                    ValidateParameterCount(1);

                    if (arguments[0].Type == DateTimeType)
                    {
                        return Expression.MakeMemberAccess(arguments[0], Methods.DateTimeDate);
                    }
                    else if (arguments[0].Type == DateTimeOffsetType)
                    {
                        return Expression.MakeMemberAccess(arguments[0], Methods.DateTimeOffsetDate);
                    }
                    else
                    {
                        return InvalidParameterTypes("DateTime, DateTimeOffset");
                    }

                case "time":
                    ValidateParameterCount(1);

                    if (arguments[0].Type == DateTimeType)
                    {
                        return Expression.MakeMemberAccess(arguments[0], Methods.DateTimeTime);
                    }
                    else if (arguments[0].Type == DateTimeOffsetType)
                    {
                        return Expression.MakeMemberAccess(arguments[0], Methods.DateTimeOffsetTime);
                    }
                    else
                    {
                        return InvalidParameterTypes("DateTime, DateTimeOffset");
                    }
                
                case "totaloffsetminutes":
                    ValidateParameterCount(1);

                    if (arguments[0].Type == DateTimeOffsetType)
                    {
                        return Expression.MakeMemberAccess(
                            Expression.MakeMemberAccess(arguments[0], Methods.DateTimeOffsetOffset), Methods.TimeSpanTotalMinutes);
                    }
                    else
                    {
                        return InvalidParameterTypes("DateTimeOffset");
                    }
                
                case "day":
                    ValidateParameterCount(1);

                    if (arguments[0].Type == DateTimeType)
                    {
                        return Expression.MakeMemberAccess(arguments[0], Methods.DateTimeDay);
                    }
                    else if (arguments[0].Type == DateTimeOffsetType)
                    {
                        return Expression.MakeMemberAccess(arguments[0], Methods.DateTimeOffsetDay);
                    }
                    else
                    {
                        return InvalidParameterTypes("DateTime, DateTimeOffset");
                    }
                
                case "month":
                    ValidateParameterCount(1);

                    if (arguments[0].Type == DateTimeType)
                    {
                        return Expression.MakeMemberAccess(arguments[0], Methods.DateTimeMonth);
                    }
                    else if (arguments[0].Type == DateTimeOffsetType)
                    {
                        return Expression.MakeMemberAccess(arguments[0], Methods.DateTimeOffsetMonth);
                    }
                    else
                    {
                        return InvalidParameterTypes("DateTime, DateTimeOffset");
                    }
                
                case "year":
                    ValidateParameterCount(1);

                    if (arguments[0].Type == DateTimeType)
                    {
                        return Expression.MakeMemberAccess(arguments[0], Methods.DateTimeYear);
                    }
                    else if (arguments[0].Type == DateTimeOffsetType)
                    {
                        return Expression.MakeMemberAccess(arguments[0], Methods.DateTimeOffsetYear);
                    }
                    else
                    {
                        return InvalidParameterTypes("DateTime, DateTimeOffset");
                    }
                
                case "hour":
                    ValidateParameterCount(1);

                    if (arguments[0].Type == DateTimeType)
                    {
                        return Expression.MakeMemberAccess(arguments[0], Methods.DateTimeHour);
                    }
                    else if (arguments[0].Type == DateTimeOffsetType)
                    {
                        return Expression.MakeMemberAccess(arguments[0], Methods.DateTimeOffsetHour);
                    }
                    else
                    {
                        return InvalidParameterTypes("DateTime, DateTimeOffset");
                    }
                
                case "minute":
                    ValidateParameterCount(1);

                    if (arguments[0].Type == DateTimeType)
                    {
                        return Expression.MakeMemberAccess(arguments[0], Methods.DateTimeMinute);
                    }
                    else if (arguments[0].Type == DateTimeOffsetType)
                    {
                        return Expression.MakeMemberAccess(arguments[0], Methods.DateTimeOffsetMinute);
                    }
                    else
                    {
                        return InvalidParameterTypes("DateTime, DateTimeOffset");
                    }
                
                case "second":
                    ValidateParameterCount(1);

                    if (arguments[0].Type == DateTimeType)
                    {
                        return Expression.MakeMemberAccess(arguments[0], Methods.DateTimeSecond);
                    }
                    else if (arguments[0].Type == DateTimeOffsetType)
                    {
                        return Expression.MakeMemberAccess(arguments[0], Methods.DateTimeOffsetSecond);
                    }
                    else
                    {
                        return InvalidParameterTypes("DateTime, DateTimeOffset");
                    }
                
                case "fractionalseconds":
                    ValidateParameterCount(1);

                    if (arguments[0].Type == DateTimeType)
                    {
                        return Expression.MakeMemberAccess(arguments[0], Methods.DateTimeMilliseconds);
                    }
                    else if (arguments[0].Type == DateTimeOffsetType)
                    {
                        return Expression.MakeMemberAccess(arguments[0], Methods.DateTimeOffsetMilliseconds);
                    }
                    else
                    {
                        return InvalidParameterTypes("DateTime, DateTimeOffset");
                    }

                case "maxdatetime":
                    ValidateParameterCount(0);
                    return Expression.Constant(DateTimeOffset.MaxValue);
                
                case "mindatetime":
                    ValidateParameterCount(0);
                    return Expression.Constant(DateTimeOffset.MinValue);
                
                case "now":
                    ValidateParameterCount(0);
                    return Expression.Constant(DateTimeOffset.UtcNow);

                // arithmetic functions
                case "ceiling":
                    ValidateParameterCount(1);

                    var ceilArg = arguments[0].Type == DoubleType
                        ? arguments[0]
                        : Expression.Convert(arguments[0], DoubleType);
                    return Expression.Call(null, Methods.MathCeiling, ceilArg);

                case "floor":
                    ValidateParameterCount(1);

                    var floorArg = arguments[0].Type == DoubleType
                        ? arguments[0]
                        : Expression.Convert(arguments[0], DoubleType);
                    return Expression.Call(null, Methods.MathFloor, floorArg);
                
                case "round":
                    ValidateParameterCount(1);

                    var roundArg = arguments[0].Type == DoubleType
                        ? arguments[0]
                        : Expression.Convert(arguments[0], DoubleType);
                    return Expression.Call(null, Methods.MathRound, roundArg);

                // type functions
                case "cast":
                    ValidateParameterCount(2);

                    var castType = ParseTargetType(arguments[1]);
                    if (castType == null)
                    {
                        throw new QueryOptionsValidationException("No proper type for cast specified");
                    }

                    return CreateCastExpression(arguments[0], castType);

                case "isof":
                    ValidateParameterCount(2);

                    var typeCheckType = ParseTargetType(arguments[1]);
                    if (typeCheckType == null)
                    {
                        throw new QueryOptionsValidationException("No proper type for type check specified");
                    }

                    return Expression.TypeIs(arguments[0], typeCheckType);

                default:
                    throw new QueryOptionsValidationException($"Could not find any function '{functionName}'");
            }
        }

        private static Type ParseTargetType(Expression argument)
        {
            var targetType = (argument as ConstantExpression)?.Value as Type;
            if (targetType != null)
            {
                return targetType;
            }

            var typeName = (argument as ConstantExpression)?.Value?.ToString();
            targetType = GetBuiltInTypeByName(typeName);
            if (targetType != null)
            {
                return targetType;
            }

            return null;
        }

        private static Expression CreateCastExpression(Expression argument, Type targetType)
        {
            if (targetType == StringType)
            {
                return Expression.Call(argument, Methods.ObjectToString);
            }

            return Expression.Convert(argument, targetType);
        }

        private static Type GetBuiltInTypeByName(string typeName)
        {
            // primitiveTypeName
            if (typeName.StartsWith("Edm."))
            {
                typeName = typeName.Substring(4);
            }

            return typeName switch
            {
                "Boolean" => typeof(bool),
                "Byte" => typeof(byte),
                "Date" => DateTimeType,
                "DateTimeOffset" => DateTimeOffsetType,
                "Decimal" => typeof(decimal),
                "Double" => DoubleType,
                "Duration" => typeof(TimeSpan),
                "Guid" => typeof(Guid),
                "Int16" => typeof(short),
                "Int32" => typeof(int),
                "Int64" => typeof(long),
                "SByte" => typeof(sbyte),
                "Single" => typeof(float),
                "String" => StringType,
                "TimeOfDay" => typeof(TimeSpan),
                _ => null
            };
        }
        
        private static readonly Type StringType = typeof(string);
        private static readonly Type DoubleType = typeof(double);
        private static readonly Type DateTimeOffsetType = typeof(DateTimeOffset);
        private static readonly Type DateTimeType = typeof(DateTime);
    }
}
