using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Scrima.Core.Exceptions;
using Scrima.Core.Model;

namespace Scrima.Core.Query;

/// <summary>
/// A class containing deserialised values from the $select query option.
/// </summary>
public sealed class SelectQueryOption
{
    /// <summary>
    /// Gets a <see cref="SelectQueryOption"/> which selects all the properties.
    /// </summary>
    public static SelectQueryOption All { get; } = new(Enumerable.Empty<SelectProperty>());

    /// <summary>
    /// Initialises a new instance of the <see cref="SelectQueryOption"/> class.
    /// </summary>
    public SelectQueryOption(IEnumerable<SelectProperty> properties)
    {
        if (properties == null) throw new ArgumentNullException(nameof(properties));

        Properties = new ReadOnlyCollection<SelectProperty>(properties.ToList());
    }

    /// <summary>
    /// Gets the properties to select; when empty, all the properties are selected.
    /// </summary>
    public IReadOnlyList<SelectProperty> Properties { get; }

    /// <summary>
    /// Creates the projection which builds a new instance of <paramref name="itemType"/> with only the selected
    /// properties populated, e.g.:
    /// <c>o => new Post { Title = o.Title, Blog = o.Blog == null ? null : new Blog { Name = o.Blog.Name } }</c>
    /// </summary>
    /// <remarks>
    /// The projection always creates instances of the declared types: instances of derived types are
    /// returned as their declared (base) type.
    /// </remarks>
    /// <param name="itemType">The type of the projected items</param>
    /// <returns>The projection, or <c>null</c> when all the properties are selected</returns>
    /// <exception cref="QueryOptionsValidationException">A selected property cannot be set, or a type cannot be created</exception>
    public LambdaExpression CreateProjection(Type itemType)
    {
        if (itemType == null) throw new ArgumentNullException(nameof(itemType));

        if (Properties.Count == 0) return null;

        var parameter = Expression.Parameter(itemType, "o");

        var paths = Properties.Select(p => p.Properties).ToList();

        return Expression.Lambda(CreateMemberInit(parameter, paths), parameter);
    }

    private static MemberInitExpression CreateMemberInit(Expression source,
        IReadOnlyCollection<IReadOnlyList<EdmProperty>> paths)
    {
        var type = source.Type;

        if (type.IsAbstract || (!type.IsValueType && type.GetConstructor(Type.EmptyTypes) == null))
        {
            throw new QueryOptionsValidationException(
                $"Type {type.Name} cannot be projected: it needs a public parameterless constructor");
        }

        var bindings = paths
            .GroupBy(path => path[0].Name)
            .Select(group => CreateBinding(source, group.Key, group.ToList()))
            .ToList();

        return Expression.MemberInit(Expression.New(type), bindings);
    }

    private static MemberBinding CreateBinding(Expression source, string propertyName,
        IReadOnlyCollection<IReadOnlyList<EdmProperty>> paths)
    {
        var property = FindProperty(source.Type, propertyName);

        if (property is not { CanWrite: true })
        {
            throw new QueryOptionsValidationException($"Property {propertyName} cannot be selected");
        }

        var value = Expression.Property(source, property);

        // selecting a whole property takes precedence over selecting some of its nested properties
        if (paths.Any(path => path.Count == 1))
        {
            return Expression.Bind(property, value);
        }

        var nestedPaths = paths.Select(path => path.Skip(1).ToList()).ToList();
        var nestedProjection = CreateMemberInit(value, nestedPaths);

        // non-nullable value types cannot be null
        if (value.Type.IsValueType && Nullable.GetUnderlyingType(value.Type) == null)
        {
            return Expression.Bind(property, nestedProjection);
        }

        var nullValue = Expression.Constant(null, value.Type);

        return Expression.Bind(property, Expression.Condition(
            Expression.Equal(value, nullValue),
            nullValue,
            nestedProjection));
    }

    /// <summary>
    /// Finds the public instance property with the specified name, starting from the most derived type,
    /// so that a property hidden with <c>new</c> resolves to the derived declaration.
    /// </summary>
    private static PropertyInfo FindProperty(Type type, string name)
    {
        for (var current = type; current != null; current = current.BaseType)
        {
            var property = current.GetProperty(name,
                BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

            if (property != null) return property;
        }

        return null;
    }

    public override string ToString() =>
        Properties.Count == 0 ? "Select=*" : $"Select={string.Join(",", Properties)}";
}
