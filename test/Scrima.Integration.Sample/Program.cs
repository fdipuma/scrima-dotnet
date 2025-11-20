using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi;
using Scrima.Integration.Sample.Models;
using Scrima.OData.AspNetCore;
using Scrima.OData.Swashbuckle;

namespace Scrima.Integration.Sample;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddMvc();
        builder.Services.AddODataQuery();
        builder.Services.AddSwaggerGen(s =>
        {
            s.SwaggerDoc("odata", new OpenApiInfo { Title = "odata test", Version = "v1" });
            s.AddScrimaOData(p =>
            {
                p.ConfigureOptionsPerType = (options, type) =>
                {
                    options.AllowSkipToken = type == typeof(BlogPost);
                };
            });
        });

        var app = builder.Build();

        app.UseSwagger(options =>
        {
            options.OpenApiVersion = OpenApiSpecVersion.OpenApi3_1;
        });
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/odata/swagger.json", "OData V1");
        });

        app.UseRouting();
        app.MapControllers();
        app.MapGet("/", () => "Hello World!");

        await app.RunAsync();
    }
}
