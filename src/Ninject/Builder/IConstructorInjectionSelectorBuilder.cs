using Ninject.Selection;
namespace Ninject.Builder
{
    using System;

    public interface IConstructorInjectionSelectorBuilder : IComponentBuilder
    {
        /// <summary>
        /// Configures an <see cref="IConstructorReflectionSelector"/> to use for composing a list of constructors that
        /// can be used to instantiate a given service.
        /// </summary>
        /// <param name="selectorBuilder">A callback to configure an <see cref="IConstructorReflectionSelector"/>.</param>
        void Selector(Action<IConstructorReflectionSelectorBuilder> selectorBuilder);
    }
}
