using System;

namespace Scrima.Core.Model
{
    /// <summary>
    /// A class which represents an entity property in the Entity Data Model.
    /// </summary>
    [System.Diagnostics.DebuggerDisplay("{Name}")]
    public sealed class EdmProperty
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="EdmProperty" /> class.
        /// </summary>
        /// <param name="name">The name of the property.</param>
        /// <param name="propertyType">Type of the edm.</param>
        /// <param name="declaringType">Type of the declaring.</param>
        /// <exception cref="System.ArgumentException">Property name must be specified</exception>
        /// <exception cref="System.ArgumentNullException">Property type and declaring type must be specified.</exception>
        public EdmProperty(string name, EdmType propertyType, EdmComplexType declaringType)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Property name must be specified", nameof(name));
            }

            Name = name;
            PropertyType = propertyType ?? throw new ArgumentNullException(nameof(propertyType));
            DeclaringType = declaringType ?? throw new ArgumentNullException(nameof(declaringType));
        }

        /// <summary>
        /// Gets the type in the Entity Data Model which declares this property.
        /// </summary>
        public EdmComplexType DeclaringType { get; }

        /// <summary>
        /// Gets the name of the property.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the type of the property in the Entity Data Model.
        /// </summary>
        public EdmType PropertyType { get; }

        /// <summary>
        /// Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString() => Name;
    }
}