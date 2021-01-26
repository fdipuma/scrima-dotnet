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
    }
}
