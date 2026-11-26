using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Scrima.Integration.Sample.Models;
using Scrima.Integration.Tests.Initializers;
using Scrima.Integration.Tests.Utility;
using Shouldly;
using Xunit;

namespace Scrima.Integration.Tests;

public abstract class FilterTests<TInit> : IntegrationTestBase<TInit> where TInit : ServicesInitBase, new()
{
    [Fact]
    public async Task Should_UseLastValue_When_UsedMultipleTimes()
    {
        const int TestUserCount = 10;
        using var server = SetupSample(Enumerable.Range(1, TestUserCount).Select(i => new User()));
        using var client = server.CreateClient();

        var response = await client.GetQueryAsync<User>("/users?$filter=true&$filter=false", cancellationToken: TestContext.Current.CancellationToken);

        response.Results.ShouldBeEmpty();
    }

    [Fact]
    public async Task Should_ReturnAll_When_FilterWithSimpleTrue()
    {
        const int TestUserCount = 10;
        using var server = SetupSample(Enumerable.Range(1, TestUserCount).Select(i => new User()));
        using var client = server.CreateClient();

        var response = await client.GetQueryAsync<User>("/users?$filter=true", cancellationToken: TestContext.Current.CancellationToken);

        response.Results.Count.ShouldBe(TestUserCount);
    }

    [Fact]
    public async Task Should_ReturnNone_When_FilterWithSimpleFalse()
    {
        const int TestUserCount = 10;
        using var server = SetupSample(Enumerable.Range(1, TestUserCount).Select(i => new User()));
        using var client = server.CreateClient();

        var response = await client.GetQueryAsync<User>("/users?$filter=false", cancellationToken: TestContext.Current.CancellationToken);

        response.Results.ShouldBeEmpty();
    }

    [Fact]
    public async Task Should_ReturnFiltered_When_FilteringOnEqualsString()
    {
        const int TestUserCount = 10;
        using var server = SetupSample(CreateUsers(TestUserCount));
        using var client = server.CreateClient();

        var response = await client.GetQueryAsync<User>("/users?$filter=username eq 'user4'", cancellationToken: TestContext.Current.CancellationToken);

        response.Results.ShouldHaveSingleItem();
        response.Results[0].Username.ShouldBe("user4");
    }

    [Fact]
    public async Task Should_ReturnFilteredStream_When_FilteringOnEqualsString_And_Using_AsyncEnumerable()
    {
        const int TestUserCount = 10;
        using var server = SetupSample(CreateUsers(TestUserCount));
        using var client = server.CreateClient();

        var response = await client.GetStreamAsync<User>("/users/stream?$filter=username eq 'user4'", cancellationToken: TestContext.Current.CancellationToken);

        response.ShouldHaveSingleItem();
        response[0].Username.ShouldBe("user4");
    }

    [Fact]
    public async Task Should_ReturnFiltered_When_FilteringOnEqualsString_On_Superclass_Property()
    {
        const int TestUserCount = 10;
        using var server = SetupSample(CreateUsers(TestUserCount));
        using var client = server.CreateClient();

        var response = await client.GetQueryAsync<User>("/users?$filter=lastname eq 'Smith4'", cancellationToken: TestContext.Current.CancellationToken);

        response.Results.ShouldHaveSingleItem();
        response.Results[0].Username.ShouldBe("user4");
    }

    [Fact]
    public async Task Should_ReturnInverseFiltered_When_FilteringOnEqualsStringAndNegating()
    {
        const int TestUserCount = 10;
        using var server = SetupSample(CreateUsers(TestUserCount));
        using var client = server.CreateClient();

        var response = await client.GetQueryAsync<User>("/users?$filter=not (username eq 'user4')", cancellationToken: TestContext.Current.CancellationToken);

        response.Results.Count.ShouldBe(TestUserCount - 1);
    }

    [Fact]
    public async Task Should_ReturnFiltered_When_FilteringOnMultipleEqualsStrings()
    {
        const int TestUserCount = 10;
        using var server = SetupSample(CreateUsers(TestUserCount));
        using var client = server.CreateClient();

        var response = await client.GetQueryAsync<User>("/users?$filter=username eq 'user4' and lastname eq 'Smith4'", cancellationToken: TestContext.Current.CancellationToken);

        response.Results.ShouldHaveSingleItem();
        response.Results[0].Username.ShouldBe("user4");
    }

    [Fact]
    public async Task Should_ReturnFiltered_When_FilteringOnStringContains()
    {
        const int TestUserCount = 10;
        using var server = SetupSample(CreateUsers(TestUserCount));
        using var client = server.CreateClient();

        var response = await client.GetQueryAsync<User>("/users?$filter=contains(username, '4')", cancellationToken: TestContext.Current.CancellationToken);

        response.Results.ShouldHaveSingleItem();
        response.Results[0].Username.ShouldBe("user4");
    }

    [Fact]
    public async Task Should_ReturnFiltered_When_FilteringOnStringContainsWithSlash()
    {
        const int TestUserCount = 10;
        using var server = SetupSample(CreateUsers(TestUserCount));
        using var client = server.CreateClient();

        var response = await client.GetQueryAsync<User>("/users?%24filter=contains(domainid%2C%27%252F4%27)", cancellationToken: TestContext.Current.CancellationToken);

        response.Results.ShouldHaveSingleItem();
        response.Results[0].Username.ShouldBe("user4");
    }

    [Fact]
    public async Task Should_ReturnFiltered_When_FilteringOnArrayContains()
    {
        const int TestUserCount = 10;
        using var server = SetupSample(CreateUsers(TestUserCount));
        using var client = server.CreateClient();

        var response = await client.GetQueryAsync<User>("/users?$filter=username in ('user4', 'user5')", cancellationToken: TestContext.Current.CancellationToken);

        response.Results.Count.ShouldBe(2);
        response.Results[0].Username.ShouldBe("user4");
        response.Results[1].Username.ShouldBe("user5");
    }

    [Fact]
    public async Task Should_ReturnFiltered_When_FilteringOnArrayContainsForEnumsStrings()
    {
        const int TestUserCount = 10;

        var users = CreateUsers(TestUserCount).ToList();

        users[0].Type = UserType.Admin;
        users[3].Type = UserType.Admin;
        users[5].Type = UserType.SuperAdmin;
        
        using var server = SetupSample(users);
        using var client = server.CreateClient();

        var response = await client.GetQueryAsync<User>("/users?$filter=type in ('admin', 'superadmin')", cancellationToken: TestContext.Current.CancellationToken);

        response.Results.Count.ShouldBe(3);
        response.Results[0].Username.ShouldBe("user1");
        response.Results[1].Username.ShouldBe("user4");
        response.Results[2].Username.ShouldBe("user6");
    }

    [Fact]
    public async Task Should_ReturnFiltered_When_FilteringOnArrayContainsForEnumsInts()
    {
        const int TestUserCount = 10;

        var users = CreateUsers(TestUserCount).ToList();

        users[0].Type = UserType.Admin;
        users[3].Type = UserType.Admin;
        users[5].Type = UserType.SuperAdmin;
        
        using var server = SetupSample(users);
        using var client = server.CreateClient();

        var response = await client.GetQueryAsync<User>("/users?$filter=type in (1, 2)", cancellationToken: TestContext.Current.CancellationToken);

        response.Results.Count.ShouldBe(3);
        response.Results[0].Username.ShouldBe("user1");
        response.Results[1].Username.ShouldBe("user4");
        response.Results[2].Username.ShouldBe("user6");
    }

    [Fact]
    public async Task Should_ReturnFiltered_When_FilteringOnArrayContainsForEnumsStrings_And_PropertyIsNullable()
    {
        const int TestUserCount = 10;

        var users = CreateUsers(TestUserCount).ToList();

        users[0].SecondaryType = UserType.Admin;
        users[3].SecondaryType = UserType.Admin;
        users[5].SecondaryType = UserType.SuperAdmin;
        
        using var server = SetupSample(users);
        using var client = server.CreateClient();

        var response = await client.GetQueryAsync<User>("/users?$filter=secondarytype in ('admin', 'superadmin')", cancellationToken: TestContext.Current.CancellationToken);

        response.Results.Count.ShouldBe(3);
        response.Results[0].Username.ShouldBe("user1");
        response.Results[1].Username.ShouldBe("user4");
        response.Results[2].Username.ShouldBe("user6");
    }

    [Fact]
    public async Task Should_ReturnFiltered_When_FilteringOnArrayContainsForEnumsInts_And_PropertyIsNullable()
    {
        const int TestUserCount = 10;

        var users = CreateUsers(TestUserCount).ToList();

        users[0].SecondaryType = UserType.Admin;
        users[3].SecondaryType = UserType.Admin;
        users[5].SecondaryType = UserType.SuperAdmin;
        
        using var server = SetupSample(users);
        using var client = server.CreateClient();

        var response = await client.GetQueryAsync<User>("/users?$filter=secondarytype in (1, 2)", cancellationToken: TestContext.Current.CancellationToken);

        response.Results.Count.ShouldBe(3);
        response.Results[0].Username.ShouldBe("user1");
        response.Results[1].Username.ShouldBe("user4");
        response.Results[2].Username.ShouldBe("user6");
    }

    [Fact]
    public async Task Should_ReturnFiltered_When_FilteringOnIntLessGreaterThan()
    {
        const int TestUserCount = 10;
        using var server = SetupSample(CreateUsers(TestUserCount));
        using var client = server.CreateClient();

        var response = await client.GetQueryAsync<User>("/users?$filter=id gt 2 and id lt 5", cancellationToken: TestContext.Current.CancellationToken);

        response.Results.Count.ShouldBe(2);
        response.Results[0].Id.ShouldBe(3);
        response.Results[1].Id.ShouldBe(4);
    }
        
    [Fact]
    public async Task Should_ReturnFiltered_When_FilteringOnCollectionLength()
    {
        const int TestUserCount = 10;
        using var server = SetupSample(CreateUsers(TestUserCount));
        using var client = server.CreateClient();

        var response = await client.GetQueryAsync<User>("/users?$filter=length(blogs) gt 0 ", cancellationToken: TestContext.Current.CancellationToken);

        response.Results.Count.ShouldBe(TestUserCount);
    }

    [Fact]
    public async Task Should_ReturnNone_When_FilteringOnCollectionLength()
    {
        const int TestUserCount = 10;
        using var server = SetupSample(CreateUsers(TestUserCount));
        using var client = server.CreateClient();

        var response = await client.GetQueryAsync<User>("/users?$filter=length(blogs) eq 0", cancellationToken: TestContext.Current.CancellationToken);

        response.Results.ShouldBeEmpty();
    }

    [Fact]
    public async Task Should_ReturnFiltered_When_FilteringOnNavigationProperty()
    {
        const int TestUserCount = 10;
        using var server = SetupSample(CreateUsers(TestUserCount));
        using var client = server.CreateClient();

        var response = await client.GetQueryAsync<BlogPost>("/blogposts?$filter=blog/owner/id eq 4", cancellationToken: TestContext.Current.CancellationToken);

        response.Results.Count.ShouldBe(2);
    }

    [Theory]
    [InlineData("6.2d")]
    [InlineData("6.2D")]
    [InlineData("6.2")]
    [InlineData("6.20")]
    public async Task Should_ReturnFiltered_When_FilteringOnDoubleProperty(string filterValue)
    {
        const int TestUserCount = 10;
        using var server = SetupSample(CreateUsers(TestUserCount));
        using var client = server.CreateClient();

        var response = await client.GetQueryAsync<User>($"/users?$filter=engagement ge {filterValue}", cancellationToken: TestContext.Current.CancellationToken);

        response.Results.Count.ShouldBe(5);
    }

    [Fact]
    public async Task Should_ReturnFiltered_When_FilteringOnDecimalProperty()
    {
        const int TestUserCount = 10;
        using var server = SetupSample(CreateUsers(TestUserCount));
        using var client = server.CreateClient();

        var response = await client.GetQueryAsync<User>("/users?$filter=PayedAmout eq 25.30", cancellationToken: TestContext.Current.CancellationToken);

        response.Results.Count.ShouldBe(5);
    }

    private static IEnumerable<User> CreateUsers(int testUserCount) =>
        Enumerable.Range(1, testUserCount).Select(i => new User
        {
            Username = $"user{i}",
            EMail = $"user{i}@email.com",
            FirstName = $"Jon{i}",
            LastName = $"Smith{i}",
            DomainId = $"Smith/{i}",
            CreatedAt = new DateTimeOffset(2021, 1, i, 10, 0, 0, TimeSpan.Zero),
            RegistrationDate = new DateOnly(2021, 1, i),
            Engagement = 0.2 + i,
            PayedAmout = (i % 2) * 25.30m,
            Blogs = new List<Blog>
            {
                new Blog
                {
                    Description = $"Blog of User{i}",
                    Posts = new List<BlogPost>
                    {
                        new BlogPost
                        {
                            Text = $"Post 1"
                        },
                        new BlogPost
                        {
                            Text = $"Post 2"
                        }
                    }
                }
            }
        });
}
    
public class InMemoryFilterTests : FilterTests<InMemoryServicesInit> { }
    
public class SqliteFilterTests : FilterTests<SqliteServicesInit> { }
