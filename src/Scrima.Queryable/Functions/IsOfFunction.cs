using System.Collections.Generic;
using System.Linq.Expressions;
using Scrima.Core.Exceptions;

namespace Scrima.Queryable.Functions;

internal class IsOfFunction : ScrimaQueryableFunction
{
    public override string FunctionName => "isof";

    public override Expression CreateExpression(IList<Expression> arguments)
    {
        ValidateParameterCount(arguments, 2);

        var typeCheckType = TypeUtilities.ParseTargetType(arguments[1]);
        if (typeCheckType == null)
        {
            throw new QueryOptionsValidationException("No proper type for type check specified");
        }

        return Expression.TypeIs(arguments[0], typeCheckType);
    }
}
