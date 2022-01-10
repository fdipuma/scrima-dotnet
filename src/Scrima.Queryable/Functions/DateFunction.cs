using System.Collections.Generic;
using System.Linq.Expressions;

namespace Scrima.Queryable.Functions
{
    internal class DateFunction : ScrimaQueryableFunction
    {
        public override string FunctionName => "date";

        public override Expression CreateExpression(IList<Expression> arguments)
        {
            ValidateParameterCount(arguments, 1);

            if (arguments[0].Type == TypeUtilities.DateTimeType)
            {
                return Expression.MakeMemberAccess(arguments[0], Methods.DateTimeDate);
            }

            if (arguments[0].Type == TypeUtilities.DateTimeOffsetType)
            {
                return Expression.MakeMemberAccess(arguments[0], Methods.DateTimeOffsetDate);
            }

            return InvalidParameterTypes("DateTime, DateTimeOffset");
        }
    }
}