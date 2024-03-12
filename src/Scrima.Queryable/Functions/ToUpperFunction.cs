using System.Collections.Generic;
using System.Linq.Expressions;

namespace Scrima.Queryable.Functions;

internal class ToUpperFunction : ScrimaQueryableFunction
{
    public override string FunctionName => "toupper";

    public override Expression CreateExpression(IList<Expression> arguments)
    {
        ValidateParameterCount(arguments, 1);

        if (arguments[0].Type != TypeUtilities.StringType)
        {
            return InvalidParameterTypes("strings");
        }

        return Expression.Call(arguments[0], Methods.StringToUpperInvariant);
    }
}
