using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Scrima.Integration.Sample.Models;
using Scrima.Integration.Tests.Initializers;
using Scrima.Integration.Tests.Utility;
using Shouldly;
using Xunit;

namespace Scrima.Integration.Tests;

public abstract class SelectTests<TInit> : IntegrationTestBase<TInit> where TInit : ServicesInitBase, new()
{
    private const int TestUserCount = 10;

    [Fact]
    public async Task Should_ReturnOnlySelectedProperties()
    {
        using var server = SetupSample(CreateUsers());
        using var client = server.CreateClient();

        var response = await client.GetQueryAsync<User>("/users?$select=username,firstName&$filter=username eq 'user1'",
            cancellationToken: TestContext.Current.CancellationToken);

        var user = response.Results.ShouldHaveSingleItem();
        user.Username.ShouldBe("user1");
        user.FirstName.ShouldBe("Jon1");
        user.LastName.ShouldBeNull();
        user.Id.ShouldBe(0);
        user.Blogs.ShouldBeNull();
    }

    [Fact]
    public async Task Should_ReturnNestedProperties_When_PathIsSelected()
    {
        using var server = SetupSample(CreateUsers());
        using var client = server.CreateClient();

        var response = await client.GetQueryAsync<BlogPost>("/blogposts?$select=title,blog/name&$filter=title eq 'Title1'",
            cancellationToken: TestContext.Current.CancellationToken);

        var post = response.Results.ShouldHaveSingleItem();
        post.Title.ShouldBe("Title1");
        post.Text.ShouldBeNull();
        post.Blog.Name.ShouldBe("Blog1");
        post.Blog.Description.ShouldBeNull();
    }

    [Fact]
    public async Task Should_ReturnRelatedCollection_When_CollectionIsSelected()
    {
        using var server = SetupSample(CreateUsers());
        using var client = server.CreateClient();

        var response = await client.GetQueryAsync<User>("/users?$select=blogs&$filter=username eq 'user1'",
            cancellationToken: TestContext.Current.CancellationToken);

        var user = response.Results.ShouldHaveSingleItem();
        user.Username.ShouldBeNull();
        user.Blogs.ShouldHaveSingleItem().Name.ShouldBe("Blog1");
    }

    [Fact]
    public async Task Should_SelectProperties_After_OrderingAndPaging()
    {
        using var server = SetupSample(CreateUsers());
        using var client = server.CreateClient();

        var response = await client.GetQueryAsync<User>("/users?$select=username&$orderby=username desc&$top=2&$count=true",
            cancellationToken: TestContext.Current.CancellationToken);

        response.Count.ShouldBe(TestUserCount);
        response.Results.Select(u => u.Username).ShouldBe(new[] { "user9", "user8" });
        response.Results.ShouldAllBe(u => u.FirstName == null);
    }

    [Fact]
    public async Task Should_SelectProperties_When_Streaming()
    {
        using var server = SetupSample(CreateUsers());
        using var client = server.CreateClient();

        var users = await client.GetStreamAsync<User>("/users/stream?$select=username",
            cancellationToken: TestContext.Current.CancellationToken);

        users.Length.ShouldBe(TestUserCount);
        users.ShouldAllBe(u => u.Username != null && u.FirstName == null);
    }

    [Fact]
    public async Task Should_ReturnAllProperties_When_SelectIsEmpty()
    {
        using var server = SetupSample(CreateUsers());
        using var client = server.CreateClient();

        var response = await client.GetQueryAsync<User>("/users?$select=&$filter=username eq 'user1'",
            cancellationToken: TestContext.Current.CancellationToken);

        var user = response.Results.ShouldHaveSingleItem();
        user.Username.ShouldBe("user1");
        user.LastName.ShouldBe("Smith1");
    }

    [Fact]
    public async Task Should_ReturnBadRequest_When_SelectedPropertyIsUnknown()
    {
        using var server = SetupSample(CreateUsers());
        using var client = server.CreateClient();

        using var response = await client.GetAsync("/users?$select=unknown", TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }

    private static IEnumerable<User> CreateUsers() =>
        Enumerable.Range(1, TestUserCount).Select(i => new User
        {
            Username = $"user{i}",
            FirstName = $"Jon{i}",
            LastName = $"Smith{i}",
            Blogs = new List<Blog>
            {
                new()
                {
                    Name = $"Blog{i}",
                    Description = $"Description{i}",
                    Posts = new List<BlogPost> { new() { Title = $"Title{i}", Text = $"Text{i}" } }
                }
            }
        });
}

public class InMemorySelectTests : SelectTests<InMemoryServicesInit>;

public class SqliteSelectTests : SelectTests<SqliteServicesInit>;
