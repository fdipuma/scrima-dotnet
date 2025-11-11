using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi;
using Scrima.Core.Model;
using Scrima.Core.Query;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Scrima.OData.Swashbuckle;

public static class SwaggerGenOptionsExtensions
{
    public static void AddScrimaOData(this SwaggerGenOptions options,
        Action<ScrimaSwaggerOptions> configureOptions = null)
    {
            var odataOptions = new ScrimaSwaggerOptions();
            configureOptions?.Invoke(odataOptions);
            
            options.OperationFilter<QueryOptionsOperationFilter>(odataOptions);
            options.SchemaFilter<OpenApiQueryTypesFilter>();
            options.DocumentFilter<OpenApiQueryTypesFilter>();
            
            // used to avoit schema generation for such types
            options.MapType<EdmComplexType>(() => new OpenApiSchema());
            options.MapType<FilterQueryOption>(() => new OpenApiSchema());
            options.MapType<OrderByQueryOption>(() => new OpenApiSchema());
        }
}
