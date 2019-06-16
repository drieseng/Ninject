namespace Ninject.Builder
{
    using System;

    using Ninject.Activation.Providers;
    using Ninject.Selection;

    internal abstract class ConstructorInjectionSelectorBuilder : IConstructorInjectionSelectorBuilder
    {
        private ConstructorReflectionSelectorBuilder selectorBuilder;

        /// <summary>
        /// Builds the constructor injection components.
        /// </summary>
        public virtual void Build(IComponentBindingRoot root)
        {
            if (this.selectorBuilder != null)
            {
                this.selectorBuilder.Build(root);
            }
            else
            {
                root.Bind<IConstructorReflectionSelector>().ToConstant(new ConstructorReflectionSelector());
            }

            // AND ONLY DO THIS IN KERNELBUILDER AS LAST FALLBACK ?
            if (!root.IsBound<IConstructorParameterValueProvider>())
            {
                root.Bind<IConstructorParameterValueProvider>().To<ConstructorParameterValueProvider>();
            }
        }

        /// <summary>
        /// Configures an <see cref="IConstructorReflectionSelector"/> to use for composing a list of constructors that
        /// can be used to instantiate a given service.
        /// </summary>
        /// <param name="selectorBuilder">A callback to configure an <see cref="IConstructorReflectionSelector"/>.</param>
        public void Selector(Action<IConstructorReflectionSelectorBuilder> selectorBuilder)
        {
            this.selectorBuilder = new ConstructorReflectionSelectorBuilder();
            selectorBuilder(this.selectorBuilder);
        }
    }
}
