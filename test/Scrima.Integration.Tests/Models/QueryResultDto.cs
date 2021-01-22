using System.Collections.Generic;

namespace Scrima.Integration.Tests.Models
{
    public class QueryResultDto<T>
    {
        public List<T> Results { get; set; }
        public long? Count { get; set; }
    }
}