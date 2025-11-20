using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Scrima.Integration.Sample.Models;
using Scrima.Integration.Tests.Initializers;
using Scrima.Integration.Tests.Utility;
using Xunit;

namespace Scrima.Integration.Tests;

public abstract class SearchTests<TInit> : IntegrationTestBase<TInit> where TInit : ServicesInitBase, new()
{
    [Fact]
    public async Task Should_ReturnFiltered_When_SearchPartial()
    {
        const int TestUserCount = 10;
        using var server =
            SetupSample(Enumerable.Range(1, TestUserCount).Select(i => new User {Username = $"user{i}"}));
        using var client = server.CreateClient();

        var response = await client.GetQueryAsync<User>("/users?$search=er2");

        response.Results.Should().HaveCount(1);
        response.Results[0].Username.Should().Be("user2");
    }

    [Fact]
    public async Task Should_UseLastValue_When_SearchUsedMultipleTimes()
    {
        const int TestUserCount = 10;
        using var server =
            SetupSample(Enumerable.Range(1, TestUserCount).Select(i => new User {Username = $"user{i}"}));
        using var client = server.CreateClient();

        var response = await client.GetQueryAsync<User>("/users?$search=er3&$search=er2");

        response.Results.Should().HaveCount(1);
        response.Results[0].Username.Should().Be("user2");
    }
}
    
public class InMemorySearchTests : SearchTests<InMemoryServicesInit> { }
public class SqliteSearchTests : SearchTests<SqliteServicesInit> { }
