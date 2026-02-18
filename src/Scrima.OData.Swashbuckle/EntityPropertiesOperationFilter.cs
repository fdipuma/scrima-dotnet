using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json.Nodes;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi;
using Scrima.OData.AspNetCore;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Scrima.OData.Swashbuckle;

/// <summary>
/// Enriches Swagger/OpenAPI for operations that use <see cref="ODataQuery{T}"/> by documenting
/// the entity type T's properties on the $filter and $orderby parameters, and adding an
/// extension so the entity schema is explicit for client generators (e.g. NSwag).
/// </summary>
// ReSharper disable once ClassNeverInstantiated.Global
public sealed class EntityPropertiesOperationFilter(
    ILogger<EntityPropertiesOperationFilter> logger,
    ScrimaSwaggerOptions options) : IOperationFilter
{
    private const string ExtensionKeyODataEntity = "x-odata-entity";

    private static readonly Dictionary<Type, string> JsonTypeMap = new()
    {
        [typeof(string)] = "string",
        [typeof(bool)] = "boolean",
        [typeof(int)] = "integer",
        [typeof(long)] = "integer",
        [typeof(short)] = "integer",
        [typeof(byte)] = "integer",
        [typeof(decimal)] = "number",
        [typeof(double)] = "number",
        [typeof(float)] = "number",
        [typeof(DateTime)] = "string (date-time)",
        [typeof(DateTimeOffset)] = "string (date-time)",
        [typeof(DateOnly)] = "string (date)",
        [typeof(TimeOnly)] = "string (time)",
        [typeof(Guid)] = "string (uuid)",
    };

    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        try
        {
            ApplyCore(operation, context);
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "ODataEntityPropertiesOperationFilter failed for operation {OperationId}; skipping OData enrichment",
                operation.OperationId
            );
        }
    }

    private void ApplyCore(OpenApiOperation operation, OperationFilterContext context)
    {
        // Resolve the entity type T from ODataQuery<T> in the action parameters
        var entityType = GetODataQueryEntityType(context);
        if (entityType == null)
        {
            return;
        }

        // Collect the public, filterable properties of T with their JSON type names
        var props = GetODataProperties(entityType);

        operation.Extensions ??= new Dictionary<string, IOpenApiExtension>();

        var oDataParamConfigs = new List<(string ParamName, string DescriptionTitle, string ExtensionKey)>();
        if (options.EntityFields.Show.HasFlag(ShowEntityFieldsOptions.OnFilter))
            oDataParamConfigs.Add(("filter", "Filterable properties", "x-odata-filterable-properties"));
        if (options.EntityFields.Show.HasFlag(ShowEntityFieldsOptions.OnOrder))
            oDataParamConfigs.Add(("orderby", "Orderable properties", "x-odata-orderable-properties"));
        // For each OData properties involed param ($filter, $orderby), enrich its description and add extension metadata
        foreach (var (paramName, title, extensionKey) in oDataParamConfigs)
        {
            var matchingParam = operation.Parameters?.FirstOrDefault(p =>
                p.Name?.TrimStart('$').Equals(paramName, StringComparison.OrdinalIgnoreCase) ?? false
            );

            // ReSharper disable once UseNullPropagation - unsopported by the pipeline
            if (matchingParam is not null)
            {
                matchingParam.Description = AppendDescription(
                    matchingParam.Description,
                    FormatPropertiesDescription(title, props)
                );
            }

            if (props.Count > 0)
            {
                operation.Extensions[extensionKey] = new JsonNodeExtension(ToOpenApiPropertyArray(props));
            }
        }

        // Tag the operation with the full entity type name so client generators can reference it
        if (options.EntityFields.ExposeAsExtensions)
            operation.Extensions[ExtensionKeyODataEntity] = new JsonNodeExtension(JsonValue.Create(
                entityType.FullName ?? entityType.Name
            )!);
    }

    /// <summary>
    /// Looks for an <see cref="ODataQuery{T}"/> parameter in both the method signature and the API description,
    /// returning the first entity type T found (or null if the action doesn't use OData).
    /// </summary>
    private static Type? GetODataQueryEntityType(OperationFilterContext context)
    {
        var fromMethod = context.MethodInfo?.GetParameters().Select(p => TryExtractODataEntityType(p.ParameterType));

        var fromDescription = context.ApiDescription.ParameterDescriptions.Select(p =>
            TryExtractODataEntityType(p.Type)
        );

        return (fromMethod ?? []).Concat(fromDescription).FirstOrDefault(t => t != null);
    }

    private static Type? TryExtractODataEntityType(Type? type) =>
        type is { IsGenericType: true } && type.GetGenericTypeDefinition() == typeof(ODataQuery<>)
            ? type.GetGenericArguments()[0]
            : null;

    /// <summary>
    /// Reflects over T's public instance properties, keeping only those whose type is
    /// OData-filterable (primitives, enums, common value types), and maps each to its JSON type name.
    /// </summary>
    private static List<(string Name, string JsonType)> GetODataProperties(Type entityType) =>
        entityType
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.CanRead && IsODataFilterableType(p.PropertyType))
            .Select(p => (p.Name, JsonType: GetJsonTypeName(p.PropertyType)))
            .ToList();

    private static Type UnwrapNullable(Type type) => Nullable.GetUnderlyingType(type) ?? type;

    private static bool IsODataFilterableType(Type type)
    {
        var unwrapped = UnwrapNullable(type);
        return unwrapped.IsPrimitive || unwrapped.IsEnum || JsonTypeMap.ContainsKey(unwrapped);
    }

    /// <summary>
    /// Returns OpenAPI/JSON type string for the given CLR type (e.g. string, integer, boolean, string (date-time)).
    /// </summary>
    private static string GetJsonTypeName(Type type)
    {
        var unwrapped = UnwrapNullable(type);

        return JsonTypeMap.TryGetValue(unwrapped, out var jsonType) ? jsonType
            : unwrapped.IsEnum ? $"string ({unwrapped.Name})"
            : "string";
    }

    private static JsonArray ToOpenApiPropertyArray(List<(string Name, string JsonType)> props)
    {
        var result = new JsonArray();
        props.ForEach(x =>
            result.Add(
                new JsonObject { ["name"] = x.Name, ["type"] = x.JsonType }
            )
        );
        return result;
    }

    /// <returns>
    /// {title}:<br/>
    /// - {property1 name} (type)<br/>
    /// - {property2 name} (type)<br/>
    /// - {property3 name} (type)<br/>
    /// ...
    /// </returns>
    private static string FormatPropertiesDescription(string title, List<(string Name, string JsonType)> props) =>
        props.Any() ? $"{title}:\n{string.Join("\n", props.Select(p => $"- {p.Name} ({p.JsonType})"))}." : string.Empty;

    /// <summary>
    /// Append <see cref="FormatPropertiesDescription"/> generated description to the existing one.
    /// If there isn't any existing description <see cref="FormatPropertiesDescription"/> will become the actual description.
    /// </summary>
    private static string AppendDescription(string? existing, string suffix) =>
        string.IsNullOrEmpty(suffix) ? existing ?? string.Empty
        : string.IsNullOrEmpty(existing) ? suffix
        : $"{existing.TrimEnd('.', ' ')}.\n{suffix}";
}
