using System.Collections.Generic;
using System.Linq.Expressions;

namespace Scrima.Queryable.Functions
{
    internal class SubStringFunction : ScrimaQueryableFunction
    {
        public override string FunctionName => "substring";

        public override Expression CreateExpression(IList<Expression> arguments)
        {
            ValidateParameterCount(arguments, 2, 3);

            if (arguments[0].Type == TypeUtilities.StringType)
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

            if (ReflectionHelper.IsEnumerable(arguments[0].Type, out var itemType))
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

            return InvalidParameterTypes("strings, enumerables");
        }
    }
}