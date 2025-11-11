using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Scrima.Integration.Sample.Data;

namespace Scrima.Integration.Tests.Initializers;

public class SqlServerServicesInit : ServicesInitBase
{
    private readonly string _databaseName;

    public SqlServerServicesInit()
    {
        _databaseName = Guid.NewGuid() + "_Blog";
    }

    public override void ConfigureServices(IServiceCollection collection)
    {
        collection.AddDbContext<BlogDbContext>(o =>
            o.UseSqlServer(
                $"Server=(localdb)\\mssqllocaldb;Initial Catalog={_databaseName};MultipleActiveResultSets=False;Integrated Security=True"));
    }

    public override void OnStop(IServiceProvider provider)
    {
        var context = provider.GetRequiredService<BlogDbContext>();
        
        context.Database.OpenConnection();
        var dbConn = context.Database.GetDbConnection();

        using var command = dbConn.CreateCommand();

        command.CommandText = $"DROP DATABASE {_databaseName}";

        command.ExecuteNonQuery();
    }
}
