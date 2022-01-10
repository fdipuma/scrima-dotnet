using System.Collections.Generic;
using System.Linq.Expressions;

namespace Scrima.Queryable.Functions
{
    internal class EndsWithFunction : ScrimaQueryableFunction
    {
        public override string FunctionName => "endswith";

        public override Expression CreateExpression(IList<Expression> arguments)
        {
            ValidateParameterCount(arguments, 2);
            return Expression.Call(arguments[0], Methods.StringEndsWith, arguments[1]);
        }
    }
}