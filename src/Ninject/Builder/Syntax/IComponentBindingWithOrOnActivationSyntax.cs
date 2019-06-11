namespace Ninject.Builder.Syntax
{
    using System;

    using Ninject.Activation;
    using Ninject.Parameters;
    using Ninject.Planning.Targets;

    public interface IComponentBindingWithOrOnActivationSyntax<T>
    {
        /// <summary>
        /// Indicates that the specified constructor argument should be overridden with the specified value.
        /// </summary>
        /// <param name="name">The name of the argument to override.</param>
        /// <param name="value">The value for the argument.</param>
        /// <returns>The fluent syntax.</returns>
        IComponentBindingWithOrOnActivationSyntax<T> WithConstructorArgument(string name, object value);

        /// <summary>
        /// Indicates that the specified constructor argument should be overridden with the specified value.
        /// </summary>
        /// <param name="name">The name of the argument to override.</param>
        /// <param name="callback">The callback to invoke to get the value for the argument.</param>
        /// <returns>The fluent syntax.</returns>
        IComponentBindingWithOrOnActivationSyntax<T> WithConstructorArgument(string name, Func<IContext, object> callback);

        /// <summary>
        /// Indicates that the specified constructor argument should be overridden with the specified value.
        /// </summary>
        /// <param name="name">The name of the argument to override.</param>
        /// <param name="callback">The callback to invoke to get the value for the argument.</param>
        /// <returns>The fluent syntax.</returns>
        IComponentBindingWithOrOnActivationSyntax<T> WithConstructorArgument(string name, Func<IContext, ITarget<IConstructorArgument>, object> callback);

        /// <summary>
        /// Indicates that the specified constructor argument should be overridden with the specified value.
        /// </summary>
        /// <typeparam name="TValue">Specifies the argument type to override.</typeparam>
        /// <param name="value">The value for the argument.</param>
        /// <returns>The fluent syntax.</returns>
        IComponentBindingWithOrOnActivationSyntax<T> WithConstructorArgument<TValue>(TValue value);

        /// <summary>
        /// Indicates that the specified constructor argument should be overridden with the specified value.
        /// </summary>
        /// <param name="type">The type of the argument to override.</param>
        /// <param name="value">The value for the argument.</param>
        /// <returns>The fluent syntax.</returns>
        IComponentBindingWithOrOnActivationSyntax<T> WithConstructorArgument(Type type, object value);

        /// <summary>
        /// Indicates that the specified constructor argument should be overridden with the specified value.
        /// </summary>
        /// <typeparam name="TValue">The type of the argument to override.</typeparam>
        /// <param name="callback">The callback to invoke to get the value for the argument.</param>
        /// <returns>The fluent syntax.</returns>
        IComponentBindingWithOrOnActivationSyntax<T> WithConstructorArgument<TValue>(Func<IContext, TValue> callback);

        /// <summary>
        /// Indicates that the specified constructor argument should be overridden with the specified value.
        /// </summary>
        /// <param name="type">The type of the argument to override.</param>
        /// <param name="callback">The callback to invoke to get the value for the argument.</param>
        /// <returns>The fluent syntax.</returns>
        IComponentBindingWithOrOnActivationSyntax<T> WithConstructorArgument(Type type, Func<IContext, object> callback);

        /// <summary>
        /// Indicates that the specified constructor argument should be overridden with the specified value.
        /// </summary>
        /// <typeparam name="TValue">The type of the argument to override.</typeparam>
        /// <param name="callback">The callback to invoke to get the value for the argument.</param>
        /// <returns>The fluent syntax.</returns>
        IComponentBindingWithOrOnActivationSyntax<T> WithConstructorArgument<TValue>(Func<IContext, ITarget<IConstructorArgument>, TValue> callback);

        /// <summary>
        /// Indicates that the specified constructor argument should be overridden with the specified value.
        /// </summary>
        /// <param name="type">The type of the argument to override.</param>
        /// <param name="callback">The callback to invoke to get the value for the argument.</param>
        /// <returns>The fluent syntax.</returns>
        IComponentBindingWithOrOnActivationSyntax<T> WithConstructorArgument(Type type, Func<IContext, ITarget<IConstructorArgument>, object> callback);

        /// <summary>
        /// Indicates that the specified property should be injected with the specified value.
        /// </summary>
        /// <param name="name">The name of the property to override.</param>
        /// <param name="value">The value for the property.</param>
        /// <returns>The fluent syntax.</returns>
        IComponentBindingWithOrOnActivationSyntax<T> WithPropertyValue(string name, object value);

        /// <summary>
        /// Indicates that the specified property should be injected with the specified value.
        /// </summary>
        /// <param name="name">The name of the property to override.</param>
        /// <param name="callback">The callback to invoke to get the value for the property.</param>
        /// <returns>The fluent syntax.</returns>
        IComponentBindingWithOrOnActivationSyntax<T> WithPropertyValue(string name, Func<IContext, object> callback);

        /// <summary>
        /// Indicates that the specified property should be injected with the specified value.
        /// </summary>
        /// <param name="name">The name of the property to override.</param>
        /// <param name="callback">The callback to invoke to get the value for the property.</param>
        /// <returns>The fluent syntax.</returns>
        IComponentBindingWithOrOnActivationSyntax<T> WithPropertyValue(string name, Func<IContext, ITarget<IPropertyValue>, object> callback);

        /// <summary>
        /// Indicates that the specified callback should be invoked when instances are activated.
        /// </summary>
        /// <param name="action">The action callback.</param>
        /// <returns>The fluent syntax.</returns>
        IComponentBindingWithOrOnActivationSyntax<T> OnActivation(Action<T> action);

        /// <summary>
        /// Indicates that the specified callback should be invoked when instances are activated.
        /// </summary>
        /// <typeparam name="TImplementation">The type of the implementation.</typeparam>
        /// <param name="action">The action callback.</param>
        /// <returns>The fluent syntax.</returns>
        IComponentBindingWithOrOnActivationSyntax<T> OnActivation<TImplementation>(Action<TImplementation> action);

        /// <summary>
        /// Indicates that the specified callback should be invoked when instances are activated.
        /// </summary>
        /// <param name="action">The action callback.</param>
        /// <returns>The fluent syntax.</returns>
        IComponentBindingWithOrOnActivationSyntax<T> OnActivation(Action<IContext, T> action);

        /// <summary>
        /// Indicates that the specified callback should be invoked when instances are activated.
        /// </summary>
        /// <typeparam name="TImplementation">The type of the implementation.</typeparam>
        /// <param name="action">The action callback.</param>
        /// <returns>The fluent syntax.</returns>
        IComponentBindingWithOrOnActivationSyntax<T> OnActivation<TImplementation>(Action<IContext, TImplementation> action);

        /// <summary>
        /// Indicates that the specified callback should be invoked when instances are deactivated.
        /// </summary>
        /// <param name="action">The action callback.</param>
        /// <returns>The fluent syntax.</returns>
        IComponentBindingWithOrOnActivationSyntax<T> OnDeactivation(Action<T> action);

        /// <summary>
        /// Indicates that the specified callback should be invoked when instances are deactivated.
        /// </summary>
        /// <typeparam name="TImplementation">The type of the implementation.</typeparam>
        /// <param name="action">The action callback.</param>
        /// <returns>The fluent syntax.</returns>
        IComponentBindingWithOrOnActivationSyntax<T> OnDeactivation<TImplementation>(Action<TImplementation> action);

        /// <summary>
        /// Indicates that the specified callback should be invoked when instances are deactivated.
        /// </summary>
        /// <param name="action">The action callback.</param>
        /// <returns>The fluent syntax.</returns>
        IComponentBindingWithOrOnActivationSyntax<T> OnDeactivation(Action<IContext, T> action);

        /// <summary>
        /// Indicates that the specified callback should be invoked when instances are deactivated.
        /// </summary>
        /// <typeparam name="TImplementation">The type of the implementation.</typeparam>
        /// <param name="action">The action callback.</param>
        /// <returns>The fluent syntax.</returns>
        IComponentBindingWithOrOnActivationSyntax<T> OnDeactivation<TImplementation>(Action<IContext, TImplementation> action);
    }
}