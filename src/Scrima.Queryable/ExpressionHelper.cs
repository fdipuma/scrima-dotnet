using System;
using System.Linq.Expressions;

namespace Scrima.Queryable
{
    internal static class ExpressionHelper
    {
        /// <summary>
        /// Rewrites an expression by substituting the argument in the expression with a constant value
        /// </summary>
        /// <param name="source">Source expression</param>
        /// <param name="argument">Constant value to bind</param>
        public static Expression<Func<T1, TResult>> BindSecondArgument<T1, T2, TResult>(Expression<Func<T1, T2, TResult>> source, T2 argument)
        {
            var arg2 = Expression.Constant(argument, typeof (T2));
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

        private static Expression Promote(Expression toPromote, Type otherType)
        {
            // https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/numeric-conversions#implicit-numeric-conversions
            if (toPromote.Type.IsValueType && otherType == typeof(object))
            {
                return Expression.Convert(toPromote, otherType);
            }

            if (toPromote.Type == typeof(string))
            {
                if (otherType.IsEnum)
                {
                    if (toPromote is ConstantExpression constant)
                    {
                        var stringValue = (string)constant.Value;
                        
                        var enumValue = Enum.Parse(otherType, stringValue, true);
                        
                        return Expression.Constant(enumValue);
                    }
                }
            }

            if (toPromote.Type == typeof(sbyte))
            {
                if (otherType == typeof(short) ||
                    otherType == typeof(int) ||
                    otherType == typeof(long) ||
                    otherType == typeof(float) ||
                    otherType == typeof(double) ||
                    otherType == typeof(decimal))
                {
                    return Expression.Convert(toPromote, otherType);
                }
            }

            if (toPromote.Type == typeof(byte))
            {
                if (otherType == typeof(short) ||
                    otherType == typeof(ushort) ||
                    otherType == typeof(int) ||
                    otherType == typeof(uint) ||
                    otherType == typeof(long) ||
                    otherType == typeof(ulong) ||
                    otherType == typeof(float) ||
                    otherType == typeof(double) ||
                    otherType == typeof(decimal))
                {
                    return Expression.Convert(toPromote, otherType);
                }
            }

            if (toPromote.Type == typeof(short))
            {
                if (otherType == typeof(int) ||
                    otherType == typeof(long) ||
                    otherType == typeof(float) ||
                    otherType == typeof(double) ||
                    otherType == typeof(decimal))
                {
                    return Expression.Convert(toPromote, otherType);
                }
            }

            if (toPromote.Type == typeof(ushort))
            {
                if (otherType == typeof(int) ||
                    otherType == typeof(uint) ||
                    otherType == typeof(long) ||
                    otherType == typeof(ulong) ||
                    otherType == typeof(float) ||
                    otherType == typeof(double) ||
                    otherType == typeof(decimal))
                {
                    return Expression.Convert(toPromote, otherType);
                }
            }

            if (toPromote.Type == typeof(int))
            {
                if (otherType == typeof(long) ||
                    otherType == typeof(float) ||
                    otherType == typeof(double) ||
                    otherType == typeof(decimal) ||
                    otherType.IsEnum)
                {
                    return Expression.Convert(toPromote, otherType);
                }
            }

            if (toPromote.Type == typeof(uint))
            {
                if (otherType == typeof(long) ||
                    otherType == typeof(ulong) ||
                    otherType == typeof(float) ||
                    otherType == typeof(double) ||
                    otherType == typeof(decimal))
                {
                    return Expression.Convert(toPromote, otherType);
                }
            }

            if (toPromote.Type == typeof(long))
            {
                if (otherType == typeof(float) ||
                    otherType == typeof(double) ||
                    otherType == typeof(decimal))
                {
                    return Expression.Convert(toPromote, otherType);
                }
            }

            if (toPromote.Type == typeof(ulong))
            {
                if (otherType == typeof(float) ||
                    otherType == typeof(double) ||
                    otherType == typeof(decimal))
                {
                    return Expression.Convert(toPromote, otherType);
                }
            }

            if (toPromote.Type == typeof(float))
            {
                if (otherType == typeof(double) ||
                    otherType == typeof(decimal))
                {
                    return Expression.Convert(toPromote, otherType);
                }
            }

            if (toPromote.Type == typeof(double))
            {
                if (otherType == typeof(decimal))
                {
                    return Expression.Convert(toPromote, otherType);
                }
            }

            return toPromote;
        }
    }
}
