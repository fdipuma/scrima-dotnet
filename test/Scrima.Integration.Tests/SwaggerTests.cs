using System.Linq;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.OpenApi.Readers;
using Scrima.Integration.Tests.Initializers;
using Xunit;

namespace Scrima.Integration.Tests;

public class SwaggerTests : IntegrationTestBase<InMemoryServicesInit>
{
    private readonly OpenApiStringReader _reader = new OpenApiStringReader();

    [Fact]
    public async Task Should_ReturnValidSwaggerSchema()
    {
        using var server = SetupSample();
        using var client = server.CreateClient();

        using var response = await client.GetAsync("/swagger/odata/swagger.json");
        var schemaText = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        schemaText.Should().NotBeNull();

        var document = _reader.Read(schemaText, out var diagnostic);

        document.Should().NotBeNull();
        diagnostic.Errors.Should().BeEmpty();
    }
        
    [Fact]
    public async Task Should_ReturnSplittedParameters_When_CheckingBlogPath()
    {
        using var server = SetupSample();
        using var client = server.CreateClient();

        using var response = await client.GetAsync("/swagger/odata/swagger.json");
        var schemaText = await response.Content.ReadAsStringAsync();
        var document = _reader.Read(schemaText, out var diagnostic);

        var (_, operation) = document.Paths["/Blogs"].Operations.First();

        operation.Parameters.Should().HaveCount(6);
        operation.Parameters.Should().Contain(p => p.Name == "$filter");
        operation.Parameters.Should().Contain(p => p.Name == "$count");
        operation.Parameters.Should().Contain(p => p.Name == "$search");
        operation.Parameters.Should().Contain(p => p.Name == "$skip");
        operation.Parameters.Should().Contain(p => p.Name == "$top");
        operation.Parameters.Should().Contain(p => p.Name == "$orderby");
    }
    
    [Fact]
    public async Task Should_ReturnSplittedParametersWithSkipToken_When_CheckingBlogPostPath()
    {
        using var server = SetupSample();
        using var client = server.CreateClient();

        using var response = await client.GetAsync("/swagger/odata/swagger.json");
        var schemaText = await response.Content.ReadAsStringAsync();
        var document = _reader.Read(schemaText, out var diagnostic);

        var (_, operation) = document.Paths["/BlogPosts"].Operations.First();

        operation.Parameters.Should().HaveCount(7);
        operation.Parameters.Should().Contain(p => p.Name == "$filter");
        operation.Parameters.Should().Contain(p => p.Name == "$count");
        operation.Parameters.Should().Contain(p => p.Name == "$search");
        operation.Parameters.Should().Contain(p => p.Name == "$skip");
        operation.Parameters.Should().Contain(p => p.Name == "$top");
        operation.Parameters.Should().Contain(p => p.Name == "$orderby");
        operation.Parameters.Should().Contain(p => p.Name == "$skiptoken");
    }
    
    [Fact]
    public async Task Should_ReturnOnlyResponseSchemas()
    {
        using var server = SetupSample();
        using var client = server.CreateClient();

        using var response = await client.GetAsync("/swagger/odata/swagger.json");
        var schemaText = await response.Content.ReadAsStringAsync();
        var document = _reader.Read(schemaText, out var diagnostic);

        document.Components.Schemas.Should().HaveCount(7);
    }
}
