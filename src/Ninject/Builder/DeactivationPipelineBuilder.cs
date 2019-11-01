using Ninject.Activation.Strategies;
using System.Collections.Generic;

namespace Ninject.Builder
{
    internal class DeactivationPipelineBuilder : IDeactivationPipelineBuilder
    {
        public DeactivationPipelineBuilder(IComponentBindingRoot componentBindingRoot, IDictionary<string, object> properties)
        {
            this.Components = componentBindingRoot;
            this.Properties = properties;
        }

        /// <summary>
        /// Gets the component bindings that make up the activation pipeline.
        /// </summary>
        /// <value>
        /// The component bindings that make up the activation pipeline.
        /// </value>
        public IComponentBindingRoot Components { get; }

        /// <summary>
        /// Gets a key/value collection that can be used to share data between components.
        /// </summary>
        /// <value>
        /// A key/value collection that can be used to share data between components.
        /// </value>
        public IDictionary<string, object> Properties { get; }

        public IDeactivationPipelineBuilder BindingAction()
        {
            this.Components.Bind<IDeactivationStrategy>()
                           .To<BindingActionStrategy>()
                           .InSingletonScope();
            return this;
        }

        public IDeactivationPipelineBuilder Disposable()
        {
            this.Components.Bind<IDeactivationStrategy>()
                           .To<DisposableStrategy>()
                           .InSingletonScope();
            return this;
        }

        public IDeactivationPipelineBuilder Stoppable()
        {
            this.Components.Bind<IDeactivationStrategy>()
                           .To<StoppableStrategy>()
                           .InSingletonScope();
            return this;
        }
    }
}
