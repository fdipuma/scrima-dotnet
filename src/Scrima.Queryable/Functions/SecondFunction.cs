﻿using System.Collections.Generic;
using System.Linq.Expressions;

namespace Scrima.Queryable.Functions
{
    internal class SecondFunction : ScrimaQueryableFunction
    {
        public override string FunctionName => "second";

        public override Expression CreateExpression(IList<Expression> arguments)
        {
            ValidateParameterCount(arguments, 1);

            if (arguments[0].Type == TypeUtilities.DateTimeType)
            {
                return Expression.MakeMemberAccess(arguments[0], Methods.DateTimeSecond);
            }

            if (arguments[0].Type == TypeUtilities.DateTimeOffsetType)
            {
                return Expression.MakeMemberAccess(arguments[0], Methods.DateTimeOffsetSecond);
            }

            return InvalidParameterTypes("DateTime, DateTimeOffset");
        }
    }
}