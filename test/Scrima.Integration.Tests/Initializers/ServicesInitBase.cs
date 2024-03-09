using System;
using Microsoft.Extensions.DependencyInjection;

namespace Scrima.Integration.Tests.Initializers;

public abstract class ServicesInitBase
{
    public abstract void ConfigureServices(IServiceCollection collection);

    public virtual void OnStop(IServiceProvider provider)
    {
        
    }
}
