using Shouldly;
using Xunit;

namespace Scrima.OData.Tests;

public class RawParserTests
{
    [Theory]
    [InlineData("?$select=name&$filter=name eq 'Jon'&$count=true&$search=myvalue&$orderby=name asc&$top=10&$skip=5&$skiptoken=mytoken")]
    [InlineData("$select=name&$filter=name eq 'Jon'&$count=true&$search=myvalue&$orderby=name asc&$top=10&$skip=5&$skiptoken=mytoken")]
    public void Should_ParseRawString_When_AllParametersProvided(string rawQueryString)
    {
        var rawQueryParsed = ODataRawQueryOptions.ParseRawQuery(rawQueryString);

        rawQueryParsed.ShouldNotBeNull();
        rawQueryParsed.Select.ShouldBe("name");
        rawQueryParsed.Count.ShouldBe("true");
        rawQueryParsed.Filter.ShouldBe("name eq 'Jon'");
        rawQueryParsed.Search.ShouldBe("myvalue");
        rawQueryParsed.OrderBy.ShouldBe("name asc");
        rawQueryParsed.Top.ShouldBe("10");
        rawQueryParsed.Skip.ShouldBe("5");
        rawQueryParsed.SkipToken.ShouldBe("mytoken");
    }
        
    [Theory]
    [InlineData("?$filter=name eq 'Jon'")]
    [InlineData("$filter=name eq 'Jon'")]
    public void Should_ParseRawString_When_SomeValueEmpty(string rawQueryString)
    {
        var rawQueryParsed = ODataRawQueryOptions.ParseRawQuery(rawQueryString);

        rawQueryParsed.ShouldNotBeNull();
        rawQueryParsed.Filter.ShouldBe("name eq 'Jon'");
        rawQueryParsed.Count.ShouldBeNull();
        rawQueryParsed.Search.ShouldBeNull();
        rawQueryParsed.OrderBy.ShouldBeNull();
        rawQueryParsed.Top.ShouldBeNull();
        rawQueryParsed.Skip.ShouldBeNull();
        rawQueryParsed.SkipToken.ShouldBeNull();
    }
        
    [Theory]
    [InlineData("?$filter=name eq '/Jon'")]
    [InlineData("$filter=name eq '/Jon'")]
    [InlineData("$filter=name eq '%2FJon'")]
    [InlineData("%24filter=name+eq+%27%2FJon%27")]
    public void Should_ParseRawString_When_ContainsSlash(string rawQueryString)
    {
        var rawQueryParsed = ODataRawQueryOptions.ParseRawQuery(rawQueryString);

        rawQueryParsed.ShouldNotBeNull();
        rawQueryParsed.Filter.ShouldBe("name eq '/Jon'");
        rawQueryParsed.Count.ShouldBeNull();
        rawQueryParsed.Search.ShouldBeNull();
        rawQueryParsed.OrderBy.ShouldBeNull();
        rawQueryParsed.Top.ShouldBeNull();
        rawQueryParsed.Skip.ShouldBeNull();
        rawQueryParsed.SkipToken.ShouldBeNull();
    }
}
