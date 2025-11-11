namespace Scrima.OData.Swashbuckle;

public class ODataSwaggerOptions
{
    internal ODataSwaggerOptions()
    {
        AllowCount = true;
        AllowSkip = true;
        AllowSkipToken = false;
        AllowSearch = true;
        AllowTop = true;
        AllowOrderBy = true;
        AllowFilter = true;
    }
        
    /// <summary>
    /// Allows to show the $count parameter in schema for the given item type. Defaults to true. 
    /// </summary>
    public bool AllowCount { get; set; }
    /// <summary>
    /// Allows to show the $skip parameter in schema for the given item type. Defaults to true. 
    /// </summary>
    public bool AllowSkip { get; set; }
    /// <summary>
    /// Allows to show the $skiptoken parameter in schema for the given item type. Defaults to false. 
    /// </summary>
    public bool AllowSkipToken { get; set; }
    /// <summary>
    /// Allows to show the $search parameter in schema for the given item type. Defaults to true. 
    /// </summary>
    public bool AllowSearch { get; set; }
    /// <summary>
    /// Allows to show the $top parameter in schema for the given item type. Defaults to true. 
    /// </summary>
    public bool AllowTop { get; set; }
    /// <summary>
    /// Allows to show the $filter parameter in schema for the given item type. Defaults to true. 
    /// </summary>
    public bool AllowFilter { get; set; }
    /// <summary>
    /// Allows to show the $orderby parameter in schema for the given item type. Defaults to true. 
    /// </summary>
    public bool AllowOrderBy { get; set; }
}
