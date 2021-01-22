using System;

namespace Scrima.OData.Swashbuckle
{
    public class ScrimaSwaggerOptions
    {
        public Action<ODataSwaggerOptions, Type> ConfigureOptionsPerType { get; set; }
    }
}
