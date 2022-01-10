using System.Collections.Generic;
using System.Linq.Expressions;

namespace Scrima.Queryable.Functions
{
    internal class MinuteFunction : ScrimaQueryableFunction
    {
        public override string FunctionName => "minute";

        public override Expression CreateExpression(IList<Expression> arguments)
        {
            ValidateParameterCount(arguments, 1);

            if (arguments[0].Type == TypeUtilities.DateTimeType)
            {
                return Expression.MakeMemberAccess(arguments[0], Methods.DateTimeMinute);
            }

            if (arguments[0].Type == TypeUtilities.DateTimeOffsetType)
            {
                return Expression.MakeMemberAccess(arguments[0], Methods.DateTimeOffsetMinute);
            }

            return InvalidParameterTypes("DateTime, DateTimeOffset");
        }
    }
}