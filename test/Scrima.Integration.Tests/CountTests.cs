using System.Linq;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Scrima.Integration.Sample.Controllers;
using Scrima.Integration.Sample.Models;
using Scrima.Integration.Tests.Initializers;
using Scrima.Integration.Tests.Utility;
using Xunit;

namespace Scrima.Integration.Tests;

public abstract class CountTests<TInit> : IntegrationTestBase<TInit> where TInit : ServicesInitBase, new()
{
    [Fact]
    public async Task Should_NotContainCount_When_NotSpecified()
    {
        const int TestUserCount = 10;
        using var server = SetupSample(Enumerable.Range(1, TestUserCount).Select(i => new User()));
        using var client = server.CreateClient();

        var response = await client.GetQueryAsync<User>("/users");
            
        response.Count.Should().BeNull();
        response.Results.Should().HaveCount(TestUserCount);
    }
        
    [Fact]
    public async Task Should_Override_DefaultTop_Settings_Using_Attributes()
    {
        const int TestUserCount = 10;
        using var server = SetupSample(Enumerable.Range(1, TestUserCount).Select(i => new User()));
        using var client = server.CreateClient();

        var response = await client.GetQueryAsync<User>("/users/override");
            
        response.Results.Should().HaveCount(UsersController.OverriddenDefaultTop);
    }
        
    [Fact]
    public async Task Should_Override_MaxTop_Settings_Using_Attributes()
    {
        const int TestUserCount = 10;
        using var server = SetupSample(Enumerable.Range(1, TestUserCount).Select(i => new User()));
        using var client = server.CreateClient();

        var response = await client.GetQueryAsync<User>("/users/override?$top=10000");
            
        response.Results.Should().HaveCount(UsersController.OverriddenMaxTop);
            
        response.Count.Should().NotBeNull();
    }
        
    [Fact]
    public async Task Should_Override_AlwaysShowCount_Settings_Using_Attributes()
    {
        const int TestUserCount = 10;
        using var server = SetupSample(Enumerable.Range(1, TestUserCount).Select(i => new User()));
        using var client = server.CreateClient();

        var response = await client.GetQueryAsync<User>("/users/override");
    }

    [Fact]
    public async Task Should_UseLastValue_When_UsedMultipleTimes()
    {
        const int TestUserCount = 10;
        using var server = SetupSample(Enumerable.Range(1, TestUserCount).Select(i => new User()));
        using var client = server.CreateClient();

        var response = await client.GetQueryAsync<User>("/users?$count=false&$count=true");
            
        response.Count.Should().Be(TestUserCount);
        response.Results.Should().HaveCount(TestUserCount);
    }

    [Fact]
    public async Task Should_NotContainCount_When_FalseInQuery()
    {
        const int TestUserCount = 10;
        using var server = SetupSample(Enumerable.Range(1, TestUserCount).Select(i => new User()));
        using var client = server.CreateClient();

        var response = await client.GetQueryAsync<User>("/users?$count=false");

        response.Count.Should().BeNull();
        response.Results.Should().HaveCount(TestUserCount);
    }

    [Fact]
    public async Task Should_ReturnBadRequest_When_InvalidValueInCount()
    {
        const int TestUserCount = 10;
        using var server = SetupSample(Enumerable.Range(1, TestUserCount).Select(i => new User()));
        using var client = server.CreateClient();

        using var response = await client.GetAsync("/users?$count=myvalue");
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Should_ContainCount_When_TrueInQuery()
    {
        const int TestUserCount = 10;
        using var server = SetupSample(Enumerable.Range(1, TestUserCount).Select(i => new User()));
        using var client = server.CreateClient();

        var response = await client.GetQueryAsync<User>("/users?$count=true");

        response.Count.Should().Be(TestUserCount);
        response.Results.Should().HaveCount(TestUserCount);
    }

    [Fact]
    public async Task Should_ContainTotalValue_When_Paging()
    {
        const int TestUserCount = 10;
        const int SelectCount = 1;
        using var server = SetupSample(Enumerable.Range(1, TestUserCount).Select(i => new User()));
        using var client = server.CreateClient();

        var response = await client.GetQueryAsync<User>($"/users?$count=true&$skip=1&$top={SelectCount}");
            
        response.Count.Should().Be(TestUserCount);
        response.Results.Should().ContainSingle();
    }

    [Fact]
    public async Task Should_ContainFilteredValue_When_Filtering()
    {
        const int TestUserCount = 10;
        const int FilterCount = 5;
        using var server = SetupSample(Enumerable.Range(1, TestUserCount).Select(i => new User()));
        using var client = server.CreateClient();

        var response = await client.GetQueryAsync<User>($"/users?$count=true&$filter=Id le {FilterCount}");
            
        response.Count.Should().Be(FilterCount);
        response.Results.Should().HaveCount(FilterCount);
    }

    [Fact]
    public async Task Should_ContainFilteredValue_When_FilteringAndPaging()
    {
        const int TestUserCount = 10;
        const int SelectCount = 1;
        const int FilterCount = 5;
        using var server = SetupSample(Enumerable.Range(1, TestUserCount).Select(i => new User()));
        using var client = server.CreateClient();

        var response = await client.GetQueryAsync<User>($"/users?$count=true&$skip=1&$top={SelectCount}&$filter=Id le {FilterCount}");
            
        response.Count.Should().Be(FilterCount);
        response.Results.Should().ContainSingle();
    }
}

public class InMemoryCountTests : CountTests<InMemoryServicesInit> { }
public class SqliteCountTests : CountTests<SqliteServicesInit> { }
