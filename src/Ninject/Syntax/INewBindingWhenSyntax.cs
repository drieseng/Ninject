﻿namespace Ninject.Syntax
{
    using System;

    using Ninject.Activation;

    public interface INewBindingWhenSyntax<T> : IFluentSyntax
    {
        #region When

        /// <summary>
        /// Indicates that the binding should be used only for requests that support the specified condition.
        /// </summary>
        /// <param name="condition">The condition.</param>
        /// <returns>The fluent syntax.</returns>
        INewBindingWhenSyntax<T> When(Func<IRequest, bool> condition);

        /// <summary>
        /// Indicates that the binding should be used only for injections on the specified type.
        /// Types that derive from the specified type are considered as valid targets.
        /// </summary>
        /// <typeparam name="TParent">The type.</typeparam>
        /// <returns>The fluent syntax.</returns>
        INewBindingWhenSyntax<T> WhenInjectedInto<TParent>();

        /// <summary>
        /// Indicates that the binding should be used only for injections on the specified type.
        /// Types that derive from the specified type are considered as valid targets.
        /// </summary>
        /// <param name="parent">The type.</param>
        /// <returns>The fluent syntax.</returns>
        INewBindingWhenSyntax<T> WhenInjectedInto(Type parent);

        /// <summary>
        /// Indicates that the binding should be used only for injections on the specified types.
        /// Types that derive from one of the specified types are considered as valid targets.
        /// Should match at lease one of the targets.
        /// </summary>
        /// <param name="parents">The types to match.</param>
        /// <returns>The fluent syntax.</returns>
        INewBindingWhenSyntax<T> WhenInjectedInto(params Type[] parents);

        /// <summary>
        /// Indicates that the binding should be used only for injections on the specified type.
        /// The type must match exactly the specified type. Types that derive from the specified type
        /// will not be considered as valid target.
        /// </summary>
        /// <typeparam name="TParent">The type.</typeparam>
        /// <returns>The fluent syntax.</returns>
        INewBindingWhenSyntax<T> WhenInjectedExactlyInto<TParent>();

        /// <summary>
        /// Indicates that the binding should be used only for injections on the specified type.
        /// The type must match exactly the specified type. Types that derive from the specified type
        /// will not be considered as valid target.
        /// </summary>
        /// <param name="parent">The type.</param>
        /// <returns>The fluent syntax.</returns>
        INewBindingWhenSyntax<T> WhenInjectedExactlyInto(Type parent);

        /// <summary>
        /// Indicates that the binding should be used only for injections on the specified type.
        /// The type must match one of the specified types exactly. Types that derive from one of the specified types
        /// will not be considered as valid target.
        /// Should match at least one of the specified targets.
        /// </summary>
        /// <param name="parents">The types.</param>
        /// <returns>The fluent syntax.</returns>
        INewBindingWhenSyntax<T> WhenInjectedExactlyInto(params Type[] parents);

        /// <summary>
        /// Indicates that the binding should be used only when the class being injected has
        /// an attribute of the specified type.
        /// </summary>
        /// <typeparam name="TAttribute">The type of attribute.</typeparam>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenSyntax<T> WhenClassHas<TAttribute>()
            where TAttribute : Attribute;

        /// <summary>
        /// Indicates that the binding should be used only when the member being injected has
        /// an attribute of the specified type.
        /// </summary>
        /// <typeparam name="TAttribute">The type of attribute.</typeparam>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenSyntax<T> WhenMemberHas<TAttribute>()
            where TAttribute : Attribute;

        /// <summary>
        /// Indicates that the binding should be used only when the target being injected has
        /// an attribute of the specified type.
        /// </summary>
        /// <typeparam name="TAttribute">The type of attribute.</typeparam>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenSyntax<T> WhenTargetHas<TAttribute>()
            where TAttribute : Attribute;

        /// <summary>
        /// Indicates that the binding should be used only when the class being injected has
        /// an attribute of the specified type.
        /// </summary>
        /// <param name="attributeType">The type of attribute.</param>
        /// <returns>The fluent syntax.</returns>
        INewBindingWhenSyntax<T> WhenClassHas(Type attributeType);

        /// <summary>
        /// Indicates that the binding should be used only when the member being injected has
        /// an attribute of the specified type.
        /// </summary>
        /// <param name="attributeType">The type of attribute.</param>
        /// <returns>The fluent syntax.</returns>
        INewBindingWhenSyntax<T> WhenMemberHas(Type attributeType);

        /// <summary>
        /// Indicates that the binding should be used only when the target being injected has
        /// an attribute of the specified type.
        /// </summary>
        /// <param name="attributeType">The type of attribute.</param>
        /// <returns>The fluent syntax.</returns>
        INewBindingWhenSyntax<T> WhenTargetHas(Type attributeType);

        /// <summary>
        /// Indicates that the binding should be used only when the service is being requested
        /// by a service bound with the specified name.
        /// </summary>
        /// <param name="name">The name to expect.</param>
        /// <returns>The fluent syntax.</returns>
        INewBindingWhenSyntax<T> WhenParentNamed(string name);

        /// <summary>
        /// Indicates that the binding should be used only when any ancestor is bound with the specified name.
        /// </summary>
        /// <param name="name">The name to expect.</param>
        /// <returns>The fluent syntax.</returns>
        INewBindingWhenSyntax<T> WhenAnyAncestorNamed(string name);

        /// <summary>
        /// Indicates that the binding should be used only when no ancestor is bound with the specified name.
        /// </summary>
        /// <param name="name">The name to expect.</param>
        /// <returns>The fluent syntax.</returns>
        INewBindingWhenSyntax<T> WhenNoAncestorNamed(string name);

        /// <summary>
        /// Indicates that the binding should be used only when any ancestor matches the specified predicate.
        /// </summary>
        /// <param name="predicate">The predicate to match.</param>
        /// <returns>The fluent syntax.</returns>
        INewBindingWhenSyntax<T> WhenAnyAncestorMatches(Predicate<IContext> predicate);

        /// <summary>
        /// Indicates that the binding should be used only when no ancestor matches the specified predicate.
        /// </summary>
        /// <param name="predicate">The predicate to match.</param>
        /// <returns>The fluent syntax.</returns>
        INewBindingWhenSyntax<T> WhenNoAncestorMatches(Predicate<IContext> predicate);

        #endregion When
    }
}