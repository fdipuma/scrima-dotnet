using System.Linq;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Scrima.OData.Swashbuckle
{
    internal class OpenApiQueryTypesFilter : IDocumentFilter, ISchemaFilter
    {
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            if (context.Type.IsODataQuery())
            {
                schema.Extensions["odata-schema"] = new OpenApiBoolean(true);
            }
        }

        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            var schemasToRemove = swaggerDoc.Components.Schemas
                .Where(s => s.Value.Extensions.TryGetValue("odata-schema", out var value) &&
                            value is OpenApiBoolean {Value: true})
                .Select(s => s.Key)
                .ToList();
            
            foreach (var schemaKey in schemasToRemove)
            {
                swaggerDoc.Components.Schemas.Remove(schemaKey);
            }
        }
    }
}
