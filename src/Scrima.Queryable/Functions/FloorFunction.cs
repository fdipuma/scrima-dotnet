using System.Collections.Generic;
using System.Linq.Expressions;

namespace Scrima.Queryable.Functions;

internal class FloorFunction : ScrimaQueryableFunction
{
    public override string FunctionName => "floor";

    public override Expression CreateExpression(IList<Expression> arguments)
    {
        ValidateParameterCount(arguments, 1);

        var floorArg = arguments[0].Type == TypeUtilities.DoubleType
            ? arguments[0]
            : Expression.Convert(arguments[0], TypeUtilities.DoubleType);
            
        return Expression.Call(null, Methods.MathFloor, floorArg);
    }
}