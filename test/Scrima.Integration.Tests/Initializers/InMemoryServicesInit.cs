using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Scrima.Integration.Sample.Data;

namespace Scrima.Integration.Tests.Initializers;

public class InMemoryServicesInit : ServicesInitBase
{
    private readonly string _databaseName;

    public InMemoryServicesInit()
    {
        _databaseName = "Blog_" + Guid.NewGuid();
    }

    public override void ConfigureServices(IServiceCollection collection)
    {
        collection.AddDbContext<BlogDbContext>(o => o.UseInMemoryDatabase(_databaseName));
    }
}
