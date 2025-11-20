using System.Collections.Generic;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Scrima.Integration.Sample.Data;
using Scrima.Integration.Sample.Models;
using Scrima.Integration.Tests.Initializers;

namespace Scrima.Integration.Tests;

public abstract class IntegrationTestBase<TInit> where TInit : ServicesInitBase, new()
{
    protected static TestServer SetupSample(IEnumerable<User> testdata = null)
    {
        var initializer = new TInit();

        var factory = new CustomWebApplicationFactory<TInit, Sample.Program>(initializer);
        var server = factory.Server;

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
