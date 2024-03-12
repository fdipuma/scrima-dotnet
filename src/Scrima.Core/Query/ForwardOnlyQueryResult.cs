using System.Collections.Generic;

namespace Scrima.Core.Query;

/// <summary>
/// Results of a Scrima query which is forward only (uses skip token)
/// </summary>
/// <typeparam name="T">Item type</typeparam>
public class ForwardOnlyQueryResult<T> : QueryResult<T>
{
    public ForwardOnlyQueryResult(IEnumerable<T> results, long? count, string nextToken) : base(results, count)
    {
        NextToken = nextToken;
    }
        
    /// <summary>
    /// The next token to use for Skip Token
    /// </summary>
    public string NextToken { get; }
}