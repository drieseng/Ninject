namespace Ninject.Syntax
{
    using System;

    using Ninject.Activation;

    /// <summary>
    /// Used to add additional actions to be performed during initialization of instances via a binding.
    /// </summary>
    /// <typeparam name="T">The service being bound.</typeparam>
    public interface IBindingOnInitializationSyntax<T>
    {
        /// <summary>
        /// Indicates that the specified callback should be invoked when instances are initialized.
        /// </summary>
        /// <param name="action">The action callback.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        IBindingOnInitializationSyntax<T> OnInitialization(Action<T> action);

        /// <summary>
        /// Indicates that the specified callback should be invoked when instances are initialized.
        /// </summary>
        /// <typeparam name="TImplementation">The type of the implementation.</typeparam>
        /// <param name="action">The action callback.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        IBindingOnInitializationSyntax<T> OnInitialization<TImplementation>(Action<TImplementation> action);

        /// <summary>
        /// Indicates that the specified callback should be invoked when instances are initialized.
        /// </summary>
        /// <param name="action">The action callback.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        IBindingOnInitializationSyntax<T> OnInitialization(Action<IContext, T> action);

        /// <summary>
        /// Indicates that the specified callback should be invoked when instances are initialized.
        /// </summary>
        /// <typeparam name="TImplementation">The type of the implementation.</typeparam>
        /// <param name="action">The action callback.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        IBindingOnInitializationSyntax<T> OnInitialization<TImplementation>(Action<IContext, TImplementation> action);

        /// <summary>
        /// Indicates that the specified callback should be invoked when instances are initialized.
        /// </summary>
        /// <typeparam name="TImplementation">The type of the implementation.</typeparam>
        /// <param name="action">The action callback.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        IBindingOnInitializationSyntax<T> OnInitialization<TImplementation>(Func<IContext, TImplementation, TImplementation> action);
    }
}
