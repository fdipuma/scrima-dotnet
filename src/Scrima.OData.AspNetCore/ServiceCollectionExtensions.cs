using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Scrima.OData.AspNetCore;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Use ODataParser services.
    /// </summary>
    /// <param name="services">The service colliction.</param>
    /// <param name="configureOptions">Optional action used to configure default options</param>
    public static IServiceCollection AddODataQuery(this IServiceCollection services, Action<ODataQueryDefaultOptions> configureOptions = null)
    {
        services.AddSingleton<IODataRawQueryParser, ODataRawRawQueryParser>();

        services.PostConfigure<MvcOptions>(o =>
        {
            o.ModelBinderProviders.Insert(0, new QueryOptionsBinderProvider());
        });

        services.AddOptions<ODataQueryDefaultOptions>();

        if (configureOptions is not null)
        {
            services.Configure(configureOptions);
        }

        services.AddTransient<IActionDescriptorProvider, RemoveQueryOptionsActionDescriptorProvider>();
        
        return services;
    }
}
