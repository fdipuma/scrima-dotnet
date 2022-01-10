using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Scrima.Queryable.Functions
{
    internal class NowFunction : ScrimaQueryableFunction
    {
        public override string FunctionName => "now";

        public override Expression CreateExpression(IList<Expression> arguments)
        {
            ValidateParameterCount(arguments, 0);
            return Expression.Constant(DateTimeOffset.UtcNow);
        }
    }
}