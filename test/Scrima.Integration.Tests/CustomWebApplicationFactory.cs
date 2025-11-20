using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Hosting;
using Scrima.Integration.Tests.Initializers;

namespace Scrima.Integration.Tests;

public class CustomWebApplicationFactory<TInit, TProgram>(TInit initializer)
    : WebApplicationFactory<TProgram>
        where TProgram : class
        where TInit : ServicesInitBase, new()
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");

        builder.ConfigureTestServices(services =>
        {
            initializer.ConfigureServices(services);
        });
    }
}
