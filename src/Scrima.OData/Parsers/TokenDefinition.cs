using System.Text.RegularExpressions;

namespace Scrima.OData.Parsers;

[System.Diagnostics.DebuggerDisplay("{_tokenType}: {Regex}")]
internal readonly struct TokenDefinition
{
    private readonly TokenType _tokenType;

    internal TokenDefinition(TokenType tokenType, string expression, bool ignore = false)
    {
        _tokenType = tokenType;
        Regex = new Regex(@"\G" + expression, RegexOptions.Singleline);
        Ignore = ignore;
    }

    internal bool Ignore { get; }

    internal Regex Regex { get; }

    internal Token CreateToken(Match match) => new(match.Value, _tokenType);
}
