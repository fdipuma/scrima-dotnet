using System.Collections.Generic;
using System.Linq.Expressions;
using Scrima.Core.Exceptions;

namespace Scrima.Queryable.Functions
{
    internal class CastFunction : ScrimaQueryableFunction
    {
        public override string FunctionName => "cast";

        public override Expression CreateExpression(IList<Expression> arguments)
        {
            ValidateParameterCount(arguments, 2);

            var castType = TypeUtilities.ParseTargetType(arguments[1]);
            if (castType == null)
            {
                throw new QueryOptionsValidationException("No proper type for cast specified");
            }

            return ExpressionHelper.CreateCastExpression(arguments[0], castType);
        }
    }
}