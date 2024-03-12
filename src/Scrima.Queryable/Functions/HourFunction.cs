using System.Collections.Generic;
using System.Linq.Expressions;

namespace Scrima.Queryable.Functions;

internal class HourFunction : ScrimaQueryableFunction
{
    public override string FunctionName => "hour";

    public override Expression CreateExpression(IList<Expression> arguments)
    {
        ValidateParameterCount(arguments, 1);

        if (arguments[0].Type == TypeUtilities.DateTimeType)
        {
            return Expression.MakeMemberAccess(arguments[0], Methods.DateTimeHour);
        }

        if (arguments[0].Type == TypeUtilities.DateTimeOffsetType)
        {
            return Expression.MakeMemberAccess(arguments[0], Methods.DateTimeOffsetHour);
        }

        return InvalidParameterTypes("DateTime, DateTimeOffset");
    }
}