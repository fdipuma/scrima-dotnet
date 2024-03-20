using System;
using System.Linq.Expressions;

namespace Scrima.Queryable;

public static class TypeUtilities
{
    public static readonly Type StringType = typeof(string);
    public static readonly Type DoubleType = typeof(double);
    public static readonly Type DateTimeOffsetType = typeof(DateTimeOffset);
    public static readonly Type DateTimeType = typeof(DateTime);
    public static readonly Type DateOnlyType = typeof(DateOnly);

    public static bool IsEnumOrNullableEnum(Type itemType, out Type enumType)
    {
        enumType = null;

        if (itemType.IsEnum)
        {
            enumType = itemType;
            return true;
        }

        var underlyingType = Nullable.GetUnderlyingType(itemType);

        if (underlyingType?.IsEnum == true)
        {
            enumType = underlyingType;
            return true;
        }

        return false;
    }

    public static Type ParseTargetType(Expression argument)
    {
        if (argument is not ConstantExpression constantExpression)
            return null;
            
        if (constantExpression.Value is Type targetType)
        {
            return targetType;
        }

        var typeName = constantExpression.Value?.ToString();
            
        return GetBuiltInTypeByName(typeName);
    }

    private static Type GetBuiltInTypeByName(string typeName)
    {
        // primitiveTypeName
        if (typeName.StartsWith("Edm."))
        {
            typeName = typeName.Substring(4);
        }

        return typeName switch
        {
            "Boolean" => typeof(bool),
            "Byte" => typeof(byte),
            "Date" => DateOnlyType,
            "DateTimeOffset" => DateTimeOffsetType,
            "Decimal" => typeof(decimal),
            "Double" => DoubleType,
            "Duration" => typeof(TimeSpan),
            "Guid" => typeof(Guid),
            "Int16" => typeof(short),
            "Int32" => typeof(int),
            "Int64" => typeof(long),
            "SByte" => typeof(sbyte),
            "Single" => typeof(float),
            "String" => StringType,
            "TimeOfDay" => typeof(TimeOnly),
            _ => null
        };
    }
}
