using System.Collections.Generic;
using System.Linq.Expressions;

namespace Scrima.Queryable.Functions
{
    internal class RoundFunction : ScrimaQueryableFunction
    {
        public override string FunctionName => "round";

        public override Expression CreateExpression(IList<Expression> arguments)
        {
            ValidateParameterCount(arguments, 1);

            var roundArg = arguments[0].Type == TypeUtilities.DoubleType
                ? arguments[0]
                : Expression.Convert(arguments[0], TypeUtilities.DoubleType);
            return Expression.Call(null, Methods.MathRound, roundArg);
        }
    }
}