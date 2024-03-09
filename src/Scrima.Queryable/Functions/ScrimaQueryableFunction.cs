using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Scrima.Core.Exceptions;

namespace Scrima.Queryable.Functions;

internal abstract class ScrimaQueryableFunction
{
    public abstract string FunctionName { get; }

    protected void ValidateParameterCount(IList<Expression> arguments, params int[] count)
    {
        if (count.Length == 1 && arguments.Count != count[0])
        {
            throw new QueryOptionsValidationException($"{FunctionName} needs {count[0]} parameters");
        }

        if (!count.Contains(arguments.Count))
        {
            var counts = string.Join(", ", count.Take(count.Length - 1)) + " or " + count.Last();
            throw new QueryOptionsValidationException($"{FunctionName} needs {counts} parameters");
        }
    }

    public abstract Expression CreateExpression(IList<Expression> arguments);

    protected Expression InvalidParameterTypes(string supportedTypes)
    {
        throw new QueryOptionsValidationException(
            $"Unsupported parameters provided to function '{FunctionName}', supported types: {supportedTypes}");
    }
}
