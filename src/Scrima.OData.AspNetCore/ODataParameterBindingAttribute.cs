using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Scrima.OData.AspNetCore
{
    internal class QueryOptionsModelBinder : IModelBinder
    {
        private readonly IODataRawQueryParser _parser;
        private readonly ILogger<QueryOptionsModelBinder> _logger;
        private readonly ODataQueryDefaultOptions _defaultOptions;

        public QueryOptionsModelBinder(IODataRawQueryParser parser, ILogger<QueryOptionsModelBinder> logger, IOptions<ODataQueryDefaultOptions> options)
        {
            _parser = parser;
            _logger = logger;
            _defaultOptions = options.Value;
        }

        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null) throw new ArgumentNullException(nameof(bindingContext));

            if (!bindingContext.ModelType.IsQueryOptions()) return Task.CompletedTask;

            try
            {
                var rawQuery = QueryCollectionHelper.CreateODataRawQueryOptions(bindingContext.HttpContext.Request.Query);
                
                var queryModelType = bindingContext.ModelType.GetGenericArguments().First();

                var queryOptions = _parser.ParseOptions(queryModelType, rawQuery, _defaultOptions);

                bindingContext.Result = ModelBindingResult.Success(queryOptions);
            }
            catch (ODataParseException ex)
            {
                bindingContext.ModelState.AddModelError(bindingContext.ModelName, ex.Message);
                _logger.LogWarning(ex, "Parsing error for OData query");
                bindingContext.Result = ModelBindingResult.Failed();
            }

            return Task.CompletedTask;
        }
    }
}
