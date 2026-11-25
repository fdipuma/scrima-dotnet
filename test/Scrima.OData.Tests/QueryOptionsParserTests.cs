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

    private QueryOptions Parse(string rawQuery) =>
        _parser.ParseOptions(typeof(Product), ODataRawQueryOptions.ParseRawQuery(rawQuery));

    private class Product
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
        public Supplier Supplier { get; set; }
    }

    private class Supplier
    {
        public string Name { get; set; }
    }
}
