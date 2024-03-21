using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Scrima.Core.Model;

namespace Scrima.Core;

public class EdmTypeProvider
{
    private static readonly Dictionary<Type, EdmType> PrimitiveTypes = new()
    {
        [typeof(byte[])] = EdmPrimitiveType.Binary,
        [typeof(bool)] = EdmPrimitiveType.Boolean,
        [typeof(bool?)] = EdmPrimitiveType.Boolean,
        [typeof(byte)] = EdmPrimitiveType.Byte,
        [typeof(byte?)] = EdmPrimitiveType.Byte,
        [typeof(DateOnly)] = EdmPrimitiveType.Date,
        [typeof(DateOnly?)] = EdmPrimitiveType.Date,
        [typeof(DateTime)] = EdmPrimitiveType.DateTimeOffset,
        [typeof(DateTime?)] = EdmPrimitiveType.DateTimeOffset,
        [typeof(DateTimeOffset)] = EdmPrimitiveType.DateTimeOffset,
        [typeof(DateTimeOffset?)] = EdmPrimitiveType.DateTimeOffset,
        [typeof(decimal)] = EdmPrimitiveType.Decimal,
        [typeof(decimal?)] = EdmPrimitiveType.Decimal,
        [typeof(double)] = EdmPrimitiveType.Double,
        [typeof(double?)] = EdmPrimitiveType.Double,
        [typeof(TimeSpan)] = EdmPrimitiveType.Duration,
        [typeof(TimeSpan?)] = EdmPrimitiveType.Duration,
        [typeof(TimeOnly)] = EdmPrimitiveType.TimeOfDay,
        [typeof(TimeOnly?)] = EdmPrimitiveType.TimeOfDay,
        [typeof(Guid)] = EdmPrimitiveType.Guid,
        [typeof(Guid?)] = EdmPrimitiveType.Guid,
        [typeof(short)] = EdmPrimitiveType.Int16,
        [typeof(short?)] = EdmPrimitiveType.Int16,
        [typeof(int)] = EdmPrimitiveType.Int32,
        [typeof(int?)] = EdmPrimitiveType.Int32,
        [typeof(long)] = EdmPrimitiveType.Int64,
        [typeof(long?)] = EdmPrimitiveType.Int64,
        [typeof(sbyte)] = EdmPrimitiveType.SByte,
        [typeof(sbyte?)] = EdmPrimitiveType.SByte,
        [typeof(float)] = EdmPrimitiveType.Single,
        [typeof(float?)] = EdmPrimitiveType.Single,
        [typeof(char)] = EdmPrimitiveType.String,
        [typeof(char?)] = EdmPrimitiveType.String,
        [typeof(string)] = EdmPrimitiveType.String
    };
        
    private readonly ConcurrentDictionary<Type, EdmType> _mapByType = new(PrimitiveTypes);

    private readonly ConcurrentDictionary<string, EdmType> _mapByName =
        new(PrimitiveTypes.Values.GroupBy(v => v.FullName).ToDictionary(g => g.Key, g => g.First()));

    public EdmType GetByClrType(Type type)
    {
        return _mapByType.GetOrAdd(type, ResolveEdmType);
    }

    public EdmType GetByName(string edmEnumTypeName)
    {
        return _mapByName.GetOrAdd(edmEnumTypeName, name =>
        {
            return _mapByType.Values.FirstOrDefault(e => e.FullName == name);
        });
    }

    private EdmType ResolveEdmType(Type clrType) => ResolveEdmType(clrType, new Dictionary<Type, EdmType>());
        
    private EdmType ResolveEdmType(Type clrType, IDictionary<Type, EdmType> visitedTypes)
    {
        if (visitedTypes.TryGetValue(clrType, out var visitedEdmType))
            return visitedEdmType; 
            
        if (clrType.IsEnum)
        {
            var members = new List<EdmEnumMember>();
            foreach (var x in Enum.GetValues(clrType))
            {
                members.Add(new EdmEnumMember(x.ToString(), (int)x));
            }

            return new EdmEnumType(clrType, members.AsReadOnly());
        }

        if (clrType.IsGenericType)
        {
            var innerType = clrType.GetGenericArguments()[0];
            if (typeof(IEnumerable<>).MakeGenericType(innerType).IsAssignableFrom(clrType))
            {
                var containedType = _mapByType.GetOrAdd(innerType, t =>  ResolveEdmType(t, visitedTypes));
                return _mapByType.GetOrAdd(clrType, t => new EdmCollectionType(t, containedType));
            }
        }

        var baseType = clrType.BaseType != typeof(object)
            ? ResolveEdmType(
                clrType: clrType.BaseType,
                visitedTypes: visitedTypes
            )
            : null;

        var clrTypeProperties = clrType
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .OrderBy(p => p.Name);

        var edmProperties = new List<EdmProperty>();
        var edmComplexType = new EdmComplexType(clrType, baseType, edmProperties);

        visitedTypes[clrType] = edmComplexType;
            
        edmProperties.AddRange(
            clrTypeProperties.Select(
                p => new EdmProperty(
                    p.Name,
                    _mapByType.GetOrAdd(p.PropertyType, t => ResolveEdmType(t, visitedTypes)),
                    edmComplexType)));

        return edmComplexType;
    }
}
