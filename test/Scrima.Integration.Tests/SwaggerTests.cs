using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.OpenApi;
using Scrima.Integration.Tests.Initializers;
using Shouldly;
using Xunit;

namespace Scrima.Integration.Tests;

public class SwaggerTests : IntegrationTestBase<InMemoryServicesInit>
{
    [Fact]
    public async Task Should_ReturnValidSwaggerSchema()
    {
        using var server = SetupSample();
        using var client = server.CreateClient();

        using var response = await client.GetAsync("/swagger/odata/swagger.json", TestContext.Current.CancellationToken);
        var schemaText = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        schemaText.ShouldNotBeNull();

        var (document, diagnostic) = OpenApiDocument.Parse(schemaText, OpenApiConstants.Json);

        document.ShouldNotBeNull();
        diagnostic.Errors.ShouldBeEmpty();
    }
        
    [Fact]
    public async Task Should_ReturnSplittedParameters_When_CheckingBlogPath()
    {
        using var server = SetupSample();
        using var client = server.CreateClient();

        using var response = await client.GetAsync("/swagger/odata/swagger.json", TestContext.Current.CancellationToken);
        var schemaText = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);

        var (document, diagnostic) = OpenApiDocument.Parse(schemaText, OpenApiConstants.Json);

        var (_, operation) = document.Paths["/Blogs"].Operations.First();

        operation.Parameters.Count.ShouldBe(6);
        operation.Parameters.ShouldContain(p => p.Name == "$filter");
        operation.Parameters.ShouldContain(p => p.Name == "$count");
        operation.Parameters.ShouldContain(p => p.Name == "$search");
        operation.Parameters.ShouldContain(p => p.Name == "$skip");
        operation.Parameters.ShouldContain(p => p.Name == "$top");
        operation.Parameters.ShouldContain(p => p.Name == "$orderby");
    }
    
    [Fact]
    public async Task Should_ReturnSplittedParametersWithSkipToken_When_CheckingBlogPostPath()
    {
        using var server = SetupSample();
        using var client = server.CreateClient();

        using var response = await client.GetAsync("/swagger/odata/swagger.json", TestContext.Current.CancellationToken);
        var schemaText = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);

        var (document, diagnostic) = OpenApiDocument.Parse(schemaText, OpenApiConstants.Json);

        var (_, operation) = document.Paths["/BlogPosts"].Operations.First();

        operation.Parameters.Count.ShouldBe(7);
        operation.Parameters.ShouldContain(p => p.Name == "$filter");
        operation.Parameters.ShouldContain(p => p.Name == "$count");
        operation.Parameters.ShouldContain(p => p.Name == "$search");
        operation.Parameters.ShouldContain(p => p.Name == "$skip");
        operation.Parameters.ShouldContain(p => p.Name == "$top");
        operation.Parameters.ShouldContain(p => p.Name == "$orderby");
        operation.Parameters.ShouldContain(p => p.Name == "$skiptoken");
    }
    
    [Fact]
    public async Task Should_ReturnOnlyResponseSchemas()
    {
        using var server = SetupSample();
        using var client = server.CreateClient();

        using var response = await client.GetAsync("/swagger/odata/swagger.json", TestContext.Current.CancellationToken);
        var schemaText = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);

        var (document, diagnostic) = OpenApiDocument.Parse(schemaText, OpenApiConstants.Json);

        document.Components.Schemas.Count.ShouldBe(7);
    }
}
