using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Scrima.Queryable.Functions;

internal class ContainsFunction : ScrimaQueryableFunction
{
    public override string FunctionName => "contains";

    public override Expression CreateExpression(IList<Expression> arguments)
    {
        ValidateParameterCount(arguments, 2);

        if (arguments[0].Type == TypeUtilities.StringType)
        {
            return Expression.Call(arguments[0], Methods.StringContains, arguments[1]);
        }

        if (!ReflectionHelper.IsEnumerable(arguments[0].Type, out var itemType))
        {
            return InvalidParameterTypes("strings, enumerables");
        }

        var sourceArg = arguments[0];
        var elementArg = arguments[1];

        var itemIsEnum = TypeUtilities.IsEnumOrNullableEnum(itemType, out var itemEnumType);
        var argIsEnum = TypeUtilities.IsEnumOrNullableEnum(elementArg.Type, out var argEnumType);

        if (itemType == elementArg.Type || (!itemIsEnum && !argIsEnum))
        {
            return Expression.Call(null, Methods.EnumerableContains.MakeGenericMethod(itemType),
                sourceArg,
                elementArg);
        }

        // enum case: we need to convert/promote expression to enum types

        var enumType = itemIsEnum ? itemEnumType : argEnumType;
        var arrayType = itemIsEnum ? itemType : elementArg.Type;

        if (sourceArg is ConstantExpression sourceConstantExpression)
        {
            var sourceEnumerable = ((IEnumerable)sourceConstantExpression.Value).Cast<object>()
                .ToArray();

            var enumArray = Array.CreateInstance(arrayType, sourceEnumerable.Length);

            for (var index = 0; index < sourceEnumerable.Length; index++)
            {
                var item = sourceEnumerable[index];
                enumArray.SetValue(ExpressionHelper.ToEnumValue(enumType, item), index);
            }

            sourceArg = Expression.Constant(enumArray, enumArray.GetType());

            itemType = arrayType;
        }
        else if (elementArg is ConstantExpression elementConstantExpression)
        {
            elementArg =
                Expression.Constant(ExpressionHelper.ToEnumValue(enumType,
                    elementConstantExpression.Value));
        }

        return Expression.Call(null, Methods.EnumerableContains.MakeGenericMethod(itemType),
            sourceArg,
            elementArg);
    }
}
