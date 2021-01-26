using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Scrima.Core;
using Scrima.Core.Model;
using Scrima.Core.Query;
using Scrima.Core.Query.Expressions;
using Xunit;

namespace Scrima.Queryable.Tests
{
    public class QueryableTests
    {
        private readonly EdmComplexType _edmType;
        private readonly IQueryable<TestModel> _queryable;

        public QueryableTests()
        {
            var elementsList = new List<TestModel>
            {
                new TestModel
                {
                    Id = 1,
                    Name = "Joe",
                    Price = 30,
                    NestedList = new []
                    {
                        new NestedModel { Id = 1, Name = "Element 1" },
                        new NestedModel { Id = 2, Name = "Element 2" }
                    },
                    NestedModel = new NestedModel { Id = 99, Name = "Element 99"},
                    OptionalDate = new DateTimeOffset(2020, 01, 01, 0, 0,0, TimeSpan.Zero)
                },
                new TestModel
                {
                    Id = 7,
                    Name = "Mark",
                    Price = 0,
                    NestedList = new []
                    {
                        new NestedModel { Id = 2, Name = "Element 2" },
                        new NestedModel { Id = 3, Name = "Element 3" }
                    },
                    NestedModel = new NestedModel { Id = 98, Name = "Element 98"},
                },
                new TestModel
                {
                    Id = 3,
                    Name = "Bob",
                    Price = 20.51m,
                    NestedList = new []
                    {
                        new NestedModel { Id = 4, Name = "Element 4" },
                        new NestedModel { Id = 5, Name = "Element 5" }
                    },
                    NestedModel = new NestedModel { Id = 97, Name = "Element 97"},
                    OptionalDate = new DateTimeOffset(2021, 01, 01, 0, 0,0, TimeSpan.Zero)
                }
            };
            
            var provider = new EdmTypeProvider();
            _edmType = provider.GetByClrType(typeof(TestModel)) as EdmComplexType;
            _queryable = elementsList.AsQueryable();
        }

        [Fact]
        public void Should_filter_on_int_property()
        {
            var query = new QueryOptions(
                _edmType,
                new FilterQueryOption(
                    new BinaryOperatorNode(
                        new PropertyAccessNode(
                                new[]
                                {
                                    new EdmProperty(nameof(TestModel.Id), EdmPrimitiveType.Int32, _edmType)
                                }
                            ),
                        BinaryOperatorKind.Equal,
                        new ConstantNode(EdmPrimitiveType.Int32, "7", 7))
                    ),
                new OrderByQueryOption(Enumerable.Empty<OrderByProperty>()),
                null,
                0,
                null,
                10,
                true
            );

            var results = _queryable.ToQueryResult(query);

            results.Results.Should().ContainSingle();
            results.Count.Should().Be(1);
            var value = results.Results.First();
            value.Id.Should().Be(7);
        }

        [Fact]
        public void Should_order_by_asc_on_int_property()
        {
            var query = new QueryOptions(
                _edmType,
                new FilterQueryOption(null),
                new OrderByQueryOption(new []
                {
                    new OrderByProperty(new EdmProperty(nameof(TestModel.Id), EdmPrimitiveType.Int32, _edmType), OrderByDirection.Ascending)
                }),
                null,
                0,
                null,
                10,
                true
            );

            var results = _queryable.ToQueryResult(query);

            results.Results.Should().HaveCount(3);
            results.Count.Should().Be(3);
            results.Results.Should().BeInAscendingOrder(o => o.Id);
        }
        
        public class TestModel
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public DateTimeOffset? OptionalDate { get; set; }
            public decimal Price { get; set; }
            public NestedModel NestedModel { get; set; }
            public IEnumerable<NestedModel> NestedList { get; set; }
        }
        
        public class NestedModel
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }
    }
}
