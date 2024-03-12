using System.Collections.Generic;
using System.Linq.Expressions;

namespace Scrima.Queryable.Functions;

internal class FractionalSecondFunction : ScrimaQueryableFunction
{
    public override string FunctionName => "fractionalseconds";

    public override Expression CreateExpression(IList<Expression> arguments)
    {
        ValidateParameterCount(arguments, 1);

        if (arguments[0].Type == TypeUtilities.DateTimeType)
        {
            return Expression.MakeMemberAccess(arguments[0], Methods.DateTimeMilliseconds);
        }

        if (arguments[0].Type == TypeUtilities.DateTimeOffsetType)
        {
            return Expression.MakeMemberAccess(arguments[0], Methods.DateTimeOffsetMilliseconds);
        }

        return InvalidParameterTypes("DateTime, DateTimeOffset");
    }
}