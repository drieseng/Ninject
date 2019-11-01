using System.Collections.Generic;

namespace Ninject.Builder
{
    internal class FeatureBuilder : IFeatureBuilder
    {
        public FeatureBuilder(IComponentBindingRoot components)
        {
            this.Components = components;
            this.Properties = new Dictionary<string, object>();
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
