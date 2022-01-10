using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Scrima.Core.Exceptions;
using Scrima.Core.Query;
using Scrima.Core.Query.Expressions;
using Scrima.Queryable.Functions;

namespace Scrima.Queryable
{
    public static partial class ScrimaExtensions
    {
        private static readonly IDictionary<string, ScrimaQueryableFunction> AvailableFunctions = typeof(ScrimaQueryableFunction)
            .Assembly
            .GetTypes()
            .Where(t => typeof(ScrimaQueryableFunction).IsAssignableFrom(t) && !t.IsAbstract)
            .Select(Activator.CreateInstance)
            .Cast<ScrimaQueryableFunction>()
            .ToDictionary(s => s.FunctionName, s => s, StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Filters a sequence of values based on the provided <see cref="FilterQueryOption"/>
        /// </summary>
        /// <param name="source">The queryable source</param>
        /// <param name="filterQueryOption">The filter options to apply</param>
        /// <returns>The filtered data source</returns>
        /// <exception cref="ArgumentNullException">source or filter are null</exception>
        public static IQueryable<TSource> Where<TSource>(this IQueryable<TSource> source,
            FilterQueryOption filterQueryOption)
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
                    CreateFunctionCallExpression(functionCall.Name,
                        functionCall.Parameters.CreateExpressions(baseExpression)),

                UnaryOperatorNode { OperatorKind: UnaryOperatorKind.Not } unaryOperator =>
                    Expression.Not(CreateExpression(unaryOperator.Operand, baseExpression)),

                BinaryOperatorNode binaryOperator =>
                    CreateBinaryOperatorExpression(binaryOperator, baseExpression),

                _ => throw new QueryOptionsValidationException($"Invalid query {queryNode.Kind}")
            };
        }

        private static IList<Expression> CreateExpressions(this IEnumerable<QueryNode> nodes, Expression baseExpression)
            => nodes.Select(n => CreateExpression(n, baseExpression)).ToArray();

        /// <summary>
        /// Creates an expression from a binary operation like: left OPERATOR right
        /// </summary>
        /// <param name="binaryOperator">The binary operator node</param>
        /// <param name="baseExpression">Base expression</param>
        /// <returns></returns>
        private static Expression CreateBinaryOperatorExpression(BinaryOperatorNode binaryOperator,
            Expression baseExpression)
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
                return CreateFunctionCallExpression("contains", new []{ right, left });

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
                case ConstantNode { Value: null }:
                    return Expression.Constant(null);

                case ConstantNode { Value: { } } constant:
                    return Expression.Constant(constant.Value, constant.EdmType.ClrType);

                case ArrayNode { Value: { } } array:
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
            if (!AvailableFunctions.TryGetValue(functionName, out var function))
            {
                throw new QueryOptionsValidationException($"Could not find any function '{functionName}'");
            }

            return function.CreateExpression(arguments);
        }
    }
}
