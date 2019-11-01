namespace Ninject.Builder
{
    using System.Collections.Generic;

    public interface IActivationPipelineBuilder
    {
        /// <summary>
        /// Gets the root of the component bindings.
        /// </summary>
        /// <value>
        /// The root of the component binding.
        /// </value>
        IComponentBindingRoot Components { get; }

        /// <summary>
        /// Gets a key/value collection that can be used to share data between components.
        /// </summary>
        /// <value>
        /// A key/value collection that can be used to share data between components.
        /// </value>
        IDictionary<string, object> Properties { get; }

        IActivationPipelineBuilder Startable();

        IActivationPipelineBuilder BindingAction();
    }
}
