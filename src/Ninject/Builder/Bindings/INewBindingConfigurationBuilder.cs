using Ninject.Planning.Bindings;
using Ninject.Syntax;

namespace Ninject.Builder.Bindings
{
    /// <summary>
    /// Builds a <see cref="IBindingConfiguration"/>.
    /// </summary>
    public interface INewBindingConfigurationBuilder
    {
        /// <summary>
        /// Builds the binding configuration.
        /// </summary>
        /// <param name="root">The resolution root.</param>
        /// <returns>
        /// The binding configuration.
        /// </returns>
        IBindingConfiguration Build(IResolutionRoot root);

        /// <summary>
        /// Returns a value indicating whether any activation actions are defined for the current binding.
        /// </summary>
        /// <value>
        /// <see langword="true"/> if at least one activation action is defined for the current binding;
        /// otherwise, <see langword="false"/>.
        /// </value>
        bool HasActivationActions { get; }

        /// <summary>
        /// Returns a value indicating whether any deactivation actions are defined for the current binding.
        /// </summary>
        /// <value>
        /// <see langword="true"/> if at least one deactivation action is defined for the current binding;
        /// otherwise, <see langword="false"/>.
        /// </value>
        bool HasDeactivationActions { get; }

        /// <summary>
        /// Returns a value indicating whether any initialization actions are defined for the current binding.
        /// </summary>
        /// <value>
        /// <see langword="true"/> if at least one initialization action is defined for the current binding;
        /// otherwise, <see langword="false"/>.
        /// </value>
        bool HasInitializationActions { get; }
    }
}
