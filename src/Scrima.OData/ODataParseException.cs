using System;

namespace Scrima.OData;

/// <summary>
/// An exception which is thrown when an OData request is not parsed correctly.
/// </summary>
[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1032:ImplementStandardExceptionConstructors", Justification = "We don't need them for this type of exception")]
[Serializable]
public sealed class ODataParseException : Exception
{
    /// <summary>
    /// Initialises a new instance of the <see cref="ODataParseException" /> class.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public ODataParseException(string message) : base(message)
    {
    }
}