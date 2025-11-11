using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Scrima.OData.AspNetCore;

internal class ODataQueryModelBinder : IModelBinder
{
    private readonly IODataRawQueryParser _parser;
    private readonly ILogger<ODataQueryModelBinder> _logger;
    private readonly ODataQueryDefaultOptions _defaultOptions;

    public ODataQueryModelBinder(IODataRawQueryParser parser, ILogger<ODataQueryModelBinder> logger, IOptions<ODataQueryDefaultOptions> options)
    {
            _parser = parser;
            _logger = logger;
            _defaultOptions = options.Value;
    }

    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
            if (bindingContext == null) throw new ArgumentNullException(nameof(bindingContext));

            if (!bindingContext.ModelType.IsODataQuery()) return Task.CompletedTask;

            try
            {
                var rawQuery = QueryCollectionHelper.CreateODataRawQueryOptions(bindingContext.HttpContext.Request.Query);
                
                var queryModelType = bindingContext.ModelType.GetGenericArguments().First();

                var odataQueryDefaultOptions = BuildODataQueryDefaultOptions(bindingContext);

                var queryOptions = _parser.ParseOptions(queryModelType, rawQuery, odataQueryDefaultOptions);
                
                var odataQuery = (ODataQuery)Activator.CreateInstance(typeof(ODataQuery<>).MakeGenericType(queryModelType));

                if (odataQuery != null)
                {
                    odataQuery.QueryOptions = queryOptions;
                    odataQuery.Count = queryOptions.ShowCount;
                    odataQuery.Skip = queryOptions.Skip;
                    odataQuery.Top = queryOptions.Top;

                    odataQuery.Filter = rawQuery.Filter;
                    odataQuery.Search = rawQuery.Search;
                    odataQuery.OrderBy = rawQuery.OrderBy;
                    odataQuery.SkipToken = rawQuery.SkipToken;
                }

                bindingContext.Result = ModelBindingResult.Success(odataQuery);
            }
            catch (ODataParseException ex)
            {
                bindingContext.ModelState.AddModelError(bindingContext.ModelName, ex.Message);
                _logger.LogWarning(ex, "Parsing error for OData query");
                bindingContext.Result = ModelBindingResult.Failed();
            }

            return Task.CompletedTask;
        }

    private ODataQueryDefaultOptions BuildODataQueryDefaultOptions(ModelBindingContext bindingContext)
    {
            if (bindingContext.ModelMetadata is not DefaultModelMetadata metadata) 
                return _defaultOptions;
            
            var optionsAttribute = metadata.Attributes.Attributes
                .OfType<ODataQueryDefaultOptionsAttribute>()
                .FirstOrDefault();

            if (optionsAttribute is null) 
                return _defaultOptions;
            
            return optionsAttribute.BuildOptions(_defaultOptions);
        }
}
