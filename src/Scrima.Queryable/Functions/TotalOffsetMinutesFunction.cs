using System.Collections.Generic;
using System.Linq.Expressions;

namespace Scrima.Queryable.Functions;

internal class TotalOffsetMinutesFunction : ScrimaQueryableFunction
{
    public override string FunctionName => "totaloffsetminutes";

    public override Expression CreateExpression(IList<Expression> arguments)
    {
        ValidateParameterCount(arguments, 1);

        if (arguments[0].Type != TypeUtilities.DateTimeOffsetType)
        {
            return InvalidParameterTypes("DateTimeOffset");
        }
        
        return Expression.MakeMemberAccess(
            Expression.MakeMemberAccess(arguments[0], Methods.DateTimeOffsetOffset),
            Methods.TimeSpanTotalMinutes);
    }
}
