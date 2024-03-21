using System.Collections.Generic;
using System.Linq.Expressions;

namespace Scrima.Queryable.Functions;

internal class MonthFunction : ScrimaQueryableFunction
{
    public override string FunctionName => "month";

    public override Expression CreateExpression(IList<Expression> arguments)
    {
        ValidateParameterCount(arguments, 1);

        if (arguments[0].Type == TypeUtilities.DateOnlyType)
        {
            return Expression.MakeMemberAccess(arguments[0], Methods.DateOnlyMonth);
        }

        if (arguments[0].Type == TypeUtilities.DateTimeType)
        {
            return Expression.MakeMemberAccess(arguments[0], Methods.DateTimeMonth);
        }

        if (arguments[0].Type == TypeUtilities.DateTimeOffsetType)
        {
            return Expression.MakeMemberAccess(arguments[0], Methods.DateTimeOffsetMonth);
        }

        return InvalidParameterTypes("DateTime, DateTimeOffset");
    }
}
