using Scrima.Core.Query.Expressions;

namespace Scrima.OData.Parsers;

internal static class UnaryOperatorKindParser
{
    internal static UnaryOperatorKind ToUnaryOperatorKind(this string operatorType)
    {
        switch (operatorType)
        {
            case "not":
                return UnaryOperatorKind.Not;

            default:
                throw new ODataParseException(string.Format(Messages.UnknownOperator, operatorType));
        }
    }
}
