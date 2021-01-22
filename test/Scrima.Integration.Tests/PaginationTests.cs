using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Scrima.Integration.Tests.Initializers;
using Scrima.Integration.Tests.Models;
using Scrima.Integration.Tests.Utility;
using Xunit;

namespace Scrima.Integration.Tests
{
    public abstract class PaginationTests<TInit> : IntegrationTestBase<TInit> where TInit : ServicesInitBase, new()
    {
        [Fact]
        public async Task Should_UseLastSkipValue_When_UsedMultipleTimes()
        {
            const int testUserCount = 10;
            const int skipCount = 2;
            using var server = SetupSample(Enumerable.Range(1, testUserCount).Select(i => new User()));
            using var client = server.CreateClient();

            var response = await client.GetQueryAsync<User>($"/users?$skip=1&$skip={skipCount}");

            response.Results.Should().HaveCount(testUserCount - skipCount);
        }

        [Fact]
        public async Task Should_ReturnRest_When_Skip_InRange()
        {
            const int testUserCount = 10;
            const int skipCount = 3;
            using var server = SetupSample(Enumerable.Range(1, testUserCount).Select(i => new User()));
            using var client = server.CreateClient();

            var response = await client.GetQueryAsync<User>($"/users?$skip={skipCount}");
            
            response.Results.Should().HaveCount(testUserCount - skipCount);
        }

        [Fact]
        public async Task Should_ReturnEmpty_When_Skip_OutOfRange()
        {
            const int testUserCount = 10;
            const int skipCount = 11;
            using var server = SetupSample(Enumerable.Range(1, testUserCount).Select(i => new User()));
            using var client = server.CreateClient();

            var response = await client.GetQueryAsync<User>($"/users?$skip={skipCount}");
            
            response.Results.Should().BeEmpty();
        }

        [Fact]
        public async Task Should_ReturnTop_When_Top_InRange()
        {
            const int testUserCount = 10;
            const int topCount = 3;
            using var server = SetupSample(Enumerable.Range(1, testUserCount).Select(i => new User()));
            using var client = server.CreateClient();

            var response = await client.GetQueryAsync<User>($"/users?$top={topCount}");
            
            response.Results.Should().HaveCount(topCount);
        }

        [Fact]
        public async Task Should_ReturnAll_When_Top_OutOfRange()
        {
            const int testUserCount = 10;
            const int skipCount = 20;
            using var server = SetupSample(Enumerable.Range(1, testUserCount).Select(i => new User()));
            using var client = server.CreateClient();

            var response = await client.GetQueryAsync<User>($"/users?$top={skipCount}");
            
            response.Results.Should().HaveCount(testUserCount);
        }

        [Fact]
        public async Task Should_UseLastTopValue_When_UsedMultipleTimes()
        {
            const int testUserCount = 10;
            const int topCount = 3;
            using var server = SetupSample(Enumerable.Range(1, testUserCount).Select(i => new User()));
            using var client = server.CreateClient();

            var response = await client.GetQueryAsync<User>($"/users?$top=1&$top={topCount}");
            
            response.Results.Should().HaveCount(topCount);
        }

        [Fact]
        public async Task Should_ReturnItems_When_SkipTop_InRange()
        {
            const int testUserCount = 10;
            const int skipCount = 2;
            const int topCount = 2;
            using var server = SetupSample(Enumerable.Range(1, testUserCount).Select(i => new User()));
            using var client = server.CreateClient();

            var response = await client.GetQueryAsync<User>($"/users?$skip={skipCount}&$top={topCount}");
            
            response.Results.Should().HaveCount(topCount);
        }

        [Fact]
        public async Task Should_ReturnRest_When_SkipTop_OutOfRange()
        {
            const int testUserCount = 10;
            const int skipCount = 5;
            const int topCount = 10;
            using var server = SetupSample(Enumerable.Range(1, testUserCount).Select(i => new User()));
            using var client = server.CreateClient();

            var response = await client.GetQueryAsync<User>($"/users?$skip={skipCount}&$top={topCount}");
            
            response.Results.Should().HaveCount(5);
        }

        [Fact]
        public async Task Should_ReturnEmpty_When_SkipTop_OutOfRange()
        {
            const int testUserCount = 10;
            const int skipCount = 20;
            const int topCount = 10;
            using var server = SetupSample(Enumerable.Range(1, testUserCount).Select(i => new User()));
            using var client = server.CreateClient();

            var response = await client.GetQueryAsync<User>($"/users?$skip={skipCount}&$top={topCount}");
            
            response.Results.Should().BeEmpty();
        }
    }
    
    public class InMemoryPaginationTests : PaginationTests<InMemoryServicesInit> { }
    public class SqlitePaginationTests : PaginationTests<SqliteServicesInit> { }
}
