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
    using System.Linq;
    using System.Reflection;
    using Ninject.Activation;
    using Ninject.Components;
    using Ninject.Infrastructure;
    using Ninject.Infrastructure.Language;
    using Ninject.Parameters;
    using Ninject.Planning.Bindings;
    using Ninject.Planning.Targets;
    using Ninject.Syntax;

    /// <summary>
    /// Builds a binding configurating for a given service.
    /// </summary>
    /// <typeparam name="T">The type of the service produces by this binding.</typeparam>
    internal sealed class BindingConfigurationBuilder<T> : BindingConfigurationBuilder,
                                                           INewBindingWhenInSyntax<T>,
                                                           INewBindingWhenWithOrOnSyntax<T>,
                                                           INewBindingWhenInWithOrOnInitializationSyntax<T>,
                                                           INewBindingWhenInNamedSyntax<T>,
                                                           INewBindingWhenNamedSyntax<T>,
                                                           INewBindingWhenSyntax<T>,
                                                           INewBindingWhenOrOnActivationSyntax<T>,
                                                           INewBindingWhenNamedOrOnActivationSyntax<T>,
                                                           INewBindingWhenWithOrOnInitializationSyntax<T>
    {
        private readonly IProviderFactory providerBuilder;
        private readonly BindingTarget target;
        private readonly BindingBuilder bindingBuilder;
        private IBindingMetadata metadata;
        private readonly List<IParameter> parameters;
        private readonly List<Func<IContext, object, object>> initializationActions;
        private readonly List<Action<IContext, object>> activationActions;
        private readonly List<Action<IContext, object>> deactivationActions;
        private Func<IRequest, bool> condition;
        private Func<IContext, object> scopeCallback;

        /// <summary>
        /// Initializes a new instance of the <see cref="BindingConfigurationBuilder{T}"/> class.
        /// </summary>
        /// <param name="providerFactory">A factory for creating an <see cref="IProvider"/>.</param>
        /// <param name="target">The type of target for the binding.</param>
        /// <param name="bindingBuilder">The <see cref="BindingBuilder"/> to provide configuration for.</param>
        internal BindingConfigurationBuilder(IProviderFactory providerFactory, BindingTarget target, BindingBuilder bindingBuilder)
        {
            this.providerBuilder = providerFactory;
            this.target = target;
            this.bindingBuilder = bindingBuilder;
            this.parameters = new List<IParameter>();
            this.initializationActions = new List<Func<IContext, object, object>>();
            this.activationActions = new List<Action<IContext, object>>();
            this.deactivationActions = new List<Action<IContext, object>>();
        }

        /// <summary>
        /// Returns a value indicating whether any activation actions are defined for the current binding.
        /// </summary>
        /// <value>
        /// <see langword="true"/> if at least one activation action is defined for the current binding;
        /// otherwise, <see langword="false"/>.
        /// </value>
        public override bool HasActivationActions
        {
            get { return this.activationActions.Count > 0; }
        }

        /// <summary>
        /// Returns a value indicating whether any deactivation actions are defined for the current binding.
        /// </summary>
        /// <value>
        /// <see langword="true"/> if at least one deactivation action is defined for the current binding;
        /// otherwise, <see langword="false"/>.
        /// </value>
        public override bool HasDeactivationActions
        {
            get { return this.deactivationActions.Count > 0; }
        }

        /// <summary>
        /// Returns a value indicating whether any initialization actions are defined for the current binding.
        /// </summary>
        /// <value>
        /// <see langword="true"/> if at least one initialization action is defined for the current binding;
        /// otherwise, <see langword="false"/>.
        /// </value>
        public override bool HasInitializationActions
        {
            get { return this.initializationActions.Count > 0; }
        }

        /// <summary>
        /// Builds the binding configuration.
        /// </summary>
        /// <param name="root">The resolution root.</param>
        /// <returns>
        /// The binding configuration.
        /// </returns>
        public override IBindingConfiguration Build(IResolutionRoot root)
        {
            return new BindingConfiguration(this.parameters,
                                            this.metadata,
                                            this.condition,
                                            this.providerBuilder.Create(root, this.parameters),
                                            this.scopeCallback ?? StandardScopeCallbacks.Transient,
                                            this.initializationActions,
                                            this.activationActions,
                                            this.deactivationActions,
                                            this.target);
        }

        #region INewBindingWhenInWithOrOnInitializationSyntax

        /// <summary>
        /// Indicates that only a single instance of the binding should be created, and then
        /// should be re-used for all subsequent requests.
        /// </summary>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenWithOrOnSyntax<T> INewBindingWhenInWithOrOnInitializationSyntax<T>.InSingletonScope()
        {
            return this.InSingletonScope();
        }

        /// <summary>
        /// Indicates that instances activated via the binding should not be re-used, nor have
        /// their lifecycle managed by Ninject.
        /// </summary>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenWithOrOnInitializationSyntax<T> INewBindingWhenInWithOrOnInitializationSyntax<T>.InTransientScope()
        {
            return this.InTransientScope();
        }

        /// <summary>
        /// Indicates that instances activated via the binding should be re-used within the same thread.
        /// </summary>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenWithOrOnSyntax<T> INewBindingWhenInWithOrOnInitializationSyntax<T>.InThreadScope()
        {
            return this.InThreadScope();
        }

        /// <summary>
        /// Indicates that instances activated via the binding should be re-used as long as the object
        /// returned by the provided callback remains alive (that is, has not been garbage collected).
        /// </summary>
        /// <param name="scope">The callback that returns the scope.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenWithOrOnSyntax<T> INewBindingWhenInWithOrOnInitializationSyntax<T>.InScope(Func<IContext, object> scope)
        {
            return this.InScope(scope);
        }

        /// <summary>
        /// Indicates that the specified callback should be invoked when instances are initialized.
        /// </summary>
        /// <param name="action">The action callback.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenInWithOrOnInitializationSyntax<T> INewBindingWhenInWithOrOnInitializationSyntax<T>.OnInitialization(Action<T> action)
        {
            return this.OnInitialization(action);
        }

        /// <summary>
        /// Indicates that the specified callback should be invoked when instances are initialized.
        /// </summary>
        /// <typeparam name="TImplementation">The type of the implementation.</typeparam>
        /// <param name="action">The action callback.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenInWithOrOnInitializationSyntax<T> INewBindingWhenInWithOrOnInitializationSyntax<T>.OnInitialization<TImplementation>(Action<TImplementation> action)
        {
            return this.OnInitialization(action);
        }

        /// <summary>
        /// Indicates that the specified callback should be invoked when instances are initialized.
        /// </summary>
        /// <param name="action">The action callback.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenInWithOrOnInitializationSyntax<T> INewBindingWhenInWithOrOnInitializationSyntax<T>.OnInitialization(Action<IContext, T> action)
        {
            return this.OnInitialization(action);
        }

        /// <summary>
        /// Indicates that the specified callback should be invoked when instances are initialized.
        /// </summary>
        /// <typeparam name="TImplementation">The type of the implementation.</typeparam>
        /// <param name="action">The action callback.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenInWithOrOnInitializationSyntax<T> INewBindingWhenInWithOrOnInitializationSyntax<T>.OnInitialization<TImplementation>(Action<IContext, TImplementation> action)
        {
            return this.OnInitialization(action);
        }

        /// <summary>
        /// Indicates that the specified callback should be invoked when instances are initialized.
        /// </summary>
        /// <typeparam name="TImplementation">The type of the implementation.</typeparam>
        /// <param name="action">The action callback.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenInWithOrOnInitializationSyntax<T> INewBindingWhenInWithOrOnInitializationSyntax<T>.OnInitialization<TImplementation>(Func<IContext, TImplementation, TImplementation> action)
        {
            return this.OnInitialization(action);
        }

        /// <summary>
        /// Indicates that the specified constructor argument should be overridden with the specified value.
        /// </summary>
        /// <param name="name">The name of the argument to override.</param>
        /// <param name="value">The value for the argument.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenInWithOrOnInitializationSyntax<T> INewBindingWhenInWithOrOnInitializationSyntax<T>.WithConstructorArgument(string name, object value)
        {
            return this.WithConstructorArgument(name, value);
        }

        /// <summary>
        /// Indicates that the specified constructor argument should be overridden with the specified value.
        /// </summary>
        /// <param name="name">The name of the argument to override.</param>
        /// <param name="callback">The callback to invoke to get the value for the argument.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenInWithOrOnInitializationSyntax<T> INewBindingWhenInWithOrOnInitializationSyntax<T>.WithConstructorArgument(string name, Func<IContext, object> callback)
        {
            return this.WithConstructorArgument(name, callback);
        }

        /// <summary>
        /// Indicates that the specified constructor argument should be overridden with the specified value.
        /// </summary>
        /// <param name="name">The name of the argument to override.</param>
        /// <param name="callback">The callback to invoke to get the value for the argument.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenInWithOrOnInitializationSyntax<T> INewBindingWhenInWithOrOnInitializationSyntax<T>.WithConstructorArgument(string name, Func<IContext, ITarget<ParameterInfo>, object> callback)
        {
            return this.WithConstructorArgument(name, callback);
        }

        /// <summary>
        /// Indicates that the specified constructor argument should be overridden with the specified value.
        /// </summary>
        /// <typeparam name="TValue">Specifies the argument type to override.</typeparam>
        /// <param name="value">The value for the argument.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenInWithOrOnInitializationSyntax<T> INewBindingWhenInWithOrOnInitializationSyntax<T>.WithConstructorArgument<TValue>(TValue value)
        {
            return this.WithConstructorArgument(value);
        }

        /// <summary>
        /// Indicates that the specified constructor argument should be overridden with the specified value.
        /// </summary>
        /// <param name="type">The type of the argument to override.</param>
        /// <param name="value">The value for the argument.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenInWithOrOnInitializationSyntax<T> INewBindingWhenInWithOrOnInitializationSyntax<T>.WithConstructorArgument(Type type, object value)
        {
            return this.WithConstructorArgument(type, (context, target) => value);
        }

        /// <summary>
        /// Indicates that the specified constructor argument should be overridden with the specified value.
        /// </summary>
        /// <typeparam name="TValue">The type of the argument type to override.</typeparam>
        /// <param name="callback">The callback to invoke to get the value for the argument.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenInWithOrOnInitializationSyntax<T> INewBindingWhenInWithOrOnInitializationSyntax<T>.WithConstructorArgument<TValue>(Func<IContext, TValue> callback)
        {
            return this.WithConstructorArgument(callback);
        }

        /// <summary>
        /// Indicates that the specified constructor argument should be overridden with the specified value.
        /// </summary>
        /// <param name="type">The type of the argument to override.</param>
        /// <param name="callback">The callback to invoke to get the value for the argument.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenInWithOrOnInitializationSyntax<T> INewBindingWhenInWithOrOnInitializationSyntax<T>.WithConstructorArgument(Type type, Func<IContext, object> callback)
        {
            return this.WithConstructorArgument(type, (context, target) => callback(context));
        }

        /// <summary>
        /// Indicates that the specified constructor argument should be overridden with the specified value.
        /// </summary>
        /// <typeparam name="TValue">The type of the argument type to override.</typeparam>
        /// <param name="callback">The callback to invoke to get the value for the argument.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenInWithOrOnInitializationSyntax<T> INewBindingWhenInWithOrOnInitializationSyntax<T>.WithConstructorArgument<TValue>(Func<IContext, ITarget<ParameterInfo>, TValue> callback)
        {
            return this.WithConstructorArgument(typeof(TValue), callback);
        }

        /// <summary>
        /// Indicates that the specified constructor argument should be overridden with the specified value.
        /// </summary>
        /// <param name="type">The type of the argument to override.</param>
        /// <param name="callback">The callback to invoke to get the value for the argument.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenInWithOrOnInitializationSyntax<T> INewBindingWhenInWithOrOnInitializationSyntax<T>.WithConstructorArgument(Type type, Func<IContext, ITarget<ParameterInfo>, object> callback)
        {
            return this.WithConstructorArgument(type, callback);
        }

        /// <summary>
        /// Indicates that the specified property should be injected with the specified value.
        /// </summary>
        /// <param name="name">The name of the property to override.</param>
        /// <param name="value">The value for the property.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenInWithOrOnInitializationSyntax<T> INewBindingWhenInWithOrOnInitializationSyntax<T>.WithPropertyValue(string name, object value)
        {
            return this.WithPropertyValue(name, value);
        }

        /// <summary>
        /// Indicates that the specified property should be injected with the specified value.
        /// </summary>
        /// <param name="name">The name of the property to override.</param>
        /// <param name="callback">The callback to invoke to get the value for the property.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenInWithOrOnInitializationSyntax<T> INewBindingWhenInWithOrOnInitializationSyntax<T>.WithPropertyValue(string name, Func<IContext, object> callback)
        {
            return this.WithPropertyValue(name, callback);
        }

        /// <summary>
        /// Indicates that the specified property should be injected with the specified value.
        /// </summary>
        /// <param name="name">The name of the property to override.</param>
        /// <param name="callback">The callback to invoke to get the value for the property.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenInWithOrOnInitializationSyntax<T> INewBindingWhenInWithOrOnInitializationSyntax<T>.WithPropertyValue(string name, Func<IContext, ITarget<PropertyInfo>, object> callback)
        {
            return this.WithPropertyValue(name, callback);
        }

        /// <summary>
        /// Adds a custom parameter to the binding.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenInWithOrOnInitializationSyntax<T> INewBindingWhenInWithOrOnInitializationSyntax<T>.WithParameter(IParameter parameter)
        {
            return this.WithParameter(parameter);
        }

        /// <summary>
        /// Sets the value of a piece of metadata on the binding.
        /// </summary>
        /// <param name="key">The metadata key.</param>
        /// <param name="value">The metadata value.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenInWithOrOnInitializationSyntax<T> INewBindingWhenInWithOrOnInitializationSyntax<T>.WithMetadata(string key, object value)
        {
            return this.WithMetadata(key, value);
        }

        /// <summary>
        /// Indicates that the binding should be used only for requests that support the specified condition.
        /// </summary>
        /// <param name="condition">The condition.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenInWithOrOnInitializationSyntax<T> INewBindingWhenInWithOrOnInitializationSyntax<T>.When(Func<IRequest, bool> condition)
        {
            return When(condition);
        }

        /// <summary>
        /// Indicates that the binding should be used only for injections on the specified type.
        /// Types that derive from the specified type are considered as valid targets.
        /// </summary>
        /// <typeparam name="TParent">The type.</typeparam>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenInWithOrOnInitializationSyntax<T> INewBindingWhenInWithOrOnInitializationSyntax<T>.WhenInjectedInto<TParent>()
        {
            return WhenInjectedInto(typeof(TParent));
        }

        /// <summary>
        /// Indicates that the binding should be used only for injections on the specified type.
        /// Types that derive from the specified type are considered as valid targets.
        /// </summary>
        /// <param name="parent">The type.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenInWithOrOnInitializationSyntax<T> INewBindingWhenInWithOrOnInitializationSyntax<T>.WhenInjectedInto(Type parent)
        {
            return WhenInjectedInto(parent);
        }

        /// <summary>
        /// Indicates that the binding should be used only for injections on the specified type.
        /// Types that derive from the specified type are considered as valid targets.
        /// </summary>
        /// <param name="parents">The type.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenInWithOrOnInitializationSyntax<T> INewBindingWhenInWithOrOnInitializationSyntax<T>.WhenInjectedInto(params Type[] parents)
        {
            return WhenInjectedInto(parents);
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
        INewBindingWhenInWithOrOnInitializationSyntax<T> INewBindingWhenInWithOrOnInitializationSyntax<T>.WhenInjectedExactlyInto<TParent>()
        {
            return this.WhenInjectedExactlyInto(typeof(TParent));
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
        INewBindingWhenInWithOrOnInitializationSyntax<T> INewBindingWhenInWithOrOnInitializationSyntax<T>.WhenInjectedExactlyInto(Type parent)
        {
            return this.WhenInjectedExactlyInto(parent);
        }

        /// <summary>
        /// Indicates that the binding should be used only for injections on the specified type.
        /// The type must match exactly the specified type. Types that derive from the specified type
        /// will not be considered as valid target.
        /// Should match at least one of the specified targets.
        /// </summary>
        /// <param name="parents">The types.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenInWithOrOnInitializationSyntax<T> INewBindingWhenInWithOrOnInitializationSyntax<T>.WhenInjectedExactlyInto(params Type[] parents)
        {
            return this.WhenInjectedExactlyInto(parents);
        }

        /// <summary>
        /// Indicates that the binding should be used only when the class being injected has
        /// an attribute of the specified type.
        /// </summary>
        /// <typeparam name="TAttribute">The type of attribute.</typeparam>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenInWithOrOnInitializationSyntax<T> INewBindingWhenInWithOrOnInitializationSyntax<T>.WhenClassHas<TAttribute>()
        {
            return this.WhenClassHas<TAttribute>();
        }

        /// <summary>
        /// Indicates that the binding should be used only when the class being injected has
        /// an attribute of the specified type.
        /// </summary>
        /// <param name="attributeType">The type of attribute.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenInWithOrOnInitializationSyntax<T> INewBindingWhenInWithOrOnInitializationSyntax<T>.WhenClassHas(Type attributeType)
        {
            return this.WhenClassHas(attributeType);
        }

        /// <summary>
        /// Indicates that the binding should be used only when the member being injected has
        /// an attribute of the specified type.
        /// </summary>
        /// <typeparam name="TAttribute">The type of attribute.</typeparam>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenInWithOrOnInitializationSyntax<T> INewBindingWhenInWithOrOnInitializationSyntax<T>.WhenMemberHas<TAttribute>()
        {
            return this.WhenMemberHas<TAttribute>();
        }

        /// <summary>
        /// Indicates that the binding should be used only when the member being injected has
        /// an attribute of the specified type.
        /// </summary>
        /// <param name="attributeType">The type of attribute.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenInWithOrOnInitializationSyntax<T> INewBindingWhenInWithOrOnInitializationSyntax<T>.WhenMemberHas(Type attributeType)
        {
            return this.WhenMemberHas(attributeType);
        }

        /// <summary>
        /// Indicates that the binding should be used only when the target being injected has
        /// an attribute of the specified type.
        /// </summary>
        /// <typeparam name="TAttribute">The type of attribute.</typeparam>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenInWithOrOnInitializationSyntax<T> INewBindingWhenInWithOrOnInitializationSyntax<T>.WhenTargetHas<TAttribute>()
        {
            return this.WhenTargetHas<TAttribute>();
        }

        /// <summary>
        /// Indicates that the binding should be used only when the target being injected has
        /// an attribute of the specified type.
        /// </summary>
        /// <param name="attributeType">The type of attribute.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenInWithOrOnInitializationSyntax<T> INewBindingWhenInWithOrOnInitializationSyntax<T>.WhenTargetHas(Type attributeType)
        {
            return this.WhenTargetHas(attributeType);
        }

        /// <summary>
        /// Indicates that the binding should be used only when the service is being requested
        /// by a service bound with the specified name.
        /// </summary>
        /// <param name="name">The name to expect.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenInWithOrOnInitializationSyntax<T> INewBindingWhenInWithOrOnInitializationSyntax<T>.WhenParentNamed(string name)
        {
            return this.WhenParentNamed(name);
        }

        /// <summary>
        /// Indicates that the binding should be used only when any ancestor is bound with the specified name.
        /// </summary>
        /// <param name="name">The name to expect.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenInWithOrOnInitializationSyntax<T> INewBindingWhenInWithOrOnInitializationSyntax<T>.WhenAnyAncestorNamed(string name)
        {
            return this.WhenAnyAncestorNamed(name);
        }

        /// <summary>
        /// Indicates that the binding should be used only when no ancestor is bound with the specified name.
        /// </summary>
        /// <param name="name">The name to expect.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenInWithOrOnInitializationSyntax<T> INewBindingWhenInWithOrOnInitializationSyntax<T>.WhenNoAncestorNamed(string name)
        {
            return this.WhenNoAncestorNamed(name);
        }

        /// <summary>
        /// Indicates that the binding should be used only when any ancestor matches the specified predicate.
        /// </summary>
        /// <param name="predicate">The predicate to match.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenInWithOrOnInitializationSyntax<T> INewBindingWhenInWithOrOnInitializationSyntax<T>.WhenAnyAncestorMatches(Predicate<IContext> predicate)
        {
            return this.WhenAnyAncestorMatches(predicate);
        }

        /// <summary>
        /// Indicates that the binding should be used only when no ancestor matches the specified predicate.
        /// </summary>
        /// <param name="predicate">The predicate to match.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenInWithOrOnInitializationSyntax<T> INewBindingWhenInWithOrOnInitializationSyntax<T>.WhenNoAncestorMatches(Predicate<IContext> predicate)
        {
            return this.WhenNoAncestorMatches(predicate);
        }

        #endregion INewBindingWhenInWithOrOnInitializationSyntax

        #region INewBindingWhenInNamedSyntax

        /// <summary>
        /// Indicates that only a single instance of the binding should be created, and then
        /// should be re-used for all subsequent requests.
        /// </summary>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenNamedOrOnActivationSyntax<T> INewBindingWhenInNamedSyntax<T>.InSingletonScope()
        {
            return InSingletonScope();
        }

        /// <summary>
        /// Indicates that instances activated via the binding should not be re-used, nor have
        /// their lifecycle managed by Ninject.
        /// </summary>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenNamedSyntax<T> INewBindingWhenInNamedSyntax<T>.InTransientScope()
        {
            return InTransientScope();
        }

        /// <summary>
        /// Indicates that instances activated via the binding should be re-used within the same thread.
        /// </summary>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenNamedOrOnActivationSyntax<T> INewBindingWhenInNamedSyntax<T>.InThreadScope()
        {
            return InThreadScope();
        }

        /// <summary>
        /// Indicates that instances activated via the binding should be re-used as long as the object
        /// returned by the provided callback remains alive (that is, has not been garbage collected).
        /// </summary>
        /// <param name="scope">The callback that returns the scope.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenNamedOrOnActivationSyntax<T> INewBindingWhenInNamedSyntax<T>.InScope(Func<IContext, object> scope)
        {
            return this.InScope(scope);
        }

        /// <summary>
        /// Indicates that the binding should be used only for requests that support the specified condition.
        /// </summary>
        /// <param name="condition">The condition.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenInNamedSyntax<T> INewBindingWhenInNamedSyntax<T>.When(Func<IRequest, bool> condition)
        {
            return When(condition);
        }

        /// <summary>
        /// Indicates that the binding should be used only for injections on the specified type.
        /// Types that derive from the specified type are considered as valid targets.
        /// </summary>
        /// <typeparam name="TParent">The type.</typeparam>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenInNamedSyntax<T> INewBindingWhenInNamedSyntax<T>.WhenInjectedInto<TParent>()
        {
            return WhenInjectedInto(typeof(TParent));
        }

        /// <summary>
        /// Indicates that the binding should be used only for injections on the specified type.
        /// Types that derive from the specified type are considered as valid targets.
        /// </summary>
        /// <param name="parent">The type.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenInNamedSyntax<T> INewBindingWhenInNamedSyntax<T>.WhenInjectedInto(Type parent)
        {
            return WhenInjectedInto(parent);
        }

        /// <summary>
        /// Indicates that the binding should be used only for injections on the specified type.
        /// Types that derive from the specified type are considered as valid targets.
        /// </summary>
        /// <param name="parents">The type.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenInNamedSyntax<T> INewBindingWhenInNamedSyntax<T>.WhenInjectedInto(params Type[] parents)
        {
            return WhenInjectedInto(parents);
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
        INewBindingWhenInNamedSyntax<T> INewBindingWhenInNamedSyntax<T>.WhenInjectedExactlyInto<TParent>()
        {
            return this.WhenInjectedExactlyInto(typeof(TParent));
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
        INewBindingWhenInNamedSyntax<T> INewBindingWhenInNamedSyntax<T>.WhenInjectedExactlyInto(Type parent)
        {
            return this.WhenInjectedExactlyInto(parent);
        }

        /// <summary>
        /// Indicates that the binding should be used only for injections on the specified type.
        /// The type must match exactly the specified type. Types that derive from the specified type
        /// will not be considered as valid target.
        /// Should match at least one of the specified targets.
        /// </summary>
        /// <param name="parents">The types.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenInNamedSyntax<T> INewBindingWhenInNamedSyntax<T>.WhenInjectedExactlyInto(params Type[] parents)
        {
            return this.WhenInjectedExactlyInto(parents);
        }

        /// <summary>
        /// Indicates that the binding should be used only when the class being injected has
        /// an attribute of the specified type.
        /// </summary>
        /// <typeparam name="TAttribute">The type of attribute.</typeparam>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenInNamedSyntax<T> INewBindingWhenInNamedSyntax<T>.WhenClassHas<TAttribute>()
        {
            return this.WhenClassHas<TAttribute>();
        }

        /// <summary>
        /// Indicates that the binding should be used only when the class being injected has
        /// an attribute of the specified type.
        /// </summary>
        /// <param name="attributeType">The type of attribute.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenInNamedSyntax<T> INewBindingWhenInNamedSyntax<T>.WhenClassHas(Type attributeType)
        {
            return this.WhenClassHas(attributeType);
        }

        /// <summary>
        /// Indicates that the binding should be used only when the member being injected has
        /// an attribute of the specified type.
        /// </summary>
        /// <typeparam name="TAttribute">The type of attribute.</typeparam>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenInNamedSyntax<T> INewBindingWhenInNamedSyntax<T>.WhenMemberHas<TAttribute>()
        {
            return this.WhenMemberHas<TAttribute>();
        }

        /// <summary>
        /// Indicates that the binding should be used only when the member being injected has
        /// an attribute of the specified type.
        /// </summary>
        /// <param name="attributeType">The type of attribute.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenInNamedSyntax<T> INewBindingWhenInNamedSyntax<T>.WhenMemberHas(Type attributeType)
        {
            return this.WhenMemberHas(attributeType);
        }

        /// <summary>
        /// Indicates that the binding should be used only when the target being injected has
        /// an attribute of the specified type.
        /// </summary>
        /// <typeparam name="TAttribute">The type of attribute.</typeparam>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenInNamedSyntax<T> INewBindingWhenInNamedSyntax<T>.WhenTargetHas<TAttribute>()
        {
            return this.WhenTargetHas<TAttribute>();
        }

        /// <summary>
        /// Indicates that the binding should be used only when the target being injected has
        /// an attribute of the specified type.
        /// </summary>
        /// <param name="attributeType">The type of attribute.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenInNamedSyntax<T> INewBindingWhenInNamedSyntax<T>.WhenTargetHas(Type attributeType)
        {
            return this.WhenTargetHas(attributeType);
        }

        /// <summary>
        /// Indicates that the binding should be used only when the service is being requested
        /// by a service bound with the specified name.
        /// </summary>
        /// <param name="name">The name to expect.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenInNamedSyntax<T> INewBindingWhenInNamedSyntax<T>.WhenParentNamed(string name)
        {
            return this.WhenParentNamed(name);
        }

        /// <summary>
        /// Indicates that the binding should be used only when any ancestor is bound with the specified name.
        /// </summary>
        /// <param name="name">The name to expect.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenInNamedSyntax<T> INewBindingWhenInNamedSyntax<T>.WhenAnyAncestorNamed(string name)
        {
            return this.WhenAnyAncestorNamed(name);
        }

        /// <summary>
        /// Indicates that the binding should be used only when no ancestor is bound with the specified name.
        /// </summary>
        /// <param name="name">The name to expect.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenInNamedSyntax<T> INewBindingWhenInNamedSyntax<T>.WhenNoAncestorNamed(string name)
        {
            return this.WhenNoAncestorNamed(name);
        }

        /// <summary>
        /// Indicates that the binding should be used only when any ancestor matches the specified predicate.
        /// </summary>
        /// <param name="predicate">The predicate to match.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenInNamedSyntax<T> INewBindingWhenInNamedSyntax<T>.WhenAnyAncestorMatches(Predicate<IContext> predicate)
        {
            return this.WhenAnyAncestorMatches(predicate);
        }

        /// <summary>
        /// Indicates that the binding should be used only when no ancestor matches the specified predicate.
        /// </summary>
        /// <param name="predicate">The predicate to match.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenInNamedSyntax<T> INewBindingWhenInNamedSyntax<T>.WhenNoAncestorMatches(Predicate<IContext> predicate)
        {
            return this.WhenNoAncestorMatches(predicate);
        }

        /// <summary>
        /// Indicates that the binding should be registered with the specified name. Names are not
        /// necessarily unique; multiple bindings for a given service may be registered with the same name.
        /// </summary>
        /// <param name="name">The name to give the binding.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenInSyntax<T> INewBindingWhenInNamedSyntax<T>.Named(string name)
        {
            return this.Named(name);
        }

        #endregion INewBindingWhenInNamedSyntax

        #region INewBindingWhenNamedOrOnActivationSyntax

        /// <summary>
        /// Indicates that the specified callback should be invoked when instances are activated.
        /// </summary>
        /// <param name="action">The action callback.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenNamedOrOnActivationSyntax<T> INewBindingWhenNamedOrOnActivationSyntax<T>.OnActivation(Action<T> action)
        {
            return this.OnActivation(action);
        }

        /// <summary>
        /// Indicates that the specified callback should be invoked when instances are activated.
        /// </summary>
        /// <typeparam name="TImplementation">The type of the implementation.</typeparam>
        /// <param name="action">The action callback.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenNamedOrOnActivationSyntax<T> INewBindingWhenNamedOrOnActivationSyntax<T>.OnActivation<TImplementation>(Action<TImplementation> action)
        {
            return this.OnActivation(action);
        }

        /// <summary>
        /// Indicates that the specified callback should be invoked when instances are activated.
        /// </summary>
        /// <param name="action">The action callback.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenNamedOrOnActivationSyntax<T> INewBindingWhenNamedOrOnActivationSyntax<T>.OnActivation(Action<IContext, T> action)
        {
            return this.OnActivation(action);
        }

        /// <summary>
        /// Indicates that the specified callback should be invoked when instances are activated.
        /// </summary>
        /// <typeparam name="TImplementation">The type of the implementation.</typeparam>
        /// <param name="action">The action callback.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenNamedOrOnActivationSyntax<T> INewBindingWhenNamedOrOnActivationSyntax<T>.OnActivation<TImplementation>(Action<IContext, TImplementation> action)
        {
            return this.OnActivation(action);
        }

        /// <summary>
        /// Indicates that the specified callback should be invoked when instances are deactivated.
        /// </summary>
        /// <param name="action">The action callback.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenNamedOrOnActivationSyntax<T> INewBindingWhenNamedOrOnActivationSyntax<T>.OnDeactivation(Action<T> action)
        {
            return this.OnDeactivation(action);
        }

        /// <summary>
        /// Indicates that the specified callback should be invoked when instances are deactivated.
        /// </summary>
        /// <param name="action">The action callback.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenNamedOrOnActivationSyntax<T> INewBindingWhenNamedOrOnActivationSyntax<T>.OnDeactivation<TImplementation>(Action<TImplementation> action)
        {
            return this.OnDeactivation(action);
        }

        /// <summary>
        /// Indicates that the specified callback should be invoked when instances are deactivated.
        /// </summary>
        /// <param name="action">The action callback.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenNamedOrOnActivationSyntax<T> INewBindingWhenNamedOrOnActivationSyntax<T>.OnDeactivation(Action<IContext, T> action)
        {
            return this.OnDeactivation(action);
        }

        /// <summary>
        /// Indicates that the specified callback should be invoked when instances are deactivated.
        /// </summary>
        /// <param name="action">The action callback.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenNamedOrOnActivationSyntax<T> INewBindingWhenNamedOrOnActivationSyntax<T>.OnDeactivation<TImplementation>(Action<IContext, TImplementation> action)
        {
            return this.OnDeactivation(action);
        }

        /// <summary>
        /// Indicates that the binding should be used only for requests that support the specified condition.
        /// </summary>
        /// <param name="condition">The condition.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenNamedOrOnActivationSyntax<T> INewBindingWhenNamedOrOnActivationSyntax<T>.When(Func<IRequest, bool> condition)
        {
            return When(condition);
        }

        /// <summary>
        /// Indicates that the binding should be used only for injections on the specified type.
        /// Types that derive from the specified type are considered as valid targets.
        /// </summary>
        /// <typeparam name="TParent">The type.</typeparam>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenNamedOrOnActivationSyntax<T> INewBindingWhenNamedOrOnActivationSyntax<T>.WhenInjectedInto<TParent>()
        {
            return WhenInjectedInto(typeof(TParent));
        }

        /// <summary>
        /// Indicates that the binding should be used only for injections on the specified type.
        /// Types that derive from the specified type are considered as valid targets.
        /// </summary>
        /// <param name="parent">The type.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenNamedOrOnActivationSyntax<T> INewBindingWhenNamedOrOnActivationSyntax<T>.WhenInjectedInto(Type parent)
        {
            return WhenInjectedInto(parent);
        }

        /// <summary>
        /// Indicates that the binding should be used only for injections on the specified type.
        /// Types that derive from the specified type are considered as valid targets.
        /// </summary>
        /// <param name="parents">The type.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenNamedOrOnActivationSyntax<T> INewBindingWhenNamedOrOnActivationSyntax<T>.WhenInjectedInto(params Type[] parents)
        {
            return WhenInjectedInto(parents);
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
        INewBindingWhenNamedOrOnActivationSyntax<T> INewBindingWhenNamedOrOnActivationSyntax<T>.WhenInjectedExactlyInto<TParent>()
        {
            return this.WhenInjectedExactlyInto(typeof(TParent));
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
        INewBindingWhenNamedOrOnActivationSyntax<T> INewBindingWhenNamedOrOnActivationSyntax<T>.WhenInjectedExactlyInto(Type parent)
        {
            return this.WhenInjectedExactlyInto(parent);
        }

        /// <summary>
        /// Indicates that the binding should be used only for injections on the specified type.
        /// The type must match exactly the specified type. Types that derive from the specified type
        /// will not be considered as valid target.
        /// Should match at least one of the specified targets.
        /// </summary>
        /// <param name="parents">The types.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenNamedOrOnActivationSyntax<T> INewBindingWhenNamedOrOnActivationSyntax<T>.WhenInjectedExactlyInto(params Type[] parents)
        {
            return this.WhenInjectedExactlyInto(parents);
        }

        /// <summary>
        /// Indicates that the binding should be used only when the class being injected has
        /// an attribute of the specified type.
        /// </summary>
        /// <typeparam name="TAttribute">The type of attribute.</typeparam>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenNamedOrOnActivationSyntax<T> INewBindingWhenNamedOrOnActivationSyntax<T>.WhenClassHas<TAttribute>()
        {
            return this.WhenClassHas<TAttribute>();
        }

        /// <summary>
        /// Indicates that the binding should be used only when the class being injected has
        /// an attribute of the specified type.
        /// </summary>
        /// <param name="attributeType">The type of attribute.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenNamedOrOnActivationSyntax<T> INewBindingWhenNamedOrOnActivationSyntax<T>.WhenClassHas(Type attributeType)
        {
            return this.WhenClassHas(attributeType);
        }

        /// <summary>
        /// Indicates that the binding should be used only when the member being injected has
        /// an attribute of the specified type.
        /// </summary>
        /// <typeparam name="TAttribute">The type of attribute.</typeparam>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenNamedOrOnActivationSyntax<T> INewBindingWhenNamedOrOnActivationSyntax<T>.WhenMemberHas<TAttribute>()
        {
            return this.WhenMemberHas<TAttribute>();
        }

        /// <summary>
        /// Indicates that the binding should be used only when the member being injected has
        /// an attribute of the specified type.
        /// </summary>
        /// <param name="attributeType">The type of attribute.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenNamedOrOnActivationSyntax<T> INewBindingWhenNamedOrOnActivationSyntax<T>.WhenMemberHas(Type attributeType)
        {
            return this.WhenMemberHas(attributeType);
        }

        /// <summary>
        /// Indicates that the binding should be used only when the target being injected has
        /// an attribute of the specified type.
        /// </summary>
        /// <typeparam name="TAttribute">The type of attribute.</typeparam>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenNamedOrOnActivationSyntax<T> INewBindingWhenNamedOrOnActivationSyntax<T>.WhenTargetHas<TAttribute>()
        {
            return this.WhenTargetHas<TAttribute>();
        }

        /// <summary>
        /// Indicates that the binding should be used only when the target being injected has
        /// an attribute of the specified type.
        /// </summary>
        /// <param name="attributeType">The type of attribute.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenNamedOrOnActivationSyntax<T> INewBindingWhenNamedOrOnActivationSyntax<T>.WhenTargetHas(Type attributeType)
        {
            return this.WhenTargetHas(attributeType);
        }

        /// <summary>
        /// Indicates that the binding should be used only when the service is being requested
        /// by a service bound with the specified name.
        /// </summary>
        /// <param name="name">The name to expect.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenNamedOrOnActivationSyntax<T> INewBindingWhenNamedOrOnActivationSyntax<T>.WhenParentNamed(string name)
        {
            return this.WhenParentNamed(name);
        }

        /// <summary>
        /// Indicates that the binding should be used only when any ancestor is bound with the specified name.
        /// </summary>
        /// <param name="name">The name to expect.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenNamedOrOnActivationSyntax<T> INewBindingWhenNamedOrOnActivationSyntax<T>.WhenAnyAncestorNamed(string name)
        {
            return this.WhenAnyAncestorNamed(name);
        }

        /// <summary>
        /// Indicates that the binding should be used only when no ancestor is bound with the specified name.
        /// </summary>
        /// <param name="name">The name to expect.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenNamedOrOnActivationSyntax<T> INewBindingWhenNamedOrOnActivationSyntax<T>.WhenNoAncestorNamed(string name)
        {
            return this.WhenNoAncestorNamed(name);
        }

        /// <summary>
        /// Indicates that the binding should be used only when any ancestor matches the specified predicate.
        /// </summary>
        /// <param name="predicate">The predicate to match.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenNamedOrOnActivationSyntax<T> INewBindingWhenNamedOrOnActivationSyntax<T>.WhenAnyAncestorMatches(Predicate<IContext> predicate)
        {
            return this.WhenAnyAncestorMatches(predicate);
        }

        /// <summary>
        /// Indicates that the binding should be used only when no ancestor matches the specified predicate.
        /// </summary>
        /// <param name="predicate">The predicate to match.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenNamedOrOnActivationSyntax<T> INewBindingWhenNamedOrOnActivationSyntax<T>.WhenNoAncestorMatches(Predicate<IContext> predicate)
        {
            return this.WhenNoAncestorMatches(predicate);
        }

        /// <summary>
        /// Indicates that the binding should be registered with the specified name. Names are not
        /// necessarily unique; multiple bindings for a given service may be registered with the same name.
        /// </summary>
        /// <param name="name">The name to give the binding.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenOrOnActivationSyntax<T> INewBindingWhenNamedOrOnActivationSyntax<T>.Named(string name)
        {
            return this.Named(name);
        }

        #endregion INewBindingWhenNamedOrOnActivationSyntax

        #region INewBindingWhenWithOrOnSyntax

        /// <summary>
        /// Indicates that the specified callback should be invoked when instances are activated.
        /// </summary>
        /// <param name="action">The action callback.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenWithOrOnSyntax<T> INewBindingWhenWithOrOnSyntax<T>.OnActivation(Action<T> action)
        {
            return this.OnActivation(action);
        }

        /// <summary>
        /// Indicates that the specified callback should be invoked when instances are activated.
        /// </summary>
        /// <typeparam name="TImplementation">The type of the implementation.</typeparam>
        /// <param name="action">The action callback.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenWithOrOnSyntax<T> INewBindingWhenWithOrOnSyntax<T>.OnActivation<TImplementation>(Action<TImplementation> action)
        {
            return this.OnActivation(action);
        }

        /// <summary>
        /// Indicates that the specified callback should be invoked when instances are activated.
        /// </summary>
        /// <param name="action">The action callback.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenWithOrOnSyntax<T> INewBindingWhenWithOrOnSyntax<T>.OnActivation(Action<IContext, T> action)
        {
            return this.OnActivation(action);
        }

        /// <summary>
        /// Indicates that the specified callback should be invoked when instances are activated.
        /// </summary>
        /// <typeparam name="TImplementation">The type of the implementation.</typeparam>
        /// <param name="action">The action callback.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenWithOrOnSyntax<T> INewBindingWhenWithOrOnSyntax<T>.OnActivation<TImplementation>(Action<IContext, TImplementation> action)
        {
            return this.OnActivation(action);
        }

        /// <summary>
        /// Indicates that the specified callback should be invoked when instances are deactivated.
        /// </summary>
        /// <param name="action">The action callback.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenWithOrOnSyntax<T> INewBindingWhenWithOrOnSyntax<T>.OnDeactivation(Action<T> action)
        {
            return this.OnDeactivation(action);
        }

        /// <summary>
        /// Indicates that the specified callback should be invoked when instances are deactivated.
        /// </summary>
        /// <param name="action">The action callback.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenWithOrOnSyntax<T> INewBindingWhenWithOrOnSyntax<T>.OnDeactivation<TImplementation>(Action<TImplementation> action)
        {
            return this.OnDeactivation(action);
        }

        /// <summary>
        /// Indicates that the specified callback should be invoked when instances are deactivated.
        /// </summary>
        /// <param name="action">The action callback.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenWithOrOnSyntax<T> INewBindingWhenWithOrOnSyntax<T>.OnDeactivation(Action<IContext, T> action)
        {
            return this.OnDeactivation(action);
        }

        /// <summary>
        /// Indicates that the specified callback should be invoked when instances are deactivated.
        /// </summary>
        /// <param name="action">The action callback.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenWithOrOnSyntax<T> INewBindingWhenWithOrOnSyntax<T>.OnDeactivation<TImplementation>(Action<IContext, TImplementation> action)
        {
            return this.OnDeactivation(action);
        }

        /// <summary>
        /// Indicates that the specified callback should be invoked when instances are initialized.
        /// </summary>
        /// <param name="action">The action callback.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenWithOrOnSyntax<T> INewBindingWhenWithOrOnSyntax<T>.OnInitialization(Action<T> action)
        {
            return this.OnInitialization(action);
        }

        /// <summary>
        /// Indicates that the specified callback should be invoked when instances are initialized.
        /// </summary>
        /// <typeparam name="TImplementation">The type of the implementation.</typeparam>
        /// <param name="action">The action callback.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenWithOrOnSyntax<T> INewBindingWhenWithOrOnSyntax<T>.OnInitialization<TImplementation>(Action<TImplementation> action)
        {
            return this.OnInitialization(action);
        }

        /// <summary>
        /// Indicates that the specified callback should be invoked when instances are initialized.
        /// </summary>
        /// <param name="action">The action callback.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenWithOrOnSyntax<T> INewBindingWhenWithOrOnSyntax<T>.OnInitialization(Action<IContext, T> action)
        {
            return this.OnInitialization(action);
        }

        /// <summary>
        /// Indicates that the specified callback should be invoked when instances are initialized.
        /// </summary>
        /// <typeparam name="TImplementation">The type of the implementation.</typeparam>
        /// <param name="action">The action callback.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenWithOrOnSyntax<T> INewBindingWhenWithOrOnSyntax<T>.OnInitialization<TImplementation>(Action<IContext, TImplementation> action)
        {
            return this.OnInitialization(action);
        }

        /// <summary>
        /// Indicates that the specified callback should be invoked when instances are initialized.
        /// </summary>
        /// <typeparam name="TImplementation">The type of the implementation.</typeparam>
        /// <param name="action">The action callback.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenWithOrOnSyntax<T> INewBindingWhenWithOrOnSyntax<T>.OnInitialization<TImplementation>(Func<IContext, TImplementation, TImplementation> action)
        {
            return this.OnInitialization(action);
        }

        /// <summary>
        /// Indicates that the specified constructor argument should be overridden with the specified value.
        /// </summary>
        /// <param name="name">The name of the argument to override.</param>
        /// <param name="value">The value for the argument.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenWithOrOnSyntax<T> INewBindingWhenWithOrOnSyntax<T>.WithConstructorArgument(string name, object value)
        {
            return this.WithConstructorArgument(name, value);
        }

        /// <summary>
        /// Indicates that the specified constructor argument should be overridden with the specified value.
        /// </summary>
        /// <param name="name">The name of the argument to override.</param>
        /// <param name="callback">The callback to invoke to get the value for the argument.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenWithOrOnSyntax<T> INewBindingWhenWithOrOnSyntax<T>.WithConstructorArgument(string name, Func<IContext, object> callback)
        {
            return this.WithConstructorArgument(name, callback);
        }

        /// <summary>
        /// Indicates that the specified constructor argument should be overridden with the specified value.
        /// </summary>
        /// <param name="name">The name of the argument to override.</param>
        /// <param name="callback">The callback to invoke to get the value for the argument.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenWithOrOnSyntax<T> INewBindingWhenWithOrOnSyntax<T>.WithConstructorArgument(string name, Func<IContext, ITarget<ParameterInfo>, object> callback)
        {
            return this.WithConstructorArgument(name, callback);
        }

        /// <summary>
        /// Indicates that the specified constructor argument should be overridden with the specified value.
        /// </summary>
        /// <typeparam name="TValue">Specifies the argument type to override.</typeparam>
        /// <param name="value">The value for the argument.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenWithOrOnSyntax<T> INewBindingWhenWithOrOnSyntax<T>.WithConstructorArgument<TValue>(TValue value)
        {
            return this.WithConstructorArgument(value);
        }

        /// <summary>
        /// Indicates that the specified constructor argument should be overridden with the specified value.
        /// </summary>
        /// <param name="type">The type of the argument to override.</param>
        /// <param name="value">The value for the argument.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenWithOrOnSyntax<T> INewBindingWhenWithOrOnSyntax<T>.WithConstructorArgument(Type type, object value)
        {
            return this.WithConstructorArgument(type, (context, target) => value);
        }

        /// <summary>
        /// Indicates that the specified constructor argument should be overridden with the specified value.
        /// </summary>
        /// <typeparam name="TValue">The type of the argument type to override.</typeparam>
        /// <param name="callback">The callback to invoke to get the value for the argument.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenWithOrOnSyntax<T> INewBindingWhenWithOrOnSyntax<T>.WithConstructorArgument<TValue>(Func<IContext, TValue> callback)
        {
            return this.WithConstructorArgument(callback);
        }

        /// <summary>
        /// Indicates that the specified constructor argument should be overridden with the specified value.
        /// </summary>
        /// <param name="type">The type of the argument to override.</param>
        /// <param name="callback">The callback to invoke to get the value for the argument.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenWithOrOnSyntax<T> INewBindingWhenWithOrOnSyntax<T>.WithConstructorArgument(Type type, Func<IContext, object> callback)
        {
            return this.WithConstructorArgument(type, (context, target) => callback(context));
        }

        /// <summary>
        /// Indicates that the specified constructor argument should be overridden with the specified value.
        /// </summary>
        /// <typeparam name="TValue">The type of the argument type to override.</typeparam>
        /// <param name="callback">The callback to invoke to get the value for the argument.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenWithOrOnSyntax<T> INewBindingWhenWithOrOnSyntax<T>.WithConstructorArgument<TValue>(Func<IContext, ITarget<ParameterInfo>, TValue> callback)
        {
            return this.WithConstructorArgument(typeof(TValue), callback);
        }

        /// <summary>
        /// Indicates that the specified constructor argument should be overridden with the specified value.
        /// </summary>
        /// <param name="type">The type of the argument to override.</param>
        /// <param name="callback">The callback to invoke to get the value for the argument.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenWithOrOnSyntax<T> INewBindingWhenWithOrOnSyntax<T>.WithConstructorArgument(Type type, Func<IContext, ITarget<ParameterInfo>, object> callback)
        {
            return this.WithConstructorArgument(type, callback);
        }

        /// <summary>
        /// Indicates that the specified property should be injected with the specified value.
        /// </summary>
        /// <param name="name">The name of the property to override.</param>
        /// <param name="value">The value for the property.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenWithOrOnSyntax<T> INewBindingWhenWithOrOnSyntax<T>.WithPropertyValue(string name, object value)
        {
            return this.WithPropertyValue(name, value);
        }

        /// <summary>
        /// Indicates that the specified property should be injected with the specified value.
        /// </summary>
        /// <param name="name">The name of the property to override.</param>
        /// <param name="callback">The callback to invoke to get the value for the property.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenWithOrOnSyntax<T> INewBindingWhenWithOrOnSyntax<T>.WithPropertyValue(string name, Func<IContext, object> callback)
        {
            return this.WithPropertyValue(name, callback);
        }

        /// <summary>
        /// Indicates that the specified property should be injected with the specified value.
        /// </summary>
        /// <param name="name">The name of the property to override.</param>
        /// <param name="callback">The callback to invoke to get the value for the property.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenWithOrOnSyntax<T> INewBindingWhenWithOrOnSyntax<T>.WithPropertyValue(string name, Func<IContext, ITarget<PropertyInfo>, object> callback)
        {
            return this.WithPropertyValue(name, callback);
        }

        /// <summary>
        /// Adds a custom parameter to the binding.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenWithOrOnSyntax<T> INewBindingWhenWithOrOnSyntax<T>.WithParameter(IParameter parameter)
        {
            return this.WithParameter(parameter);
        }

        /// <summary>
        /// Sets the value of a piece of metadata on the binding.
        /// </summary>
        /// <param name="key">The metadata key.</param>
        /// <param name="value">The metadata value.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenWithOrOnSyntax<T> INewBindingWhenWithOrOnSyntax<T>.WithMetadata(string key, object value)
        {
            return this.WithMetadata(key, value);
        }

        /// <summary>
        /// Indicates that the binding should be used only for requests that support the specified condition.
        /// </summary>
        /// <param name="condition">The condition.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenWithOrOnSyntax<T> INewBindingWhenWithOrOnSyntax<T>.When(Func<IRequest, bool> condition)
        {
            return When(condition);
        }

        /// <summary>
        /// Indicates that the binding should be used only for injections on the specified type.
        /// Types that derive from the specified type are considered as valid targets.
        /// </summary>
        /// <typeparam name="TParent">The type.</typeparam>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenWithOrOnSyntax<T> INewBindingWhenWithOrOnSyntax<T>.WhenInjectedInto<TParent>()
        {
            return WhenInjectedInto(typeof(TParent));
        }

        /// <summary>
        /// Indicates that the binding should be used only for injections on the specified type.
        /// Types that derive from the specified type are considered as valid targets.
        /// </summary>
        /// <param name="parent">The type.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenWithOrOnSyntax<T> INewBindingWhenWithOrOnSyntax<T>.WhenInjectedInto(Type parent)
        {
            return WhenInjectedInto(parent);
        }

        /// <summary>
        /// Indicates that the binding should be used only for injections on the specified type.
        /// Types that derive from the specified type are considered as valid targets.
        /// </summary>
        /// <param name="parents">The type.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenWithOrOnSyntax<T> INewBindingWhenWithOrOnSyntax<T>.WhenInjectedInto(params Type[] parents)
        {
            return WhenInjectedInto(parents);
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
        INewBindingWhenWithOrOnSyntax<T> INewBindingWhenWithOrOnSyntax<T>.WhenInjectedExactlyInto<TParent>()
        {
            return this.WhenInjectedExactlyInto(typeof(TParent));
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
        INewBindingWhenWithOrOnSyntax<T> INewBindingWhenWithOrOnSyntax<T>.WhenInjectedExactlyInto(Type parent)
        {
            return this.WhenInjectedExactlyInto(parent);
        }

        /// <summary>
        /// Indicates that the binding should be used only for injections on the specified type.
        /// The type must match exactly the specified type. Types that derive from the specified type
        /// will not be considered as valid target.
        /// Should match at least one of the specified targets.
        /// </summary>
        /// <param name="parents">The types.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenWithOrOnSyntax<T> INewBindingWhenWithOrOnSyntax<T>.WhenInjectedExactlyInto(params Type[] parents)
        {
            return this.WhenInjectedExactlyInto(parents);
        }

        /// <summary>
        /// Indicates that the binding should be used only when the class being injected has
        /// an attribute of the specified type.
        /// </summary>
        /// <typeparam name="TAttribute">The type of attribute.</typeparam>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenWithOrOnSyntax<T> INewBindingWhenWithOrOnSyntax<T>.WhenClassHas<TAttribute>()
        {
            return this.WhenClassHas<TAttribute>();
        }

        /// <summary>
        /// Indicates that the binding should be used only when the class being injected has
        /// an attribute of the specified type.
        /// </summary>
        /// <param name="attributeType">The type of attribute.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenWithOrOnSyntax<T> INewBindingWhenWithOrOnSyntax<T>.WhenClassHas(Type attributeType)
        {
            return this.WhenClassHas(attributeType);
        }

        /// <summary>
        /// Indicates that the binding should be used only when the member being injected has
        /// an attribute of the specified type.
        /// </summary>
        /// <typeparam name="TAttribute">The type of attribute.</typeparam>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenWithOrOnSyntax<T> INewBindingWhenWithOrOnSyntax<T>.WhenMemberHas<TAttribute>()
        {
            return this.WhenMemberHas<TAttribute>();
        }

        /// <summary>
        /// Indicates that the binding should be used only when the member being injected has
        /// an attribute of the specified type.
        /// </summary>
        /// <param name="attributeType">The type of attribute.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenWithOrOnSyntax<T> INewBindingWhenWithOrOnSyntax<T>.WhenMemberHas(Type attributeType)
        {
            return this.WhenMemberHas(attributeType);
        }

        /// <summary>
        /// Indicates that the binding should be used only when the target being injected has
        /// an attribute of the specified type.
        /// </summary>
        /// <typeparam name="TAttribute">The type of attribute.</typeparam>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenWithOrOnSyntax<T> INewBindingWhenWithOrOnSyntax<T>.WhenTargetHas<TAttribute>()
        {
            return this.WhenTargetHas<TAttribute>();
        }

        /// <summary>
        /// Indicates that the binding should be used only when the target being injected has
        /// an attribute of the specified type.
        /// </summary>
        /// <param name="attributeType">The type of attribute.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenWithOrOnSyntax<T> INewBindingWhenWithOrOnSyntax<T>.WhenTargetHas(Type attributeType)
        {
            return this.WhenTargetHas(attributeType);
        }

        /// <summary>
        /// Indicates that the binding should be used only when the service is being requested
        /// by a service bound with the specified name.
        /// </summary>
        /// <param name="name">The name to expect.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenWithOrOnSyntax<T> INewBindingWhenWithOrOnSyntax<T>.WhenParentNamed(string name)
        {
            return this.WhenParentNamed(name);
        }

        /// <summary>
        /// Indicates that the binding should be used only when any ancestor is bound with the specified name.
        /// </summary>
        /// <param name="name">The name to expect.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenWithOrOnSyntax<T> INewBindingWhenWithOrOnSyntax<T>.WhenAnyAncestorNamed(string name)
        {
            return this.WhenAnyAncestorNamed(name);
        }

        /// <summary>
        /// Indicates that the binding should be used only when no ancestor is bound with the specified name.
        /// </summary>
        /// <param name="name">The name to expect.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenWithOrOnSyntax<T> INewBindingWhenWithOrOnSyntax<T>.WhenNoAncestorNamed(string name)
        {
            return this.WhenNoAncestorNamed(name);
        }

        /// <summary>
        /// Indicates that the binding should be used only when any ancestor matches the specified predicate.
        /// </summary>
        /// <param name="predicate">The predicate to match.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenWithOrOnSyntax<T> INewBindingWhenWithOrOnSyntax<T>.WhenAnyAncestorMatches(Predicate<IContext> predicate)
        {
            return this.WhenAnyAncestorMatches(predicate);
        }

        /// <summary>
        /// Indicates that the binding should be used only when no ancestor matches the specified predicate.
        /// </summary>
        /// <param name="predicate">The predicate to match.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenWithOrOnSyntax<T> INewBindingWhenWithOrOnSyntax<T>.WhenNoAncestorMatches(Predicate<IContext> predicate)
        {
            return this.WhenNoAncestorMatches(predicate);
        }

        #endregion INewBindingWhenWithOrOnSyntax

        #region INewBindingWhenInSyntax

        /// <summary>
        /// Indicates that only a single instance of the binding should be created, and then
        /// should be re-used for all subsequent requests.
        /// </summary>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenOrOnActivationSyntax<T> INewBindingWhenInSyntax<T>.InSingletonScope()
        {
            return this.InSingletonScope();
        }

        /// <summary>
        /// Indicates that instances activated via the binding should be re-used within the same thread.
        /// </summary>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenOrOnActivationSyntax<T> INewBindingWhenInSyntax<T>.InThreadScope()
        {
            return this.InThreadScope();
        }

        /// <summary>
        /// Indicates that instances activated via the binding should be re-used as long as the object
        /// returned by the provided callback remains alive (that is, has not been garbage collected).
        /// </summary>
        /// <param name="scope">The callback that returns the scope.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenOrOnActivationSyntax<T> INewBindingWhenInSyntax<T>.InScope(Func<IContext, object> scope)
        {
            return this.InScope(scope);
        }

        /// <summary>
        /// Indicates that instances activated via the binding should not be re-used, nor have
        /// their lifecycle managed by Ninject.
        /// </summary>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenSyntax<T> INewBindingWhenInSyntax<T>.InTransientScope()
        {
            return this.InTransientScope();
        }

        /// <summary>
        /// Indicates that the binding should be used only for requests that support the specified condition.
        /// </summary>
        /// <param name="condition">The condition.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenInSyntax<T> INewBindingWhenInSyntax<T>.When(Func<IRequest, bool> condition)
        {
            return When(condition);
        }

        /// <summary>
        /// Indicates that the binding should be used only for injections on the specified type.
        /// Types that derive from the specified type are considered as valid targets.
        /// </summary>
        /// <typeparam name="TParent">The type.</typeparam>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenInSyntax<T> INewBindingWhenInSyntax<T>.WhenInjectedInto<TParent>()
        {
            return WhenInjectedInto(typeof(TParent));
        }

        /// <summary>
        /// Indicates that the binding should be used only for injections on the specified type.
        /// Types that derive from the specified type are considered as valid targets.
        /// </summary>
        /// <param name="parent">The type.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenInSyntax<T> INewBindingWhenInSyntax<T>.WhenInjectedInto(Type parent)
        {
            return WhenInjectedInto(parent);
        }

        /// <summary>
        /// Indicates that the binding should be used only for injections on the specified type.
        /// Types that derive from the specified type are considered as valid targets.
        /// </summary>
        /// <param name="parents">The type.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenInSyntax<T> INewBindingWhenInSyntax<T>.WhenInjectedInto(params Type[] parents)
        {
            return WhenInjectedInto(parents);
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
        INewBindingWhenInSyntax<T> INewBindingWhenInSyntax<T>.WhenInjectedExactlyInto<TParent>()
        {
            return this.WhenInjectedExactlyInto(typeof(TParent));
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
        INewBindingWhenInSyntax<T> INewBindingWhenInSyntax<T>.WhenInjectedExactlyInto(Type parent)
        {
            return this.WhenInjectedExactlyInto(parent);
        }

        /// <summary>
        /// Indicates that the binding should be used only for injections on the specified type.
        /// The type must match exactly the specified type. Types that derive from the specified type
        /// will not be considered as valid target.
        /// Should match at least one of the specified targets.
        /// </summary>
        /// <param name="parents">The types.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenInSyntax<T> INewBindingWhenInSyntax<T>.WhenInjectedExactlyInto(params Type[] parents)
        {
            return this.WhenInjectedExactlyInto(parents);
        }

        /// <summary>
        /// Indicates that the binding should be used only when the class being injected has
        /// an attribute of the specified type.
        /// </summary>
        /// <typeparam name="TAttribute">The type of attribute.</typeparam>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenInSyntax<T> INewBindingWhenInSyntax<T>.WhenClassHas<TAttribute>()
        {
            return this.WhenClassHas<TAttribute>();
        }

        /// <summary>
        /// Indicates that the binding should be used only when the class being injected has
        /// an attribute of the specified type.
        /// </summary>
        /// <param name="attributeType">The type of attribute.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenInSyntax<T> INewBindingWhenInSyntax<T>.WhenClassHas(Type attributeType)
        {
            return this.WhenClassHas(attributeType);
        }

        /// <summary>
        /// Indicates that the binding should be used only when the member being injected has
        /// an attribute of the specified type.
        /// </summary>
        /// <typeparam name="TAttribute">The type of attribute.</typeparam>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenInSyntax<T> INewBindingWhenInSyntax<T>.WhenMemberHas<TAttribute>()
        {
            return this.WhenMemberHas<TAttribute>();
        }

        /// <summary>
        /// Indicates that the binding should be used only when the member being injected has
        /// an attribute of the specified type.
        /// </summary>
        /// <param name="attributeType">The type of attribute.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenInSyntax<T> INewBindingWhenInSyntax<T>.WhenMemberHas(Type attributeType)
        {
            return this.WhenMemberHas(attributeType);
        }

        /// <summary>
        /// Indicates that the binding should be used only when the target being injected has
        /// an attribute of the specified type.
        /// </summary>
        /// <typeparam name="TAttribute">The type of attribute.</typeparam>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenInSyntax<T> INewBindingWhenInSyntax<T>.WhenTargetHas<TAttribute>()
        {
            return this.WhenTargetHas<TAttribute>();
        }

        /// <summary>
        /// Indicates that the binding should be used only when the target being injected has
        /// an attribute of the specified type.
        /// </summary>
        /// <param name="attributeType">The type of attribute.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenInSyntax<T> INewBindingWhenInSyntax<T>.WhenTargetHas(Type attributeType)
        {
            return this.WhenTargetHas(attributeType);
        }

        /// <summary>
        /// Indicates that the binding should be used only when the service is being requested
        /// by a service bound with the specified name.
        /// </summary>
        /// <param name="name">The name to expect.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenInSyntax<T> INewBindingWhenInSyntax<T>.WhenParentNamed(string name)
        {
            return this.WhenParentNamed(name);
        }

        /// <summary>
        /// Indicates that the binding should be used only when any ancestor is bound with the specified name.
        /// </summary>
        /// <param name="name">The name to expect.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenInSyntax<T> INewBindingWhenInSyntax<T>.WhenAnyAncestorNamed(string name)
        {
            return this.WhenAnyAncestorNamed(name);
        }

        /// <summary>
        /// Indicates that the binding should be used only when no ancestor is bound with the specified name.
        /// </summary>
        /// <param name="name">The name to expect.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenInSyntax<T> INewBindingWhenInSyntax<T>.WhenNoAncestorNamed(string name)
        {
            return this.WhenNoAncestorNamed(name);
        }

        /// <summary>
        /// Indicates that the binding should be used only when any ancestor matches the specified predicate.
        /// </summary>
        /// <param name="predicate">The predicate to match.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenInSyntax<T> INewBindingWhenInSyntax<T>.WhenAnyAncestorMatches(Predicate<IContext> predicate)
        {
            return this.WhenAnyAncestorMatches(predicate);
        }

        /// <summary>
        /// Indicates that the binding should be used only when no ancestor matches the specified predicate.
        /// </summary>
        /// <param name="predicate">The predicate to match.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenInSyntax<T> INewBindingWhenInSyntax<T>.WhenNoAncestorMatches(Predicate<IContext> predicate)
        {
            return this.WhenNoAncestorMatches(predicate);
        }

        #endregion INewBindingWhenInSyntax

        #region INewBindingWhenOrOnActivationSyntax

        /// <summary>
        /// Indicates that the specified callback should be invoked when instances are activated.
        /// </summary>
        /// <param name="action">The action callback.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenOrOnActivationSyntax<T> INewBindingWhenOrOnActivationSyntax<T>.OnActivation(Action<T> action)
        {
            return this.OnActivation(action);
        }

        /// <summary>
        /// Indicates that the specified callback should be invoked when instances are activated.
        /// </summary>
        /// <typeparam name="TImplementation">The type of the implementation.</typeparam>
        /// <param name="action">The action callback.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenOrOnActivationSyntax<T> INewBindingWhenOrOnActivationSyntax<T>.OnActivation<TImplementation>(Action<TImplementation> action)
        {
            return this.OnActivation(action);
        }

        /// <summary>
        /// Indicates that the specified callback should be invoked when instances are activated.
        /// </summary>
        /// <param name="action">The action callback.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenOrOnActivationSyntax<T> INewBindingWhenOrOnActivationSyntax<T>.OnActivation(Action<IContext, T> action)
        {
            return this.OnActivation(action);
        }

        /// <summary>
        /// Indicates that the specified callback should be invoked when instances are activated.
        /// </summary>
        /// <typeparam name="TImplementation">The type of the implementation.</typeparam>
        /// <param name="action">The action callback.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenOrOnActivationSyntax<T> INewBindingWhenOrOnActivationSyntax<T>.OnActivation<TImplementation>(Action<IContext, TImplementation> action)
        {
            return this.OnActivation(action);
        }

        /// <summary>
        /// Indicates that the specified callback should be invoked when instances are deactivated.
        /// </summary>
        /// <param name="action">The action callback.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenOrOnActivationSyntax<T> INewBindingWhenOrOnActivationSyntax<T>.OnDeactivation(Action<T> action)
        {
            return this.OnDeactivation(action);
        }

        /// <summary>
        /// Indicates that the specified callback should be invoked when instances are deactivated.
        /// </summary>
        /// <param name="action">The action callback.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenOrOnActivationSyntax<T> INewBindingWhenOrOnActivationSyntax<T>.OnDeactivation<TImplementation>(Action<TImplementation> action)
        {
            return this.OnDeactivation(action);
        }

        /// <summary>
        /// Indicates that the specified callback should be invoked when instances are deactivated.
        /// </summary>
        /// <param name="action">The action callback.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenOrOnActivationSyntax<T> INewBindingWhenOrOnActivationSyntax<T>.OnDeactivation(Action<IContext, T> action)
        {
            return this.OnDeactivation(action);
        }

        /// <summary>
        /// Indicates that the specified callback should be invoked when instances are deactivated.
        /// </summary>
        /// <param name="action">The action callback.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenOrOnActivationSyntax<T> INewBindingWhenOrOnActivationSyntax<T>.OnDeactivation<TImplementation>(Action<IContext, TImplementation> action)
        {
            return this.OnDeactivation(action);
        }

        /// <summary>
        /// Indicates that the binding should be used only for requests that support the specified condition.
        /// </summary>
        /// <param name="condition">The condition.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenOrOnActivationSyntax<T> INewBindingWhenOrOnActivationSyntax<T>.When(Func<IRequest, bool> condition)
        {
            return When(condition);
        }

        /// <summary>
        /// Indicates that the binding should be used only for injections on the specified type.
        /// Types that derive from the specified type are considered as valid targets.
        /// </summary>
        /// <typeparam name="TParent">The type.</typeparam>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenOrOnActivationSyntax<T> INewBindingWhenOrOnActivationSyntax<T>.WhenInjectedInto<TParent>()
        {
            return WhenInjectedInto(typeof(TParent));
        }

        /// <summary>
        /// Indicates that the binding should be used only for injections on the specified type.
        /// Types that derive from the specified type are considered as valid targets.
        /// </summary>
        /// <param name="parent">The type.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenOrOnActivationSyntax<T> INewBindingWhenOrOnActivationSyntax<T>.WhenInjectedInto(Type parent)
        {
            return WhenInjectedInto(parent);
        }

        /// <summary>
        /// Indicates that the binding should be used only for injections on the specified type.
        /// Types that derive from the specified type are considered as valid targets.
        /// </summary>
        /// <param name="parents">The type.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenOrOnActivationSyntax<T> INewBindingWhenOrOnActivationSyntax<T>.WhenInjectedInto(params Type[] parents)
        {
            return WhenInjectedInto(parents);
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
        INewBindingWhenOrOnActivationSyntax<T> INewBindingWhenOrOnActivationSyntax<T>.WhenInjectedExactlyInto<TParent>()
        {
            return this.WhenInjectedExactlyInto(typeof(TParent));
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
        INewBindingWhenOrOnActivationSyntax<T> INewBindingWhenOrOnActivationSyntax<T>.WhenInjectedExactlyInto(Type parent)
        {
            return this.WhenInjectedExactlyInto(parent);
        }

        /// <summary>
        /// Indicates that the binding should be used only for injections on the specified type.
        /// The type must match exactly the specified type. Types that derive from the specified type
        /// will not be considered as valid target.
        /// Should match at least one of the specified targets.
        /// </summary>
        /// <param name="parents">The types.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenOrOnActivationSyntax<T> INewBindingWhenOrOnActivationSyntax<T>.WhenInjectedExactlyInto(params Type[] parents)
        {
            return this.WhenInjectedExactlyInto(parents);
        }

        /// <summary>
        /// Indicates that the binding should be used only when the class being injected has
        /// an attribute of the specified type.
        /// </summary>
        /// <typeparam name="TAttribute">The type of attribute.</typeparam>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenOrOnActivationSyntax<T> INewBindingWhenOrOnActivationSyntax<T>.WhenClassHas<TAttribute>()
        {
            return this.WhenClassHas<TAttribute>();
        }

        /// <summary>
        /// Indicates that the binding should be used only when the class being injected has
        /// an attribute of the specified type.
        /// </summary>
        /// <param name="attributeType">The type of attribute.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenOrOnActivationSyntax<T> INewBindingWhenOrOnActivationSyntax<T>.WhenClassHas(Type attributeType)
        {
            return this.WhenClassHas(attributeType);
        }

        /// <summary>
        /// Indicates that the binding should be used only when the member being injected has
        /// an attribute of the specified type.
        /// </summary>
        /// <typeparam name="TAttribute">The type of attribute.</typeparam>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenOrOnActivationSyntax<T> INewBindingWhenOrOnActivationSyntax<T>.WhenMemberHas<TAttribute>()
        {
            return this.WhenMemberHas<TAttribute>();
        }

        /// <summary>
        /// Indicates that the binding should be used only when the member being injected has
        /// an attribute of the specified type.
        /// </summary>
        /// <param name="attributeType">The type of attribute.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenOrOnActivationSyntax<T> INewBindingWhenOrOnActivationSyntax<T>.WhenMemberHas(Type attributeType)
        {
            return this.WhenMemberHas(attributeType);
        }

        /// <summary>
        /// Indicates that the binding should be used only when the target being injected has
        /// an attribute of the specified type.
        /// </summary>
        /// <typeparam name="TAttribute">The type of attribute.</typeparam>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenOrOnActivationSyntax<T> INewBindingWhenOrOnActivationSyntax<T>.WhenTargetHas<TAttribute>()
        {
            return this.WhenTargetHas<TAttribute>();
        }

        /// <summary>
        /// Indicates that the binding should be used only when the target being injected has
        /// an attribute of the specified type.
        /// </summary>
        /// <param name="attributeType">The type of attribute.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenOrOnActivationSyntax<T> INewBindingWhenOrOnActivationSyntax<T>.WhenTargetHas(Type attributeType)
        {
            return this.WhenTargetHas(attributeType);
        }

        /// <summary>
        /// Indicates that the binding should be used only when the service is being requested
        /// by a service bound with the specified name.
        /// </summary>
        /// <param name="name">The name to expect.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenOrOnActivationSyntax<T> INewBindingWhenOrOnActivationSyntax<T>.WhenParentNamed(string name)
        {
            return this.WhenParentNamed(name);
        }

        /// <summary>
        /// Indicates that the binding should be used only when any ancestor is bound with the specified name.
        /// </summary>
        /// <param name="name">The name to expect.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenOrOnActivationSyntax<T> INewBindingWhenOrOnActivationSyntax<T>.WhenAnyAncestorNamed(string name)
        {
            return this.WhenAnyAncestorNamed(name);
        }

        /// <summary>
        /// Indicates that the binding should be used only when no ancestor is bound with the specified name.
        /// </summary>
        /// <param name="name">The name to expect.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenOrOnActivationSyntax<T> INewBindingWhenOrOnActivationSyntax<T>.WhenNoAncestorNamed(string name)
        {
            return this.WhenNoAncestorNamed(name);
        }

        /// <summary>
        /// Indicates that the binding should be used only when any ancestor matches the specified predicate.
        /// </summary>
        /// <param name="predicate">The predicate to match.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenOrOnActivationSyntax<T> INewBindingWhenOrOnActivationSyntax<T>.WhenAnyAncestorMatches(Predicate<IContext> predicate)
        {
            return this.WhenAnyAncestorMatches(predicate);
        }

        /// <summary>
        /// Indicates that the binding should be used only when no ancestor matches the specified predicate.
        /// </summary>
        /// <param name="predicate">The predicate to match.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenOrOnActivationSyntax<T> INewBindingWhenOrOnActivationSyntax<T>.WhenNoAncestorMatches(Predicate<IContext> predicate)
        {
            return this.WhenNoAncestorMatches(predicate);
        }

        #endregion INewBindingWhenOrOnActivationSyntax

        #region INewBindingWhenWithOrOnInitializationSyntax

        /// <summary>
        /// Indicates that the specified callback should be invoked when instances are initialized.
        /// </summary>
        /// <param name="action">The action callback.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenWithOrOnInitializationSyntax<T> INewBindingWhenWithOrOnInitializationSyntax<T>.OnInitialization(Action<T> action)
        {
            return this.OnInitialization(action);
        }

        /// <summary>
        /// Indicates that the specified callback should be invoked when instances are initialized.
        /// </summary>
        /// <typeparam name="TImplementation">The type of the implementation.</typeparam>
        /// <param name="action">The action callback.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenWithOrOnInitializationSyntax<T> INewBindingWhenWithOrOnInitializationSyntax<T>.OnInitialization<TImplementation>(Action<TImplementation> action)
        {
            return this.OnInitialization(action);
        }

        /// <summary>
        /// Indicates that the specified callback should be invoked when instances are initialized.
        /// </summary>
        /// <param name="action">The action callback.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenWithOrOnInitializationSyntax<T> INewBindingWhenWithOrOnInitializationSyntax<T>.OnInitialization(Action<IContext, T> action)
        {
            return this.OnInitialization(action);
        }

        /// <summary>
        /// Indicates that the specified callback should be invoked when instances are initialized.
        /// </summary>
        /// <typeparam name="TImplementation">The type of the implementation.</typeparam>
        /// <param name="action">The action callback.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenWithOrOnInitializationSyntax<T> INewBindingWhenWithOrOnInitializationSyntax<T>.OnInitialization<TImplementation>(Action<IContext, TImplementation> action)
        {
            return this.OnInitialization(action);
        }

        /// <summary>
        /// Indicates that the specified callback should be invoked when instances are initialized.
        /// </summary>
        /// <typeparam name="TImplementation">The type of the implementation.</typeparam>
        /// <param name="action">The action callback.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenWithOrOnInitializationSyntax<T> INewBindingWhenWithOrOnInitializationSyntax<T>.OnInitialization<TImplementation>(Func<IContext, TImplementation, TImplementation> action)
        {
            return this.OnInitialization(action);
        }

        /// <summary>
        /// Indicates that the specified constructor argument should be overridden with the specified value.
        /// </summary>
        /// <param name="name">The name of the argument to override.</param>
        /// <param name="value">The value for the argument.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenWithOrOnInitializationSyntax<T> INewBindingWhenWithOrOnInitializationSyntax<T>.WithConstructorArgument(string name, object value)
        {
            return this.WithConstructorArgument(name, value);
        }

        /// <summary>
        /// Indicates that the specified constructor argument should be overridden with the specified value.
        /// </summary>
        /// <param name="name">The name of the argument to override.</param>
        /// <param name="callback">The callback to invoke to get the value for the argument.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenWithOrOnInitializationSyntax<T> INewBindingWhenWithOrOnInitializationSyntax<T>.WithConstructorArgument(string name, Func<IContext, object> callback)
        {
            return this.WithConstructorArgument(name, callback);
        }

        /// <summary>
        /// Indicates that the specified constructor argument should be overridden with the specified value.
        /// </summary>
        /// <param name="name">The name of the argument to override.</param>
        /// <param name="callback">The callback to invoke to get the value for the argument.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenWithOrOnInitializationSyntax<T> INewBindingWhenWithOrOnInitializationSyntax<T>.WithConstructorArgument(string name, Func<IContext, ITarget<ParameterInfo>, object> callback)
        {
            return this.WithConstructorArgument(name, callback);
        }

        /// <summary>
        /// Indicates that the specified constructor argument should be overridden with the specified value.
        /// </summary>
        /// <typeparam name="TValue">Specifies the argument type to override.</typeparam>
        /// <param name="value">The value for the argument.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenWithOrOnInitializationSyntax<T> INewBindingWhenWithOrOnInitializationSyntax<T>.WithConstructorArgument<TValue>(TValue value)
        {
            return this.WithConstructorArgument(value);
        }

        /// <summary>
        /// Indicates that the specified constructor argument should be overridden with the specified value.
        /// </summary>
        /// <param name="type">The type of the argument to override.</param>
        /// <param name="value">The value for the argument.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenWithOrOnInitializationSyntax<T> INewBindingWhenWithOrOnInitializationSyntax<T>.WithConstructorArgument(Type type, object value)
        {
            return this.WithConstructorArgument(type, (context, target) => value);
        }

        /// <summary>
        /// Indicates that the specified constructor argument should be overridden with the specified value.
        /// </summary>
        /// <typeparam name="TValue">The type of the argument type to override.</typeparam>
        /// <param name="callback">The callback to invoke to get the value for the argument.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenWithOrOnInitializationSyntax<T> INewBindingWhenWithOrOnInitializationSyntax<T>.WithConstructorArgument<TValue>(Func<IContext, TValue> callback)
        {
            return this.WithConstructorArgument(callback);
        }

        /// <summary>
        /// Indicates that the specified constructor argument should be overridden with the specified value.
        /// </summary>
        /// <param name="type">The type of the argument to override.</param>
        /// <param name="callback">The callback to invoke to get the value for the argument.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenWithOrOnInitializationSyntax<T> INewBindingWhenWithOrOnInitializationSyntax<T>.WithConstructorArgument(Type type, Func<IContext, object> callback)
        {
            return this.WithConstructorArgument(type, (context, target) => callback(context));
        }

        /// <summary>
        /// Indicates that the specified constructor argument should be overridden with the specified value.
        /// </summary>
        /// <typeparam name="TValue">The type of the argument type to override.</typeparam>
        /// <param name="callback">The callback to invoke to get the value for the argument.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenWithOrOnInitializationSyntax<T> INewBindingWhenWithOrOnInitializationSyntax<T>.WithConstructorArgument<TValue>(Func<IContext, ITarget<ParameterInfo>, TValue> callback)
        {
            return this.WithConstructorArgument(typeof(TValue), callback);
        }

        /// <summary>
        /// Indicates that the specified constructor argument should be overridden with the specified value.
        /// </summary>
        /// <param name="type">The type of the argument to override.</param>
        /// <param name="callback">The callback to invoke to get the value for the argument.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenWithOrOnInitializationSyntax<T> INewBindingWhenWithOrOnInitializationSyntax<T>.WithConstructorArgument(Type type, Func<IContext, ITarget<ParameterInfo>, object> callback)
        {
            return this.WithConstructorArgument(type, callback);
        }

        /// <summary>
        /// Indicates that the specified property should be injected with the specified value.
        /// </summary>
        /// <param name="name">The name of the property to override.</param>
        /// <param name="value">The value for the property.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenWithOrOnInitializationSyntax<T> INewBindingWhenWithOrOnInitializationSyntax<T>.WithPropertyValue(string name, object value)
        {
            return this.WithPropertyValue(name, value);
        }

        /// <summary>
        /// Indicates that the specified property should be injected with the specified value.
        /// </summary>
        /// <param name="name">The name of the property to override.</param>
        /// <param name="callback">The callback to invoke to get the value for the property.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenWithOrOnInitializationSyntax<T> INewBindingWhenWithOrOnInitializationSyntax<T>.WithPropertyValue(string name, Func<IContext, object> callback)
        {
            return this.WithPropertyValue(name, callback);
        }

        /// <summary>
        /// Indicates that the specified property should be injected with the specified value.
        /// </summary>
        /// <param name="name">The name of the property to override.</param>
        /// <param name="callback">The callback to invoke to get the value for the property.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenWithOrOnInitializationSyntax<T> INewBindingWhenWithOrOnInitializationSyntax<T>.WithPropertyValue(string name, Func<IContext, ITarget<PropertyInfo>, object> callback)
        {
            return this.WithPropertyValue(name, callback);
        }

        /// <summary>
        /// Adds a custom parameter to the binding.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenWithOrOnInitializationSyntax<T> INewBindingWhenWithOrOnInitializationSyntax<T>.WithParameter(IParameter parameter)
        {
            return this.WithParameter(parameter);
        }

        /// <summary>
        /// Sets the value of a piece of metadata on the binding.
        /// </summary>
        /// <param name="key">The metadata key.</param>
        /// <param name="value">The metadata value.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenWithOrOnInitializationSyntax<T> INewBindingWhenWithOrOnInitializationSyntax<T>.WithMetadata(string key, object value)
        {
            return this.WithMetadata(key, value);
        }

        /// <summary>
        /// Indicates that the binding should be used only for requests that support the specified condition.
        /// </summary>
        /// <param name="condition">The condition.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenWithOrOnInitializationSyntax<T> INewBindingWhenWithOrOnInitializationSyntax<T>.When(Func<IRequest, bool> condition)
        {
            return When(condition);
        }

        /// <summary>
        /// Indicates that the binding should be used only for injections on the specified type.
        /// Types that derive from the specified type are considered as valid targets.
        /// </summary>
        /// <typeparam name="TParent">The type.</typeparam>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenWithOrOnInitializationSyntax<T> INewBindingWhenWithOrOnInitializationSyntax<T>.WhenInjectedInto<TParent>()
        {
            return WhenInjectedInto(typeof(TParent));
        }

        /// <summary>
        /// Indicates that the binding should be used only for injections on the specified type.
        /// Types that derive from the specified type are considered as valid targets.
        /// </summary>
        /// <param name="parent">The type.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenWithOrOnInitializationSyntax<T> INewBindingWhenWithOrOnInitializationSyntax<T>.WhenInjectedInto(Type parent)
        {
            return WhenInjectedInto(parent);
        }

        /// <summary>
        /// Indicates that the binding should be used only for injections on the specified type.
        /// Types that derive from the specified type are considered as valid targets.
        /// </summary>
        /// <param name="parents">The type.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenWithOrOnInitializationSyntax<T> INewBindingWhenWithOrOnInitializationSyntax<T>.WhenInjectedInto(params Type[] parents)
        {
            return WhenInjectedInto(parents);
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
        INewBindingWhenWithOrOnInitializationSyntax<T> INewBindingWhenWithOrOnInitializationSyntax<T>.WhenInjectedExactlyInto<TParent>()
        {
            return this.WhenInjectedExactlyInto(typeof(TParent));
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
        INewBindingWhenWithOrOnInitializationSyntax<T> INewBindingWhenWithOrOnInitializationSyntax<T>.WhenInjectedExactlyInto(Type parent)
        {
            return this.WhenInjectedExactlyInto(parent);
        }

        /// <summary>
        /// Indicates that the binding should be used only for injections on the specified type.
        /// The type must match exactly the specified type. Types that derive from the specified type
        /// will not be considered as valid target.
        /// Should match at least one of the specified targets.
        /// </summary>
        /// <param name="parents">The types.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenWithOrOnInitializationSyntax<T> INewBindingWhenWithOrOnInitializationSyntax<T>.WhenInjectedExactlyInto(params Type[] parents)
        {
            return this.WhenInjectedExactlyInto(parents);
        }

        /// <summary>
        /// Indicates that the binding should be used only when the class being injected has
        /// an attribute of the specified type.
        /// </summary>
        /// <typeparam name="TAttribute">The type of attribute.</typeparam>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenWithOrOnInitializationSyntax<T> INewBindingWhenWithOrOnInitializationSyntax<T>.WhenClassHas<TAttribute>()
        {
            return this.WhenClassHas<TAttribute>();
        }

        /// <summary>
        /// Indicates that the binding should be used only when the class being injected has
        /// an attribute of the specified type.
        /// </summary>
        /// <param name="attributeType">The type of attribute.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenWithOrOnInitializationSyntax<T> INewBindingWhenWithOrOnInitializationSyntax<T>.WhenClassHas(Type attributeType)
        {
            return this.WhenClassHas(attributeType);
        }

        /// <summary>
        /// Indicates that the binding should be used only when the member being injected has
        /// an attribute of the specified type.
        /// </summary>
        /// <typeparam name="TAttribute">The type of attribute.</typeparam>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenWithOrOnInitializationSyntax<T> INewBindingWhenWithOrOnInitializationSyntax<T>.WhenMemberHas<TAttribute>()
        {
            return this.WhenMemberHas<TAttribute>();
        }

        /// <summary>
        /// Indicates that the binding should be used only when the member being injected has
        /// an attribute of the specified type.
        /// </summary>
        /// <param name="attributeType">The type of attribute.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenWithOrOnInitializationSyntax<T> INewBindingWhenWithOrOnInitializationSyntax<T>.WhenMemberHas(Type attributeType)
        {
            return this.WhenMemberHas(attributeType);
        }

        /// <summary>
        /// Indicates that the binding should be used only when the target being injected has
        /// an attribute of the specified type.
        /// </summary>
        /// <typeparam name="TAttribute">The type of attribute.</typeparam>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenWithOrOnInitializationSyntax<T> INewBindingWhenWithOrOnInitializationSyntax<T>.WhenTargetHas<TAttribute>()
        {
            return this.WhenTargetHas<TAttribute>();
        }

        /// <summary>
        /// Indicates that the binding should be used only when the target being injected has
        /// an attribute of the specified type.
        /// </summary>
        /// <param name="attributeType">The type of attribute.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenWithOrOnInitializationSyntax<T> INewBindingWhenWithOrOnInitializationSyntax<T>.WhenTargetHas(Type attributeType)
        {
            return this.WhenTargetHas(attributeType);
        }

        /// <summary>
        /// Indicates that the binding should be used only when the service is being requested
        /// by a service bound with the specified name.
        /// </summary>
        /// <param name="name">The name to expect.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenWithOrOnInitializationSyntax<T> INewBindingWhenWithOrOnInitializationSyntax<T>.WhenParentNamed(string name)
        {
            return this.WhenParentNamed(name);
        }

        /// <summary>
        /// Indicates that the binding should be used only when any ancestor is bound with the specified name.
        /// </summary>
        /// <param name="name">The name to expect.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenWithOrOnInitializationSyntax<T> INewBindingWhenWithOrOnInitializationSyntax<T>.WhenAnyAncestorNamed(string name)
        {
            return this.WhenAnyAncestorNamed(name);
        }

        /// <summary>
        /// Indicates that the binding should be used only when no ancestor is bound with the specified name.
        /// </summary>
        /// <param name="name">The name to expect.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenWithOrOnInitializationSyntax<T> INewBindingWhenWithOrOnInitializationSyntax<T>.WhenNoAncestorNamed(string name)
        {
            return this.WhenNoAncestorNamed(name);
        }

        /// <summary>
        /// Indicates that the binding should be used only when any ancestor matches the specified predicate.
        /// </summary>
        /// <param name="predicate">The predicate to match.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenWithOrOnInitializationSyntax<T> INewBindingWhenWithOrOnInitializationSyntax<T>.WhenAnyAncestorMatches(Predicate<IContext> predicate)
        {
            return this.WhenAnyAncestorMatches(predicate);
        }

        /// <summary>
        /// Indicates that the binding should be used only when no ancestor matches the specified predicate.
        /// </summary>
        /// <param name="predicate">The predicate to match.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenWithOrOnInitializationSyntax<T> INewBindingWhenWithOrOnInitializationSyntax<T>.WhenNoAncestorMatches(Predicate<IContext> predicate)
        {
            return this.WhenNoAncestorMatches(predicate);
        }

        #endregion INewBindingWhenWithOrOnInitializationSyntax

        #region INewBindingWhenNamedSyntax

        /// <summary>
        /// Indicates that the binding should be used only for requests that support the specified condition.
        /// </summary>
        /// <param name="condition">The condition.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenNamedSyntax<T> INewBindingWhenNamedSyntax<T>.When(Func<IRequest, bool> condition)
        {
            return When(condition);
        }

        /// <summary>
        /// Indicates that the binding should be used only for injections on the specified type.
        /// Types that derive from the specified type are considered as valid targets.
        /// </summary>
        /// <typeparam name="TParent">The type.</typeparam>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenNamedSyntax<T> INewBindingWhenNamedSyntax<T>.WhenInjectedInto<TParent>()
        {
            return WhenInjectedInto(typeof(TParent));
        }

        /// <summary>
        /// Indicates that the binding should be used only for injections on the specified type.
        /// Types that derive from the specified type are considered as valid targets.
        /// </summary>
        /// <param name="parent">The type.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenNamedSyntax<T> INewBindingWhenNamedSyntax<T>.WhenInjectedInto(Type parent)
        {
            return WhenInjectedInto(parent);
        }

        /// <summary>
        /// Indicates that the binding should be used only for injections on the specified type.
        /// Types that derive from the specified type are considered as valid targets.
        /// </summary>
        /// <param name="parents">The type.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenNamedSyntax<T> INewBindingWhenNamedSyntax<T>.WhenInjectedInto(params Type[] parents)
        {
            return WhenInjectedInto(parents);
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
        INewBindingWhenNamedSyntax<T> INewBindingWhenNamedSyntax<T>.WhenInjectedExactlyInto<TParent>()
        {
            return this.WhenInjectedExactlyInto(typeof(TParent));
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
        INewBindingWhenNamedSyntax<T> INewBindingWhenNamedSyntax<T>.WhenInjectedExactlyInto(Type parent)
        {
            return this.WhenInjectedExactlyInto(parent);
        }

        /// <summary>
        /// Indicates that the binding should be used only for injections on the specified type.
        /// The type must match exactly the specified type. Types that derive from the specified type
        /// will not be considered as valid target.
        /// Should match at least one of the specified targets.
        /// </summary>
        /// <param name="parents">The types.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenNamedSyntax<T> INewBindingWhenNamedSyntax<T>.WhenInjectedExactlyInto(params Type[] parents)
        {
            return this.WhenInjectedExactlyInto(parents);
        }

        /// <summary>
        /// Indicates that the binding should be used only when the class being injected has
        /// an attribute of the specified type.
        /// </summary>
        /// <typeparam name="TAttribute">The type of attribute.</typeparam>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenNamedSyntax<T> INewBindingWhenNamedSyntax<T>.WhenClassHas<TAttribute>()
        {
            return this.WhenClassHas<TAttribute>();
        }

        /// <summary>
        /// Indicates that the binding should be used only when the class being injected has
        /// an attribute of the specified type.
        /// </summary>
        /// <param name="attributeType">The type of attribute.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenNamedSyntax<T> INewBindingWhenNamedSyntax<T>.WhenClassHas(Type attributeType)
        {
            return this.WhenClassHas(attributeType);
        }

        /// <summary>
        /// Indicates that the binding should be used only when the member being injected has
        /// an attribute of the specified type.
        /// </summary>
        /// <typeparam name="TAttribute">The type of attribute.</typeparam>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenNamedSyntax<T> INewBindingWhenNamedSyntax<T>.WhenMemberHas<TAttribute>()
        {
            return this.WhenMemberHas<TAttribute>();
        }

        /// <summary>
        /// Indicates that the binding should be used only when the member being injected has
        /// an attribute of the specified type.
        /// </summary>
        /// <param name="attributeType">The type of attribute.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenNamedSyntax<T> INewBindingWhenNamedSyntax<T>.WhenMemberHas(Type attributeType)
        {
            return this.WhenMemberHas(attributeType);
        }

        /// <summary>
        /// Indicates that the binding should be used only when the target being injected has
        /// an attribute of the specified type.
        /// </summary>
        /// <typeparam name="TAttribute">The type of attribute.</typeparam>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenNamedSyntax<T> INewBindingWhenNamedSyntax<T>.WhenTargetHas<TAttribute>()
        {
            return this.WhenTargetHas<TAttribute>();
        }

        /// <summary>
        /// Indicates that the binding should be used only when the target being injected has
        /// an attribute of the specified type.
        /// </summary>
        /// <param name="attributeType">The type of attribute.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenNamedSyntax<T> INewBindingWhenNamedSyntax<T>.WhenTargetHas(Type attributeType)
        {
            return this.WhenTargetHas(attributeType);
        }

        /// <summary>
        /// Indicates that the binding should be used only when the service is being requested
        /// by a service bound with the specified name.
        /// </summary>
        /// <param name="name">The name to expect.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenNamedSyntax<T> INewBindingWhenNamedSyntax<T>.WhenParentNamed(string name)
        {
            return this.WhenParentNamed(name);
        }

        /// <summary>
        /// Indicates that the binding should be used only when any ancestor is bound with the specified name.
        /// </summary>
        /// <param name="name">The name to expect.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenNamedSyntax<T> INewBindingWhenNamedSyntax<T>.WhenAnyAncestorNamed(string name)
        {
            return this.WhenAnyAncestorNamed(name);
        }

        /// <summary>
        /// Indicates that the binding should be used only when no ancestor is bound with the specified name.
        /// </summary>
        /// <param name="name">The name to expect.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenNamedSyntax<T> INewBindingWhenNamedSyntax<T>.WhenNoAncestorNamed(string name)
        {
            return this.WhenNoAncestorNamed(name);
        }

        /// <summary>
        /// Indicates that the binding should be used only when any ancestor matches the specified predicate.
        /// </summary>
        /// <param name="predicate">The predicate to match.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenNamedSyntax<T> INewBindingWhenNamedSyntax<T>.WhenAnyAncestorMatches(Predicate<IContext> predicate)
        {
            return this.WhenAnyAncestorMatches(predicate);
        }

        /// <summary>
        /// Indicates that the binding should be used only when no ancestor matches the specified predicate.
        /// </summary>
        /// <param name="predicate">The predicate to match.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenNamedSyntax<T> INewBindingWhenNamedSyntax<T>.WhenNoAncestorMatches(Predicate<IContext> predicate)
        {
            return this.WhenNoAncestorMatches(predicate);
        }

        /// <summary>
        /// Indicates that the binding should be registered with the specified name. Names are not
        /// necessarily unique; multiple bindings for a given service may be registered with the same name.
        /// </summary>
        /// <param name="name">The name to give the binding.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenSyntax<T> INewBindingWhenNamedSyntax<T>.Named(string name)
        {
            return this.Named(name);
        }

        #endregion INewBindingWhenNamedSyntax

        #region INewBindingWhenSyntax

        /// <summary>
        /// Indicates that the binding should be used only for requests that support the specified condition.
        /// </summary>
        /// <param name="condition">The condition.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenSyntax<T> INewBindingWhenSyntax<T>.When(Func<IRequest, bool> condition)
        {
            return When(condition);
        }

        /// <summary>
        /// Indicates that the binding should be used only for injections on the specified type.
        /// Types that derive from the specified type are considered as valid targets.
        /// </summary>
        /// <typeparam name="TParent">The type.</typeparam>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenSyntax<T> INewBindingWhenSyntax<T>.WhenInjectedInto<TParent>()
        {
            return WhenInjectedInto(typeof(TParent));
        }

        /// <summary>
        /// Indicates that the binding should be used only for injections on the specified type.
        /// Types that derive from the specified type are considered as valid targets.
        /// </summary>
        /// <param name="parent">The type.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenSyntax<T> INewBindingWhenSyntax<T>.WhenInjectedInto(Type parent)
        {
            return WhenInjectedInto(parent);
        }

        /// <summary>
        /// Indicates that the binding should be used only for injections on the specified type.
        /// Types that derive from the specified type are considered as valid targets.
        /// </summary>
        /// <param name="parents">The type.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenSyntax<T> INewBindingWhenSyntax<T>.WhenInjectedInto(params Type[] parents)
        {
            return WhenInjectedInto(parents);
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
        INewBindingWhenSyntax<T> INewBindingWhenSyntax<T>.WhenInjectedExactlyInto<TParent>()
        {
            return this.WhenInjectedExactlyInto(typeof(TParent));
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
        INewBindingWhenSyntax<T> INewBindingWhenSyntax<T>.WhenInjectedExactlyInto(Type parent)
        {
            return this.WhenInjectedExactlyInto(parent);
        }

        /// <summary>
        /// Indicates that the binding should be used only for injections on the specified type.
        /// The type must match exactly the specified type. Types that derive from the specified type
        /// will not be considered as valid target.
        /// Should match at least one of the specified targets.
        /// </summary>
        /// <param name="parents">The types.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenSyntax<T> INewBindingWhenSyntax<T>.WhenInjectedExactlyInto(params Type[] parents)
        {
            return this.WhenInjectedExactlyInto(parents);
        }

        /// <summary>
        /// Indicates that the binding should be used only when the class being injected has
        /// an attribute of the specified type.
        /// </summary>
        /// <typeparam name="TAttribute">The type of attribute.</typeparam>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenSyntax<T> INewBindingWhenSyntax<T>.WhenClassHas<TAttribute>()
        {
            return this.WhenClassHas<TAttribute>();
        }

        /// <summary>
        /// Indicates that the binding should be used only when the class being injected has
        /// an attribute of the specified type.
        /// </summary>
        /// <param name="attributeType">The type of attribute.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenSyntax<T> INewBindingWhenSyntax<T>.WhenClassHas(Type attributeType)
        {
            return this.WhenClassHas(attributeType);
        }

        /// <summary>
        /// Indicates that the binding should be used only when the member being injected has
        /// an attribute of the specified type.
        /// </summary>
        /// <typeparam name="TAttribute">The type of attribute.</typeparam>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenSyntax<T> INewBindingWhenSyntax<T>.WhenMemberHas<TAttribute>()
        {
            return this.WhenMemberHas<TAttribute>();
        }

        /// <summary>
        /// Indicates that the binding should be used only when the member being injected has
        /// an attribute of the specified type.
        /// </summary>
        /// <param name="attributeType">The type of attribute.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenSyntax<T> INewBindingWhenSyntax<T>.WhenMemberHas(Type attributeType)
        {
            return this.WhenMemberHas(attributeType);
        }

        /// <summary>
        /// Indicates that the binding should be used only when the target being injected has
        /// an attribute of the specified type.
        /// </summary>
        /// <typeparam name="TAttribute">The type of attribute.</typeparam>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenSyntax<T> INewBindingWhenSyntax<T>.WhenTargetHas<TAttribute>()
        {
            return this.WhenTargetHas<TAttribute>();
        }

        /// <summary>
        /// Indicates that the binding should be used only when the target being injected has
        /// an attribute of the specified type.
        /// </summary>
        /// <param name="attributeType">The type of attribute.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenSyntax<T> INewBindingWhenSyntax<T>.WhenTargetHas(Type attributeType)
        {
            return this.WhenTargetHas(attributeType);
        }

        /// <summary>
        /// Indicates that the binding should be used only when the service is being requested
        /// by a service bound with the specified name.
        /// </summary>
        /// <param name="name">The name to expect.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenSyntax<T> INewBindingWhenSyntax<T>.WhenParentNamed(string name)
        {
            return this.WhenParentNamed(name);
        }

        /// <summary>
        /// Indicates that the binding should be used only when any ancestor is bound with the specified name.
        /// </summary>
        /// <param name="name">The name to expect.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenSyntax<T> INewBindingWhenSyntax<T>.WhenAnyAncestorNamed(string name)
        {
            return this.WhenAnyAncestorNamed(name);
        }

        /// <summary>
        /// Indicates that the binding should be used only when no ancestor is bound with the specified name.
        /// </summary>
        /// <param name="name">The name to expect.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenSyntax<T> INewBindingWhenSyntax<T>.WhenNoAncestorNamed(string name)
        {
            return this.WhenNoAncestorNamed(name);
        }

        /// <summary>
        /// Indicates that the binding should be used only when any ancestor matches the specified predicate.
        /// </summary>
        /// <param name="predicate">The predicate to match.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenSyntax<T> INewBindingWhenSyntax<T>.WhenAnyAncestorMatches(Predicate<IContext> predicate)
        {
            return this.WhenAnyAncestorMatches(predicate);
        }

        /// <summary>
        /// Indicates that the binding should be used only when no ancestor matches the specified predicate.
        /// </summary>
        /// <param name="predicate">The predicate to match.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingWhenSyntax<T> INewBindingWhenSyntax<T>.WhenNoAncestorMatches(Predicate<IContext> predicate)
        {
            return this.WhenNoAncestorMatches(predicate);
        }

        #endregion INewBindingWhenSyntax

        /// <summary>
        /// Indicates that the binding should be used only for requests that support the specified condition.
        /// </summary>
        /// <param name="condition">The condition.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        private BindingConfigurationBuilder<T>  When(Func<IRequest, bool> condition)
        {
            this.condition = condition;
            return this;
        }

        /// <summary>
        /// Indicates that the binding should be used only for injections on the specified type.
        /// Types that derive from the specified type are considered as valid targets.
        /// </summary>
        /// <param name="parent">The type.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        private BindingConfigurationBuilder<T> WhenInjectedInto(Type parent)
        {
            if (parent.IsGenericTypeDefinition)
            {
                if (parent.IsInterface)
                {
                    this.condition = r =>
                        r.Target != null &&
                        r.Target.Member.ReflectedType.GetInterfaces().Any(i =>
                            i.IsGenericType &&
                            i.GetGenericTypeDefinition() == parent);
                }
                else
                {
                    this.condition = r =>
                        r.Target != null &&
                        r.Target.Member.ReflectedType.GetAllBaseTypes().Any(i =>
                            i.IsGenericType &&
                            i.GetGenericTypeDefinition() == parent);
                }
            }
            else
            {
                this.condition = r => r.Target != null && parent.IsAssignableFrom(r.Target.Member.ReflectedType);
            }

            return this;
        }

        /// <summary>
        /// Indicates that the binding should be used only for injections on the specified type.
        /// Types that derive from the specified type are considered as valid targets.
        /// </summary>
        /// <param name="parents">The type.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        private BindingConfigurationBuilder<T> WhenInjectedInto(params Type[] parents)
        {
            this.condition = r =>
                {
                    foreach (var parent in parents)
                    {
                        bool matches = false;
                        if (parent.IsGenericTypeDefinition)
                        {
                            if (parent.IsInterface)
                            {
                                matches =
                                    r.Target != null &&
                                    r.Target.Member.ReflectedType.GetInterfaces().Any(i =>
                                        i.IsGenericType &&
                                        i.GetGenericTypeDefinition() == parent);
                            }
                            else
                            {
                                matches =
                                    r.Target != null &&
                                    r.Target.Member.ReflectedType.GetAllBaseTypes().Any(i =>
                                        i.IsGenericType &&
                                        i.GetGenericTypeDefinition() == parent);
                            }
                        }
                        else
                        {
                            matches = r.Target != null && parent.IsAssignableFrom(r.Target.Member.ReflectedType);
                        }

                        if (matches)
                        {
                            return true;
                        }
                    }

                    return false;
                };

            return this;
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
        private BindingConfigurationBuilder<T> WhenInjectedExactlyInto(Type parent)
        {
            if (parent.IsGenericTypeDefinition)
            {
                this.condition = r => r.Target != null &&
                                      r.Target.Member.ReflectedType.IsGenericType &&
                                      parent == r.Target.Member.ReflectedType.GetGenericTypeDefinition();
            }
            else
            {
                this.condition = r => r.Target != null && r.Target.Member.ReflectedType == parent;
            }

            return this;
        }

        /// <summary>
        /// Indicates that the binding should be used only for injections on the specified type.
        /// The type must match exactly the specified type. Types that derive from the specified type
        /// will not be considered as valid target.
        /// Should match at least one of the specified targets.
        /// </summary>
        /// <param name="parents">The types.</param>
        /// <returns>The fluent syntax.</returns>
        private BindingConfigurationBuilder<T> WhenInjectedExactlyInto(params Type[] parents)
        {
            this.condition = r =>
            {
                foreach (var parent in parents)
                {
                    bool matches = false;
                    if (parent.IsGenericTypeDefinition)
                    {
                        matches = r.Target != null &&
                                  r.Target.Member.ReflectedType.IsGenericType &&
                                  parent == r.Target.Member.ReflectedType.GetGenericTypeDefinition();
                    }
                    else
                    {
                        matches = r.Target != null && r.Target.Member.ReflectedType == parent;
                    }

                    if (matches)
                    {
                        return true;
                    }
                }

                return false;
            };

            return this;
        }

        /// <summary>
        /// Adds a custom parameter to the binding.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        internal BindingConfigurationBuilder<T> WithParameter(IParameter parameter)
        {
            this.parameters.Add(parameter);
            return this;
        }

        /// <summary>
        /// Indicates that instances activated via the binding should be re-used as long as the object
        /// returned by the provided callback remains alive (that is, has not been garbage collected).
        /// </summary>
        /// <param name="scope">The callback that returns the scope.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        private BindingConfigurationBuilder<T> InScope(Func<IContext, object> scope)
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
        private BindingConfigurationBuilder<T> InSingletonScope()
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
        private BindingConfigurationBuilder<T> InThreadScope()
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
        private BindingConfigurationBuilder<T> InTransientScope()
        {
            this.scopeCallback = StandardScopeCallbacks.Transient;
            return this;
        }

        /// <summary>
        /// Indicates that the specified callback should be invoked when instances are initialized.
        /// </summary>
        /// <param name="action">The action callback.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        private BindingConfigurationBuilder<T> OnInitialization(Action<T> action)
        {
            return this.OnInitialization<T>(action);
        }

        /// <summary>
        /// Indicates that the specified callback should be invoked when instances are initialized.
        /// </summary>
        /// <typeparam name="TImplementation">The type of the implementation.</typeparam>
        /// <param name="action">The action callback.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        private BindingConfigurationBuilder<T> OnInitialization<TImplementation>(Action<TImplementation> action)
        {
            this.initializationActions.Add((instance, context) =>
                {
                    action((TImplementation)instance);
                    return instance;
                });
            return this;
        }

        /// <summary>
        /// Indicates that the specified callback should be invoked when instances are initialized.
        /// </summary>
        /// <param name="action">The action callback.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        private BindingConfigurationBuilder<T> OnInitialization(Action<IContext, T> action)
        {
            return this.OnInitialization<T>(action);
        }

        /// <summary>
        /// Indicates that the specified callback should be invoked when instances are initialized.
        /// </summary>
        /// <typeparam name="TImplementation">The type of the implementation.</typeparam>
        /// <param name="action">The action callback.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        private BindingConfigurationBuilder<T> OnInitialization<TImplementation>(Action<IContext, TImplementation> action)
        {
            this.initializationActions.Add((context, instance) =>
            {
                action(context, (TImplementation)instance);
                return instance;
            });
            return this;
        }

        /// <summary>
        /// Indicates that the specified callback should be invoked when instances are initialized.
        /// </summary>
        /// <typeparam name="TImplementation">The type of the implementation.</typeparam>
        /// <param name="action">The action callback.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        private BindingConfigurationBuilder<T> OnInitialization<TImplementation>(Func<IContext, TImplementation, TImplementation> action)
        {
            this.initializationActions.Add((context, instance) => action(context, (TImplementation)instance));
            return this;
        }

        /// <summary>
        /// Indicates that the specified callback should be invoked when instances are activated.
        /// </summary>
        /// <param name="action">The action callback.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        private BindingConfigurationBuilder<T> OnActivation(Action<T> action)
        {
            this.activationActions.Add((context, instance) => action((T)instance));
            return this;
        }

        /// <summary>
        /// Indicates that the specified callback should be invoked when instances are activated.
        /// </summary>
        /// <typeparam name="TImplementation">The type of the implementation.</typeparam>
        /// <param name="action">The action callback.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        private BindingConfigurationBuilder<T> OnActivation<TImplementation>(Action<TImplementation> action)
        {
            this.activationActions.Add((context, instance) => action((TImplementation)instance));
            return this;
        }

        /// <summary>
        /// Indicates that the specified callback should be invoked when instances are activated.
        /// </summary>
        /// <param name="action">The action callback.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        private BindingConfigurationBuilder<T> OnActivation(Action<IContext, T> action)
        {
            this.activationActions.Add((context, instance) => action(context, (T)instance));
            return this;
        }

        /// <summary>
        /// Indicates that the specified callback should be invoked when instances are activated.
        /// </summary>
        /// <typeparam name="TImplementation">The type of the implementation.</typeparam>
        /// <param name="action">The action callback.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        private BindingConfigurationBuilder<T> OnActivation<TImplementation>(Action<IContext, TImplementation> action)
        {
            this.activationActions.Add((context, instance) => action(context, (TImplementation)instance));
            return this;
        }

        /// <summary>
        /// Indicates that the specified callback should be invoked when instances are deactivated.
        /// </summary>
        /// <param name="action">The action callback.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        private BindingConfigurationBuilder<T> OnDeactivation(Action<T> action)
        {
            this.deactivationActions.Add((context, instance) => action((T)instance));
            return this;
        }

        /// <summary>
        /// Indicates that the specified callback should be invoked when instances are deactivated.
        /// </summary>
        /// <param name="action">The action callback.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        private BindingConfigurationBuilder<T> OnDeactivation<TImplementation>(Action<TImplementation> action)
        {
            this.deactivationActions.Add((context, instance) => action((TImplementation)instance));
            return this;
        }

        /// <summary>
        /// Indicates that the specified callback should be invoked when instances are deactivated.
        /// </summary>
        /// <param name="action">The action callback.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        private BindingConfigurationBuilder<T> OnDeactivation(Action<IContext, T> action)
        {
            this.deactivationActions.Add((context, instance) => action(context, (T)instance));
            return this;
        }

        /// <summary>
        /// Indicates that the specified callback should be invoked when instances are deactivated.
        /// </summary>
        /// <param name="action">The action callback.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        private BindingConfigurationBuilder<T> OnDeactivation<TImplementation>(Action<IContext, TImplementation> action)
        {
            this.deactivationActions.Add((context, instance) => action(context, (TImplementation)instance));
            return this;
        }

        /// <summary>
        /// Indicates that the specified constructor argument should be overridden with the specified value.
        /// </summary>
        /// <param name="name">The name of the argument to override.</param>
        /// <param name="value">The value for the argument.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        private BindingConfigurationBuilder<T> WithConstructorArgument(string name, object value)
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
        private BindingConfigurationBuilder<T> WithConstructorArgument(string name, Func<IContext, object> callback)
        {
            this.parameters.Add(new ConstructorArgument(name, callback));
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
        private BindingConfigurationBuilder<T> WithConstructorArgument(string name, Func<IContext, ITarget<ParameterInfo>, object> callback)
        {
            this.parameters.Add(new ConstructorArgument(name, callback));
            return this;
        }

        /// <summary>
        /// Indicates that the specified constructor argument should be overridden with the specified value.
        /// </summary>
        /// <typeparam name="TValue">Specifies the argument type to override.</typeparam>
        /// <param name="value">The value for the argument.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        private BindingConfigurationBuilder<T> WithConstructorArgument<TValue>(TValue value)
        {
            this.parameters.Add(new TypeMatchingConstructorArgument(typeof(TValue), (context, target) => value));
            return this;
        }

        /// <summary>
        /// Indicates that the specified constructor argument should be overridden with the specified value.
        /// </summary>
        /// <param name="type">The type of the argument to override.</param>
        /// <param name="callback">The callback to invoke to get the value for the argument.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        private BindingConfigurationBuilder<T> WithConstructorArgument(Type type, Func<IContext, ITarget<ParameterInfo>, object> callback)
        {
            this.parameters.Add(new TypeMatchingConstructorArgument(type, callback));
            return this;
        }

        /// <summary>
        /// Indicates that the specified constructor argument should be overridden with the specified value.
        /// </summary>
        /// <param name="type">The type of the argument to override.</param>
        /// <param name="value">The value for the argument.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        private BindingConfigurationBuilder<T> WithConstructorArgument(Type type, object value)
        {
            return this.WithConstructorArgument(type, (context, target) => value);
        }

        /// <summary>
        /// Indicates that the specified constructor argument should be overridden with the specified value.
        /// </summary>
        /// <typeparam name="TValue">The type of the argument type to override.</typeparam>
        /// <param name="callback">The callback to invoke to get the value for the argument.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        private BindingConfigurationBuilder<T> WithConstructorArgument<TValue>(Func<IContext, TValue> callback)
        {
            return this.WithConstructorArgument(typeof(TValue), (context, target) => callback(context));
        }

        /// <summary>
        /// Indicates that the specified property should be injected with the specified value.
        /// </summary>
        /// <param name="name">The name of the property to override.</param>
        /// <param name="value">The value for the property.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        private BindingConfigurationBuilder<T> WithPropertyValue(string name, object value)
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
        private BindingConfigurationBuilder<T> WithPropertyValue(string name, Func<IContext, object> callback)
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
        private BindingConfigurationBuilder<T> WithPropertyValue(string name, Func<IContext, ITarget<PropertyInfo>, object> callback)
        {
            this.parameters.Add(new PropertyValue(name, callback));
            return this;
        }

        /// <summary>
        /// Sets the value of a piece of metadata on the binding.
        /// </summary>
        /// <param name="key">The metadata key.</param>
        /// <param name="value">The metadata value.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        private BindingConfigurationBuilder<T> WithMetadata(string key, object value)
        {
            if (this.metadata == null)
            {
                this.metadata = new BindingMetadata();
            }

            this.metadata.Set(key, value);
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
        private BindingConfigurationBuilder<T> Named(string name)
        {
            string.Intern(name);

            if (this.metadata == null)
            {
                this.metadata = new BindingMetadata();
            }

            this.metadata.Name = name;
            return this;
        }

        /// <summary>
        /// Indicates that the binding should be used only when the class being injected has
        /// an attribute of the specified type.
        /// </summary>
        /// <typeparam name="TAttribute">The type of attribute.</typeparam>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        private BindingConfigurationBuilder<T> WhenClassHas<TAttribute>()
            where TAttribute : Attribute
        {
            this.condition = r => r.Target != null && r.Target.Member.ReflectedType.HasAttribute(typeof(TAttribute));
            return this;
        }

        /// <summary>
        /// Indicates that the binding should be used only when the class being injected has
        /// an attribute of the specified type.
        /// </summary>
        /// <param name="attributeType">The type of attribute.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        private BindingConfigurationBuilder<T> WhenClassHas(Type attributeType)
        {
            if (!typeof(Attribute).IsAssignableFrom(attributeType))
            {
                throw new InvalidOperationException(ExceptionFormatter.InvalidAttributeTypeUsedInBindingCondition(this.bindingBuilder.ServiceNames, nameof(WhenClassHas), attributeType));
            }

            this.condition = r => r.Target != null && r.Target.Member.ReflectedType.HasAttribute(attributeType);
            return this;
        }

        /// <summary>
        /// Indicates that the binding should be used only when the member being injected has
        /// an attribute of the specified type.
        /// </summary>
        /// <typeparam name="TAttribute">The type of attribute.</typeparam>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        private BindingConfigurationBuilder<T> WhenMemberHas<TAttribute>()
        {
            this.condition = r => r.Target != null && r.Target.Member.HasAttribute(typeof(TAttribute));
            return this;
        }

        /// <summary>
        /// Indicates that the binding should be used only when the member being injected has
        /// an attribute of the specified type.
        /// </summary>
        /// <param name="attributeType">The type of attribute.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        private BindingConfigurationBuilder<T> WhenMemberHas(Type attributeType)
        {
            if (!typeof(Attribute).IsAssignableFrom(attributeType))
            {
                throw new InvalidOperationException(ExceptionFormatter.InvalidAttributeTypeUsedInBindingCondition(this.bindingBuilder.ServiceNames, nameof(WhenMemberHas), attributeType));
            }

            this.condition = r => r.Target != null && r.Target.Member.HasAttribute(attributeType);
            return this;
        }

        /// <summary>
        /// Indicates that the binding should be used only when the target being injected has
        /// an attribute of the specified type.
        /// </summary>
        /// <typeparam name="TAttribute">The type of attribute.</typeparam>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        private BindingConfigurationBuilder<T> WhenTargetHas<TAttribute>()
        {
            this.condition = r => r.Target != null && r.Target.HasAttribute(typeof(TAttribute));
            return this;
        }

        /// <summary>
        /// Indicates that the binding should be used only when the target being injected has
        /// an attribute of the specified type.
        /// </summary>
        /// <param name="attributeType">The type of attribute.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        private BindingConfigurationBuilder<T> WhenTargetHas(Type attributeType)
        {
            if (!typeof(Attribute).IsAssignableFrom(attributeType))
            {
                throw new InvalidOperationException(ExceptionFormatter.InvalidAttributeTypeUsedInBindingCondition(this.bindingBuilder.ServiceNames, nameof(WhenTargetHas), attributeType));
            }

            this.condition = r => r.Target != null && r.Target.HasAttribute(attributeType);
            return this;
        }

        /// <summary>
        /// Indicates that the binding should be used only when the service is being requested
        /// by a service bound with the specified name.
        /// </summary>
        /// <param name="name">The name to expect.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        private BindingConfigurationBuilder<T> WhenParentNamed(string name)
        {
            string.Intern(name);
            this.condition = r => r.ParentContext != null && string.Equals(r.ParentContext.Binding.Metadata.Name, name, StringComparison.Ordinal);
            return this;
        }

        /// <summary>
        /// Indicates that the binding should be used only when any ancestor is bound with the specified name.
        /// </summary>
        /// <param name="name">The name to expect.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        private BindingConfigurationBuilder<T> WhenAnyAncestorNamed(string name)
        {
            return this.WhenAnyAncestorMatches(ctx => ctx.Binding.Metadata.Name == name);
        }

        /// <summary>
        /// Indicates that the binding should be used only when any ancestor matches the specified predicate.
        /// </summary>
        /// <param name="predicate">The predicate to match.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        private BindingConfigurationBuilder<T> WhenAnyAncestorMatches(Predicate<IContext> predicate)
        {
            this.condition = r => DoesAnyAncestorMatch(r, predicate);
            return this;
        }

        /// <summary>
        /// Indicates that the binding should be used only when no ancestor is bound with the specified name.
        /// </summary>
        /// <param name="name">The name to expect.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        private BindingConfigurationBuilder<T> WhenNoAncestorNamed(string name)
        {
            return this.WhenNoAncestorMatches(ctx => ctx.Binding.Metadata.Name == name);
        }

        /// <summary>
        /// Indicates that the binding should be used only when no ancestor matches the specified predicate.
        /// </summary>
        /// <param name="predicate">The predicate to match.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        private BindingConfigurationBuilder<T> WhenNoAncestorMatches(Predicate<IContext> predicate)
        {
            this.condition = r => !DoesAnyAncestorMatch(r, predicate);
            return this;
        }

        private static bool DoesAnyAncestorMatch(IRequest request, Predicate<IContext> predicate)
        {
            var parentContext = request.ParentContext;
            if (parentContext == null)
            {
                return false;
            }

            return predicate(parentContext) || DoesAnyAncestorMatch(parentContext.Request, predicate);
        }
    }
}