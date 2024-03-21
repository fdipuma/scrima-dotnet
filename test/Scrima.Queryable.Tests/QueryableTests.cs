using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using FluentAssertions;
using Scrima.Core;
using Scrima.Core.Model;
using Scrima.Core.Query;
using Scrima.Core.Query.Expressions;
using Xunit;

namespace Scrima.Queryable.Tests;

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
                NullableLong = 1,
                Name = "Joe",
                Price = 30,
                NestedList =
                    new[]
                    {
                        new NestedModel {Id = 1, Name = "Element 1"},
                        new NestedModel {Id = 2, Name = "Element 2"}
                    },
                NestedModel = new NestedModel {Id = 99, Name = "Element 99"},
                OptionalDateTimeOffset = new DateTimeOffset(2020, 01, 01, 0, 0, 0, TimeSpan.Zero),
                OptionalDateTime = new DateTime(2020, 01, 01, 0, 0, 0),
                OptionalDateOnly = new DateOnly(2020, 01, 01),
                
            },
            new TestModel
            {
                Id = 7,
                NullableLong = 7,
                Name = "Mark",
                Price = 0,
                NullableInt = 2,
                NestedList =
                    new[]
                    {
                        new NestedModel {Id = 2, Name = "Element 2"},
                        new NestedModel {Id = 3, Name = "Element 3"}
                    },
                EnumValue = TestEnum.ThirdValue,
                OptionalEnumValue = TestEnum.ThirdValue,
                NestedModel = new NestedModel {Id = 98, Name = "Element 98"},
            },
            new TestModel
            {
                Id = 3,
                NullableLong = 3,
                Name = "Bob",
                Price = 20.51m,
                NestedList =
                    new[]
                    {
                        new NestedModel {Id = 4, Name = "Element 4"},
                        new NestedModel {Id = 5, Name = "Element 5"}
                    },
                NestedModel = new NestedModel {Id = 97, Name = "Element 97"},
                EnumValue = TestEnum.SecondValue,
                OptionalEnumValue = TestEnum.SecondValue,
                OptionalDateTimeOffset = new DateTimeOffset(2021, 02, 01, 0, 0, 0, TimeSpan.Zero),
                OptionalDateTime = new DateTime(2021, 02, 01, 0, 0, 0),
                OptionalDateOnly = new DateOnly(2021, 02, 01)
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
                        new[] {new EdmProperty(nameof(TestModel.Id), EdmPrimitiveType.Int32, _edmType)}
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
    public void Should_filter_on_nullable_int_property()
    {
        var query = new QueryOptions(
            _edmType,
            new FilterQueryOption(
                new BinaryOperatorNode(
                    new PropertyAccessNode(
                        new[] {new EdmProperty(nameof(TestModel.NullableInt), EdmPrimitiveType.Int32, _edmType)}
                    ),
                    BinaryOperatorKind.Equal,
                    new ConstantNode(EdmPrimitiveType.Int32, "2", 2))
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
    public void Should_filter_on_nullable_long_property()
    {
        var query = new QueryOptions(
            _edmType,
            new FilterQueryOption(
                new BinaryOperatorNode(
                    new PropertyAccessNode(
                        new[] {new EdmProperty(nameof(TestModel.NullableLong), EdmPrimitiveType.Int64, _edmType)}
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
    public void Should_filter_on_nullable_int_property_when_passing_null()
    {
        var query = new QueryOptions(
            _edmType,
            new FilterQueryOption(
                new BinaryOperatorNode(
                    new PropertyAccessNode(
                        new[] {new EdmProperty(nameof(TestModel.NullableInt), EdmPrimitiveType.Int32, _edmType)}
                    ),
                    BinaryOperatorKind.Equal,
                    ConstantNode.Null)
            ),
            new OrderByQueryOption(Enumerable.Empty<OrderByProperty>()),
            null,
            0,
            null,
            10,
            true
        );

        var results = _queryable.ToQueryResult(query);

        results.Results.Should().HaveCount(2);
        results.Count.Should().Be(2);
    }

    [Fact]
    public void Should_filter_on_enum_property_when_input_is_int()
    {
        var query = new QueryOptions(
            _edmType,
            new FilterQueryOption(
                new BinaryOperatorNode(
                    new PropertyAccessNode(
                        new[]
                        {
                            new EdmProperty(nameof(TestModel.EnumValue),
                                new EdmEnumType(typeof(TestEnum),
                                    new[]
                                    {
                                        new EdmEnumMember("FirstValue", 0),
                                        new EdmEnumMember("SecondValue", 1)
                                    }), _edmType)
                        }
                    ),
                    BinaryOperatorKind.Equal,
                    new ConstantNode(EdmPrimitiveType.Int32, "1", 1))
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
        value.EnumValue.Should().Be(TestEnum.SecondValue);
    }
        
    [Fact]
    public void Should_filter_on_nullable_enum_property_when_input_is_string()
    {
        var query = new QueryOptions(
            _edmType,
            new FilterQueryOption(
                new BinaryOperatorNode(
                    new PropertyAccessNode(
                        new[]
                        {
                            new EdmProperty(nameof(TestModel.OptionalEnumValue),
                                new EdmEnumType(typeof(TestEnum),
                                    new[]
                                    {
                                        new EdmEnumMember("FirstValue", 0),
                                        new EdmEnumMember("SecondValue", 1),
                                        new EdmEnumMember("ThirdValue", 2),
                                    }), _edmType)
                        }
                    ),
                    BinaryOperatorKind.Equal,
                    new ConstantNode(EdmPrimitiveType.String, "SecondValue", "SecondValue"))
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
        value.OptionalEnumValue.Should().Be(TestEnum.SecondValue);
    }

    [Fact]
    public void Should_filter_on_nullable_enum_property_when_input_is_int()
    {
        var query = new QueryOptions(
            _edmType,
            new FilterQueryOption(
                new BinaryOperatorNode(
                    new PropertyAccessNode(
                        new[]
                        {
                            new EdmProperty(nameof(TestModel.OptionalEnumValue),
                                new EdmEnumType(typeof(TestEnum),
                                    new[]
                                    {
                                        new EdmEnumMember("FirstValue", 0),
                                        new EdmEnumMember("SecondValue", 1),
                                        new EdmEnumMember("ThirdValue", 2),
                                    }), _edmType)
                        }
                    ),
                    BinaryOperatorKind.Equal,
                    new ConstantNode(EdmPrimitiveType.Int32, "1", 1))
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
        value.OptionalEnumValue.Should().Be(TestEnum.SecondValue);
    }

    [Fact]
    public void Should_filter_on_nullable_enum_property_when_input_is_null()
    {
        var query = new QueryOptions(
            _edmType,
            new FilterQueryOption(
                new BinaryOperatorNode(
                    new PropertyAccessNode(
                        new[]
                        {
                            new EdmProperty(nameof(TestModel.OptionalEnumValue),
                                new EdmEnumType(typeof(TestEnum),
                                    new[]
                                    {
                                        new EdmEnumMember("FirstValue", 0),
                                        new EdmEnumMember("SecondValue", 1),
                                        new EdmEnumMember("ThirdValue", 2),
                                    }), _edmType)
                        }
                    ),
                    BinaryOperatorKind.Equal,
                    ConstantNode.Null)
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
        value.OptionalEnumValue.Should().BeNull();
    }

    [Fact]
    public void Should_filter_on_enum_property_when_input_is_string()
    {
        var query = new QueryOptions(
            _edmType,
            new FilterQueryOption(
                new BinaryOperatorNode(
                    new PropertyAccessNode(
                        new[]
                        {
                            new EdmProperty(nameof(TestModel.EnumValue),
                                new EdmEnumType(typeof(TestEnum),
                                    new[]
                                    {
                                        new EdmEnumMember("FirstValue", 0),
                                        new EdmEnumMember("SecondValue", 1)
                                    }), _edmType)
                        }
                    ),
                    BinaryOperatorKind.Equal,
                    new ConstantNode(EdmPrimitiveType.String, "SecondValue", "SecondValue"))
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
        value.EnumValue.Should().Be(TestEnum.SecondValue);
    }
        
    [Fact]
    public void Should_filter_on_nullable_enum_property_when_input_is_array_of_strings()
    {
        var query = new QueryOptions(
            _edmType,
            new FilterQueryOption(
                new BinaryOperatorNode(
                    new PropertyAccessNode(
                        new[]
                        {
                            new EdmProperty(nameof(TestModel.OptionalEnumValue),
                                new EdmEnumType(typeof(TestEnum),
                                    new[]
                                    {
                                        new EdmEnumMember("FirstValue", 0),
                                        new EdmEnumMember("SecondValue", 1),
                                        new EdmEnumMember("ThirdValue", 2),
                                    }), _edmType)
                        }
                    ),
                    BinaryOperatorKind.In,
                    new ArrayNode(new []
                    {
                        new ConstantNode(EdmPrimitiveType.String, "SecondValue", "SecondValue"),
                        new ConstantNode(EdmPrimitiveType.String, "ThirdValue", "ThirdValue"),
                    }))
            ),
            new OrderByQueryOption(Enumerable.Empty<OrderByProperty>()),
            null,
            0,
            null,
            10,
            true
        );

        var results = _queryable.ToQueryResult(query);

        results.Results.Should().HaveCount(2);
        results.Count.Should().Be(2);
        
        results.Results.First().OptionalEnumValue.Should().Be(TestEnum.ThirdValue);
        results.Results.Last().OptionalEnumValue.Should().Be(TestEnum.SecondValue);
    }

    [Fact]
    public void Should_filter_on_nullable_enum_property_when_input_is_array_of_int()
    {
        var query = new QueryOptions(
            _edmType,
            new FilterQueryOption(
                new BinaryOperatorNode(
                    new PropertyAccessNode(
                        new[]
                        {
                            new EdmProperty(nameof(TestModel.OptionalEnumValue),
                                new EdmEnumType(typeof(TestEnum),
                                    new[]
                                    {
                                        new EdmEnumMember("FirstValue", 0),
                                        new EdmEnumMember("SecondValue", 1),
                                        new EdmEnumMember("ThirdValue", 2),
                                    }), _edmType)
                        }
                    ),
                    BinaryOperatorKind.In,
                    new ArrayNode(new []
                    {
                        new ConstantNode(EdmPrimitiveType.Int32, "1", 1),
                        new ConstantNode(EdmPrimitiveType.Int32, "2", 2),
                    }))
            ),
            new OrderByQueryOption(Enumerable.Empty<OrderByProperty>()),
            null,
            0,
            null,
            10,
            true
        );

        var results = _queryable.ToQueryResult(query);

        results.Results.Should().HaveCount(2);
        results.Count.Should().Be(2);
        
        results.Results.First().OptionalEnumValue.Should().Be(TestEnum.ThirdValue);
        results.Results.Last().OptionalEnumValue.Should().Be(TestEnum.SecondValue);
    }

    [Fact]
    public void Should_filter_on_enum_property_when_input_is_string_and_enum_member_is_used()
    {
        var query = new QueryOptions(
            _edmType,
            new FilterQueryOption(
                new BinaryOperatorNode(
                    new PropertyAccessNode(
                        new[]
                        {
                            new EdmProperty(nameof(TestModel.EnumValue),
                                new EdmEnumType(typeof(TestEnum),
                                    new[]
                                    {
                                        new EdmEnumMember("FirstValue", 0),
                                        new EdmEnumMember("SecondValue", 1),
                                        new EdmEnumMember("ThirdValue", 2)
                                    }), _edmType)
                        }
                    ),
                    BinaryOperatorKind.Equal,
                    new ConstantNode(EdmPrimitiveType.String, "third", "third"))
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
        value.EnumValue.Should().Be(TestEnum.ThirdValue);
    }

    [Fact]
    public void Should_filter_on_datetimeoffset_property_when_input_is_datetimeoffset()
    {
        var query = new QueryOptions(
            _edmType,
            new FilterQueryOption(
                new BinaryOperatorNode(
                    new PropertyAccessNode(
                        new[]
                        {
                            new EdmProperty(nameof(TestModel.OptionalDateTimeOffset), EdmPrimitiveType.DateTimeOffset,
                                _edmType)
                        }
                    ),
                    BinaryOperatorKind.GreaterThan,
                    new ConstantNode(EdmPrimitiveType.DateTimeOffset, "2020-01-15T00:00:00Z",
                        new DateTimeOffset(2020, 01, 15, 0, 0, 0, TimeSpan.Zero)))
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
        value.Id.Should().Be(3);
    }

    [Fact]
    public void Should_filter_on_datetimeoffset_property_when_input_is_date()
    {
        var query = new QueryOptions(
            _edmType,
            new FilterQueryOption(
                new BinaryOperatorNode(
                    new PropertyAccessNode(
                        new[]
                        {
                            new EdmProperty(nameof(TestModel.OptionalDateTimeOffset), EdmPrimitiveType.DateTimeOffset,
                                _edmType)
                        }
                    ),
                    BinaryOperatorKind.GreaterThan,
                    ConstantNode.Date("2020-01-15", new DateOnly(2020, 01, 15)))
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
        value.Id.Should().Be(3);
    }

    [Fact]
    public void Should_filter_on_date_property_when_input_is_date()
    {
        var query = new QueryOptions(
            _edmType,
            new FilterQueryOption(
                new BinaryOperatorNode(
                    new PropertyAccessNode(
                        new[]
                        {
                            new EdmProperty(nameof(TestModel.OptionalDateOnly), EdmPrimitiveType.Date,
                                _edmType)
                        }
                    ),
                    BinaryOperatorKind.GreaterThan,
                    ConstantNode.Date("2020-01-15", new DateOnly(2020, 01, 15)))
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
        value.Id.Should().Be(3);
    }

    [Fact]
    public void Should_filter_on_date_property_when_input_is_datetimeoffset()
    {
        var query = new QueryOptions(
            _edmType,
            new FilterQueryOption(
                new BinaryOperatorNode(
                    new PropertyAccessNode(
                        new[]
                        {
                            new EdmProperty(nameof(TestModel.OptionalDateOnly), EdmPrimitiveType.Date,
                                _edmType)
                        }
                    ),
                    BinaryOperatorKind.GreaterThan,
                    ConstantNode.DateTimeOffset("2020-01-15T00:00:00Z", new DateTimeOffset(2020, 01, 15, 0, 0, 0, TimeSpan.Zero)))
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
        value.Id.Should().Be(3);
    }

    [Fact]
    public void Should_filter_on_datetime_property_when_input_is_datetimeoffset()
    {
        var query = new QueryOptions(
            _edmType,
            new FilterQueryOption(
                new BinaryOperatorNode(
                    new PropertyAccessNode(
                        new[]
                        {
                            new EdmProperty(nameof(TestModel.OptionalDateTime), EdmPrimitiveType.Date,
                                _edmType)
                        }
                    ),
                    BinaryOperatorKind.GreaterThan,
                    new ConstantNode(EdmPrimitiveType.DateTimeOffset, "2020-01-15T00:00:00Z",
                        new DateTimeOffset(2020, 01, 15, 0, 0, 0, TimeSpan.Zero)))
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
        value.Id.Should().Be(3);
    }
        
    [Theory]
    [InlineData(OrderByDirection.Ascending, null)]
    [InlineData(OrderByDirection.Descending, "2/1/2021 12:00:00 AM")]
    public void Should_orderby_on_datetimenullable_property(OrderByDirection direction, string? expected)
    {
        DateTime? expectedDateTime = null;

        if (expected is not null)
        {
            expectedDateTime = DateTime.Parse(expected, CultureInfo.InvariantCulture);
        }
        
        var query = new QueryOptions(
            _edmType,
            new FilterQueryOption(null),
            new OrderByQueryOption(
                new []{new OrderByProperty(new EdmProperty(nameof(TestModel.OptionalDateTime), EdmPrimitiveType.Date, _edmType), direction)}    
            ),
            null,
            0,
            null,
            10,
            true
        );

        var results = _queryable.ToQueryResult(query);

        results.Count.Should().Be(3);
        results.Results.First().OptionalDateTime.Should().Be(expectedDateTime);
    }

    [Fact]
    public void Should_filter_on_datetime_property_when_input_is_date()
    {
        var query = new QueryOptions(
            _edmType,
            new FilterQueryOption(
                new BinaryOperatorNode(
                    new PropertyAccessNode(
                        new[]
                        {
                            new EdmProperty(nameof(TestModel.OptionalDateTime), EdmPrimitiveType.DateTimeOffset,
                                _edmType)
                        }
                    ),
                    BinaryOperatorKind.GreaterThan,
                    ConstantNode.Date("2020-01-15", new DateOnly(2020, 01, 15)))
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
        value.Id.Should().Be(3);
    }

    [Fact]
    public void Should_order_by_asc_on_int_property()
    {
        var query = new QueryOptions(
            _edmType,
            new FilterQueryOption(null),
            new OrderByQueryOption(new[]
            {
                new OrderByProperty(new EdmProperty(nameof(TestModel.Id), EdmPrimitiveType.Int32, _edmType),
                    OrderByDirection.Ascending)
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

    [Fact]
    public void Should_order_by_nested_property()
    {
        var edmTypeProvider = new EdmTypeProvider();
        var nestedType = edmTypeProvider.GetByClrType(typeof(NestedModel)) as EdmComplexType;
        
        var query = new QueryOptions(
            _edmType,
            new FilterQueryOption(null),
            new OrderByQueryOption(new[]
            {
                new OrderByProperty(
                    new[]
                    {
                        new EdmProperty(nameof(TestModel.NestedModel), nestedType, _edmType),
                        new EdmProperty(nameof(NestedModel.Id), EdmPrimitiveType.Int32, nestedType),
                    },
                    OrderByDirection.Ascending)
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
        results.Results.Should().BeInAscendingOrder(o => o.NestedModel.Id);
    }

    public class TestModel
    {
        public int Id { get; set; }
        public long? NullableLong { get; set; }
        public string Name { get; set; }
        public DateTimeOffset? OptionalDateTimeOffset { get; set; }
        public DateTime? OptionalDateTime { get; set; }
        public DateOnly? OptionalDateOnly { get; set; }
        public decimal Price { get; set; }
        public int? NullableInt { get; set; }
        public TestEnum EnumValue { get; set; }
        public TestEnum? OptionalEnumValue { get; set; }
        public NestedModel NestedModel { get; set; }
        public IEnumerable<NestedModel> NestedList { get; set; }
    }

    public class NestedModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public enum TestEnum
    {
        FirstValue,
        SecondValue,
        [EnumMember(Value = "third")] ThirdValue
    }
}
