using System.Collections.Generic;
using System.Linq.Expressions;

namespace Scrima.Queryable.Functions
{
    internal class ToLowerFunction : ScrimaQueryableFunction
    {
        public override string FunctionName => "tolower";

        public override Expression CreateExpression(IList<Expression> arguments)
        {
            ValidateParameterCount(arguments, 1);

            if (arguments[0].Type != TypeUtilities.StringType)
            {
                return InvalidParameterTypes("strings");
            }

            return Expression.Call(arguments[0], Methods.StringToLowerInvariant);
        }
    }
}