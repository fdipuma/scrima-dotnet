using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Scrima.Integration.Sample.Data;

namespace Scrima.Integration.Tests.Initializers;

public class SqliteServicesInit : ServicesInitBase
{
    private readonly SqliteConnection _connection;

    public SqliteServicesInit()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();
    }

    public override void ConfigureServices(IServiceCollection collection)
    {
        collection.AddDbContext<BlogDbContext>(o => o.UseSqlite(_connection));
    }
}
