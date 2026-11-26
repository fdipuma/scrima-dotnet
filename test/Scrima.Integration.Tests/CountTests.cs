using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Scrima.Integration.Sample.Controllers;
using Scrima.Integration.Sample.Models;
using Scrima.Integration.Tests.Initializers;
using Scrima.Integration.Tests.Utility;
using Shouldly;
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

        var response = await client.GetQueryAsync<User>("/users", cancellationToken: TestContext.Current.CancellationToken);
            
        response.Count.ShouldBeNull();
        response.Results.Count.ShouldBe(TestUserCount);
    }
        
    [Fact]
    public async Task Should_Override_DefaultTop_Settings_Using_Attributes()
    {
        const int TestUserCount = 10;
        using var server = SetupSample(Enumerable.Range(1, TestUserCount).Select(i => new User()));
        using var client = server.CreateClient();

        var response = await client.GetQueryAsync<User>("/users/override", cancellationToken: TestContext.Current.CancellationToken);
            
        response.Results.Count.ShouldBe(UsersController.OverriddenDefaultTop);
    }
        
    [Fact]
    public async Task Should_Override_MaxTop_Settings_Using_Attributes()
    {
        const int TestUserCount = 10;
        using var server = SetupSample(Enumerable.Range(1, TestUserCount).Select(i => new User()));
        using var client = server.CreateClient();

        var response = await client.GetQueryAsync<User>("/users/override?$top=10000", cancellationToken: TestContext.Current.CancellationToken);
            
        response.Results.Count.ShouldBe(UsersController.OverriddenMaxTop);
            
        response.Count.ShouldNotBeNull();
    }
        
    [Fact]
    public async Task Should_Override_AlwaysShowCount_Settings_Using_Attributes()
    {
        const int TestUserCount = 10;
        using var server = SetupSample(Enumerable.Range(1, TestUserCount).Select(i => new User()));
        using var client = server.CreateClient();

        var response = await client.GetQueryAsync<User>("/users/override", cancellationToken: TestContext.Current.CancellationToken);
    }

    [Fact]
    public async Task Should_UseLastValue_When_UsedMultipleTimes()
    {
        const int TestUserCount = 10;
        using var server = SetupSample(Enumerable.Range(1, TestUserCount).Select(i => new User()));
        using var client = server.CreateClient();

        var response = await client.GetQueryAsync<User>("/users?$count=false&$count=true", cancellationToken: TestContext.Current.CancellationToken);
            
        response.Count.ShouldBe(TestUserCount);
        response.Results.Count.ShouldBe(TestUserCount);
    }

    [Fact]
    public async Task Should_NotContainCount_When_FalseInQuery()
    {
        const int TestUserCount = 10;
        using var server = SetupSample(Enumerable.Range(1, TestUserCount).Select(i => new User()));
        using var client = server.CreateClient();

        var response = await client.GetQueryAsync<User>("/users?$count=false", cancellationToken: TestContext.Current.CancellationToken);

        response.Count.ShouldBeNull();
        response.Results.Count.ShouldBe(TestUserCount);
    }

    [Fact]
    public async Task Should_ReturnBadRequest_When_InvalidValueInCount()
    {
        const int TestUserCount = 10;
        using var server = SetupSample(Enumerable.Range(1, TestUserCount).Select(i => new User()));
        using var client = server.CreateClient();

        using var response = await client.GetAsync("/users?$count=myvalue", TestContext.Current.CancellationToken);
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Should_ContainCount_When_TrueInQuery()
    {
        const int TestUserCount = 10;
        using var server = SetupSample(Enumerable.Range(1, TestUserCount).Select(i => new User()));
        using var client = server.CreateClient();

        var response = await client.GetQueryAsync<User>("/users?$count=true", cancellationToken: TestContext.Current.CancellationToken);

        response.Count.ShouldBe(TestUserCount);
        response.Results.Count.ShouldBe(TestUserCount);
    }

    [Fact]
    public async Task Should_ContainTotalValue_When_Paging()
    {
        const int TestUserCount = 10;
        const int SelectCount = 1;
        using var server = SetupSample(Enumerable.Range(1, TestUserCount).Select(i => new User()));
        using var client = server.CreateClient();

        var response = await client.GetQueryAsync<User>($"/users?$count=true&$skip=1&$top={SelectCount}", cancellationToken: TestContext.Current.CancellationToken);
            
        response.Count.ShouldBe(TestUserCount);
        response.Results.ShouldHaveSingleItem();
    }

    [Fact]
    public async Task Should_ContainFilteredValue_When_Filtering()
    {
        const int TestUserCount = 10;
        const int FilterCount = 5;
        using var server = SetupSample(Enumerable.Range(1, TestUserCount).Select(i => new User()));
        using var client = server.CreateClient();

        var response = await client.GetQueryAsync<User>($"/users?$count=true&$filter=Id le {FilterCount}", cancellationToken: TestContext.Current.CancellationToken);
            
        response.Count.ShouldBe(FilterCount);
        response.Results.Count.ShouldBe(FilterCount);
    }

    [Fact]
    public async Task Should_ContainFilteredValue_When_FilteringAndPaging()
    {
        const int TestUserCount = 10;
        const int SelectCount = 1;
        const int FilterCount = 5;
        using var server = SetupSample(Enumerable.Range(1, TestUserCount).Select(i => new User()));
        using var client = server.CreateClient();

        var response = await client.GetQueryAsync<User>($"/users?$count=true&$skip=1&$top={SelectCount}&$filter=Id le {FilterCount}", cancellationToken: TestContext.Current.CancellationToken);
            
        response.Count.ShouldBe(FilterCount);
        response.Results.ShouldHaveSingleItem();
    }
}

public class InMemoryCountTests : CountTests<InMemoryServicesInit> { }
public class SqliteCountTests : CountTests<SqliteServicesInit> { }
