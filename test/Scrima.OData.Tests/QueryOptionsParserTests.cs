using System.Linq;
using Scrima.Core.Query;
using Shouldly;
using Xunit;

namespace Scrima.OData.Tests;

public class QueryOptionsParserTests
{
    private readonly ODataRawRawQueryParser _parser = new();

    [Theory]
    [InlineData("$top=10", new string[0])]
    [InlineData("$select=*", new string[0])]
    [InlineData("$select=name,*", new string[0])]
    [InlineData("$select=name", new[] { "Name" })]
    [InlineData("$select=Name, price", new[] { "Name", "Price" })]
    [InlineData("$select=name,supplier/name", new[] { "Name", "Supplier/Name" })]
    public void Should_ParseSelectedProperties(string rawQuery, string[] expectedPaths)
    {
        var select = Parse(rawQuery).Select;

        select.Properties
            .Select(property => string.Join("/", property.Properties.Select(p => p.Name)))
            .ShouldBe(expectedPaths);
    }

    [Theory]
    [InlineData("$select=unknown")]
    [InlineData("$select=supplier/unknown")]
    [InlineData("$select=name/length")]
    [InlineData("$select=name,")]
    [InlineData("$select=name,,price")]
    public void Should_Throw_When_SelectIsInvalid(string rawQuery)
    {
        Should.Throw<ODataParseException>(() => Parse(rawQuery));
    }

    [Theory]
    [InlineData("$filter=unknown eq 1")]
    [InlineData("$orderby=unknown")]
    public void Should_Throw_When_PropertyIsUnknown(string rawQuery)
    {
        Should.Throw<ODataParseException>(() => Parse(rawQuery));
    }

    [Theory]
    [InlineData("$select=")]
    [InlineData("$select=contact")]
    [InlineData("$select=cost/amount")]
    public void Should_NotThrow_When_SelectIsValid(string rawQuery)
    {
        Should.NotThrow(() => Parse(rawQuery));
    }

    [Theory]
    [InlineData("$select=code")]
    [InlineData("$select=contact/email")]
    public void Should_Throw_When_SelectionCannotBeProjected(string rawQuery)
    {
        Should.Throw<ODataParseException>(() => Parse(rawQuery));
    }

    [Fact]
    public void Should_Throw_When_ItemTypeHasNoParameterlessConstructor()
    {
        Should.Throw<ODataParseException>(() =>
            _parser.ParseOptions(typeof(ProductDto), ODataRawQueryOptions.ParseRawQuery("$select=name")));
    }

    [Fact]
    public void Should_FormatSelectWithPathSeparator()
    {
        Parse("$select=name,supplier/name").Select.ToString().ShouldBe("Select=Name,Supplier/Name");
    }

    private QueryOptions Parse(string rawQuery) =>
        _parser.ParseOptions(typeof(Product), ODataRawQueryOptions.ParseRawQuery(rawQuery));

    private class Product
    {
        public string Name { get; set; }
        public string Code => Name;
        public decimal Price { get; set; }
        public Money Cost { get; set; }
        public Supplier Supplier { get; set; }
        public Contact Contact { get; set; }
    }

    private class Supplier
    {
        public string Name { get; set; }
    }

    private struct Money
    {
        public decimal Amount { get; set; }
    }

    private class Contact(string email)
    {
        public string Email { get; set; } = email;
    }

    private record ProductDto(string Name);
}
