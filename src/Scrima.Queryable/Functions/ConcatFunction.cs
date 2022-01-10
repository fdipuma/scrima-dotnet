using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Scrima.Core.Exceptions;

namespace Scrima.Queryable.Functions
{
    internal class ConcatFunction : ScrimaQueryableFunction
    {
        public override string FunctionName => "concat";

        public override Expression CreateExpression(IList<Expression> arguments)
        {
            if (arguments.Count < 1)
            {
                throw new QueryOptionsValidationException($"{FunctionName} needs at least 1 parameter");
            }

            if (ReflectionHelper.IsEnumerable(arguments[0].Type, out var itemType))
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

            if (arguments[0].Type == TypeUtilities.StringType)
            {
                return Expression.Call(null, Methods.StringConcat,
                    Expression.NewArrayInit(typeof(object), arguments));
            }

            return InvalidParameterTypes("strings, enumerables");
        }
    }
}