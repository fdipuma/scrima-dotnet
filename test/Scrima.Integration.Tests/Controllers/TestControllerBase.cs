using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Scrima.Core.Query;
using Scrima.EntityFrameworkCore;
using Scrima.Integration.Tests.Models;

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
        public async Task<ActionResult<QueryResultDto<T>>> GetAsync(
            ScrimaQueryOptions<T> queryOptions,
            CancellationToken cancellationToken)
        {
            var queryResult =
                await _records.ToQueryResultAsync(queryOptions, GetSearchPredicate() , cancellationToken: cancellationToken);

            var dto = new QueryResultDto<T>
            {
                Results = queryResult.Results.ToList(),
                Count = queryResult.Count
            };

            return Ok(dto);
        }

        protected abstract Expression<Func<T, string, bool>> GetSearchPredicate();
    }
}
