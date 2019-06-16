namespace Ninject.Syntax
{
    using Ninject.Activation;
    using System;

    public interface INewBindingWhenInNamedSyntax<T> : IFluentSyntax
    {
        #region In

        /// <summary>
        /// Indicates that only a single instance of the binding should be created, and then
        /// should be re-used for all subsequent requests.
        /// </summary>
        /// <returns>The fluent syntax.</returns>
        INewBindingWhenNamedOrOnActivationSyntax<T> InSingletonScope();

        /// <summary>
        /// Indicates that instances activated via the binding should not be re-used, nor have
        /// their lifecycle managed by Ninject.
        /// </summary>
        /// <returns>The fluent syntax.</returns>
        INewBindingWhenNamedSyntax<T> InTransientScope();

        /// <summary>
        /// Indicates that instances activated via the binding should be re-used within the same thread.
        /// </summary>
        /// <returns>The fluent syntax.</returns>
        INewBindingWhenNamedOrOnActivationSyntax<T> InThreadScope();

        /// <summary>
        /// Indicates that instances activated via the binding should be re-used as long as the object
        /// returned by the provided callback remains alive (that is, has not been garbage collected).
        /// </summary>
        /// <param name="scope">The callback that returns the scope.</param>
        /// <returns>The fluent syntax.</returns>
        INewBindingWhenNamedOrOnActivationSyntax<T> InScope(Func<IContext, object> scope);

        #endregion In

        #region When

        /// <summary>
        /// Indicates that the binding should be used only for requests that support the specified condition.
        /// </summary>
        /// <param name="condition">The condition.</param>
        /// <returns>The fluent syntax.</returns>
        INewBindingWhenInNamedSyntax<T> When(Func<IRequest, bool> condition);

        /// <summary>
        /// Indicates that the binding should be used only for injections on the specified type.
        /// Types that derive from the specified type are considered as valid targets.
        /// </summary>
        /// <typeparam name="TParent">The type.</typeparam>
        /// <returns>The fluent syntax.</returns>
        INewBindingWhenInNamedSyntax<T> WhenInjectedInto<TParent>();

        /// <summary>
        /// Indicates that the binding should be used only for injections on the specified type.
        /// Types that derive from the specified type are considered as valid targets.
        /// </summary>
        /// <param name="parent">The type.</param>
        /// <returns>The fluent syntax.</returns>
        INewBindingWhenInNamedSyntax<T> WhenInjectedInto(Type parent);

        /// <summary>
        /// Indicates that the binding should be used only for injections on the specified types.
        /// Types that derive from one of the specified types are considered as valid targets.
        /// Should match at lease one of the targets.
        /// </summary>
        /// <param name="parents">The types to match.</param>
        /// <returns>The fluent syntax.</returns>
        INewBindingWhenInNamedSyntax<T> WhenInjectedInto(params Type[] parents);

        /// <summary>
        /// Indicates that the binding should be used only for injections on the specified type.
        /// The type must match exactly the specified type. Types that derive from the specified type
        /// will not be considered as valid target.
        /// </summary>
        /// <typeparam name="TParent">The type.</typeparam>
        /// <returns>The fluent syntax.</returns>
        INewBindingWhenInNamedSyntax<T> WhenInjectedExactlyInto<TParent>();

        /// <summary>
        /// Indicates that the binding should be used only for injections on the specified type.
        /// The type must match exactly the specified type. Types that derive from the specified type
        /// will not be considered as valid target.
        /// </summary>
        /// <param name="parent">The type.</param>
        /// <returns>The fluent syntax.</returns>
        INewBindingWhenInNamedSyntax<T> WhenInjectedExactlyInto(Type parent);

        /// <summary>
        /// Indicates that the binding should be used only for injections on the specified type.
        /// The type must match one of the specified types exactly. Types that derive from one of the specified types
        /// will not be considered as valid target.
        /// Should match at least one of the specified targets.
        /// </summary>
        /// <param name="parents">The types.</param>
        /// <returns>The fluent syntax.</returns>
        INewBindingWhenInNamedSyntax<T> WhenInjectedExactlyInto(params Type[] parents);

        /// <summary>
        /// Indicates that the binding should be used only when the class being injected has
        /// an attribute of the specified type.
        /// </summary>
        /// <typeparam name="TAttribute">The type of attribute.</typeparam>
        /// <returns>The fluent syntax.</returns>
        INewBindingWhenInNamedSyntax<T> WhenClassHas<TAttribute>()
            where TAttribute : Attribute;

        /// <summary>
        /// Indicates that the binding should be used only when the member being injected has
        /// an attribute of the specified type.
        /// </summary>
        /// <typeparam name="TAttribute">The type of attribute.</typeparam>
        /// <returns>The fluent syntax.</returns>
        INewBindingWhenInNamedSyntax<T> WhenMemberHas<TAttribute>()
            where TAttribute : Attribute;

        /// <summary>
        /// Indicates that the binding should be used only when the target being injected has
        /// an attribute of the specified type.
        /// </summary>
        /// <typeparam name="TAttribute">The type of attribute.</typeparam>
        /// <returns>The fluent syntax.</returns>
        INewBindingWhenInNamedSyntax<T> WhenTargetHas<TAttribute>()
            where TAttribute : Attribute;

        /// <summary>
        /// Indicates that the binding should be used only when the class being injected has
        /// an attribute of the specified type.
        /// </summary>
        /// <param name="attributeType">The type of attribute.</param>
        /// <returns>The fluent syntax.</returns>
        INewBindingWhenInNamedSyntax<T> WhenClassHas(Type attributeType);

        /// <summary>
        /// Indicates that the binding should be used only when the member being injected has
        /// an attribute of the specified type.
        /// </summary>
        /// <param name="attributeType">The type of attribute.</param>
        /// <returns>The fluent syntax.</returns>
        INewBindingWhenInNamedSyntax<T> WhenMemberHas(Type attributeType);

        /// <summary>
        /// Indicates that the binding should be used only when the target being injected has
        /// an attribute of the specified type.
        /// </summary>
        /// <param name="attributeType">The type of attribute.</param>
        /// <returns>The fluent syntax.</returns>
        INewBindingWhenInNamedSyntax<T> WhenTargetHas(Type attributeType);

        /// <summary>
        /// Indicates that the binding should be used only when the service is being requested
        /// by a service bound with the specified name.
        /// </summary>
        /// <param name="name">The name to expect.</param>
        /// <returns>The fluent syntax.</returns>
        INewBindingWhenInNamedSyntax<T> WhenParentNamed(string name);

        /// <summary>
        /// Indicates that the binding should be used only when any ancestor is bound with the specified name.
        /// </summary>
        /// <param name="name">The name to expect.</param>
        /// <returns>The fluent syntax.</returns>
        INewBindingWhenInNamedSyntax<T> WhenAnyAncestorNamed(string name);

        /// <summary>
        /// Indicates that the binding should be used only when no ancestor is bound with the specified name.
        /// </summary>
        /// <param name="name">The name to expect.</param>
        /// <returns>The fluent syntax.</returns>
        INewBindingWhenInNamedSyntax<T> WhenNoAncestorNamed(string name);

        /// <summary>
        /// Indicates that the binding should be used only when any ancestor matches the specified predicate.
        /// </summary>
        /// <param name="predicate">The predicate to match.</param>
        /// <returns>The fluent syntax.</returns>
        INewBindingWhenInNamedSyntax<T> WhenAnyAncestorMatches(Predicate<IContext> predicate);

        /// <summary>
        /// Indicates that the binding should be used only when no ancestor matches the specified predicate.
        /// </summary>
        /// <param name="predicate">The predicate to match.</param>
        /// <returns>The fluent syntax.</returns>
        INewBindingWhenInNamedSyntax<T> WhenNoAncestorMatches(Predicate<IContext> predicate);

        #endregion When

        #region Named

        /// <summary>
        /// Indicates that the binding should be registered with the specified name. Names are not
        /// necessarily unique; multiple bindings for a given service may be registered with the same name.
        /// </summary>
        /// <param name="name">The name to give the binding.</param>
        /// <returns>The fluent syntax.</returns>
        INewBindingWhenInSyntax<T> Named(string name);

        #endregion Named
    }
}
