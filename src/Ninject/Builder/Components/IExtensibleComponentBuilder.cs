namespace Ninject.Builder.Components
{
    using System.Collections.Generic;
    using System.ComponentModel;

    public interface IExtensibleComponentBuilder : IComponentBuilder
    {
        /// <summary>
        /// Gets the registered component builders.
        /// </summary>
        /// <value>
        /// The registered component builders.
        /// </value>
        /// <remarks>
        /// This acts as the extensibility point to allow additional builders to be registered, and existing builders to be
        /// removed or updated.
        /// </remarks>
        [EditorBrowsable(EditorBrowsableState.Never)]
        IList<IComponentBuilder> Components { get; }
    }
}
