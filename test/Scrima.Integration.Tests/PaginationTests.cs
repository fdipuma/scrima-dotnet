using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Scrima.Integration.Sample.Models;
using Scrima.Integration.Tests.Initializers;
using Scrima.Integration.Tests.Utility;
using Xunit;

namespace Scrima.Integration.Tests;

public abstract class PaginationTests<TInit> : IntegrationTestBase<TInit> where TInit : ServicesInitBase, new()
{
    [Fact]
    public async Task Should_UseLastSkipValue_When_UsedMultipleTimes()
    {
        const int TestUserCount = 10;
        const int SkipCount = 2;
        using var server = SetupSample(Enumerable.Range(1, TestUserCount).Select(i => new User()));
        using var client = server.CreateClient();

        var response = await client.GetQueryAsync<User>($"/users?$skip=1&$skip={SkipCount}");

        response.Results.Should().HaveCount(TestUserCount - SkipCount);
    }

    [Fact]
    public async Task Should_ReturnRest_When_Skip_InRange()
    {
        const int TestUserCount = 10;
        const int SkipCount = 3;
        using var server = SetupSample(Enumerable.Range(1, TestUserCount).Select(i => new User()));
        using var client = server.CreateClient();

        var response = await client.GetQueryAsync<User>($"/users?$skip={SkipCount}");
        
        response.Results.Should().HaveCount(TestUserCount - SkipCount);
    }

    [Fact]
    public async Task Should_ReturnEmpty_When_Skip_OutOfRange()
    {
        const int TestUserCount = 10;
        const int SkipCount = 11;
        using var server = SetupSample(Enumerable.Range(1, TestUserCount).Select(i => new User()));
        using var client = server.CreateClient();

        var response = await client.GetQueryAsync<User>($"/users?$skip={SkipCount}");
        
        response.Results.Should().BeEmpty();
    }

    [Fact]
    public async Task Should_ReturnTop_When_Top_InRange()
    {
        const int TestUserCount = 10;
        const int TopCount = 3;
        using var server = SetupSample(Enumerable.Range(1, TestUserCount).Select(i => new User()));
        using var client = server.CreateClient();

        var response = await client.GetQueryAsync<User>($"/users?$top={TopCount}");
        
        response.Results.Should().HaveCount(TopCount);
    }

    [Fact]
    public async Task Should_ReturnAll_When_Top_OutOfRange()
    {
        const int TestUserCount = 10;
        const int SkipCount = 20;
        using var server = SetupSample(Enumerable.Range(1, TestUserCount).Select(i => new User()));
        using var client = server.CreateClient();

        var response = await client.GetQueryAsync<User>($"/users?$top={SkipCount}");
        
        response.Results.Should().HaveCount(TestUserCount);
    }

    [Fact]
    public async Task Should_UseLastTopValue_When_UsedMultipleTimes()
    {
        const int TestUserCount = 10;
        const int TopCount = 3;
        using var server = SetupSample(Enumerable.Range(1, TestUserCount).Select(i => new User()));
        using var client = server.CreateClient();

        var response = await client.GetQueryAsync<User>($"/users?$top=1&$top={TopCount}");
        
        response.Results.Should().HaveCount(TopCount);
    }

    [Fact]
    public async Task Should_ReturnItems_When_SkipTop_InRange()
    {
        const int TestUserCount = 10;
        const int SkipCount = 2;
        const int TopCount = 2;
        using var server = SetupSample(Enumerable.Range(1, TestUserCount).Select(i => new User()));
        using var client = server.CreateClient();

        var response = await client.GetQueryAsync<User>($"/users?$skip={SkipCount}&$top={TopCount}");
        
        response.Results.Should().HaveCount(TopCount);
    }

    [Fact]
    public async Task Should_ReturnRest_When_SkipTop_OutOfRange()
    {
        const int TestUserCount = 10;
        const int SkipCount = 5;
        const int TopCount = 10;
        using var server = SetupSample(Enumerable.Range(1, TestUserCount).Select(i => new User()));
        using var client = server.CreateClient();

        var response = await client.GetQueryAsync<User>($"/users?$skip={SkipCount}&$top={TopCount}");
        
        response.Results.Should().HaveCount(5);
    }

    [Fact]
    public async Task Should_ReturnEmpty_When_SkipTop_OutOfRange()
    {
        const int TestUserCount = 10;
        const int SkipCount = 20;
        const int TopCount = 10;
        using var server = SetupSample(Enumerable.Range(1, TestUserCount).Select(i => new User()));
        using var client = server.CreateClient();

        var response = await client.GetQueryAsync<User>($"/users?$skip={SkipCount}&$top={TopCount}");
        
        response.Results.Should().BeEmpty();
    }
}
    
public class InMemoryPaginationTests : PaginationTests<InMemoryServicesInit> { }
public class SqlitePaginationTests : PaginationTests<SqliteServicesInit> { }
