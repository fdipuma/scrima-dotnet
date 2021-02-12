using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Scrima.Integration.Tests.Initializers;
using Scrima.Integration.Tests.Models;
using Scrima.Integration.Tests.Utility;
using Xunit;

namespace Scrima.Integration.Tests
{
    public abstract class FilterTests<TInit> : IntegrationTestBase<TInit> where TInit : ServicesInitBase, new()
    {
        [Fact]
        public async Task Should_UseLastValue_When_UsedMultipleTimes()
        {
            const int testUserCount = 10;
            using var server = SetupSample(Enumerable.Range(1, testUserCount).Select(i => new User()));
            using var client = server.CreateClient();

            var response = await client.GetQueryAsync<User>("/users?$filter=true&$filter=false");

            response.Results.Should().BeEmpty();
        }

        [Fact]
        public async Task Should_ReturnAll_When_FilterWithSimpleTrue()
        {
            const int testUserCount = 10;
            using var server = SetupSample(Enumerable.Range(1, testUserCount).Select(i => new User()));
            using var client = server.CreateClient();

            var response = await client.GetQueryAsync<User>("/users?$filter=true");

            response.Results.Should().HaveCount(testUserCount);
        }

        [Fact]
        public async Task Should_ReturnNone_When_FilterWithSimpleFalse()
        {
            const int testUserCount = 10;
            using var server = SetupSample(Enumerable.Range(1, testUserCount).Select(i => new User()));
            using var client = server.CreateClient();

            var response = await client.GetQueryAsync<User>("/users?$filter=false");

            response.Results.Should().BeEmpty();
        }

        [Fact]
        public async Task Should_ReturnFiltered_When_FilteringOnEqualsString()
        {
            const int testUserCount = 10;
            using var server = SetupSample(CreateUsers(testUserCount));
            using var client = server.CreateClient();

            var response = await client.GetQueryAsync<User>("/users?$filter=username eq 'user4'");

            response.Results.Should().ContainSingle();
            response.Results[0].Username.Should().Be("user4");
        }

        [Fact]
        public async Task Should_ReturnFiltered_When_FilteringOnEqualsString_On_Superclass_Property()
        {
            const int testUserCount = 10;
            using var server = SetupSample(CreateUsers(testUserCount));
            using var client = server.CreateClient();

            var response = await client.GetQueryAsync<User>("/users?$filter=lastname eq 'Smith4'");

            response.Results.Should().ContainSingle();
            response.Results[0].Username.Should().Be("user4");
        }

        [Fact]
        public async Task Should_ReturnInverseFiltered_When_FilteringOnEqualsStringAndNegating()
        {
            const int testUserCount = 10;
            using var server = SetupSample(CreateUsers(testUserCount));
            using var client = server.CreateClient();

            var response = await client.GetQueryAsync<User>("/users?$filter=not (username eq 'user4')");

            response.Results.Should().HaveCount(testUserCount - 1);
        }

        [Fact]
        public async Task Should_ReturnFiltered_When_FilteringOnMultipleEqualsStrings()
        {
            const int testUserCount = 10;
            using var server = SetupSample(CreateUsers(testUserCount));
            using var client = server.CreateClient();

            var response = await client.GetQueryAsync<User>("/users?$filter=username eq 'user4' and lastname eq 'Smith4'");

            response.Results.Should().ContainSingle();
            response.Results[0].Username.Should().Be("user4");
        }

        [Fact]
        public async Task Should_ReturnFiltered_When_FilteringOnStringContains()
        {
            const int testUserCount = 10;
            using var server = SetupSample(CreateUsers(testUserCount));
            using var client = server.CreateClient();

            var response = await client.GetQueryAsync<User>("/users?$filter=contains(username, '4')");

            response.Results.Should().ContainSingle();
            response.Results[0].Username.Should().Be("user4");
        }

        [Fact]
        public async Task Should_ReturnFiltered_When_FilteringOnIntLessGreaterThan()
        {
            const int testUserCount = 10;
            using var server = SetupSample(CreateUsers(testUserCount));
            using var client = server.CreateClient();

            var response = await client.GetQueryAsync<User>("/users?$filter=id gt 2 and id lt 5");

            response.Results.Should().HaveCount(2);
            response.Results[0].Id.Should().Be(3);
            response.Results[1].Id.Should().Be(4);
        }
        
        [Fact]
        public async Task Should_ReturnFiltered_When_FilteringOnCollectionLength()
        {
            const int testUserCount = 10;
            using var server = SetupSample(CreateUsers(testUserCount));
            using var client = server.CreateClient();

            var response = await client.GetQueryAsync<User>("/users?$filter=length(blogs) gt 0 ");

            response.Results.Should().HaveCount(testUserCount);
        }

        [Fact]
        public async Task Should_ReturnNone_When_FilteringOnCollectionLength()
        {
            const int testUserCount = 10;
            using var server = SetupSample(CreateUsers(testUserCount));
            using var client = server.CreateClient();

            var response = await client.GetQueryAsync<User>("/users?$filter=length(blogs) eq 0");

            response.Results.Should().BeEmpty();
        }

        [Fact]
        public async Task Should_ReturnFiltered_When_FilteringOnNavigationProperty()
        {
            const int testUserCount = 10;
            using var server = SetupSample(CreateUsers(testUserCount));
            using var client = server.CreateClient();

            var response = await client.GetQueryAsync<BlogPost>("/blogposts?$filter=blog/owner/id eq 4");

            response.Results.Should().HaveCount(2);
        }

        [Theory]
        [InlineData("6.2d")]
        [InlineData("6.2D")]
        [InlineData("6.2")]
        [InlineData("6.20")]
        public async Task Should_ReturnFiltered_When_FilteringOnDoubleProperty(string filterValue)
        {
            const int testUserCount = 10;
            using var server = SetupSample(CreateUsers(testUserCount));
            using var client = server.CreateClient();

            var response = await client.GetQueryAsync<User>($"/users?$filter=engagement ge {filterValue}");

            response.Results.Should().HaveCount(5);
        }

        [Fact]
        public async Task Should_ReturnFiltered_When_FilteringOnDecimalProperty()
        {
            const int testUserCount = 10;
            using var server = SetupSample(CreateUsers(testUserCount));
            using var client = server.CreateClient();

            var response = await client.GetQueryAsync<User>("/users?$filter=PayedAmout eq 25.30");

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
}
