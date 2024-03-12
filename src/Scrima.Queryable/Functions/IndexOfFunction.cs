using System.Collections.Generic;
using System.Linq.Expressions;

namespace Scrima.Queryable.Functions;

internal class IndexOfFunction : ScrimaQueryableFunction
{
    public override string FunctionName => "indexof";

    public override Expression CreateExpression(IList<Expression> arguments)
    {
        ValidateParameterCount(arguments, 2);
        return Expression.Call(arguments[0], Methods.StringIndexOf, arguments[1]);
    }
}