using Microsoft.AspNetCore.Mvc;

namespace Scrima.OData.AspNetCore
{
    /// <summary>
    /// Class used by the Api Descriptor to substitute
    /// original QueryOptions (for documentation) 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class ODataQueryParams<T>
    {
        [FromQuery(Name = "$filter")]
        public string Filter { get; set; }
        
        [FromQuery(Name = "$search")]
        public string Search { get; set; }
        
        [FromQuery(Name = "$count")]
        public string Count { get; set; }
        
        [FromQuery(Name = "$skip")]
        public string Skip { get; set; }
        
        [FromQuery(Name = "$top")]
        public string Top { get; set; }
        
        [FromQuery(Name = "$skiptoken")]
        public string SkipToken { get; set; }
        
        [FromQuery(Name = "$orderby")]
        public string OrderBy { get; set; }
    }
}
