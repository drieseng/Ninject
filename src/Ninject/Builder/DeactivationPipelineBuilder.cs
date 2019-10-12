using Ninject.Activation.Strategies;
using Ninject.Builder.Components;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Ninject.Builder
{
    internal class DeactivationPipelineBuilder : IDeactivationPipelineBuilder
    {
        private readonly List<IComponentBuilder> components;

        public DeactivationPipelineBuilder()
        {
            this.components = new List<IComponentBuilder>();
        }

        public IDeactivationPipelineBuilder BindingAction()
        {
            components.Add(new ComponentBuilder<IDeactivationStrategy, BindingActionStrategy>());
            return this;
        }

        public IDeactivationPipelineBuilder Disposable()
        {
            components.Add(new ComponentBuilder<IDeactivationStrategy, DisposableStrategy>());
            return this;
        }

        public IDeactivationPipelineBuilder Stoppable()
        {
            components.Add(new ComponentBuilder<IDeactivationStrategy, StoppableStrategy>());
            return this;
        }

        public void Build(IComponentBindingRoot root)
        {
            if (this.components.Count == 0)
            {
                return;
            }

            // If any deactivation strategy is defined, make sure to register the DeactivationCacheStrategy as
            // first in the deactivation pipeline
            root.Bind<IDeactivationStrategy>().To<DeactivationCacheStrategy>();

            foreach (var component in this.components)
            {
                component.Build(root);
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        IDeactivationPipelineBuilder IDeactivationPipelineBuilder.AddStage(Func<IComponentBuilder> componentDelegate)
        {
            this.components.Add(componentDelegate());
            return this;
        }
    }
}
