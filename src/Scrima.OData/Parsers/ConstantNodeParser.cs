using System;
using System.Globalization;
using Scrima.Core;
using Scrima.Core.Model;
using Scrima.Core.Query.Expressions;

namespace Scrima.OData.Parsers;

internal static class ConstantNodeParser
{
    private const string ODataDateFormat = "yyyy-MM-dd";

    internal static ConstantNode ParseConstantNode(Token token, EdmTypeProvider typeProvider)
    {
        switch (token.TokenType)
        {
            case TokenType.Date:
                var dateOnlyValue = DateOnly.ParseExact(token.Value, ODataDateFormat, CultureInfo.InvariantCulture);
                return ConstantNode.Date(token.Value, dateOnlyValue);

            case TokenType.DateTimeOffset:
                var dateTimeOffsetValue = DateTimeOffset.Parse(token.Value, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal);
                return ConstantNode.DateTimeOffset(token.Value, dateTimeOffsetValue);

            case TokenType.Decimal:
                var decimalText = token.Value.Substring(0, token.Value.Length - 1);
                var decimalValue = decimal.Parse(decimalText, CultureInfo.InvariantCulture);
                return ConstantNode.Decimal(token.Value, decimalValue);

            case TokenType.Double:
                var doubleText = token.Value.EndsWith("d", StringComparison.OrdinalIgnoreCase)
                    ? token.Value.Substring(0, token.Value.Length - 1)
                    : token.Value;
                var doubleValue = double.Parse(doubleText, CultureInfo.InvariantCulture);
                return ConstantNode.Double(token.Value, doubleValue);

            case TokenType.Duration:
                var durationText = token.Value.Substring(9, token.Value.Length - 10)
                    .Replace("P", string.Empty)
                    .Replace("DT", ".")
                    .Replace("H", ":")
                    .Replace("M", ":")
                    .Replace("S", string.Empty);
                var durationTimeSpanValue = TimeSpan.Parse(durationText, CultureInfo.InvariantCulture);
                return ConstantNode.Duration(token.Value, durationTimeSpanValue);

            case TokenType.Enum:
                var firstQuote = token.Value.IndexOf('\'');
                var edmEnumTypeName = token.Value.Substring(0, firstQuote);
                var edmEnumType = (EdmEnumType)typeProvider.GetByName(edmEnumTypeName);
                var edmEnumMemberName = token.Value.Substring(firstQuote + 1, token.Value.Length - firstQuote - 2);
                var enumValue = edmEnumType.GetClrValue(edmEnumMemberName);

                return new ConstantNode(edmEnumType, token.Value, enumValue);

            case TokenType.False:
                return ConstantNode.False;

            case TokenType.Guid:
                var guidValue = Guid.ParseExact(token.Value, "D");
                return ConstantNode.Guid(token.Value, guidValue);

            case TokenType.Integer:
                var integerText = token.Value;

                switch (integerText)
                {
                    case "0":
                        return ConstantNode.Int32Zero;
                    case "0l" or "0L":
                        return ConstantNode.Int64Zero;
                }

                var is64BitSuffix = integerText.EndsWith("l", StringComparison.OrdinalIgnoreCase);

                if (!is64BitSuffix && int.TryParse(integerText, out var int32Value))
                {
                    return ConstantNode.Int32(token.Value, int32Value);
                }

                var int64Text = !is64BitSuffix ? integerText : integerText.Substring(0, integerText.Length - 1);
                var int64Value = long.Parse(int64Text, CultureInfo.InvariantCulture);
                return ConstantNode.Int64(token.Value, int64Value);

            case TokenType.Null:
                return ConstantNode.Null;

            case TokenType.Single:
                var singleText = token.Value.Substring(0, token.Value.Length - 1);
                var singleValue = float.Parse(singleText, CultureInfo.InvariantCulture);
                return ConstantNode.Single(token.Value, singleValue);

            case TokenType.String:
                var stringText = UnescapeStringText(token.Value);
                return ConstantNode.String(token.Value, stringText);

            case TokenType.TimeOfDay:
                var timeSpanTimeOfDayValue = TimeSpan.Parse(token.Value, CultureInfo.InvariantCulture);
                return ConstantNode.Time(token.Value, timeSpanTimeOfDayValue);

            case TokenType.True:
                return ConstantNode.True;

            default:
                throw new NotSupportedException(token.TokenType.ToString());
        }
    }

    private static string UnescapeStringText(string tokenValue)
    {
        return tokenValue
                .Trim('\'')
                .Replace("''", "'")
                .Replace("%2B", "+")
                .Replace("%2F", "/")
                .Replace("%3F", "?")
                .Replace("%25", "%")
                .Replace("%23", "#")
                .Replace("%26", "&");
    }
}
