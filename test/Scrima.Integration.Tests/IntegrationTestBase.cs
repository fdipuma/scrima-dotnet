using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Scrima.Integration.Tests.Data;
using Scrima.Integration.Tests.Initializers;
using Scrima.Integration.Tests.Models;
using Scrima.OData.AspNetCore;
using Scrima.OData.Swashbuckle;

namespace Scrima.Integration.Tests;

public abstract class IntegrationTestBase<TInit> where TInit : ServicesInitBase, new()
{
    protected static TestServer SetupSample(
        IEnumerable<User> testdata = null,
        Action<WebHostBuilder> setup = null
    )
    {
        var builder = new WebHostBuilder();
        builder.UseStartup<Startup>();

        var initializer = new TInit();
        builder.ConfigureServices(initializer.ConfigureServices);
        setup?.Invoke(builder);

        var server = new TestServer(builder);
        if (testdata != null)
        {
            using (var scope = server.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<BlogDbContext>();
                context.Database.EnsureCreated();
                context.Users.AddRange(testdata);
                context.SaveChanges();
            }
        }

        var lifetime = server.Services.GetRequiredService<IHostApplicationLifetime>();

        lifetime.ApplicationStopping.Register(() =>
        {
            initializer.OnStop(server.Services);
        });
        
        return server;
    }
}

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers();
        services.AddODataQuery();
        services.AddSwaggerGen(s =>
        {
            s.SwaggerDoc("odata", new OpenApiInfo {Title = "odata test", Version = "1"});
            s.AddScrimaOData(p =>
            {
                p.ConfigureOptionsPerType = (options, type) =>
                {
                    options.AllowSkipToken = type == typeof(BlogPost);
                };
            });
        });
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseRouting();

        app.UseSwagger();

        app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
    }
}
