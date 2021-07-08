﻿
namespace Loupedeck.AudioDevicePlugin.Settings
{
    using System.Collections.Generic;
    using System.Configuration;

    /// <summary>
    /// A collection of jit group element.
    /// </summary>
    [ConfigurationCollection(typeof(CustomImageElement))]
    public class CustomImageElementCollection : ConfigurationElementCollection, IEnumerable<CustomImageElement>
    {
        /// <summary>
        /// Gets the current collection type.
        /// </summary>
        public override ConfigurationElementCollectionType CollectionType
        {
            get
            {
                return ConfigurationElementCollectionType.BasicMap;
            }
        }

        /// <summary>
        /// Gets the element name for jitgroups elements.
        /// </summary>
        protected override string ElementName
        {
            get
            {
                return "customImage";
            }
        }

        /// <summary>
        /// Gets an enumerator that iterates through the <see cref="JitGroupElement"/> collection.
        /// </summary>
        /// <returns>A <see cref="JitGroupElement"/> enumerator.</returns>
        IEnumerator<CustomImageElement> IEnumerable<CustomImageElement>.GetEnumerator()
        {
            for (int i = 0; i < this.Count; i++)
            {
                yield return (CustomImageElement)this.BaseGet(i);
            }
        }

        /// <summary>
        /// Craets a new <see cref="JitGroupElement"/>elemment.
        /// </summary>
        /// <returns>A newly created element.</returns>
        protected override ConfigurationElement CreateNewElement()
        {
            return new CustomImageElement();
        }

        /// <summary>
        ///   Gets the element key for a specified configuration element.
        /// </summary>
        /// <param name="element">the config element.</param>
        /// <returns>A <see cref="JitGroupElement"/> object. </returns>
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((CustomImageElement)element).DeviceName;
        }
    }
}
