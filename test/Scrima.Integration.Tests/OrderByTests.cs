using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Scrima.Integration.Sample.Models;
using Scrima.Integration.Tests.Initializers;
using Scrima.Integration.Tests.Utility;
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

        var response = await client.GetQueryAsync<User>("/users?$orderby=firstname&$orderby=username");

        response.Results.Should().BeInAscendingOrder(r => r.Username);
    }

    [Fact]
    public async Task Should_UseDefaultOrderAscending_When_SinglePropertyProvided()
    {
        using var server = SetupSample(OrderByTestUsers);
        using var client = server.CreateClient();

        var response = await client.GetQueryAsync<User>("/users?$orderby=username");
            
        response.Results.Should().BeInAscendingOrder(r => r.Username);
    }

    [Fact]
    public async Task Should_SortByNestedProperty_When_SlashIsFoundInName()
    {
        using var server = SetupSample(OrderByTestUsers);
        using var client = server.CreateClient();

        var response = await client.GetQueryAsync<Blog>("/blogs?$orderby=owner/username desc");
            
        response.Results.Should().BeInDescendingOrder(r => r.Name);
    }

    [Fact]
    public async Task Should_UseOrderAscending_When_SinglePropertyProvidedWithAscendingSpecified()
    {
        using var server = SetupSample(OrderByTestUsers);
        using var client = server.CreateClient();

        var response = await client.GetQueryAsync<User>("/users?$orderby=username asc");
            
        response.Results.Should().BeInAscendingOrder(r => r.Username);
    }

    [Fact]
    public async Task Should_UseOrderDescending_When_SinglePropertyProvidedWithDescendingSpecified()
    {
        using var server = SetupSample(OrderByTestUsers);
        using var client = server.CreateClient();

        var response = await client.GetQueryAsync<User>("/users?$orderby=username desc");
            
        response.Results.Should().BeInDescendingOrder(r => r.Username);
    }

    [Fact]
    public async Task Should_UseDefaultOrderAscending_When_MultiplePropertiesAreProvided()
    {
        using var server = SetupSample(OrderByTestUsers);
        using var client = server.CreateClient();
        var expectedUsernameOrder = new[] {"B", "D", "A", "C"};

        var response = await client.GetQueryAsync<User>("/users?$orderby=firstName, username");

        response.Results.Select(o => o.Username).Should().ContainInOrder(expectedUsernameOrder);
    }

    [Fact]
    public async Task Should_UseOrderAscending_When_MultiplePropertiesProvidedWithAscendingSpecified()
    {
        using var server = SetupSample(OrderByTestUsers);
        using var client = server.CreateClient();
        var expectedUsernameOrder = new[] {"B", "D", "A", "C"};

        var response = await client.GetQueryAsync<User>("/users?$orderby=firstName asc, username asc");

        response.Results.Select(o => o.Username).Should().ContainInOrder(expectedUsernameOrder);
    }

    [Fact]
    public async Task Should_UseOrderDescending_When_MultiplePropertiesProvidedWithDescendingSpecified()
    {
        using var server = SetupSample(OrderByTestUsers);
        using var client = server.CreateClient();
        var expectedUsernameOrder = new[] {"C", "A", "D", "B"};

        var response = await client.GetQueryAsync<User>("/users?$orderby=firstName desc, username desc");

        response.Results.Select(o => o.Username).Should().ContainInOrder(expectedUsernameOrder);
    }

    [Fact]
    public async Task Should_UseSpecifiedOrder_When_MixedPropertiesAreProvided()
    {
        using var server = SetupSample(OrderByTestUsers);
        using var client = server.CreateClient();
        var expectedUsernameOrder = new[] {"D", "B", "C", "A"};
            
        var response = await client.GetQueryAsync<User>("/users?$orderby=firstName asc, username desc");

        response.Results.Select(o => o.Username).Should().ContainInOrder(expectedUsernameOrder);
    }
}

public class InMemoryOrderByTests : OrderByTests<InMemoryServicesInit> { }
public class SqliteOrderByTests : OrderByTests<SqliteServicesInit> { }
