// -------------------------------------------------------------------------------------------------
// <copyright file="BindingConfigurationBuilder{T}.cs" company="Ninject Project Contributors">
//   Copyright (c) 2007-2010 Enkari, Ltd. All rights reserved.
//   Copyright (c) 2010-2019 Ninject Project Contributors. All rights reserved.
//
//   Dual-licensed under the Apache License, Version 2.0, and the Microsoft Public License (Ms-PL).
//   You may not use this file except in compliance with one of the Licenses.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//   or
//       http://www.microsoft.com/opensource/licenses.mspx
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Ninject.Builder
{
    using System;
    using System.Collections.Generic;

    using Ninject.Activation;
    using Ninject.Components;
    using Ninject.Infrastructure;
    using Ninject.Parameters;
    using Ninject.Planning.Bindings;
    using Ninject.Planning.Targets;
    using Ninject.Syntax;

    /// <summary>
    /// Builds a binding configurating for a given service.
    /// </summary>
    /// <typeparam name="T">The type of the service produces by this binding.</typeparam>
    internal sealed class BindingConfigurationBuilder<T> : BindingConfigurationBuilder, IBindingConfigurationSyntax<T>
    {
        private readonly ComponentContainer components;
        private readonly IProviderFactory providerBuilder;
        private readonly BindingTarget target;
        private readonly List<IParameter> parameters;
        private Func<IContext, object> scopeCallback;

        /// <summary>
        /// Initializes a new instance of the <see cref="BindingConfigurationBuilder{T}"/> class.
        /// </summary>
        /// <param name="components">The components.</param>
        /// <param name="providerFactory">A factory for creating an <see cref="IProvider"/>.</param>
        /// <param name="target">The type of target for the binding.</param>
        internal BindingConfigurationBuilder(ComponentContainer components, IProviderFactory providerFactory, BindingTarget target)
        {
            this.components = components;
            this.providerBuilder = providerFactory;
            this.target = target;
            this.parameters = new List<IParameter>();
        }

        /// <summary>
        /// Gets the binding.
        /// </summary>
        public IBindingConfiguration BindingConfiguration => throw new NotImplementedException();

        /// <summary>
        /// Builds the binding configuration.
        /// </summary>
        /// <returns>
        /// The binding configuration.
        /// </returns>
        public override IBindingConfiguration Build()
        {
            return new BindingConfiguration(this.parameters)
                {
                    Provider = this.providerBuilder.Create(this.parameters),
                    ScopeCallback = this.scopeCallback ?? StandardScopeCallbacks.Transient,
                    Target = this.target,
                };
        }

        /// <summary>
        /// Indicates that instances activated via the binding should be re-used as long as the object
        /// returned by the provided callback remains alive (that is, has not been garbage collected).
        /// </summary>
        /// <param name="scope">The callback that returns the scope.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        public IBindingNamedWithOrOnSyntax<T> InScope(Func<IContext, object> scope)
        {
            this.scopeCallback = scope;
            return this;
        }

        /// <summary>
        /// Indicates that only a single instance of the binding should be created, and then
        /// should be re-used for all subsequent requests.
        /// </summary>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        public IBindingNamedWithOrOnSyntax<T> InSingletonScope()
        {
            this.scopeCallback = StandardScopeCallbacks.Singleton;
            return this;
        }

        /// <summary>
        /// Indicates that instances activated via the binding should be re-used within the same thread.
        /// </summary>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        public IBindingNamedWithOrOnSyntax<T> InThreadScope()
        {
            this.scopeCallback = StandardScopeCallbacks.Thread;
            return this;
        }

        /// <summary>
        /// Indicates that instances activated via the binding should not be re-used, nor have
        /// their lifecycle managed by Ninject.
        /// </summary>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        public IBindingNamedWithOrOnSyntax<T> InTransientScope()
        {
            this.scopeCallback = StandardScopeCallbacks.Transient;
            return this;
        }

        /// <summary>
        /// Indicates that the binding should be registered with the specified name. Names are not
        /// necessarily unique; multiple bindings for a given service may be registered with the same name.
        /// </summary>
        /// <param name="name">The name to give the binding.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        public IBindingWithOrOnSyntax<T> Named(string name)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Indicates that the specified callback should be invoked when instances are activated.
        /// </summary>
        /// <param name="action">The action callback.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        public IBindingOnSyntax<T> OnActivation(Action<T> action)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Indicates that the specified callback should be invoked when instances are activated.
        /// </summary>
        /// <typeparam name="TImplementation">The type of the implementation.</typeparam>
        /// <param name="action">The action callback.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        public IBindingOnSyntax<T> OnActivation<TImplementation>(Action<TImplementation> action)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Indicates that the specified callback should be invoked when instances are activated.
        /// </summary>
        /// <param name="action">The action callback.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        public IBindingOnSyntax<T> OnActivation(Action<IContext, T> action)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Indicates that the specified callback should be invoked when instances are activated.
        /// </summary>
        /// <typeparam name="TImplementation">The type of the implementation.</typeparam>
        /// <param name="action">The action callback.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        public IBindingOnSyntax<T> OnActivation<TImplementation>(Action<IContext, TImplementation> action)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Indicates that the specified callback should be invoked when instances are deactivated.
        /// </summary>
        /// <param name="action">The action callback.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        public IBindingOnSyntax<T> OnDeactivation(Action<T> action)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Indicates that the specified callback should be invoked when instances are deactivated.
        /// </summary>
        /// <typeparam name="TImplementation">The type of the implementation.</typeparam>
        /// <param name="action">The action callback.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        public IBindingOnSyntax<T> OnDeactivation<TImplementation>(Action<TImplementation> action)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Indicates that the specified callback should be invoked when instances are deactivated.
        /// </summary>
        /// <param name="action">The action callback.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        public IBindingOnSyntax<T> OnDeactivation(Action<IContext, T> action)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Indicates that the specified callback should be invoked when instances are deactivated.
        /// </summary>
        /// <typeparam name="TImplementation">The type of the implementation.</typeparam>
        /// <param name="action">The action callback.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        public IBindingOnSyntax<T> OnDeactivation<TImplementation>(Action<IContext, TImplementation> action)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Indicates that the binding should be used only for requests that support the specified condition.
        /// </summary>
        /// <param name="condition">The condition.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        public IBindingInNamedWithOrOnSyntax<T> When(Func<IRequest, bool> condition)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Indicates that the binding should be used only when any ancestor matches the specified predicate.
        /// </summary>
        /// <param name="predicate">The predicate to match.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        public IBindingInNamedWithOrOnSyntax<T> WhenAnyAncestorMatches(Predicate<IContext> predicate)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Indicates that the binding should be used only when any ancestor is bound with the specified name.
        /// </summary>
        /// <param name="name">The name to expect.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        public IBindingInNamedWithOrOnSyntax<T> WhenAnyAncestorNamed(string name)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Indicates that the binding should be used only when any ancestor is bound with the specified name.
        /// </summary>
        /// <param name="name">The name to expect.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        public IBindingInNamedWithOrOnSyntax<T> WhenAnyAnchestorNamed(string name)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Indicates that the binding should be used only when the class being injected has
        /// an attribute of the specified type.
        /// </summary>
        /// <typeparam name="TAttribute">The type of attribute.</typeparam>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        public IBindingInNamedWithOrOnSyntax<T> WhenClassHas<TAttribute>()
            where TAttribute : Attribute
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Indicates that the binding should be used only when the class being injected has
        /// an attribute of the specified type.
        /// </summary>
        /// <param name="attributeType">The type of attribute.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        public IBindingInNamedWithOrOnSyntax<T> WhenClassHas(Type attributeType)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Indicates that the binding should be used only for injections on the specified type.
        /// The type must match exactly the specified type. Types that derive from the specified type
        /// will not be considered as valid target.
        /// </summary>
        /// <typeparam name="TParent">The type.</typeparam>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        public IBindingInNamedWithOrOnSyntax<T> WhenInjectedExactlyInto<TParent>()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Indicates that the binding should be used only for injections on the specified type.
        /// The type must match exactly the specified type. Types that derive from the specified type
        /// will not be considered as valid target.
        /// </summary>
        /// <param name="parent">The type.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        public IBindingInNamedWithOrOnSyntax<T> WhenInjectedExactlyInto(Type parent)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Indicates that the binding should be used only for injections on the specified type.
        /// The type must match one of the specified types exactly. Types that derive from one of the specified types
        /// will not be considered as valid target.
        /// Should match at least one of the specified targets.
        /// </summary>
        /// <param name="parents">The types.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        public IBindingInNamedWithOrOnSyntax<T> WhenInjectedExactlyInto(params Type[] parents)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Indicates that the binding should be used only for injections on the specified type.
        /// Types that derive from the specified type are considered as valid targets.
        /// </summary>
        /// <typeparam name="TParent">The type.</typeparam>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        public IBindingInNamedWithOrOnSyntax<T> WhenInjectedInto<TParent>()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Indicates that the binding should be used only for injections on the specified type.
        /// Types that derive from the specified type are considered as valid targets.
        /// </summary>
        /// <param name="parent">The type.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        public IBindingInNamedWithOrOnSyntax<T> WhenInjectedInto(Type parent)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Indicates that the binding should be used only for injections on the specified types.
        /// Types that derive from one of the specified types are considered as valid targets.
        /// Should match at lease one of the targets.
        /// </summary>
        /// <param name="parents">The types to match.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        public IBindingInNamedWithOrOnSyntax<T> WhenInjectedInto(params Type[] parents)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Indicates that the binding should be used only when the member being injected has
        /// an attribute of the specified type.
        /// </summary>
        /// <typeparam name="TAttribute">The type of attribute.</typeparam>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        public IBindingInNamedWithOrOnSyntax<T> WhenMemberHas<TAttribute>()
            where TAttribute : Attribute
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Indicates that the binding should be used only when the member being injected has
        /// an attribute of the specified type.
        /// </summary>
        /// <param name="attributeType">The type of attribute.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        public IBindingInNamedWithOrOnSyntax<T> WhenMemberHas(Type attributeType)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Indicates that the binding should be used only when no ancestor matches the specified predicate.
        /// </summary>
        /// <param name="predicate">The predicate to match.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        public IBindingInNamedWithOrOnSyntax<T> WhenNoAncestorMatches(Predicate<IContext> predicate)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Indicates that the binding should be used only when no ancestor is bound with the specified name.
        /// </summary>
        /// <param name="name">The name to expect.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        public IBindingInNamedWithOrOnSyntax<T> WhenNoAncestorNamed(string name)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Indicates that the binding should be used only when the service is being requested
        /// by a service bound with the specified name.
        /// </summary>
        /// <param name="name">The name to expect.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        public IBindingInNamedWithOrOnSyntax<T> WhenParentNamed(string name)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Indicates that the binding should be used only when the target being injected has
        /// an attribute of the specified type.
        /// </summary>
        /// <typeparam name="TAttribute">The type of attribute.</typeparam>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        public IBindingInNamedWithOrOnSyntax<T> WhenTargetHas<TAttribute>()
            where TAttribute : Attribute
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Indicates that the binding should be used only when the target being injected has
        /// an attribute of the specified type.
        /// </summary>
        /// <param name="attributeType">The type of attribute.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        public IBindingInNamedWithOrOnSyntax<T> WhenTargetHas(Type attributeType)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Indicates that the specified constructor argument should be overridden with the specified value.
        /// </summary>
        /// <param name="name">The name of the argument to override.</param>
        /// <param name="value">The value for the argument.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        public IBindingWithOrOnSyntax<T> WithConstructorArgument(string name, object value)
        {
            this.parameters.Add(new ConstructorArgument(name, value));
            return this;
        }

        /// <summary>
        /// Indicates that the specified constructor argument should be overridden with the specified value.
        /// </summary>
        /// <param name="name">The name of the argument to override.</param>
        /// <param name="callback">The callback to invoke to get the value for the argument.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        public IBindingWithOrOnSyntax<T> WithConstructorArgument(string name, Func<IContext, object> callback)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Indicates that the specified constructor argument should be overridden with the specified value.
        /// </summary>
        /// <param name="name">The name of the argument to override.</param>
        /// <param name="callback">The callback to invoke to get the value for the argument.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        public IBindingWithOrOnSyntax<T> WithConstructorArgument(string name, Func<IContext, ITarget, object> callback)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Indicates that the specified constructor argument should be overridden with the specified value.
        /// </summary>
        /// <typeparam name="TValue">Specifies the argument type to override.</typeparam>
        /// <param name="value">The value for the argument.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        public IBindingWithOrOnSyntax<T> WithConstructorArgument<TValue>(TValue value)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Indicates that the specified constructor argument should be overridden with the specified value.
        /// </summary>
        /// <param name="type">The type of the argument to override.</param>
        /// <param name="value">The value for the argument.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        public IBindingWithOrOnSyntax<T> WithConstructorArgument(Type type, object value)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Indicates that the specified constructor argument should be overridden with the specified value.
        /// </summary>
        /// <typeparam name="TValue">The type of the argument to override.</typeparam>
        /// <param name="callback">The callback to invoke to get the value for the argument.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        public IBindingWithOrOnSyntax<T> WithConstructorArgument<TValue>(Func<IContext, TValue> callback)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Indicates that the specified constructor argument should be overridden with the specified value.
        /// </summary>
        /// <param name="type">The type of the argument to override.</param>
        /// <param name="callback">The callback to invoke to get the value for the argument.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        public IBindingWithOrOnSyntax<T> WithConstructorArgument(Type type, Func<IContext, object> callback)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Indicates that the specified constructor argument should be overridden with the specified value.
        /// </summary>
        /// <typeparam name="TValue">The type of the argument to override.</typeparam>
        /// <param name="callback">The callback to invoke to get the value for the argument.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        public IBindingWithOrOnSyntax<T> WithConstructorArgument<TValue>(Func<IContext, ITarget, TValue> callback)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Indicates that the specified constructor argument should be overridden with the specified value.
        /// </summary>
        /// <param name="type">The type of the argument to override.</param>
        /// <param name="callback">The callback to invoke to get the value for the argument.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        public IBindingWithOrOnSyntax<T> WithConstructorArgument(Type type, Func<IContext, ITarget, object> callback)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Sets the value of a piece of metadata on the binding.
        /// </summary>
        /// <param name="key">The metadata key.</param>
        /// <param name="value">The metadata value.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        public IBindingWithOrOnSyntax<T> WithMetadata(string key, object value)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Adds a custom parameter to the binding.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        public IBindingWithOrOnSyntax<T> WithParameter(IParameter parameter)
        {
            this.parameters.Add(parameter);
            return this;
        }

        /// <summary>
        /// Indicates that the specified property should be injected with the specified value.
        /// </summary>
        /// <param name="name">The name of the property to override.</param>
        /// <param name="value">The value for the property.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        public IBindingWithOrOnSyntax<T> WithPropertyValue(string name, object value)
        {
            this.parameters.Add(new PropertyValue(name, value));
            return this;
        }

        /// <summary>
        /// Indicates that the specified property should be injected with the specified value.
        /// </summary>
        /// <param name="name">The name of the property to override.</param>
        /// <param name="callback">The callback to invoke to get the value for the property.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        public IBindingWithOrOnSyntax<T> WithPropertyValue(string name, Func<IContext, object> callback)
        {
            this.parameters.Add(new PropertyValue(name, callback));
            return this;
        }

        /// <summary>
        /// Indicates that the specified property should be injected with the specified value.
        /// </summary>
        /// <param name="name">The name of the property to override.</param>
        /// <param name="callback">The callback to invoke to get the value for the property.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        public IBindingWithOrOnSyntax<T> WithPropertyValue(string name, Func<IContext, ITarget, object> callback)
        {
            this.parameters.Add(new PropertyValue(name, callback));
            return this;
        }
    }
}