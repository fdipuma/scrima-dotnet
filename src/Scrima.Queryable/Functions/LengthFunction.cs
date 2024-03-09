using System.Collections.Generic;
using System.Linq.Expressions;

namespace Scrima.Queryable.Functions;

internal class LengthFunction : ScrimaQueryableFunction
{
    public override string FunctionName => "length";

    public override Expression CreateExpression(IList<Expression> arguments)
    {
        ValidateParameterCount(arguments, 1);

        if (arguments[0].Type == TypeUtilities.StringType)
        {
            return Expression.MakeMemberAccess(arguments[0], Methods.StringLength);
        }

        if (ReflectionHelper.IsEnumerable(arguments[0].Type, out var itemType))
        {
            return Expression.Call(null, Methods.EnumerableCount.MakeGenericMethod(itemType), arguments[0]);
        }

        return InvalidParameterTypes("strings, enumerables");
    }
}