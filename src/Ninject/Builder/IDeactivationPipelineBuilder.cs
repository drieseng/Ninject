namespace Ninject.Builder
{
    using Ninject.Syntax;
    using System.Collections.Generic;
    using System.ComponentModel;

    public interface IDeactivationPipelineBuilder : IFluentSyntax
    {
        /// <summary>
        /// Gets the root of the component bindings.
        /// </summary>
        /// <value>
        /// The root of the component binding.
        /// </value>
        [EditorBrowsable(EditorBrowsableState.Never)]
        IComponentBindingRoot Components { get; }

        /// <summary>
        /// Gets a key/value collection that can be used to share data between components.
        /// </summary>
        /// <value>
        /// A key/value collection that can be used to share data between components.
        /// </value>
        [EditorBrowsable(EditorBrowsableState.Never)]
        IDictionary<string, object> Properties { get; }

        IDeactivationPipelineBuilder BindingAction();

        IDeactivationPipelineBuilder Disposable();

        IDeactivationPipelineBuilder Stoppable();
    }
}
