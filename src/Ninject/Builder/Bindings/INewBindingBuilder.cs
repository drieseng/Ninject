using Ninject.Infrastructure;
using Ninject.Planning.Bindings;
using Ninject.Syntax;
using System;

namespace Ninject.Builder.Bindings
{
    public interface INewBindingBuilder
    {
        /// <summary>
        /// Gets the <see cref="Type"/> of the service to bind.
        /// </summary>
        /// <value>
        /// The <see cref="Type"/> of the service to bind.
        /// </value>
        Type Service { get; }

        /// <summary>
        /// Gets the names of the services that this instance builds a binding for.
        /// </summary>
        /// <value>
        /// The names of the services that this instance builds a binding for.
        /// </value>
        string ServiceNames { get; }

        /// <summary>
        /// Gets the binding being built.
        /// </summary>
        INewBindingConfigurationBuilder BindingConfigurationBuilder { get; }

        /// <summary>
        /// Builds the binding(s) of this instance.
        /// </summary>
        /// <param name="root">The resolution root.</param>
        /// <param name="bindingVisitor">Gathers the bindings that are built from this <see cref="INewBindingBuilder"/>.</param>
        void Build(IResolutionRoot root, IVisitor<IBinding> bindingVisitor);
    }
}
