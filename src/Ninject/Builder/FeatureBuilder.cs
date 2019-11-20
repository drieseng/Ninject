using System.Collections.Generic;

namespace Ninject.Builder
{
    internal class FeatureBuilder : IFeatureBuilder
    {
        public FeatureBuilder(IComponentBindingRoot components, Dictionary<string, object> properties)
        {
            this.Components = components;
            this.Properties = properties;
        }

        /// <summary>
        /// Gets the root of the component bindings.
        /// </summary>
        /// <value>
        /// The root of the component binding.
        /// </value>
        public IComponentBindingRoot Components { get; }

        /// <summary>
        /// Gets a key/value collection that can be used to share data between components.
        /// </summary>
        /// <value>
        /// A key/value collection that can be used to share data between components.
        /// </value>
        public IDictionary<string, object> Properties { get; }
    }
}
