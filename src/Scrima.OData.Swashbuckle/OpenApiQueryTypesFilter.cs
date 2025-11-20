using System.Linq;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Scrima.OData.Swashbuckle;

internal class OpenApiQueryTypesFilter : IDocumentFilter, ISchemaFilter
{
    public void Apply(IOpenApiSchema schema, SchemaFilterContext context)
    {
        if (context.Type.IsODataQuery() && schema.Extensions != null)
        {
            schema.Extensions["odata-schema"] = new JsonNodeExtension(true);
        }
    }
    
    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        var schemasToRemove = swaggerDoc.Components?.Schemas
            ?.Where(s => 
                s.Value.Extensions != null &&
                s.Value.Extensions.TryGetValue("odata-schema", out var value) &&
                value is { })
            .Select(s => s.Key)
            .ToList();
        
        if (schemasToRemove is null)
        {
            return;
        }

        foreach (var schemaKey in schemasToRemove)
        {
            swaggerDoc.Components?.Schemas?.Remove(schemaKey);
        }
    }
}
