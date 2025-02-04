using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Scrima.Core.Query;
using Scrima.EntityFrameworkCore;
using Scrima.OData.AspNetCore;

namespace Scrima.Integration.Tests.Controllers;

[ApiController]
[Route("[controller]")]
public abstract class TestControllerBase<T> : Controller where T : class
{
    public const int OverriddenDefaultTop = 2;
    public const int OverriddenMaxTop = 7;
    private readonly DbSet<T> _records;

    protected TestControllerBase(DbSet<T> records)
    {
        _records = records;
    }

    [HttpGet]
    public async Task<ActionResult<QueryResult<T>>> GetAsync(
        [FromQuery] ODataQuery<T> queryOptions,
        CancellationToken cancellationToken)
    {
        var queryResult =
            await _records.ToQueryResultAsync(queryOptions, GetSearchPredicate(), cancellationToken: cancellationToken);

        return Ok(queryResult);
    }

    [HttpGet("stream")]
    public ActionResult<QueryResult<T>> GetStreamAsync([FromQuery] ODataQuery<T> queryOptions)
    {
        var queryResult = _records.AsAsyncEnumerable(queryOptions, GetSearchPredicate());

        return Ok(queryResult);
    }

    [HttpGet("override")]
    public async Task<ActionResult<QueryResult<T>>> OverriddenGetAsync(
        [FromQuery, ODataQueryDefaultOptions(DefaultTop = OverriddenDefaultTop, MaxTop = OverriddenMaxTop, AlwaysShowCount = true)]
        ODataQuery<T> queryOptions,
        CancellationToken cancellationToken)
    {
        var queryResult =
            await _records.ToQueryResultAsync(queryOptions, GetSearchPredicate(),
                cancellationToken: cancellationToken);

        return Ok(queryResult);
    }

    protected abstract Expression<Func<T, string, bool>> GetSearchPredicate();
}
