using System;
﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Scrima.Integration.Sample.Data;
using Scrima.Integration.Sample.Models;
using Scrima.Integration.Tests.Initializers;

namespace Scrima.Integration.Tests;

public abstract class IntegrationTestBase<TInit> : IAsyncDisposable where TInit : ServicesInitBase, new()
{
    // Disposing the TestServer doesn't stop the host: the factory owns it and is disposed after each test
    private CustomWebApplicationFactory<TInit, Sample.Program> _factory;

    protected TestServer SetupSample(IEnumerable<User> testdata = null)
    {
        var initializer = new TInit();

        _factory = new CustomWebApplicationFactory<TInit, Sample.Program>(initializer);
        var server = _factory.Server;

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

    public async ValueTask DisposeAsync()
    {
        if (_factory is not null)
        {
            await _factory.DisposeAsync();
        }
    }
}
