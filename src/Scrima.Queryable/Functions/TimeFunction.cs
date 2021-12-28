﻿using System.Collections.Generic;
using System.Linq.Expressions;

namespace Scrima.Queryable.Functions
{
    internal class TimeFunction : ScrimaQueryableFunction
    {
        public override string FunctionName => "time";

        public override Expression CreateExpression(IList<Expression> arguments)
        {
            ValidateParameterCount(arguments, 1);

            if (arguments[0].Type == TypeUtilities.DateTimeType)
            {
                return Expression.MakeMemberAccess(arguments[0], Methods.DateTimeTime);
            }

            if (arguments[0].Type == TypeUtilities.DateTimeOffsetType)
            {
                return Expression.MakeMemberAccess(arguments[0], Methods.DateTimeOffsetTime);
            }

            return InvalidParameterTypes("DateTime, DateTimeOffset");
        }
    }
}