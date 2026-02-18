using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi;
using Scrima.Integration.Sample.Data;
using Scrima.OData.Swashbuckle;
using Swashbuckle.AspNetCore.SwaggerGen;
using Xunit;

namespace Scrima.Integration.Tests;

public class EntityFieldsSwaggerTests
{
    private static TestServer SetupWithEntityFields(Action<EntityFieldsOptions> configure)
    {
        var factory = new WebApplicationFactory<Sample.Program>()
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

        return factory.Server;
    }

    private static async Task<OpenApiDocument> GetSwaggerDocument(TestServer server)
    {
        using var client = server.CreateClient();
        using var response = await client.GetAsync("/swagger/odata/swagger.json");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var schemaText = await response.Content.ReadAsStringAsync();
        var (document, diagnostic) = OpenApiDocument.Parse(schemaText, OpenApiConstants.Json);
        document.Should().NotBeNull();
        diagnostic.Errors.Should().BeEmpty();
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

        filterParam.Description.Should().NotContain("Filterable properties");
        orderByParam.Description.Should().NotContain("Orderable properties");

        var extensions = operation.Extensions;
        (extensions?.ContainsKey("x-odata-filterable-properties") ?? false).Should().BeFalse();
        (extensions?.ContainsKey("x-odata-orderable-properties") ?? false).Should().BeFalse();
        (extensions?.ContainsKey("x-odata-entity") ?? false).Should().BeFalse();
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
        filterParam.Description.Should().Contain("Filterable properties:");
        filterParam.Description.Should().Contain("Id (integer)");
        filterParam.Description.Should().Contain("Name (string)");
        filterParam.Description.Should().Contain("Description (string)");
        filterParam.Description.Should().Contain("OwnerId (integer)");

        var orderByParam = operation.Parameters.First(p => p.Name == "$orderby");
        orderByParam.Description.Should().Contain("Orderable properties:");
        orderByParam.Description.Should().Contain("Id (integer)");
        orderByParam.Description.Should().Contain("Name (string)");

        operation.Extensions.Should().NotBeNull();
        operation.Extensions.Should().ContainKey("x-odata-filterable-properties");
        operation.Extensions.Should().ContainKey("x-odata-orderable-properties");
        operation.Extensions.Should().ContainKey("x-odata-entity");
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
        filterParam.Description.Should().Contain("Filterable properties:");

        var orderByParam = operation.Parameters.First(p => p.Name == "$orderby");
        orderByParam.Description.Should().NotContain("Orderable properties");

        operation.Extensions.Should().NotBeNull();
        operation.Extensions.Should().ContainKey("x-odata-filterable-properties");
        operation.Extensions.Keys.Should().NotContain("x-odata-orderable-properties");
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
        filterParam.Description.Should().Contain("Filterable properties:");

        operation.Extensions.Should().NotBeNull();
        operation.Extensions.Should().ContainKey("x-odata-filterable-properties");
        operation.Extensions.Should().ContainKey("x-odata-orderable-properties");
        operation.Extensions.Keys.Should().NotContain("x-odata-entity");
    }
}
