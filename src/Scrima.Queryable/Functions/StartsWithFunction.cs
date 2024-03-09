using System.Collections.Generic;
using System.Linq.Expressions;

namespace Scrima.Queryable.Functions;

internal class StartsWithFunction : ScrimaQueryableFunction
{
    public override string FunctionName => "startswith";

    public override Expression CreateExpression(IList<Expression> arguments)
    {
        ValidateParameterCount(arguments, 2);
        return Expression.Call(arguments[0], Methods.StringStartsWith, arguments[1]);
    }
}
