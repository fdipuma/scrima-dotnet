using System;

namespace Scrima.Core.Model
{
    /// <summary>
    /// Represents a collection type in the Entity Data Model.
    /// </summary>
    /// <seealso cref="EdmType" />
    [System.Diagnostics.DebuggerDisplay("{FullName}: {ClrType}")]
    public sealed class EdmCollectionType : EdmType
    {
        public EdmCollectionType(Type clrType, EdmType containedType)
            : base("Collection", $"Collection({containedType.FullName})", clrType)
        {
            ContainedType = containedType;
        }

        /// <summary>
        /// Gets the <see cref="EdmType"/> type contained in the collection.
        /// </summary>
        public EdmType ContainedType { get; }
    }
}