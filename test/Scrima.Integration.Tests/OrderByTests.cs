using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Scrima.Integration.Sample.Models;
using Scrima.Integration.Tests.Initializers;
using Scrima.Integration.Tests.Utility;
using Shouldly;
using Xunit;

namespace Scrima.Integration.Tests;

public abstract class OrderByTests<TInit> : IntegrationTestBase<TInit> where TInit : ServicesInitBase, new()
{
    private static User[] OrderByTestUsers => new[]
    {
        new User {Username = "A", FirstName = "Name2", Blogs = new List<Blog> { new() { Name = "Blog1"}}},
        new User {Username = "B", FirstName = "Name1", Blogs = new List<Blog> { new() { Name = "Blog2"}}},
        new User {Username = "C", FirstName = "Name2", Blogs = new List<Blog> { new() { Name = "Blog3"}}},
        new User {Username = "D", FirstName = "Name1", Blogs = new List<Blog> { new() { Name = "Blog4"}}},
    };

    [Fact]
    public async Task Should_UseLastValue_When_UsedMultipleTimes()
    {
        using var server = SetupSample(OrderByTestUsers);
        using var client = server.CreateClient();

        var response = await client.GetQueryAsync<User>("/users?$orderby=firstname&$orderby=username", cancellationToken: TestContext.Current.CancellationToken);

        response.Results.Select(r => r.Username).ShouldBeInOrder(SortDirection.Ascending);
    }

    [Fact]
    public async Task Should_UseDefaultOrderAscending_When_SinglePropertyProvided()
    {
        using var server = SetupSample(OrderByTestUsers);
        using var client = server.CreateClient();

        var response = await client.GetQueryAsync<User>("/users?$orderby=username", cancellationToken: TestContext.Current.CancellationToken);
            
        response.Results.Select(r => r.Username).ShouldBeInOrder(SortDirection.Ascending);
    }

    [Fact]
    public async Task Should_SortByNestedProperty_When_SlashIsFoundInName()
    {
        using var server = SetupSample(OrderByTestUsers);
        using var client = server.CreateClient();

        var response = await client.GetQueryAsync<Blog>("/blogs?$orderby=owner/username desc", cancellationToken: TestContext.Current.CancellationToken);
            
        response.Results.Select(r => r.Name).ShouldBeInOrder(SortDirection.Descending);
    }

    [Fact]
    public async Task Should_UseOrderAscending_When_SinglePropertyProvidedWithAscendingSpecified()
    {
        using var server = SetupSample(OrderByTestUsers);
        using var client = server.CreateClient();

        var response = await client.GetQueryAsync<User>("/users?$orderby=username asc", cancellationToken: TestContext.Current.CancellationToken);
            
        response.Results.Select(r => r.Username).ShouldBeInOrder(SortDirection.Ascending);
    }

    [Fact]
    public async Task Should_UseOrderDescending_When_SinglePropertyProvidedWithDescendingSpecified()
    {
        using var server = SetupSample(OrderByTestUsers);
        using var client = server.CreateClient();

        var response = await client.GetQueryAsync<User>("/users?$orderby=username desc", cancellationToken: TestContext.Current.CancellationToken);
            
        response.Results.Select(r => r.Username).ShouldBeInOrder(SortDirection.Descending);
    }

    [Fact]
    public async Task Should_UseDefaultOrderAscending_When_MultiplePropertiesAreProvided()
    {
        using var server = SetupSample(OrderByTestUsers);
        using var client = server.CreateClient();
        var expectedUsernameOrder = new[] {"B", "D", "A", "C"};

        var response = await client.GetQueryAsync<User>("/users?$orderby=firstName, username", cancellationToken: TestContext.Current.CancellationToken);

        response.Results.Select(o => o.Username).ShouldBe(expectedUsernameOrder);
    }

    [Fact]
    public async Task Should_UseOrderAscending_When_MultiplePropertiesProvidedWithAscendingSpecified()
    {
        using var server = SetupSample(OrderByTestUsers);
        using var client = server.CreateClient();
        var expectedUsernameOrder = new[] {"B", "D", "A", "C"};

        var response = await client.GetQueryAsync<User>("/users?$orderby=firstName asc, username asc", cancellationToken: TestContext.Current.CancellationToken);

        response.Results.Select(o => o.Username).ShouldBe(expectedUsernameOrder);
    }

    [Fact]
    public async Task Should_UseOrderDescending_When_MultiplePropertiesProvidedWithDescendingSpecified()
    {
        using var server = SetupSample(OrderByTestUsers);
        using var client = server.CreateClient();
        var expectedUsernameOrder = new[] {"C", "A", "D", "B"};

        var response = await client.GetQueryAsync<User>("/users?$orderby=firstName desc, username desc", cancellationToken: TestContext.Current.CancellationToken);

        response.Results.Select(o => o.Username).ShouldBe(expectedUsernameOrder);
    }

    [Fact]
    public async Task Should_UseSpecifiedOrder_When_MixedPropertiesAreProvided()
    {
        using var server = SetupSample(OrderByTestUsers);
        using var client = server.CreateClient();
        var expectedUsernameOrder = new[] {"D", "B", "C", "A"};
            
        var response = await client.GetQueryAsync<User>("/users?$orderby=firstName asc, username desc", cancellationToken: TestContext.Current.CancellationToken);

        response.Results.Select(o => o.Username).ShouldBe(expectedUsernameOrder);
    }
}

public class InMemoryOrderByTests : OrderByTests<InMemoryServicesInit> { }
public class SqliteOrderByTests : OrderByTests<SqliteServicesInit> { }
