namespace Scrima.OData
{
    public class ODataQueryDefaultOptions
    {
        /// <summary>
        /// Default $top value when no value is provided in query string
        /// </summary>
        public int? DefaultTop { get; set; }
        
        /// <summary>
        /// Max allowed $top value
        /// </summary>
        public int? MaxTop { get; set; }
        
        /// <summary>
        /// Always show count in responses like $count=true
        /// </summary>
        public bool AlwaysShowCount { get; set; } = false;
    }
}
