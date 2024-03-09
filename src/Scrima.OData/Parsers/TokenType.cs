namespace Scrima.OData.Parsers;

internal enum TokenType
{
    OpenParentheses,
    CloseParentheses,
    And,
    Or,
    True,
    False,
    Null,
    BinaryOperator,
    Comma,
    Whitespace,
    Decimal,
    Double,
    Integer,
    Single,
    Date,
    DateTimeOffset,
    Guid,
    String,
    FunctionName,
    PropertyName,
    UnaryOperator,
    TimeOfDay,
    Duration,
    Enum
}