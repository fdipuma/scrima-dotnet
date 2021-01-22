using System.Linq;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
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
            var optionsParameter = context.ApiDescription.ParameterDescriptions.FirstOrDefault(p => p.Type.IsScrimaQueryOptions());

            if (optionsParameter is not null)
            {
                AddQueryOptions(operation, optionsParameter);
                operation.RequestBody.Content.Clear();
            }
        }

        private void AddQueryOptions(OpenApiOperation operation, ApiParameterDescription apiParameterDescription)
        {
            var itemTypeOptions = DefaultTypeOptions;

            if (_options.ConfigureOptionsPerType is not null)
            {
                var itemType = apiParameterDescription.Type.GetScrimaQueryOptionsItemType();

                if (itemType is not null)
                {
                    itemTypeOptions = new ODataSwaggerOptions();
                    _options.ConfigureOptionsPerType.Invoke(itemTypeOptions, itemType);
                }
            }

            if (itemTypeOptions.AllowCount)
            {
                operation.Parameters.Add(new OpenApiParameter
                {
                    Name = "$count",
                    In = ParameterLocation.Query,
                    Schema = new OpenApiSchema {Type = "boolean"},
                    Description = "Whether to include the total number of items in the result set before paging",
                });
            }

            if (itemTypeOptions.AllowFilter)
            {
                operation.Parameters.Add(new OpenApiParameter
                {
                    Name = "$filter",
                    In = ParameterLocation.Query,
                    Schema = new OpenApiSchema {Type = "string"},
                    Description =
                        "A filter to select only a subset of the overall results using the OData 4.0 Syntax (http://docs.oasis-open.org/odata/odata/v4.01/odata-v4.01-part1-protocol.html#_Toc31358948)"
                });
            }

            if (itemTypeOptions.AllowSearch)
            {
                operation.Parameters.Add(new OpenApiParameter
                {
                    Name = "$search",
                    In = ParameterLocation.Query,
                    Schema = new OpenApiSchema {Type = "string"},
                    Description =
                        "A global text filter applied to multiple properties to select only a subset of the overall results"
                });
            }

            if (itemTypeOptions.AllowOrderBy)
            {
                operation.Parameters.Add(new OpenApiParameter
                {
                    Name = "$orderby",
                    In = ParameterLocation.Query,
                    Schema = new OpenApiSchema {Type = "string"},
                    Description =
                        "A comma separated list of properties to sort the result set using the OData 4.0 Syntax (http://docs.oasis-open.org/odata/odata/v4.01/odata-v4.01-part1-protocol.html#_Toc31358952)"
                });
            }

            if (itemTypeOptions.AllowSkip)
            {
                operation.Parameters.Add(new OpenApiParameter
                {
                    Name = "$skip",
                    In = ParameterLocation.Query,
                    Schema = new OpenApiSchema {Type = "integer"},
                    Description = "The number of items to skip for paging"
                });
            }

            if (itemTypeOptions.AllowSkipToken)
            {
                operation.Parameters.Add(new OpenApiParameter
                {
                    Name = "$skiptoken",
                    In = ParameterLocation.Query,
                    Schema = new OpenApiSchema {Type = "string"},
                    Description = "The number of items to skip for paging"
                });
            }

            if (itemTypeOptions.AllowTop)
            {
                operation.Parameters.Add(new OpenApiParameter
                {
                    Name = "$top",
                    In = ParameterLocation.Query,
                    Schema = new OpenApiSchema {Type = "integer"},
                    Description = "The number of elements to include from the result set for paging"
                });
            }
        }
    }
}
