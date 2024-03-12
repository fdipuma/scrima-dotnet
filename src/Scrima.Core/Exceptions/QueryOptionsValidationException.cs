using System;
using System.Runtime.Serialization;

namespace Scrima.Core.Exceptions;

/// <summary>
/// An exception which is thrown when a query options object is invalid.
/// </summary>
[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1032:ImplementStandardExceptionConstructors", Justification = "We don't need them for this type of exception")]
[Serializable]
public sealed class QueryOptionsValidationException : Exception
{
    /// <summary>
    /// Initialises a new instance of the <see cref="QueryOptionsValidationException" /> class.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="target">The target of the exception.</param>
    public QueryOptionsValidationException(string message, string target = null)
        : base(message)
    {
        Target = target;
    }

    private QueryOptionsValidationException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
        Target = info.GetString("Target");
    }

    /// <summary>
    /// Gets or sets the target of the exception.
    /// </summary>
    public string Target { get; set; }

    /// <summary>
    /// sets the System.Runtime.Serialization.SerializationInfo with information about the exception.
    /// </summary>
    /// <param name="info">The System.Runtime.Serialization.SerializationInfo that holds the serialized object data about the exception being thrown.</param>
    /// <param name="context">The System.Runtime.Serialization.StreamingContext that contains contextual information about the source or destination.</param>
    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        info.AddValue("Target", Target);
        base.GetObjectData(info, context);
    }
}