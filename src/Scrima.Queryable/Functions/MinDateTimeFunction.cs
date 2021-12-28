using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Scrima.Queryable.Functions
{
    internal class MinDateTimeFunction : ScrimaQueryableFunction
    {
        public override string FunctionName => "mindatetime";

        public override Expression CreateExpression(IList<Expression> arguments)
        {
            ValidateParameterCount(arguments, 0);
            return Expression.Constant(DateTimeOffset.MinValue);
        }
    }
}