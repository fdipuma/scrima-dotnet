using FluentAssertions;
using Xunit;

namespace Scrima.OData.Tests;

public class RawParserTests
{
    [Theory]
    [InlineData("?$filter=name eq 'Jon'&$count=true&$search=myvalue&$orderby=name asc&$top=10&$skip=5&$skiptoken=mytoken")]
    [InlineData("$filter=name eq 'Jon'&$count=true&$search=myvalue&$orderby=name asc&$top=10&$skip=5&$skiptoken=mytoken")]
    public void Should_ParseRawString_When_AllParametersProvided(string rawQueryString)
    {
        var rawQueryParsed = ODataRawQueryOptions.ParseRawQuery(rawQueryString);

        rawQueryParsed.Should().NotBeNull();
        rawQueryParsed.Count.Should().Be("true");
        rawQueryParsed.Filter.Should().Be("name eq 'Jon'");
        rawQueryParsed.Search.Should().Be("myvalue");
        rawQueryParsed.OrderBy.Should().Be("name asc");
        rawQueryParsed.Top.Should().Be("10");
        rawQueryParsed.Skip.Should().Be("5");
        rawQueryParsed.SkipToken.Should().Be("mytoken");
    }
        
    [Theory]
    [InlineData("?$filter=name eq 'Jon'")]
    [InlineData("$filter=name eq 'Jon'")]
    public void Should_ParseRawString_When_SomeValueEmpty(string rawQueryString)
    {
        var rawQueryParsed = ODataRawQueryOptions.ParseRawQuery(rawQueryString);

        rawQueryParsed.Should().NotBeNull();
        rawQueryParsed.Filter.Should().Be("name eq 'Jon'");
        rawQueryParsed.Count.Should().BeNull();
        rawQueryParsed.Search.Should().BeNull();
        rawQueryParsed.OrderBy.Should().BeNull();
        rawQueryParsed.Top.Should().BeNull();
        rawQueryParsed.Skip.Should().BeNull();
        rawQueryParsed.SkipToken.Should().BeNull();
    }
        
    [Theory]
    [InlineData("?$filter=name eq '/Jon'")]
    [InlineData("$filter=name eq '/Jon'")]
    [InlineData("$filter=name eq '%2FJon'")]
    [InlineData("%24filter=name+eq+%27%2FJon%27")]
    public void Should_ParseRawString_When_ContainsSlash(string rawQueryString)
    {
        var rawQueryParsed = ODataRawQueryOptions.ParseRawQuery(rawQueryString);

        rawQueryParsed.Should().NotBeNull();
        rawQueryParsed.Filter.Should().Be("name eq '/Jon'");
        rawQueryParsed.Count.Should().BeNull();
        rawQueryParsed.Search.Should().BeNull();
        rawQueryParsed.OrderBy.Should().BeNull();
        rawQueryParsed.Top.Should().BeNull();
        rawQueryParsed.Skip.Should().BeNull();
        rawQueryParsed.SkipToken.Should().BeNull();
    }
}
