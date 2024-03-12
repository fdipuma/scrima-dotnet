namespace Scrima.OData.Parsers;

[System.Diagnostics.DebuggerDisplay("{TokenType}: {Value}")]
internal struct Token
{
    internal Token(string value, TokenType tokenType)
    {
        Value = value;
        TokenType = tokenType;
    }

    internal TokenType TokenType { get; }

    internal string Value { get; }
}