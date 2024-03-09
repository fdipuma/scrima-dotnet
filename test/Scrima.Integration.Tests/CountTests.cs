using System.Linq;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Scrima.Integration.Tests.Initializers;
using Scrima.Integration.Tests.Models;
using Scrima.Integration.Tests.Utility;
using Xunit;

namespace Scrima.Integration.Tests;

public abstract class CountTests<TInit> : IntegrationTestBase<TInit> where TInit : ServicesInitBase, new()
{
    [Fact]
    public async Task Should_NotContainCount_When_NotSpecified()
    {
        const int testUserCount = 10;
        using var server = SetupSample(Enumerable.Range(1, testUserCount).Select(i => new User()));
        using var client = server.CreateClient();

        var response = await client.GetQueryAsync<User>("/users");
        
        response.Count.Should().BeNull();
        response.Results.Should().HaveCount(testUserCount);
    }

    [Fact]
    public async Task Should_UseLastValue_When_UsedMultipleTimes()
    {
        const int testUserCount = 10;
        using var server = SetupSample(Enumerable.Range(1, testUserCount).Select(i => new User()));
        using var client = server.CreateClient();

        var response = await client.GetQueryAsync<User>("/users?$count=false&$count=true");
        
        response.Count.Should().Be(testUserCount);
        response.Results.Should().HaveCount(testUserCount);
    }

    [Fact]
    public async Task Should_NotContainCount_When_FalseInQuery()
    {
        const int testUserCount = 10;
        using var server = SetupSample(Enumerable.Range(1, testUserCount).Select(i => new User()));
        using var client = server.CreateClient();

        var response = await client.GetQueryAsync<User>("/users?$count=false");

        response.Count.Should().BeNull();
        response.Results.Should().HaveCount(testUserCount);
    }

    [Fact]
    public async Task Should_ReturnBadRequest_When_InvalidValueInCount()
    {
        const int testUserCount = 10;
        using var server = SetupSample(Enumerable.Range(1, testUserCount).Select(i => new User()));
        using var client = server.CreateClient();

        using var response = await client.GetAsync("/users?$count=myvalue");
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Should_ContainCount_When_TrueInQuery()
    {
        const int testUserCount = 10;
        using var server = SetupSample(Enumerable.Range(1, testUserCount).Select(i => new User()));
        using var client = server.CreateClient();

        var response = await client.GetQueryAsync<User>("/users?$count=true");

        response.Count.Should().Be(testUserCount);
        response.Results.Should().HaveCount(testUserCount);
    }

    [Fact]
    public async Task Should_ContainTotalValue_When_Paging()
    {
        const int testUserCount = 10;
        const int selectCount = 1;
        using var server = SetupSample(Enumerable.Range(1, testUserCount).Select(i => new User()));
        using var client = server.CreateClient();

        var response = await client.GetQueryAsync<User>($"/users?$count=true&$skip=1&$top={selectCount}");
        
        response.Count.Should().Be(testUserCount);
        response.Results.Should().ContainSingle();
    }

    [Fact]
    public async Task Should_ContainFilteredValue_When_Filtering()
    {
        const int testUserCount = 10;
        const int filterCount = 5;
        using var server = SetupSample(Enumerable.Range(1, testUserCount).Select(i => new User()));
        using var client = server.CreateClient();

        var response = await client.GetQueryAsync<User>($"/users?$count=true&$filter=Id le {filterCount}");
        
        response.Count.Should().Be(filterCount);
        response.Results.Should().HaveCount(filterCount);
    }

    [Fact]
    public async Task Should_ContainFilteredValue_When_FilteringAndPaging()
    {
        const int testUserCount = 10;
        const int selectCount = 1;
        const int filterCount = 5;
        using var server = SetupSample(Enumerable.Range(1, testUserCount).Select(i => new User()));
        using var client = server.CreateClient();

        var response = await client.GetQueryAsync<User>($"/users?$count=true&$skip=1&$top={selectCount}&$filter=Id le {filterCount}");
        
        response.Count.Should().Be(filterCount);
        response.Results.Should().ContainSingle();
    }
}

public class InMemoryCountTests : CountTests<InMemoryServicesInit> { }
public class SqliteCountTests : CountTests<SqliteServicesInit> { }
