namespace Ninject.Syntax
{
    using System;

    using Ninject.Activation;
    using Ninject.Parameters;
    using Ninject.Planning.Targets;

    public interface INewBindingWhenNamedWithOrOnInitialization<T> : IFluentSyntax
    {
        #region OnInitialization

        /// <summary>
        /// Indicates that the specified callback should be invoked when instances are initialized.
        /// </summary>
        /// <param name="action">The action callback.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenNamedWithOrOnInitialization<T> OnInitialization(Action<T> action);

        /// <summary>
        /// Indicates that the specified callback should be invoked when instances are initialized.
        /// </summary>
        /// <typeparam name="TImplementation">The type of the implementation.</typeparam>
        /// <param name="action">The action callback.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenNamedWithOrOnInitialization<T> OnInitialization<TImplementation>(Action<TImplementation> action);

        /// <summary>
        /// Indicates that the specified callback should be invoked when instances are initialized.
        /// </summary>
        /// <param name="action">The action callback.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenNamedWithOrOnInitialization<T> OnInitialization(Action<IContext, T> action);

        /// <summary>
        /// Indicates that the specified callback should be invoked when instances are initialized.
        /// </summary>
        /// <typeparam name="TImplementation">The type of the implementation.</typeparam>
        /// <param name="action">The action callback.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenNamedWithOrOnInitialization<T> OnInitialization<TImplementation>(Action<IContext, TImplementation> action);

        /// <summary>
        /// Indicates that the specified callback should be invoked when instances are initialized.
        /// </summary>
        /// <typeparam name="TImplementation">The type of the implementation.</typeparam>
        /// <param name="action">The action callback.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenNamedWithOrOnInitialization<T> OnInitialization<TImplementation>(Func<IContext, TImplementation, TImplementation> action);

        #endregion OnInitialization

        #region With

        /// <summary>
        /// Indicates that the specified constructor argument should be overridden with the specified value.
        /// </summary>
        /// <param name="name">The name of the argument to override.</param>
        /// <param name="value">The value for the argument.</param>
        /// <returns>The fluent syntax.</returns>
        INewBindingWhenNamedWithOrOnInitialization<T> WithConstructorArgument(string name, object value);

        /// <summary>
        /// Indicates that the specified constructor argument should be overridden with the specified value.
        /// </summary>
        /// <param name="name">The name of the argument to override.</param>
        /// <param name="callback">The callback to invoke to get the value for the argument.</param>
        /// <returns>The fluent syntax.</returns>
        INewBindingWhenNamedWithOrOnInitialization<T> WithConstructorArgument(string name, Func<IContext, object> callback);

        /// <summary>
        /// Indicates that the specified constructor argument should be overridden with the specified value.
        /// </summary>
        /// <param name="name">The name of the argument to override.</param>
        /// <param name="callback">The callback to invoke to get the value for the argument.</param>
        /// <returns>The fluent syntax.</returns>
        INewBindingWhenNamedWithOrOnInitialization<T> WithConstructorArgument(string name, Func<IContext, ITarget<IConstructorArgument>, object> callback);

        /// <summary>
        /// Indicates that the specified constructor argument should be overridden with the specified value.
        /// </summary>
        /// <typeparam name="TValue">Specifies the argument type to override.</typeparam>
        /// <param name="value">The value for the argument.</param>
        /// <returns>The fluent syntax.</returns>
        INewBindingWhenNamedWithOrOnInitialization<T> WithConstructorArgument<TValue>(TValue value);

        /// <summary>
        /// Indicates that the specified constructor argument should be overridden with the specified value.
        /// </summary>
        /// <param name="type">The type of the argument to override.</param>
        /// <param name="value">The value for the argument.</param>
        /// <returns>The fluent syntax.</returns>
        INewBindingWhenNamedWithOrOnInitialization<T> WithConstructorArgument(Type type, object value);

        /// <summary>
        /// Indicates that the specified constructor argument should be overridden with the specified value.
        /// </summary>
        /// <typeparam name="TValue">The type of the argument to override.</typeparam>
        /// <param name="callback">The callback to invoke to get the value for the argument.</param>
        /// <returns>The fluent syntax.</returns>
        INewBindingWhenNamedWithOrOnInitialization<T> WithConstructorArgument<TValue>(Func<IContext, TValue> callback);

        /// <summary>
        /// Indicates that the specified constructor argument should be overridden with the specified value.
        /// </summary>
        /// <param name="type">The type of the argument to override.</param>
        /// <param name="callback">The callback to invoke to get the value for the argument.</param>
        /// <returns>The fluent syntax.</returns>
        INewBindingWhenNamedWithOrOnInitialization<T> WithConstructorArgument(Type type, Func<IContext, object> callback);

        /// <summary>
        /// Indicates that the specified constructor argument should be overridden with the specified value.
        /// </summary>
        /// <typeparam name="TValue">The type of the argument to override.</typeparam>
        /// <param name="callback">The callback to invoke to get the value for the argument.</param>
        /// <returns>The fluent syntax.</returns>
        INewBindingWhenNamedWithOrOnInitialization<T> WithConstructorArgument<TValue>(Func<IContext, ITarget<IConstructorArgument>, TValue> callback);

        /// <summary>
        /// Indicates that the specified constructor argument should be overridden with the specified value.
        /// </summary>
        /// <param name="type">The type of the argument to override.</param>
        /// <param name="callback">The callback to invoke to get the value for the argument.</param>
        /// <returns>The fluent syntax.</returns>
        INewBindingWhenNamedWithOrOnInitialization<T> WithConstructorArgument(Type type, Func<IContext, ITarget<IConstructorArgument>, object> callback);

        /// <summary>
        /// Indicates that the specified property should be injected with the specified value.
        /// </summary>
        /// <param name="name">The name of the property to override.</param>
        /// <param name="value">The value for the property.</param>
        /// <returns>The fluent syntax.</returns>
        INewBindingWhenNamedWithOrOnInitialization<T> WithPropertyValue(string name, object value);

        /// <summary>
        /// Indicates that the specified property should be injected with the specified value.
        /// </summary>
        /// <param name="name">The name of the property to override.</param>
        /// <param name="callback">The callback to invoke to get the value for the property.</param>
        /// <returns>The fluent syntax.</returns>
        INewBindingWhenNamedWithOrOnInitialization<T> WithPropertyValue(string name, Func<IContext, object> callback);

        /// <summary>
        /// Indicates that the specified property should be injected with the specified value.
        /// </summary>
        /// <param name="name">The name of the property to override.</param>
        /// <param name="callback">The callback to invoke to get the value for the property.</param>
        /// <returns>The fluent syntax.</returns>
        INewBindingWhenNamedWithOrOnInitialization<T> WithPropertyValue(string name, Func<IContext, ITarget<IPropertyValue>, object> callback);

        /// <summary>
        /// Adds a custom parameter to the binding.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        /// <returns>The fluent syntax.</returns>
        INewBindingWhenNamedWithOrOnInitialization<T> WithParameter(IParameter parameter);

        /// <summary>
        /// Sets the value of a piece of metadata on the binding.
        /// </summary>
        /// <param name="key">The metadata key.</param>
        /// <param name="value">The metadata value.</param>
        /// <returns>The fluent syntax.</returns>
        INewBindingWhenNamedWithOrOnInitialization<T> WithMetadata(string key, object value);

        #endregion With

        #region When

        /// <summary>
        /// Indicates that the binding should be used only for requests that support the specified condition.
        /// </summary>
        /// <param name="condition">The condition.</param>
        /// <returns>The fluent syntax.</returns>
        INewBindingWhenNamedWithOrOnInitialization<T> When(Func<IRequest, bool> condition);

        /// <summary>
        /// Indicates that the binding should be used only for injections on the specified type.
        /// Types that derive from the specified type are considered as valid targets.
        /// </summary>
        /// <typeparam name="TParent">The type.</typeparam>
        /// <returns>The fluent syntax.</returns>
        INewBindingWhenNamedWithOrOnInitialization<T> WhenInjectedInto<TParent>();

        /// <summary>
        /// Indicates that the binding should be used only for injections on the specified type.
        /// Types that derive from the specified type are considered as valid targets.
        /// </summary>
        /// <param name="parent">The type.</param>
        /// <returns>The fluent syntax.</returns>
        INewBindingWhenNamedWithOrOnInitialization<T> WhenInjectedInto(Type parent);

        /// <summary>
        /// Indicates that the binding should be used only for injections on the specified types.
        /// Types that derive from one of the specified types are considered as valid targets.
        /// Should match at lease one of the targets.
        /// </summary>
        /// <param name="parents">The types to match.</param>
        /// <returns>The fluent syntax.</returns>
        INewBindingWhenNamedWithOrOnInitialization<T> WhenInjectedInto(params Type[] parents);

        /// <summary>
        /// Indicates that the binding should be used only for injections on the specified type.
        /// The type must match exactly the specified type. Types that derive from the specified type
        /// will not be considered as valid target.
        /// </summary>
        /// <typeparam name="TParent">The type.</typeparam>
        /// <returns>The fluent syntax.</returns>
        INewBindingWhenNamedWithOrOnInitialization<T> WhenInjectedExactlyInto<TParent>();

        /// <summary>
        /// Indicates that the binding should be used only for injections on the specified type.
        /// The type must match exactly the specified type. Types that derive from the specified type
        /// will not be considered as valid target.
        /// </summary>
        /// <param name="parent">The type.</param>
        /// <returns>The fluent syntax.</returns>
        INewBindingWhenNamedWithOrOnInitialization<T> WhenInjectedExactlyInto(Type parent);

        /// <summary>
        /// Indicates that the binding should be used only for injections on the specified type.
        /// The type must match one of the specified types exactly. Types that derive from one of the specified types
        /// will not be considered as valid target.
        /// Should match at least one of the specified targets.
        /// </summary>
        /// <param name="parents">The types.</param>
        /// <returns>The fluent syntax.</returns>
        INewBindingWhenNamedWithOrOnInitialization<T> WhenInjectedExactlyInto(params Type[] parents);

        /// <summary>
        /// Indicates that the binding should be used only when the class being injected has
        /// an attribute of the specified type.
        /// </summary>
        /// <typeparam name="TAttribute">The type of attribute.</typeparam>
        /// <returns>The fluent syntax.</returns>
        INewBindingWhenNamedWithOrOnInitialization<T> WhenClassHas<TAttribute>()
            where TAttribute : Attribute;

        /// <summary>
        /// Indicates that the binding should be used only when the member being injected has
        /// an attribute of the specified type.
        /// </summary>
        /// <typeparam name="TAttribute">The type of attribute.</typeparam>
        /// <returns>The fluent syntax.</returns>
        INewBindingWhenNamedWithOrOnInitialization<T> WhenMemberHas<TAttribute>()
            where TAttribute : Attribute;

        /// <summary>
        /// Indicates that the binding should be used only when the target being injected has
        /// an attribute of the specified type.
        /// </summary>
        /// <typeparam name="TAttribute">The type of attribute.</typeparam>
        /// <returns>The fluent syntax.</returns>
        INewBindingWhenNamedWithOrOnInitialization<T> WhenTargetHas<TAttribute>()
            where TAttribute : Attribute;

        /// <summary>
        /// Indicates that the binding should be used only when the class being injected has
        /// an attribute of the specified type.
        /// </summary>
        /// <param name="attributeType">The type of attribute.</param>
        /// <returns>The fluent syntax.</returns>
        INewBindingWhenNamedWithOrOnInitialization<T> WhenClassHas(Type attributeType);

        /// <summary>
        /// Indicates that the binding should be used only when the member being injected has
        /// an attribute of the specified type.
        /// </summary>
        /// <param name="attributeType">The type of attribute.</param>
        /// <returns>The fluent syntax.</returns>
        INewBindingWhenNamedWithOrOnInitialization<T> WhenMemberHas(Type attributeType);

        /// <summary>
        /// Indicates that the binding should be used only when the target being injected has
        /// an attribute of the specified type.
        /// </summary>
        /// <param name="attributeType">The type of attribute.</param>
        /// <returns>The fluent syntax.</returns>
        INewBindingWhenNamedWithOrOnInitialization<T> WhenTargetHas(Type attributeType);

        /// <summary>
        /// Indicates that the binding should be used only when the service is being requested
        /// by a service bound with the specified name.
        /// </summary>
        /// <param name="name">The name to expect.</param>
        /// <returns>The fluent syntax.</returns>
        INewBindingWhenNamedWithOrOnInitialization<T> WhenParentNamed(string name);

        /// <summary>
        /// Indicates that the binding should be used only when any ancestor is bound with the specified name.
        /// </summary>
        /// <param name="name">The name to expect.</param>
        /// <returns>The fluent syntax.</returns>
        [Obsolete("Use WhenAnyAncestorNamed(string name)")]
        INewBindingWhenNamedWithOrOnInitialization<T> WhenAnyAnchestorNamed(string name);

        /// <summary>
        /// Indicates that the binding should be used only when any ancestor is bound with the specified name.
        /// </summary>
        /// <param name="name">The name to expect.</param>
        /// <returns>The fluent syntax.</returns>
        INewBindingWhenNamedWithOrOnInitialization<T> WhenAnyAncestorNamed(string name);

        /// <summary>
        /// Indicates that the binding should be used only when no ancestor is bound with the specified name.
        /// </summary>
        /// <param name="name">The name to expect.</param>
        /// <returns>The fluent syntax.</returns>
        INewBindingWhenNamedWithOrOnInitialization<T> WhenNoAncestorNamed(string name);

        /// <summary>
        /// Indicates that the binding should be used only when any ancestor matches the specified predicate.
        /// </summary>
        /// <param name="predicate">The predicate to match.</param>
        /// <returns>The fluent syntax.</returns>
        INewBindingWhenNamedWithOrOnInitialization<T> WhenAnyAncestorMatches(Predicate<IContext> predicate);

        /// <summary>
        /// Indicates that the binding should be used only when no ancestor matches the specified predicate.
        /// </summary>
        /// <param name="predicate">The predicate to match.</param>
        /// <returns>The fluent syntax.</returns>
        INewBindingWhenNamedWithOrOnInitialization<T> WhenNoAncestorMatches(Predicate<IContext> predicate);

        #endregion When

        #region Named

        /// <summary>
        /// Indicates that the binding should be registered with the specified name. Names are not
        /// necessarily unique; multiple bindings for a given service may be registered with the same name.
        /// </summary>
        /// <param name="name">The name to give the binding.</param>
        /// <returns>The fluent syntax.</returns>
        INewBindingWhenWithOrOnInitializationSyntax<T> Named(string name);

        #endregion Named
    }
}