using System;
using Scrima.Core.Model;

namespace Scrima.Core.Query.Expressions;

/// <summary>
/// A QueryNode which represents a constant value.
/// </summary>
[System.Diagnostics.DebuggerDisplay("{LiteralText}")]
public sealed class ConstantNode : ValueNode
{
    /// <summary>
    /// Initialises a new instance of the <see cref="ConstantNode" /> class.
    /// </summary>
    /// <param name="edmType">The <see cref="EdmType"/> of the value.</param>
    /// <param name="literalText">The literal text.</param>
    /// <param name="value">The value.</param>
    public ConstantNode(EdmType edmType, string literalText, object value)
    {
        EdmType = edmType;
        LiteralText = literalText;
        Value = value;
    }

    /// <summary>
    /// Gets the <see cref="EdmType"/> of the value.
    /// </summary>
    public EdmType EdmType { get; }

    /// <summary>
    /// Gets the <see cref="EdmType"/> of the value.
    /// </summary>
    public override EdmType EdmValueType => EdmType;

    /// <summary>
    /// Gets the kind of query node.
    /// </summary>
    public override QueryNodeKind Kind => QueryNodeKind.Constant;

    /// <summary>
    /// Gets the literal text if the constant value.
    /// </summary>
    public string LiteralText { get; }

    /// <summary>
    /// Gets the constant value as an object.
    /// </summary>
    public object Value { get; }

    public override string ToString() => LiteralText;

    /// <summary>
    /// Gets the ConstantNode which represents a value of false.
    /// </summary>
    public static ConstantNode False { get; } = new ConstantNode(EdmPrimitiveType.Boolean, "false", false);

    /// <summary>
    /// Gets the ConstantNode which represents a 32bit integer value of 0.
    /// </summary>
    public static ConstantNode Int32Zero { get; } = new ConstantNode(EdmPrimitiveType.Int32, "0", 0);

    /// <summary>
    /// Gets the ConstantNode which represents a 64bit integer value of 0.
    /// </summary>
    public static ConstantNode Int64Zero { get; } = new ConstantNode(EdmPrimitiveType.Int64, "0L", 0L);

    /// <summary>
    /// Gets the ConstantNode which represents a value of null.
    /// </summary>
    public static ConstantNode Null { get; } = new ConstantNode(null, "null", null);

    /// <summary>
    /// Gets the ConstantNode which represents a value of true.
    /// </summary>
    public static ConstantNode True { get; } = new ConstantNode(EdmPrimitiveType.Boolean, "true", true);

    /// <summary>
    /// Gets a ConstantNode which represents a Date value.
    /// </summary>
    /// <param name="literalText">The literal text.</param>
    /// <param name="value">The value.</param>
    /// <returns>A ConstantNode representing a Date value.</returns>
    public static ConstantNode Date(string literalText, DateOnly value) =>
        new ConstantNode(EdmPrimitiveType.Date, literalText, value);

    /// <summary>
    /// Gets a ConstantNode which represents a DateTimeOffset value.
    /// </summary>
    /// <param name="literalText">The literal text.</param>
    /// <param name="value">The value.</param>
    /// <returns>A ConstantNode representing a DateTimeOffset value.</returns>
    public static ConstantNode DateTimeOffset(string literalText, DateTimeOffset value) =>
        new ConstantNode(EdmPrimitiveType.DateTimeOffset, literalText, value);

    /// <summary>
    /// Gets a ConstantNode which represents a decimal value.
    /// </summary>
    /// <param name="literalText">The literal text.</param>
    /// <param name="value">The value.</param>
    /// <returns>A ConstantNode representing a decimal value.</returns>
    public static ConstantNode Decimal(string literalText, decimal value) =>
        new ConstantNode(EdmPrimitiveType.Decimal, literalText, value);

    /// <summary>
    /// Gets a ConstantNode which represents a double value.
    /// </summary>
    /// <param name="literalText">The literal text.</param>
    /// <param name="value">The value.</param>
    /// <returns>A ConstantNode representing a double value.</returns>
    public static ConstantNode Double(string literalText, double value) =>
        new ConstantNode(EdmPrimitiveType.Double, literalText, value);

    /// <summary>
    /// Gets a ConstantNode which represents a duration value.
    /// </summary>
    /// <param name="literalText">The literal text.</param>
    /// <param name="value">The value.</param>
    /// <returns>A ConstantNode representing a duration value.</returns>
    public static ConstantNode Duration(string literalText, TimeSpan value) =>
        new ConstantNode(EdmPrimitiveType.Duration, literalText, value);

    /// <summary>
    /// Gets a ConstantNode which represents a Guid value.
    /// </summary>
    /// <param name="literalText">The literal text.</param>
    /// <param name="value">The value.</param>
    /// <returns>A ConstantNode representing a Guid value.</returns>
    public static ConstantNode Guid(string literalText, Guid value) =>
        new ConstantNode(EdmPrimitiveType.Guid, literalText, value);

    /// <summary>
    /// Gets a ConstantNode which represents a 32 bit signed integer value.
    /// </summary>
    /// <param name="literalText">The literal text.</param>
    /// <param name="value">The value.</param>
    /// <returns>A ConstantNode representing a 32 bit signed integer value.</returns>
    public static ConstantNode Int32(string literalText, int value) =>
        new ConstantNode(EdmPrimitiveType.Int32, literalText, value);

    /// <summary>
    /// Gets a ConstantNode which represents a 64 bit signed integer value.
    /// </summary>
    /// <param name="literalText">The literal text.</param>
    /// <param name="value">The value.</param>
    /// <returns>A ConstantNode representing a 64 bit signed integer value.</returns>
    public static ConstantNode Int64(string literalText, long value) =>
        new ConstantNode(EdmPrimitiveType.Int64, literalText, value);

    /// <summary>
    /// Gets a ConstantNode which represents a float value.
    /// </summary>
    /// <param name="literalText">The literal text.</param>
    /// <param name="value">The value.</param>
    /// <returns>A ConstantNode representing a float value.</returns>
    public static ConstantNode Single(string literalText, float value) =>
        new ConstantNode(EdmPrimitiveType.Single, literalText, value);

    /// <summary>
    /// Gets a ConstantNode which represents a string value.
    /// </summary>
    /// <param name="literalText">The literal text.</param>
    /// <param name="value">The value.</param>
    /// <returns>A ConstantNode representing a string value.</returns>
    public static ConstantNode String(string literalText, string value) =>
        new ConstantNode(EdmPrimitiveType.String, literalText, value);

    /// <summary>
    /// Gets a ConstantNode which represents a time value.
    /// </summary>
    /// <param name="literalText">The literal text.</param>
    /// <param name="value">The value.</param>
    /// <returns>A ConstantNode representing a time value.</returns>
    public static ConstantNode Time(string literalText, TimeSpan value) =>
        new ConstantNode(EdmPrimitiveType.TimeOfDay, literalText, value);

    /// <summary>
    /// Gets a ConstantNode which represents a time value.
    /// </summary>
    /// <param name="elements"></param>
    /// <returns>A ConstantNode representing a time value.</returns>
    public static ArrayNode Array(params ConstantNode[] elements) => new ArrayNode(elements);
}
