namespace Scrima.Core.Query
{
    /// <summary>
    /// The valid order by directions.
    /// </summary>
    public enum OrderByDirection
    {
        /// <summary>
        /// The results are to be filtered by the named property in ascending order.
        /// </summary>
        Ascending = 0,

        /// <summary>
        /// The results are to be filtered by the named property in descending order.
        /// </summary>
        Descending = 1,
    }
}