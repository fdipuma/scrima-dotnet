using System.Collections.Generic;
using System.Linq.Expressions;

namespace Scrima.Queryable.Functions;

internal class CeilingFunction : ScrimaQueryableFunction
{
    public override string FunctionName => "ceiling";

    public override Expression CreateExpression(IList<Expression> arguments)
    {
        ValidateParameterCount(arguments, 1);

        var ceilArg = arguments[0].Type == TypeUtilities.DoubleType
            ? arguments[0]
            : Expression.Convert(arguments[0], TypeUtilities.DoubleType);
        return Expression.Call(null, Methods.MathCeiling, ceilArg);
    }
}