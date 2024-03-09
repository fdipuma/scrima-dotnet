using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Scrima.Integration.Tests.Initializers;
using Scrima.Integration.Tests.Models;
using Scrima.Integration.Tests.Utility;
using Xunit;

namespace Scrima.Integration.Tests;

public abstract class DateFilterTests<TInit> : IntegrationTestBase<TInit> where TInit : ServicesInitBase, new()
{
    [Fact]
    public async Task Should_ReturnNone_When_FilteringOnDateYear()
    {
        const int testUserCount = 10;
        using var server = SetupSample(CreateUsers(testUserCount));
        using var client = server.CreateClient();

        var response = await client.GetQueryAsync<User>("/users?$filter=year(createdAt) eq 2020");

        response.Results.Should().BeEmpty();
    }

    [Fact]
    public async Task Should_ReturnFiltered_When_FilteringOnDate()
    {
        const int testUserCount = 10;
        using var server = SetupSample(CreateUsers(testUserCount));
        using var client = server.CreateClient();

        var minDate = new DateTimeOffset(2021, 1, 5, 10, 0, 0, TimeSpan.Zero).ToString("O");

        minDate = Uri.EscapeDataString(minDate);
        
        var response = await client.GetQueryAsync<User>($"/users?$filter=createdAt gt {minDate}");

        response.Results.Should().HaveCount(5);
    }

    [Fact]
    public async Task Should_ReturnFiltered_When_FilteringOnDateWithoutTime()
    {
        const int testUserCount = 10;
        using var server = SetupSample(CreateUsers(testUserCount));
        using var client = server.CreateClient();
        
        var response = await client.GetQueryAsync<User>($"/users?$filter=registrationDate gt 2021-01-05");

        response.Results.Should().HaveCount(5);
    }

    private static IEnumerable<User> CreateUsers(int testUserCount) =>
        Enumerable.Range(1, testUserCount).Select(i => new User
        {
            Username = $"user{i}",
            EMail = $"user{i}@email.com",
            FirstName = $"Jon{i}",
            LastName = $"Smith{i}",
            CreatedAt = new DateTimeOffset(2021, 1, i, 10, 0, 0, TimeSpan.Zero),
            RegistrationDate = new DateTime(2021, 1, i, 0, 0, 0),
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
    
public class InMemoryDateFilterTests : DateFilterTests<InMemoryServicesInit> { }
    
/// <summary>
/// replace internal modifier with public to run tests on SqlServer
/// </summary>
internal class SqlServerDateFilterTests : DateFilterTests<SqlServerServicesInit> { }
