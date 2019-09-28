using Ninject.Activation.Strategies;
using Ninject.Builder.Components;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Ninject.Builder
{
    internal class ActivationPipelineBuilder : IActivationPipelineBuilder
    {
        private readonly List<IComponentBuilder> components;

        public ActivationPipelineBuilder()
        {
            this.components = new List<IComponentBuilder>();
        }

        public IActivationPipelineBuilder BindingAction()
        {
            components.Add(new ComponentBuilder<IActivationStrategy, BindingActionStrategy>());
            return this;
        }

        public IActivationPipelineBuilder Startable()
        {
            components.Add(new ComponentBuilder<IActivationStrategy, StartableStrategy>());
            return this;
        }

        public void Build(IComponentBindingRoot root)
        {
            if (this.components.Count == 0)
            {
                return;
            }

            // If any activation strategy is defined, make sure to register the ActivationCacheStrategy as
            // first in the activation pipeline
            root.Bind<IActivationStrategy>().To<ActivationCacheStrategy>();

            foreach (var component in this.components)
            {
                component.Build(root);
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        IActivationPipelineBuilder IActivationPipelineBuilder.AddStage(Func<IComponentBuilder> componentDelegate)
        {
            this.components.Add(componentDelegate());
            return this;
        }
    }
}
