using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi;
using Scrima.Integration.Sample.Data;
using Scrima.OData.Swashbuckle;
using Shouldly;
using Swashbuckle.AspNetCore.SwaggerGen;
using Xunit;

namespace Scrima.Integration.Tests;

public class EntityFieldsSwaggerTests : IAsyncDisposable
{
    private WebApplicationFactory<Sample.Program> _factory;

    private TestServer SetupWithEntityFields(Action<EntityFieldsOptions> configure)
    {
        _factory = new WebApplicationFactory<Sample.Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.UseEnvironment("Development");
                builder.ConfigureTestServices(services =>
                {
                    var dbName = "Blog_" + Guid.NewGuid();
                    services.AddDbContext<BlogDbContext>(o => o.UseInMemoryDatabase(dbName));

                    services.PostConfigure<SwaggerGenOptions>(swaggerOptions =>
                    {
                        swaggerOptions.OperationFilterDescriptors.RemoveAll(
                            d => d.Type == typeof(EntityPropertiesOperationFilter));

                        var scrimaOptions = new ScrimaSwaggerOptions();
                        configure(scrimaOptions.EntityFields);

                        swaggerOptions.OperationFilterDescriptors.Add(new FilterDescriptor
                        {
                            Type = typeof(EntityPropertiesOperationFilter),
                            Arguments = [scrimaOptions]
                        });
                    });
                });
            });

        return _factory.Server;
    }

    public async ValueTask DisposeAsync()
    {
        if (_factory is not null)
        {
            await _factory.DisposeAsync();
        }
    }

    private static async Task<OpenApiDocument> GetSwaggerDocument(TestServer server)
    {
        using var client = server.CreateClient();
        using var response = await client.GetAsync("/swagger/odata/swagger.json");
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var schemaText = await response.Content.ReadAsStringAsync();
        var (document, diagnostic) = OpenApiDocument.Parse(schemaText, OpenApiConstants.Json);
        document.ShouldNotBeNull();
        diagnostic.Errors.ShouldBeEmpty();
        return document;
    }

    [Fact]
    public async Task Should_NotEnrichParameters_When_EntityFieldsDisabled()
    {
        using var server = SetupWithEntityFields(opts =>
        {
            opts.Show = ShowEntityFieldsOptions.None;
            opts.ExposeAsExtensions = false;
        });

        var document = await GetSwaggerDocument(server);
        var (_, operation) = document.Paths["/Blogs"].Operations.First();

        var filterParam = operation.Parameters.First(p => p.Name == "$filter");
        var orderByParam = operation.Parameters.First(p => p.Name == "$orderby");

        filterParam.Description.ShouldNotContain("Filterable properties", Case.Sensitive);
        orderByParam.Description.ShouldNotContain("Orderable properties", Case.Sensitive);

        var extensions = operation.Extensions;
        (extensions?.ContainsKey("x-odata-filterable-properties") ?? false).ShouldBeFalse();
        (extensions?.ContainsKey("x-odata-orderable-properties") ?? false).ShouldBeFalse();
        (extensions?.ContainsKey("x-odata-entity") ?? false).ShouldBeFalse();
    }

    [Fact]
    public async Task Should_EnrichFilterAndOrderBy_When_OnAllODataFields()
    {
        using var server = SetupWithEntityFields(opts =>
        {
            opts.Show = ShowEntityFieldsOptions.OnAllODataFields;
        });

        var document = await GetSwaggerDocument(server);
        var (_, operation) = document.Paths["/Blogs"].Operations.First();

        var filterParam = operation.Parameters.First(p => p.Name == "$filter");
        filterParam.Description.ShouldContain("Filterable properties:", Case.Sensitive);
        filterParam.Description.ShouldContain("Id (integer)", Case.Sensitive);
        filterParam.Description.ShouldContain("Name (string)", Case.Sensitive);
        filterParam.Description.ShouldContain("Description (string)", Case.Sensitive);
        filterParam.Description.ShouldContain("OwnerId (integer)", Case.Sensitive);

        var orderByParam = operation.Parameters.First(p => p.Name == "$orderby");
        orderByParam.Description.ShouldContain("Orderable properties:", Case.Sensitive);
        orderByParam.Description.ShouldContain("Id (integer)", Case.Sensitive);
        orderByParam.Description.ShouldContain("Name (string)", Case.Sensitive);

        operation.Extensions.ShouldNotBeNull();
        operation.Extensions.ShouldContainKey("x-odata-filterable-properties");
        operation.Extensions.ShouldContainKey("x-odata-orderable-properties");
        operation.Extensions.ShouldContainKey("x-odata-entity");
    }

    [Fact]
    public async Task Should_OnlyEnrichFilter_When_ShowOnFilterOnly()
    {
        using var server = SetupWithEntityFields(opts =>
        {
            opts.Show = ShowEntityFieldsOptions.OnFilter;
        });

        var document = await GetSwaggerDocument(server);
        var (_, operation) = document.Paths["/Blogs"].Operations.First();

        var filterParam = operation.Parameters.First(p => p.Name == "$filter");
        filterParam.Description.ShouldContain("Filterable properties:", Case.Sensitive);

        var orderByParam = operation.Parameters.First(p => p.Name == "$orderby");
        orderByParam.Description.ShouldNotContain("Orderable properties", Case.Sensitive);

        operation.Extensions.ShouldNotBeNull();
        operation.Extensions.ShouldContainKey("x-odata-filterable-properties");
        operation.Extensions.Keys.ShouldNotContain("x-odata-orderable-properties");
    }

    [Fact]
    public async Task Should_NotExposeEntityExtension_When_ExposeAsExtensionsFalse()
    {
        using var server = SetupWithEntityFields(opts =>
        {
            opts.Show = ShowEntityFieldsOptions.OnAllODataFields;
            opts.ExposeAsExtensions = false;
        });

        var document = await GetSwaggerDocument(server);
        var (_, operation) = document.Paths["/Blogs"].Operations.First();

        var filterParam = operation.Parameters.First(p => p.Name == "$filter");
        filterParam.Description.ShouldContain("Filterable properties:", Case.Sensitive);

        operation.Extensions.ShouldNotBeNull();
        operation.Extensions.ShouldContainKey("x-odata-filterable-properties");
        operation.Extensions.ShouldContainKey("x-odata-orderable-properties");
        operation.Extensions.Keys.ShouldNotContain("x-odata-entity");
    }
}
