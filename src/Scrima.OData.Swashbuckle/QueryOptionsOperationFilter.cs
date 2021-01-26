using System;
using System.Linq;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Scrima.OData.Swashbuckle
{
    internal class QueryOptionsOperationFilter : IOperationFilter
    {
        private readonly ScrimaSwaggerOptions _options;
        private static readonly ODataSwaggerOptions DefaultTypeOptions = new();

        public QueryOptionsOperationFilter(ScrimaSwaggerOptions options)
        {
            _options = options;
        }
        
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

            if (_options.ConfigureOptionsPerType is not null)
            {
                var itemType = type.GetScrimaQueryOptionsItemType();

                if (itemType is not null)
                {
                    itemTypeOptions = new ODataSwaggerOptions();
                    _options.ConfigureOptionsPerType.Invoke(itemTypeOptions, itemType);
                }
            }

            UpdateParamInfo(operation, "$count", "boolean",
                "Whether to include the total number of items in the result set before paging",
                itemTypeOptions.AllowCount);
            
            UpdateParamInfo(operation, "$filter", "string",
                "A filter to select only a subset of the overall results using the OData 4.0 Syntax (http://docs.oasis-open.org/odata/odata/v4.01/odata-v4.01-part1-protocol.html#_Toc31358948)",
                itemTypeOptions.AllowFilter);

            UpdateParamInfo(operation, "$search", "string",
                "A global text filter applied to multiple properties to select only a subset of the overall results",
                itemTypeOptions.AllowSearch);
            
            UpdateParamInfo(operation, "$orderby", "string",
                "A comma separated list of properties to sort the result set using the OData 4.0 Syntax (http://docs.oasis-open.org/odata/odata/v4.01/odata-v4.01-part1-protocol.html#_Toc31358952)",
                itemTypeOptions.AllowOrderBy);
            
            UpdateParamInfo(operation, "$skip", "integer",
                "The number of items to skip for paging",
                itemTypeOptions.AllowSkip);
            
            UpdateParamInfo(operation, "$skiptoken", "string",
                "Token used for skipping results",
                itemTypeOptions.AllowSkipToken);
            
            UpdateParamInfo(operation, "$top", "integer",
                "The number of elements to include from the result set for paging",
                itemTypeOptions.AllowTop);
        }

        private static void UpdateParamInfo(OpenApiOperation operation, string paramName, string paramType,
            string paramDescription, bool isParamAllowed)
        {
            var param = operation.Parameters.FirstOrDefault(p => p.Name == paramName);
            
            if (isParamAllowed)
            {
                if (param is null)
                {
                    param = new OpenApiParameter {Name = paramName};
                    operation.Parameters.Add(param);
                }

                param.In = ParameterLocation.Query;
                param.Schema = new OpenApiSchema {Type = paramType};
                param.Description = paramDescription;
            }
            else
            {
                if (param is not null)
                {
                    operation.Parameters.Remove(param);
                }
            }
        }
    }
}
