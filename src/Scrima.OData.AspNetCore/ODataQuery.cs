using System.Text;
using Microsoft.AspNetCore.Mvc;
using Scrima.Core.Query;

namespace Scrima.OData.AspNetCore
{
    /// <summary>
    /// Class used by the Api Descriptor to substitute
    /// original QueryOptions (for documentation) 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class ODataQuery<T> : ODataQuery
    {
        
    }
    
    public abstract class ODataQuery
    {
        [FromQuery(Name = "$filter")]
        public string Filter { get; internal set; }
        
        [FromQuery(Name = "$search")]
        public string Search { get; internal set; }
        
        [FromQuery(Name = "$count")]
        public bool? Count { get; internal set; }
        
        [FromQuery(Name = "$skip")]
        public long? Skip { get; internal set; }
        
        [FromQuery(Name = "$top")]
        public long? Top { get; internal set; }
        
        [FromQuery(Name = "$skiptoken")]
        public string SkipToken { get; internal set; }
        
        [FromQuery(Name = "$orderby")]
        public string OrderBy { get; internal set; }
        
        internal QueryOptions QueryOptions { get; set; }

        public QueryOptions ToQueryOptions() => QueryOptions;

        public static implicit operator QueryOptions(ODataQuery query) => query?.ToQueryOptions();

        public override string ToString()
        {
            var builder = new StringBuilder("?");

            if (Filter is not null)
            {
                builder.Append("$filter=");
                builder.Append(Filter);
                builder.Append('&');
            }

            if (Search is not null)
            {
                builder.Append("$search=");
                builder.Append(Search);
                builder.Append('&');
            }

            if (Count is not null)
            {
                builder.Append("$count=");
                builder.Append(Count);
                builder.Append('&');
            }

            if (Skip is not null)
            {
                builder.Append("$skip=");
                builder.Append(Skip);
                builder.Append('&');
            }

            if (Top is not null)
            {
                builder.Append("$top=");
                builder.Append(Top);
                builder.Append('&');
            }

            if (SkipToken is not null)
            {
                builder.Append("$skiptoken=");
                builder.Append(SkipToken);
                builder.Append('&');
            }

            if (OrderBy is not null)
            {
                builder.Append("$orderby=");
                builder.Append(OrderBy);
                builder.Append('&');
            }

            return builder.ToString(0, builder.Length > 0 ? builder.Length - 1 : builder.Length);
        }
    }
}
