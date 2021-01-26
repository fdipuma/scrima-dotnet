using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Scrima.Core.Query;
using Scrima.EntityFrameworkCore;

namespace Scrima.Integration.Tests.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public abstract class TestControllerBase<T> : Controller where T : class
    {
        private readonly DbSet<T> _records;

        protected TestControllerBase(DbSet<T> records)
        {
            _records = records;
        }

        [HttpGet]
        public async Task<ActionResult<QueryResult<T>>> GetAsync(
            QueryOptions<T> queryOptions,
            CancellationToken cancellationToken)
        {
            var queryResult =
                await _records.ToQueryResultAsync(queryOptions, GetSearchPredicate() , cancellationToken: cancellationToken);

            return Ok(queryResult);
        }

        protected abstract Expression<Func<T, string, bool>> GetSearchPredicate();
    }
}
