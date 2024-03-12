using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Serialization;
using Scrima.Core.Exceptions;

namespace Scrima.Queryable;

internal static class ExpressionHelper
{
    /// <summary>
    /// Rewrites an expression by substituting the argument in the expression with a constant value
    /// </summary>
    /// <param name="source">Source expression</param>
    /// <param name="argument">Constant value to bind</param>
    public static Expression<Func<T1, TResult>> BindSecondArgument<T1, T2, TResult>(
        Expression<Func<T1, T2, TResult>> source, T2 argument)
    {
        var arg2 = Expression.Constant(argument, typeof(T2));
        var rewriter = new Rewriter(source.Parameters[1], arg2);
        var newBody = rewriter.Visit(source.Body);
        return Expression.Lambda<Func<T1, TResult>>(newBody!, source.Parameters[0]);
    }

    private class Rewriter : ExpressionVisitor
    {
        private readonly Expression _candidate;
        private readonly Expression _replacement;

        public Rewriter(Expression candidate, Expression replacement)
        {
            _candidate = candidate ?? throw new ArgumentNullException(nameof(candidate));
            _replacement = replacement ?? throw new ArgumentNullException(nameof(replacement));
        }

        public override Expression Visit(Expression node)
        {
            return node == _candidate ? _replacement : base.Visit(node);
        }
    }

    /// <summary>
    /// Promotes two expressions into a compatible type (e.g. float, double => double, double)
    /// </summary>
    /// <param name="toPromote">Expression to promote</param>
    /// <param name="other">Other side of expression</param>
    /// <returns></returns>
    public static Expression Promote(Expression toPromote, Expression other)
    {
        return Promote(toPromote, other.Type);
    }

    private static Expression Promote(Expression toPromote, Type otherExpressionType)
    {
        if (toPromote.Type == otherExpressionType)
        {
            return toPromote;
        }

        var (toPromotePrimiveType, toPromoteNullableType) = GetUnderlyingType(toPromote.Type);
        var (otherPrimitiveType, otherNullableType) = GetUnderlyingType(otherExpressionType);

        var toPromoteIsNullable = toPromoteNullableType is not null;
            
        if (toPromote.Type.IsValueType && otherExpressionType == typeof(object) &&
            toPromoteIsNullable)
        {
            return Expression.Convert(toPromote, otherExpressionType);
        }

        if (toPromotePrimiveType == typeof(string) && otherPrimitiveType.IsEnum)
        {
            if (toPromote is ConstantExpression constant) // we can promote only ConstantsExpressions from string
            {
                if (constant.Value is null)
                {
                    return constant;
                }

                var stringValue = (string)constant.Value;

                var enumValue = ParseEnum(otherPrimitiveType, stringValue);

                return Expression.Constant(enumValue, otherNullableType ?? otherPrimitiveType);
            }
        }

        if (toPromotePrimiveType == typeof(DateTime) && otherPrimitiveType == typeof(DateTimeOffset))
        {
            return ConvertExpression(toPromote, toPromoteIsNullable, otherPrimitiveType, otherNullableType);
        }

        if (toPromotePrimiveType == typeof(sbyte))
        {
            if (otherPrimitiveType == typeof(short) ||
                otherPrimitiveType == typeof(int) ||
                otherPrimitiveType == typeof(long) ||
                otherPrimitiveType == typeof(float) ||
                otherPrimitiveType == typeof(double) ||
                otherPrimitiveType == typeof(decimal))
            {
                return ConvertExpression(toPromote, toPromoteIsNullable, otherPrimitiveType, otherNullableType);
            }
        }

        if (toPromotePrimiveType == typeof(byte))
        {
            if (otherPrimitiveType == typeof(short) ||
                otherPrimitiveType == typeof(ushort) ||
                otherPrimitiveType == typeof(int) ||
                otherPrimitiveType == typeof(uint) ||
                otherPrimitiveType == typeof(long) ||
                otherPrimitiveType == typeof(ulong) ||
                otherPrimitiveType == typeof(float) ||
                otherPrimitiveType == typeof(double) ||
                otherPrimitiveType == typeof(decimal))
            {
                return ConvertExpression(toPromote, toPromoteIsNullable, otherPrimitiveType, otherNullableType);
            }
        }

        if (toPromotePrimiveType == typeof(short))
        {
            if (otherPrimitiveType == typeof(int) ||
                otherPrimitiveType == typeof(long) ||
                otherPrimitiveType == typeof(float) ||
                otherPrimitiveType == typeof(double) ||
                otherPrimitiveType == typeof(decimal))
            {
                return ConvertExpression(toPromote, toPromoteIsNullable, otherPrimitiveType, otherNullableType);
            }
        }

        if (toPromotePrimiveType == typeof(ushort))
        {
            if (otherPrimitiveType == typeof(int) ||
                otherPrimitiveType == typeof(uint) ||
                otherPrimitiveType == typeof(long) ||
                otherPrimitiveType == typeof(ulong) ||
                otherPrimitiveType == typeof(float) ||
                otherPrimitiveType == typeof(double) ||
                otherPrimitiveType == typeof(decimal))
            {
                return ConvertExpression(toPromote, toPromoteIsNullable, otherPrimitiveType, otherNullableType);
            }
        }

        if (toPromotePrimiveType == typeof(int))
        {
            if (otherPrimitiveType == typeof(long) ||
                otherPrimitiveType == typeof(float) ||
                otherPrimitiveType == typeof(double) ||
                otherPrimitiveType == typeof(decimal) ||
                otherPrimitiveType.IsEnum)
            {
                return ConvertExpression(toPromote, toPromoteIsNullable, otherPrimitiveType, otherNullableType);
            }
        }

        if (toPromotePrimiveType == typeof(uint))
        {
            if (otherPrimitiveType == typeof(long) ||
                otherPrimitiveType == typeof(ulong) ||
                otherPrimitiveType == typeof(float) ||
                otherPrimitiveType == typeof(double) ||
                otherPrimitiveType == typeof(decimal))
            {
                return ConvertExpression(toPromote, toPromoteIsNullable, otherPrimitiveType, otherNullableType);
            }
        }

        if (toPromotePrimiveType == typeof(long))
        {
            if (otherPrimitiveType == typeof(float) ||
                otherPrimitiveType == typeof(double) ||
                otherPrimitiveType == typeof(decimal))
            {
                return ConvertExpression(toPromote, toPromoteIsNullable, otherPrimitiveType, otherNullableType);
            }
        }

        if (toPromotePrimiveType == typeof(ulong))
        {
            if (otherPrimitiveType == typeof(float) ||
                otherPrimitiveType == typeof(double) ||
                otherPrimitiveType == typeof(decimal))
            {
                return ConvertExpression(toPromote, toPromoteIsNullable, otherPrimitiveType, otherNullableType);
            }
        }

        if (toPromotePrimiveType == typeof(float))
        {
            if (otherPrimitiveType == typeof(double) ||
                otherPrimitiveType == typeof(decimal))
            {
                return ConvertExpression(toPromote, toPromoteIsNullable, otherPrimitiveType, otherNullableType);
            }
        }

        if (toPromotePrimiveType == typeof(double))
        {
            if (otherPrimitiveType == typeof(decimal))
            {
                return ConvertExpression(toPromote, toPromoteIsNullable, otherPrimitiveType, otherNullableType);
            }
        }
            
        if (otherNullableType is not null && !otherPrimitiveType.IsEnum && toPromotePrimiveType != typeof(object))
        {
            // this will promote nullables to the best matching type of nullable (in this case is the same type of the "toPromote" type)
                
            toPromoteNullableType ??= typeof(Nullable<>).MakeGenericType(toPromotePrimiveType); 
            return ConvertExpression(toPromote, toPromoteIsNullable, toPromotePrimiveType, toPromoteNullableType);
        }

        return toPromote;
    }

    private static Expression ConvertExpression(Expression toPromote, bool expressionTypeIsNullable, Type otherType, Type otherNullableType)
    {
        if (!expressionTypeIsNullable && otherNullableType is null)
            return Expression.Convert(toPromote, otherType);

        if (otherNullableType is not null)
            return Expression.Convert(toPromote, otherNullableType);

        var nullableType = typeof(Nullable<>).MakeGenericType(otherType);

        return Expression.Convert(toPromote, nullableType);
    }

    private static (Type primitiveType, Type nullableType) GetUnderlyingType(Type type)
    {
        var nullableUnderlyingType = Nullable.GetUnderlyingType(type);

        if (nullableUnderlyingType is not null)
        {
            return (nullableUnderlyingType, type);
        }

        return (type, null);
    }

    public static object ToEnumValue(Type enumType, object value)
    {
        if (value is int) return Enum.ToObject(enumType, value);

        if (value is string stringValue) return ParseEnum(enumType, stringValue);

        return Enum.ToObject(enumType, 0);
    }
        
    public static object ParseEnum(Type enumType, string value)
    {
        if (value == null) throw new ArgumentNullException(nameof(value));

        var names = Enum.GetNames(enumType);
        var values = Enum.GetValues(enumType);

        for (var index = 0; index < Enum.GetNames(enumType).Length; index++)
        {
            var name = names[index];

            if (string.Equals(name, value, StringComparison.OrdinalIgnoreCase))
            {
                return values.GetValue(index);
            }

            var enumMemberAttribute = enumType.GetField(name).GetCustomAttribute<EnumMemberAttribute>();

            if (enumMemberAttribute is not null && string.Equals(value, enumMemberAttribute.Value,
                    StringComparison.OrdinalIgnoreCase))
            {
                return values.GetValue(index);
            }
        }

        throw new QueryOptionsValidationException($"Invalid value '{values}' for enum type {enumType.Name}");
    }

    public static Expression CreateCastExpression(Expression argument, Type targetType)
    {
        if (targetType == TypeUtilities.StringType)
        {
            return Expression.Call(argument, Methods.ObjectToString);
        }

        return Expression.Convert(argument, targetType);
    }
}