using System;
using System.Linq;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Scrima.OData.Swashbuckle;

internal class QueryOptionsOperationFilter(ScrimaSwaggerOptions options) : IOperationFilter
{
    private static readonly ODataSwaggerOptions DefaultTypeOptions = new();

    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var optionsType = OptionsType(context);

        if (optionsType is not null)
        {
            ChangeQueryOptions(operation, optionsType);
        }
    }

    private static Type OptionsType(OperationFilterContext context)
    {
        if (context.ApiDescription.ActionDescriptor.Properties.TryGetValue("scrima-odata-query", out var queryType))
        {
            return queryType as Type;
        }
        
        return context.ApiDescription.ParameterDescriptions.FirstOrDefault(p => p.Type.IsODataQuery())?.Type;
    }

    private void ChangeQueryOptions(OpenApiOperation operation, Type type)
    {
        var itemTypeOptions = DefaultTypeOptions;

        if (options.ConfigureOptionsPerType is not null)
        {
            var itemType = type.GetScrimaQueryOptionsItemType();

            if (itemType is not null)
            {
                itemTypeOptions = new ODataSwaggerOptions();
                options.ConfigureOptionsPerType.Invoke(itemTypeOptions, itemType);
            }
        }

        UpdateParamInfo(operation, "$count", JsonSchemaType.Boolean,
            "Whether to include the total number of items in the result set before paging",
            itemTypeOptions.AllowCount);
        
        UpdateParamInfo(operation, "$filter", JsonSchemaType.String,
            "A filter to select only a subset of the overall results using the OData 4.0 Syntax (http://docs.oasis-open.org/odata/odata/v4.01/odata-v4.01-part1-protocol.html#_Toc31358948)",
            itemTypeOptions.AllowFilter);

        UpdateParamInfo(operation, "$search", JsonSchemaType.String,
            "A global text filter applied to multiple properties to select only a subset of the overall results",
            itemTypeOptions.AllowSearch);
        
        UpdateParamInfo(operation, "$orderby", JsonSchemaType.String,
            "A comma separated list of properties to sort the result set using the OData 4.0 Syntax (http://docs.oasis-open.org/odata/odata/v4.01/odata-v4.01-part1-protocol.html#_Toc31358952)",
            itemTypeOptions.AllowOrderBy);
        
        UpdateParamInfo(operation, "$skip", JsonSchemaType.Integer,
            "The number of items to skip for paging",
            itemTypeOptions.AllowSkip);
        
        UpdateParamInfo(operation, "$skiptoken", JsonSchemaType.String,
            "Token used for skipping results",
            itemTypeOptions.AllowSkipToken);
        
        UpdateParamInfo(operation, "$top", JsonSchemaType.Integer,
            "The number of elements to include from the result set for paging",
            itemTypeOptions.AllowTop);
    }

    private static void UpdateParamInfo(OpenApiOperation operation, string paramName, JsonSchemaType paramType,
        string paramDescription, bool isParamAllowed)
    {
        var existingParam = operation.Parameters?.FirstOrDefault(p => p.Name == paramName);
        
        if (isParamAllowed)
        {
            if (existingParam is not null)
            {
                operation.Parameters.Remove(existingParam);
            }
            
            var param = new OpenApiParameter
            {
                Name = paramName,
                In = ParameterLocation.Query,
                Description = paramDescription,
                Schema = new OpenApiSchema { Type = paramType }
            };

            operation.Parameters?.Add(param);
        }
        else
        {
            if (existingParam is not null)
            {
                operation.Parameters.Remove(existingParam);
            }
        }
    }
}
