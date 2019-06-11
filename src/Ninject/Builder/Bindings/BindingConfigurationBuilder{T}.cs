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
    using Ninject.Builder.Syntax;
    using Ninject.Infrastructure;
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
                                                           INewBindingWhenWithOrOnInitializationSyntax<T>,
                                                           IComponentBindingInScopeSyntax<T>,
                                                           IComponentBindingWithOrOnActivationSyntax<T>,
                                                           IComponentBindingWithSyntax<T>
    {
        private readonly IProviderFactory providerBuilder;
        private readonly BindingTarget target;
        private readonly List<IParameter> parameters;
        private readonly List<Func<IContext, object, object>> initializationActions;
        private readonly List<Action<IContext, object>> activationActions;
        private readonly List<Action<IContext, object>> deactivationActions;
        private Func<IContext, object> scopeCallback;

        /// <summary>
        /// Initializes a new instance of the <see cref="BindingConfigurationBuilder{T}"/> class.
        /// </summary>
        /// <param name="providerFactory">A factory for creating an <see cref="IProvider"/>.</param>
        /// <param name="target">The type of target for the binding.</param>
        internal BindingConfigurationBuilder(IProviderFactory providerFactory, BindingTarget target)
        {
            this.providerBuilder = providerFactory;
            this.target = target;
            this.parameters = new List<IParameter>();
            this.initializationActions = new List<Func<IContext, object, object>>();
            this.activationActions = new List<Action<IContext, object>>();
            this.deactivationActions = new List<Action<IContext, object>>();
        }

        public override bool HasActivationActions
        {
            get { return this.activationActions.Count > 0; }
        }

        public override bool HasDeactivationActions
        {
            get { return this.deactivationActions.Count > 0; }
        }

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
            return new BindingConfiguration(this.parameters)
                {
                    Provider = this.providerBuilder.Create(root, this.parameters),
                    ScopeCallback = this.scopeCallback ?? StandardScopeCallbacks.Transient,
                    Target = this.target,
                };
        }

        #region IComponentBindingWithOrOnActivationSyntax

        /// <summary>
        /// Indicates that the specified callback should be invoked when instances are activated.
        /// </summary>
        /// <param name="action">The action callback.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        IComponentBindingWithOrOnActivationSyntax<T> IComponentBindingWithOrOnActivationSyntax<T>.OnActivation(Action<T> action)
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
        IComponentBindingWithOrOnActivationSyntax<T> IComponentBindingWithOrOnActivationSyntax<T>.OnActivation<TImplementation>(Action<TImplementation> action)
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
        IComponentBindingWithOrOnActivationSyntax<T> IComponentBindingWithOrOnActivationSyntax<T>.OnActivation(Action<IContext, T> action)
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
        IComponentBindingWithOrOnActivationSyntax<T> IComponentBindingWithOrOnActivationSyntax<T>.OnActivation<TImplementation>(Action<IContext, TImplementation> action)
        {
            return this.OnActivation(action);
        }

        #endregion IComponentBindingWithOrOnActivationSyntax

        #region IComponentBindingInScopeSyntax

        IComponentBindingWithOrOnActivationSyntax<T> IComponentBindingInScopeSyntax<T>.InSingletonScope()
        {
            return InSingletonScope();
        }

        IComponentBindingWithSyntax<T> IComponentBindingInScopeSyntax<T>.InTransientScope()
        {
            return InTransientScope();
        }

        #endregion IComponentBindingInScopeSyntax

        #region IComponentBindingWithSyntax

        IComponentBindingWithSyntax<T> IComponentBindingWithSyntax<T>.WithConstructorArgument(string name, object value)
        {
            return this.WithConstructorArgument(name, value);
        }

        IComponentBindingWithSyntax<T> IComponentBindingWithSyntax<T>.WithConstructorArgument(string name, Func<IContext, object> callback)
        {
            throw new NotImplementedException();
        }

        IComponentBindingWithSyntax<T> IComponentBindingWithSyntax<T>.WithConstructorArgument(string name, Func<IContext, ITarget, object> callback)
        {
            throw new NotImplementedException();
        }

        IComponentBindingWithSyntax<T> IComponentBindingWithSyntax<T>.WithConstructorArgument<TValue>(TValue value)
        {
            throw new NotImplementedException();
        }

        IComponentBindingWithSyntax<T> IComponentBindingWithSyntax<T>.WithConstructorArgument(Type type, object value)
        {
            throw new NotImplementedException();
        }

        IComponentBindingWithSyntax<T> IComponentBindingWithSyntax<T>.WithConstructorArgument<TValue>(Func<IContext, TValue> callback)
        {
            throw new NotImplementedException();
        }

        IComponentBindingWithSyntax<T> IComponentBindingWithSyntax<T>.WithConstructorArgument(Type type, Func<IContext, object> callback)
        {
            throw new NotImplementedException();
        }

        IComponentBindingWithSyntax<T> IComponentBindingWithSyntax<T>.WithConstructorArgument<TValue>(Func<IContext, ITarget, TValue> callback)
        {
            throw new NotImplementedException();
        }

        IComponentBindingWithSyntax<T> IComponentBindingWithSyntax<T>.WithConstructorArgument(Type type, Func<IContext, ITarget, object> callback)
        {
            throw new NotImplementedException();
        }

        IComponentBindingWithSyntax<T> IComponentBindingWithSyntax<T>.WithPropertyValue(string name, object value)
        {
            return this.WithPropertyValue(name, value);
        }

        IComponentBindingWithSyntax<T> IComponentBindingWithSyntax<T>.WithPropertyValue(string name, Func<IContext, object> callback)
        {
            return this.WithPropertyValue(name, callback);
        }

        IComponentBindingWithSyntax<T> IComponentBindingWithSyntax<T>.WithPropertyValue(string name, Func<IContext, ITarget, object> callback)
        {
            return this.WithPropertyValue(name, callback);
        }

        #endregion IComponentBindingWithSyntax

        #region IComponentBindingWithOrOnActivationSyntax

        IComponentBindingWithOrOnActivationSyntax<T> IComponentBindingWithOrOnActivationSyntax<T>.WithConstructorArgument(string name, object value)
        {
            return this.WithConstructorArgument(name, value);
        }

        IComponentBindingWithOrOnActivationSyntax<T> IComponentBindingWithOrOnActivationSyntax<T>.WithConstructorArgument(string name, Func<IContext, object> callback)
        {
            throw new NotImplementedException();
        }

        IComponentBindingWithOrOnActivationSyntax<T> IComponentBindingWithOrOnActivationSyntax<T>.WithConstructorArgument(string name, Func<IContext, ITarget<IConstructorArgument>, object> callback)
        {
            throw new NotImplementedException();
        }

        IComponentBindingWithOrOnActivationSyntax<T> IComponentBindingWithOrOnActivationSyntax<T>.WithConstructorArgument<TValue>(TValue value)
        {
            throw new NotImplementedException();
        }

        IComponentBindingWithOrOnActivationSyntax<T> IComponentBindingWithOrOnActivationSyntax<T>.WithConstructorArgument(Type type, object value)
        {
            throw new NotImplementedException();
        }

        IComponentBindingWithOrOnActivationSyntax<T> IComponentBindingWithOrOnActivationSyntax<T>.WithConstructorArgument<TValue>(Func<IContext, TValue> callback)
        {
            throw new NotImplementedException();
        }

        IComponentBindingWithOrOnActivationSyntax<T> IComponentBindingWithOrOnActivationSyntax<T>.WithConstructorArgument(Type type, Func<IContext, object> callback)
        {
            throw new NotImplementedException();
        }

        IComponentBindingWithOrOnActivationSyntax<T> IComponentBindingWithOrOnActivationSyntax<T>.WithConstructorArgument<TValue>(Func<IContext, ITarget<IConstructorArgument>, TValue> callback)
        {
            throw new NotImplementedException();
        }

        IComponentBindingWithOrOnActivationSyntax<T> IComponentBindingWithOrOnActivationSyntax<T>.WithConstructorArgument(Type type, Func<IContext, ITarget<IConstructorArgument>, object> callback)
        {
            throw new NotImplementedException();
        }

        IComponentBindingWithOrOnActivationSyntax<T> IComponentBindingWithOrOnActivationSyntax<T>.WithPropertyValue(string name, object value)
        {
            return this.WithPropertyValue(name, value);
        }

        IComponentBindingWithOrOnActivationSyntax<T> IComponentBindingWithOrOnActivationSyntax<T>.WithPropertyValue(string name, Func<IContext, object> callback)
        {
            return this.WithPropertyValue(name, callback);
        }

        IComponentBindingWithOrOnActivationSyntax<T> IComponentBindingWithOrOnActivationSyntax<T>.WithPropertyValue(string name, Func<IContext, ITarget<IPropertyValue>, object> callback)
        {
            return this.WithPropertyValue(name, callback);
        }

        IComponentBindingWithOrOnActivationSyntax<T> IComponentBindingWithOrOnActivationSyntax<T>.OnDeactivation(Action<T> action)
        {
            return this.OnDeactivation(action);
        }

        IComponentBindingWithOrOnActivationSyntax<T> IComponentBindingWithOrOnActivationSyntax<T>.OnDeactivation<TImplementation>(Action<TImplementation> action)
        {
            return this.OnDeactivation(action);
        }

        IComponentBindingWithOrOnActivationSyntax<T> IComponentBindingWithOrOnActivationSyntax<T>.OnDeactivation(Action<IContext, T> action)
        {
            return this.OnDeactivation(action);
        }

        IComponentBindingWithOrOnActivationSyntax<T> IComponentBindingWithOrOnActivationSyntax<T>.OnDeactivation<TImplementation>(Action<IContext, TImplementation> action)
        {
            return this.OnDeactivation(action);
        }

        #endregion IComponentBindingWithOrOnActivationSyntax

        #region INewBindingWhenInWithOrOnInitializationSyntax

        INewBindingWhenWithOrOnSyntax<T> INewBindingWhenInWithOrOnInitializationSyntax<T>.InSingletonScope()
        {
            return this.InSingletonScope();
        }

        INewBindingWhenWithOrOnInitializationSyntax<T> INewBindingWhenInWithOrOnInitializationSyntax<T>.InTransientScope()
        {
            return this.InTransientScope();
        }

        INewBindingWhenWithOrOnSyntax<T> INewBindingWhenInWithOrOnInitializationSyntax<T>.InThreadScope()
        {
            return this.InThreadScope();
        }

        INewBindingWhenWithOrOnSyntax<T> INewBindingWhenInWithOrOnInitializationSyntax<T>.InScope(Func<IContext, object> scope)
        {
            return this.InScope(scope);
        }

        INewBindingWhenInWithOrOnInitializationSyntax<T> INewBindingWhenInWithOrOnInitializationSyntax<T>.OnInitialization(Action<T> action)
        {
            return this.OnInitialization(action);
        }

        INewBindingWhenInWithOrOnInitializationSyntax<T> INewBindingWhenInWithOrOnInitializationSyntax<T>.OnInitialization<TImplementation>(Action<TImplementation> action)
        {
            return this.OnInitialization(action);
        }

        INewBindingWhenInWithOrOnInitializationSyntax<T> INewBindingWhenInWithOrOnInitializationSyntax<T>.OnInitialization(Action<IContext, T> action)
        {
            return this.OnInitialization(action);
        }

        INewBindingWhenInWithOrOnInitializationSyntax<T> INewBindingWhenInWithOrOnInitializationSyntax<T>.OnInitialization<TImplementation>(Action<IContext, TImplementation> action)
        {
            return this.OnInitialization(action);
        }

        INewBindingWhenInWithOrOnInitializationSyntax<T> INewBindingWhenInWithOrOnInitializationSyntax<T>.OnInitialization<TImplementation>(Func<IContext, TImplementation, TImplementation> action)
        {
            return this.OnInitialization(action);
        }

        INewBindingWhenInWithOrOnInitializationSyntax<T> INewBindingWhenInWithOrOnInitializationSyntax<T>.WithConstructorArgument(string name, object value)
        {
            return this.WithConstructorArgument(name, value);
        }

        INewBindingWhenInWithOrOnInitializationSyntax<T> INewBindingWhenInWithOrOnInitializationSyntax<T>.WithConstructorArgument(string name, Func<IContext, object> callback)
        {
            throw new NotImplementedException();
        }

        INewBindingWhenInWithOrOnInitializationSyntax<T> INewBindingWhenInWithOrOnInitializationSyntax<T>.WithConstructorArgument(string name, Func<IContext, ITarget<IConstructorArgument>, object> callback)
        {
            throw new NotImplementedException();
        }

        INewBindingWhenInWithOrOnInitializationSyntax<T> INewBindingWhenInWithOrOnInitializationSyntax<T>.WithConstructorArgument<TValue>(TValue value)
        {
            throw new NotImplementedException();
        }

        INewBindingWhenInWithOrOnInitializationSyntax<T> INewBindingWhenInWithOrOnInitializationSyntax<T>.WithConstructorArgument(Type type, object value)
        {
            throw new NotImplementedException();
        }

        INewBindingWhenInWithOrOnInitializationSyntax<T> INewBindingWhenInWithOrOnInitializationSyntax<T>.WithConstructorArgument<TValue>(Func<IContext, TValue> callback)
        {
            throw new NotImplementedException();
        }

        INewBindingWhenInWithOrOnInitializationSyntax<T> INewBindingWhenInWithOrOnInitializationSyntax<T>.WithConstructorArgument(Type type, Func<IContext, object> callback)
        {
            throw new NotImplementedException();
        }

        INewBindingWhenInWithOrOnInitializationSyntax<T> INewBindingWhenInWithOrOnInitializationSyntax<T>.WithConstructorArgument<TValue>(Func<IContext, ITarget<IConstructorArgument>, TValue> callback)
        {
            throw new NotImplementedException();
        }

        INewBindingWhenInWithOrOnInitializationSyntax<T> INewBindingWhenInWithOrOnInitializationSyntax<T>.WithConstructorArgument(Type type, Func<IContext, ITarget<IConstructorArgument>, object> callback)
        {
            throw new NotImplementedException();
        }

        INewBindingWhenInWithOrOnInitializationSyntax<T> INewBindingWhenInWithOrOnInitializationSyntax<T>.WithPropertyValue(string name, object value)
        {
            return this.WithPropertyValue(name, value);
        }

        INewBindingWhenInWithOrOnInitializationSyntax<T> INewBindingWhenInWithOrOnInitializationSyntax<T>.WithPropertyValue(string name, Func<IContext, object> callback)
        {
            return this.WithPropertyValue(name, callback);
        }

        INewBindingWhenInWithOrOnInitializationSyntax<T> INewBindingWhenInWithOrOnInitializationSyntax<T>.WithPropertyValue(string name, Func<IContext, ITarget<IPropertyValue>, object> callback)
        {
            return this.WithPropertyValue(name, callback);
        }

        INewBindingWhenInWithOrOnInitializationSyntax<T> INewBindingWhenInWithOrOnInitializationSyntax<T>.WithParameter(IParameter parameter)
        {
            return this.WithParameter(parameter);
        }

        INewBindingWhenInWithOrOnInitializationSyntax<T> INewBindingWhenInWithOrOnInitializationSyntax<T>.WithMetadata(string key, object value)
        {
            throw new NotImplementedException();
        }

        INewBindingWhenInWithOrOnInitializationSyntax<T> INewBindingWhenInWithOrOnInitializationSyntax<T>.When(Func<IRequest, bool> condition)
        {
            throw new NotImplementedException();
        }

        INewBindingWhenInWithOrOnInitializationSyntax<T> INewBindingWhenInWithOrOnInitializationSyntax<T>.WhenInjectedInto<TParent>()
        {
            throw new NotImplementedException();
        }

        INewBindingWhenInWithOrOnInitializationSyntax<T> INewBindingWhenInWithOrOnInitializationSyntax<T>.WhenInjectedInto(Type parent)
        {
            throw new NotImplementedException();
        }

        INewBindingWhenInWithOrOnInitializationSyntax<T> INewBindingWhenInWithOrOnInitializationSyntax<T>.WhenInjectedInto(params Type[] parents)
        {
            throw new NotImplementedException();
        }

        INewBindingWhenInWithOrOnInitializationSyntax<T> INewBindingWhenInWithOrOnInitializationSyntax<T>.WhenInjectedExactlyInto<TParent>()
        {
            throw new NotImplementedException();
        }

        INewBindingWhenInWithOrOnInitializationSyntax<T> INewBindingWhenInWithOrOnInitializationSyntax<T>.WhenInjectedExactlyInto(Type parent)
        {
            throw new NotImplementedException();
        }

        INewBindingWhenInWithOrOnInitializationSyntax<T> INewBindingWhenInWithOrOnInitializationSyntax<T>.WhenInjectedExactlyInto(params Type[] parents)
        {
            throw new NotImplementedException();
        }

        INewBindingWhenInWithOrOnInitializationSyntax<T> INewBindingWhenInWithOrOnInitializationSyntax<T>.WhenClassHas<TAttribute>()
        {
            throw new NotImplementedException();
        }

        INewBindingWhenInWithOrOnInitializationSyntax<T> INewBindingWhenInWithOrOnInitializationSyntax<T>.WhenMemberHas<TAttribute>()
        {
            throw new NotImplementedException();
        }

        INewBindingWhenInWithOrOnInitializationSyntax<T> INewBindingWhenInWithOrOnInitializationSyntax<T>.WhenTargetHas<TAttribute>()
        {
            throw new NotImplementedException();
        }

        INewBindingWhenInWithOrOnInitializationSyntax<T> INewBindingWhenInWithOrOnInitializationSyntax<T>.WhenClassHas(Type attributeType)
        {
            throw new NotImplementedException();
        }

        INewBindingWhenInWithOrOnInitializationSyntax<T> INewBindingWhenInWithOrOnInitializationSyntax<T>.WhenMemberHas(Type attributeType)
        {
            throw new NotImplementedException();
        }

        INewBindingWhenInWithOrOnInitializationSyntax<T> INewBindingWhenInWithOrOnInitializationSyntax<T>.WhenTargetHas(Type attributeType)
        {
            throw new NotImplementedException();
        }

        INewBindingWhenInWithOrOnInitializationSyntax<T> INewBindingWhenInWithOrOnInitializationSyntax<T>.WhenParentNamed(string name)
        {
            throw new NotImplementedException();
        }

        INewBindingWhenInWithOrOnInitializationSyntax<T> INewBindingWhenInWithOrOnInitializationSyntax<T>.WhenAnyAnchestorNamed(string name)
        {
            throw new NotImplementedException();
        }

        INewBindingWhenInWithOrOnInitializationSyntax<T> INewBindingWhenInWithOrOnInitializationSyntax<T>.WhenAnyAncestorNamed(string name)
        {
            throw new NotImplementedException();
        }

        INewBindingWhenInWithOrOnInitializationSyntax<T> INewBindingWhenInWithOrOnInitializationSyntax<T>.WhenNoAncestorNamed(string name)
        {
            throw new NotImplementedException();
        }

        INewBindingWhenInWithOrOnInitializationSyntax<T> INewBindingWhenInWithOrOnInitializationSyntax<T>.WhenAnyAncestorMatches(Predicate<IContext> predicate)
        {
            throw new NotImplementedException();
        }

        INewBindingWhenInWithOrOnInitializationSyntax<T> INewBindingWhenInWithOrOnInitializationSyntax<T>.WhenNoAncestorMatches(Predicate<IContext> predicate)
        {
            throw new NotImplementedException();
        }

        #endregion INewBindingWhenInWithOrOnInitializationSyntax

        #region INewBindingWhenInNamedSyntax

        INewBindingWhenNamedOrOnActivationSyntax<T> INewBindingWhenInNamedSyntax<T>.InSingletonScope()
        {
            return InSingletonScope();
        }

        INewBindingWhenNamedSyntax<T> INewBindingWhenInNamedSyntax<T>.InTransientScope()
        {
            return InTransientScope();
        }

        INewBindingWhenNamedOrOnActivationSyntax<T> INewBindingWhenInNamedSyntax<T>.InThreadScope()
        {
            return InThreadScope();
        }

        INewBindingWhenNamedOrOnActivationSyntax<T> INewBindingWhenInNamedSyntax<T>.InScope(Func<IContext, object> scope)
        {
            return this.InScope(scope);
        }

        INewBindingWhenInNamedSyntax<T> INewBindingWhenInNamedSyntax<T>.When(Func<IRequest, bool> condition)
        {
            throw new NotImplementedException();
        }

        INewBindingWhenInNamedSyntax<T> INewBindingWhenInNamedSyntax<T>.WhenInjectedInto<TParent>()
        {
            throw new NotImplementedException();
        }

        INewBindingWhenInNamedSyntax<T> INewBindingWhenInNamedSyntax<T>.WhenInjectedInto(Type parent)
        {
            throw new NotImplementedException();
        }

        INewBindingWhenInNamedSyntax<T> INewBindingWhenInNamedSyntax<T>.WhenInjectedInto(params Type[] parents)
        {
            throw new NotImplementedException();
        }

        INewBindingWhenInNamedSyntax<T> INewBindingWhenInNamedSyntax<T>.WhenInjectedExactlyInto<TParent>()
        {
            throw new NotImplementedException();
        }

        INewBindingWhenInNamedSyntax<T> INewBindingWhenInNamedSyntax<T>.WhenInjectedExactlyInto(Type parent)
        {
            throw new NotImplementedException();
        }

        INewBindingWhenInNamedSyntax<T> INewBindingWhenInNamedSyntax<T>.WhenInjectedExactlyInto(params Type[] parents)
        {
            throw new NotImplementedException();
        }

        INewBindingWhenInNamedSyntax<T> INewBindingWhenInNamedSyntax<T>.WhenClassHas<TAttribute>()
        {
            throw new NotImplementedException();
        }

        INewBindingWhenInNamedSyntax<T> INewBindingWhenInNamedSyntax<T>.WhenMemberHas<TAttribute>()
        {
            throw new NotImplementedException();
        }

        INewBindingWhenInNamedSyntax<T> INewBindingWhenInNamedSyntax<T>.WhenTargetHas<TAttribute>()
        {
            throw new NotImplementedException();
        }

        INewBindingWhenInNamedSyntax<T> INewBindingWhenInNamedSyntax<T>.WhenClassHas(Type attributeType)
        {
            throw new NotImplementedException();
        }

        INewBindingWhenInNamedSyntax<T> INewBindingWhenInNamedSyntax<T>.WhenMemberHas(Type attributeType)
        {
            throw new NotImplementedException();
        }

        INewBindingWhenInNamedSyntax<T> INewBindingWhenInNamedSyntax<T>.WhenTargetHas(Type attributeType)
        {
            throw new NotImplementedException();
        }

        INewBindingWhenInNamedSyntax<T> INewBindingWhenInNamedSyntax<T>.WhenParentNamed(string name)
        {
            throw new NotImplementedException();
        }

        INewBindingWhenInNamedSyntax<T> INewBindingWhenInNamedSyntax<T>.WhenAnyAnchestorNamed(string name)
        {
            throw new NotImplementedException();
        }

        INewBindingWhenInNamedSyntax<T> INewBindingWhenInNamedSyntax<T>.WhenAnyAncestorNamed(string name)
        {
            throw new NotImplementedException();
        }

        INewBindingWhenInNamedSyntax<T> INewBindingWhenInNamedSyntax<T>.WhenNoAncestorNamed(string name)
        {
            throw new NotImplementedException();
        }

        INewBindingWhenInNamedSyntax<T> INewBindingWhenInNamedSyntax<T>.WhenAnyAncestorMatches(Predicate<IContext> predicate)
        {
            throw new NotImplementedException();
        }

        INewBindingWhenInNamedSyntax<T> INewBindingWhenInNamedSyntax<T>.WhenNoAncestorMatches(Predicate<IContext> predicate)
        {
            throw new NotImplementedException();
        }

        INewBindingWhenInSyntax<T> INewBindingWhenInNamedSyntax<T>.Named(string name)
        {
            return this.Named(name);
        }

        #endregion INewBindingWhenInNamedSyntax

        #region INewBindingWhenNamedOrOnActivationSyntax

        INewBindingWhenNamedOrOnActivationSyntax<T> INewBindingWhenNamedOrOnActivationSyntax<T>.OnActivation(Action<T> action)
        {
            return this.OnActivation(action);
        }

        INewBindingWhenNamedOrOnActivationSyntax<T> INewBindingWhenNamedOrOnActivationSyntax<T>.OnActivation<TImplementation>(Action<TImplementation> action)
        {
            return this.OnActivation(action);
        }

        INewBindingWhenNamedOrOnActivationSyntax<T> INewBindingWhenNamedOrOnActivationSyntax<T>.OnActivation(Action<IContext, T> action)
        {
            return this.OnActivation(action);
        }

        INewBindingWhenNamedOrOnActivationSyntax<T> INewBindingWhenNamedOrOnActivationSyntax<T>.OnActivation<TImplementation>(Action<IContext, TImplementation> action)
        {
            return this.OnActivation(action);
        }

        INewBindingWhenNamedOrOnActivationSyntax<T> INewBindingWhenNamedOrOnActivationSyntax<T>.OnDeactivation(Action<T> action)
        {
            return this.OnDeactivation(action);
        }

        INewBindingWhenNamedOrOnActivationSyntax<T> INewBindingWhenNamedOrOnActivationSyntax<T>.OnDeactivation<TImplementation>(Action<TImplementation> action)
        {
            return this.OnDeactivation(action);
        }

        INewBindingWhenNamedOrOnActivationSyntax<T> INewBindingWhenNamedOrOnActivationSyntax<T>.OnDeactivation(Action<IContext, T> action)
        {
            return this.OnDeactivation(action);
        }

        INewBindingWhenNamedOrOnActivationSyntax<T> INewBindingWhenNamedOrOnActivationSyntax<T>.OnDeactivation<TImplementation>(Action<IContext, TImplementation> action)
        {
            return this.OnDeactivation(action);
        }

        INewBindingWhenNamedOrOnActivationSyntax<T> INewBindingWhenNamedOrOnActivationSyntax<T>.When(Func<IRequest, bool> condition)
        {
            throw new NotImplementedException();
        }

        INewBindingWhenNamedOrOnActivationSyntax<T> INewBindingWhenNamedOrOnActivationSyntax<T>.WhenInjectedInto<TParent>()
        {
            throw new NotImplementedException();
        }

        INewBindingWhenNamedOrOnActivationSyntax<T> INewBindingWhenNamedOrOnActivationSyntax<T>.WhenInjectedInto(Type parent)
        {
            throw new NotImplementedException();
        }

        INewBindingWhenNamedOrOnActivationSyntax<T> INewBindingWhenNamedOrOnActivationSyntax<T>.WhenInjectedInto(params Type[] parents)
        {
            throw new NotImplementedException();
        }

        INewBindingWhenNamedOrOnActivationSyntax<T> INewBindingWhenNamedOrOnActivationSyntax<T>.WhenInjectedExactlyInto<TParent>()
        {
            throw new NotImplementedException();
        }

        INewBindingWhenNamedOrOnActivationSyntax<T> INewBindingWhenNamedOrOnActivationSyntax<T>.WhenInjectedExactlyInto(Type parent)
        {
            throw new NotImplementedException();
        }

        INewBindingWhenNamedOrOnActivationSyntax<T> INewBindingWhenNamedOrOnActivationSyntax<T>.WhenInjectedExactlyInto(params Type[] parents)
        {
            throw new NotImplementedException();
        }

        INewBindingWhenNamedOrOnActivationSyntax<T> INewBindingWhenNamedOrOnActivationSyntax<T>.WhenClassHas<TAttribute>()
        {
            throw new NotImplementedException();
        }

        INewBindingWhenNamedOrOnActivationSyntax<T> INewBindingWhenNamedOrOnActivationSyntax<T>.WhenMemberHas<TAttribute>()
        {
            throw new NotImplementedException();
        }

        INewBindingWhenNamedOrOnActivationSyntax<T> INewBindingWhenNamedOrOnActivationSyntax<T>.WhenTargetHas<TAttribute>()
        {
            throw new NotImplementedException();
        }

        INewBindingWhenNamedOrOnActivationSyntax<T> INewBindingWhenNamedOrOnActivationSyntax<T>.WhenClassHas(Type attributeType)
        {
            throw new NotImplementedException();
        }

        INewBindingWhenNamedOrOnActivationSyntax<T> INewBindingWhenNamedOrOnActivationSyntax<T>.WhenMemberHas(Type attributeType)
        {
            throw new NotImplementedException();
        }

        INewBindingWhenNamedOrOnActivationSyntax<T> INewBindingWhenNamedOrOnActivationSyntax<T>.WhenTargetHas(Type attributeType)
        {
            throw new NotImplementedException();
        }

        INewBindingWhenNamedOrOnActivationSyntax<T> INewBindingWhenNamedOrOnActivationSyntax<T>.WhenParentNamed(string name)
        {
            throw new NotImplementedException();
        }

        INewBindingWhenNamedOrOnActivationSyntax<T> INewBindingWhenNamedOrOnActivationSyntax<T>.WhenAnyAnchestorNamed(string name)
        {
            throw new NotImplementedException();
        }

        INewBindingWhenNamedOrOnActivationSyntax<T> INewBindingWhenNamedOrOnActivationSyntax<T>.WhenAnyAncestorNamed(string name)
        {
            throw new NotImplementedException();
        }

        INewBindingWhenNamedOrOnActivationSyntax<T> INewBindingWhenNamedOrOnActivationSyntax<T>.WhenNoAncestorNamed(string name)
        {
            throw new NotImplementedException();
        }

        INewBindingWhenNamedOrOnActivationSyntax<T> INewBindingWhenNamedOrOnActivationSyntax<T>.WhenAnyAncestorMatches(Predicate<IContext> predicate)
        {
            throw new NotImplementedException();
        }

        INewBindingWhenNamedOrOnActivationSyntax<T> INewBindingWhenNamedOrOnActivationSyntax<T>.WhenNoAncestorMatches(Predicate<IContext> predicate)
        {
            throw new NotImplementedException();
        }

        INewBindingWhenOrOnActivationSyntax<T> INewBindingWhenNamedOrOnActivationSyntax<T>.Named(string name)
        {
            throw new NotImplementedException();
        }

        #endregion INewBindingWhenNamedOrOnActivationSyntax

        #region INewBindingWhenWithOrOnSyntax

        INewBindingWhenWithOrOnSyntax<T> INewBindingWhenWithOrOnSyntax<T>.OnActivation(Action<T> action)
        {
            return this.OnActivation(action);
        }

        INewBindingWhenWithOrOnSyntax<T> INewBindingWhenWithOrOnSyntax<T>.OnActivation<TImplementation>(Action<TImplementation> action)
        {
            return this.OnActivation(action);
        }

        INewBindingWhenWithOrOnSyntax<T> INewBindingWhenWithOrOnSyntax<T>.OnActivation(Action<IContext, T> action)
        {
            return this.OnActivation(action);
        }

        INewBindingWhenWithOrOnSyntax<T> INewBindingWhenWithOrOnSyntax<T>.OnActivation<TImplementation>(Action<IContext, TImplementation> action)
        {
            return this.OnActivation(action);
        }

        INewBindingWhenWithOrOnSyntax<T> INewBindingWhenWithOrOnSyntax<T>.OnDeactivation(Action<T> action)
        {
            return this.OnDeactivation(action);
        }

        INewBindingWhenWithOrOnSyntax<T> INewBindingWhenWithOrOnSyntax<T>.OnDeactivation<TImplementation>(Action<TImplementation> action)
        {
            return this.OnDeactivation(action);
        }

        INewBindingWhenWithOrOnSyntax<T> INewBindingWhenWithOrOnSyntax<T>.OnDeactivation(Action<IContext, T> action)
        {
            return this.OnDeactivation(action);
        }

        INewBindingWhenWithOrOnSyntax<T> INewBindingWhenWithOrOnSyntax<T>.OnDeactivation<TImplementation>(Action<IContext, TImplementation> action)
        {
            return this.OnDeactivation(action);
        }

        INewBindingWhenWithOrOnSyntax<T> INewBindingWhenWithOrOnSyntax<T>.OnInitialization(Action<T> action)
        {
            return this.OnInitialization(action);
        }

        INewBindingWhenWithOrOnSyntax<T> INewBindingWhenWithOrOnSyntax<T>.OnInitialization<TImplementation>(Action<TImplementation> action)
        {
            return this.OnInitialization(action);
        }

        INewBindingWhenWithOrOnSyntax<T> INewBindingWhenWithOrOnSyntax<T>.OnInitialization(Action<IContext, T> action)
        {
            return this.OnInitialization(action);
        }

        INewBindingWhenWithOrOnSyntax<T> INewBindingWhenWithOrOnSyntax<T>.OnInitialization<TImplementation>(Action<IContext, TImplementation> action)
        {
            return this.OnInitialization(action);
        }

        INewBindingWhenWithOrOnSyntax<T> INewBindingWhenWithOrOnSyntax<T>.OnInitialization<TImplementation>(Func<IContext, TImplementation, TImplementation> action)
        {
            return this.OnInitialization(action);
        }

        INewBindingWhenWithOrOnSyntax<T> INewBindingWhenWithOrOnSyntax<T>.WithConstructorArgument(string name, object value)
        {
            return this.WithConstructorArgument(name, value);
        }

        INewBindingWhenWithOrOnSyntax<T> INewBindingWhenWithOrOnSyntax<T>.WithConstructorArgument(string name, Func<IContext, object> callback)
        {
            throw new NotImplementedException();
        }

        INewBindingWhenWithOrOnSyntax<T> INewBindingWhenWithOrOnSyntax<T>.WithConstructorArgument(string name, Func<IContext, ITarget<IConstructorArgument>, object> callback)
        {
            throw new NotImplementedException();
        }

        INewBindingWhenWithOrOnSyntax<T> INewBindingWhenWithOrOnSyntax<T>.WithConstructorArgument<TValue>(TValue value)
        {
            throw new NotImplementedException();
        }

        INewBindingWhenWithOrOnSyntax<T> INewBindingWhenWithOrOnSyntax<T>.WithConstructorArgument(Type type, object value)
        {
            throw new NotImplementedException();
        }

        INewBindingWhenWithOrOnSyntax<T> INewBindingWhenWithOrOnSyntax<T>.WithConstructorArgument<TValue>(Func<IContext, TValue> callback)
        {
            throw new NotImplementedException();
        }

        INewBindingWhenWithOrOnSyntax<T> INewBindingWhenWithOrOnSyntax<T>.WithConstructorArgument(Type type, Func<IContext, object> callback)
        {
            throw new NotImplementedException();
        }

        INewBindingWhenWithOrOnSyntax<T> INewBindingWhenWithOrOnSyntax<T>.WithConstructorArgument<TValue>(Func<IContext, ITarget<IConstructorArgument>, TValue> callback)
        {
            throw new NotImplementedException();
        }

        INewBindingWhenWithOrOnSyntax<T> INewBindingWhenWithOrOnSyntax<T>.WithConstructorArgument(Type type, Func<IContext, ITarget<IConstructorArgument>, object> callback)
        {
            throw new NotImplementedException();
        }

        INewBindingWhenWithOrOnSyntax<T> INewBindingWhenWithOrOnSyntax<T>.WithPropertyValue(string name, object value)
        {
            return this.WithPropertyValue(name, value);
        }

        INewBindingWhenWithOrOnSyntax<T> INewBindingWhenWithOrOnSyntax<T>.WithPropertyValue(string name, Func<IContext, object> callback)
        {
            return this.WithPropertyValue(name, callback);
        }

        INewBindingWhenWithOrOnSyntax<T> INewBindingWhenWithOrOnSyntax<T>.WithPropertyValue(string name, Func<IContext, ITarget<IPropertyValue>, object> callback)
        {
            return this.WithPropertyValue(name, callback);
        }

        INewBindingWhenWithOrOnSyntax<T> INewBindingWhenWithOrOnSyntax<T>.WithParameter(IParameter parameter)
        {
            return this.WithParameter(parameter);
        }

        INewBindingWhenWithOrOnSyntax<T> INewBindingWhenWithOrOnSyntax<T>.WithMetadata(string key, object value)
        {
            throw new NotImplementedException();
        }

        INewBindingWhenWithOrOnSyntax<T> INewBindingWhenWithOrOnSyntax<T>.When(Func<IRequest, bool> condition)
        {
            throw new NotImplementedException();
        }

        INewBindingWhenWithOrOnSyntax<T> INewBindingWhenWithOrOnSyntax<T>.WhenInjectedInto<TParent>()
        {
            throw new NotImplementedException();
        }

        INewBindingWhenWithOrOnSyntax<T> INewBindingWhenWithOrOnSyntax<T>.WhenInjectedInto(Type parent)
        {
            throw new NotImplementedException();
        }

        INewBindingWhenWithOrOnSyntax<T> INewBindingWhenWithOrOnSyntax<T>.WhenInjectedInto(params Type[] parents)
        {
            throw new NotImplementedException();
        }

        INewBindingWhenWithOrOnSyntax<T> INewBindingWhenWithOrOnSyntax<T>.WhenInjectedExactlyInto<TParent>()
        {
            throw new NotImplementedException();
        }

        INewBindingWhenWithOrOnSyntax<T> INewBindingWhenWithOrOnSyntax<T>.WhenInjectedExactlyInto(Type parent)
        {
            throw new NotImplementedException();
        }

        INewBindingWhenWithOrOnSyntax<T> INewBindingWhenWithOrOnSyntax<T>.WhenInjectedExactlyInto(params Type[] parents)
        {
            throw new NotImplementedException();
        }

        INewBindingWhenWithOrOnSyntax<T> INewBindingWhenWithOrOnSyntax<T>.WhenClassHas<TAttribute>()
        {
            throw new NotImplementedException();
        }

        INewBindingWhenWithOrOnSyntax<T> INewBindingWhenWithOrOnSyntax<T>.WhenMemberHas<TAttribute>()
        {
            throw new NotImplementedException();
        }

        INewBindingWhenWithOrOnSyntax<T> INewBindingWhenWithOrOnSyntax<T>.WhenTargetHas<TAttribute>()
        {
            throw new NotImplementedException();
        }

        INewBindingWhenWithOrOnSyntax<T> INewBindingWhenWithOrOnSyntax<T>.WhenClassHas(Type attributeType)
        {
            throw new NotImplementedException();
        }

        INewBindingWhenWithOrOnSyntax<T> INewBindingWhenWithOrOnSyntax<T>.WhenMemberHas(Type attributeType)
        {
            throw new NotImplementedException();
        }

        INewBindingWhenWithOrOnSyntax<T> INewBindingWhenWithOrOnSyntax<T>.WhenTargetHas(Type attributeType)
        {
            throw new NotImplementedException();
        }

        INewBindingWhenWithOrOnSyntax<T> INewBindingWhenWithOrOnSyntax<T>.WhenParentNamed(string name)
        {
            throw new NotImplementedException();
        }

        INewBindingWhenWithOrOnSyntax<T> INewBindingWhenWithOrOnSyntax<T>.WhenAnyAnchestorNamed(string name)
        {
            throw new NotImplementedException();
        }

        INewBindingWhenWithOrOnSyntax<T> INewBindingWhenWithOrOnSyntax<T>.WhenAnyAncestorNamed(string name)
        {
            throw new NotImplementedException();
        }

        INewBindingWhenWithOrOnSyntax<T> INewBindingWhenWithOrOnSyntax<T>.WhenNoAncestorNamed(string name)
        {
            throw new NotImplementedException();
        }

        INewBindingWhenWithOrOnSyntax<T> INewBindingWhenWithOrOnSyntax<T>.WhenAnyAncestorMatches(Predicate<IContext> predicate)
        {
            throw new NotImplementedException();
        }

        INewBindingWhenWithOrOnSyntax<T> INewBindingWhenWithOrOnSyntax<T>.WhenNoAncestorMatches(Predicate<IContext> predicate)
        {
            throw new NotImplementedException();
        }

        #endregion INewBindingWhenWithOrOnSyntax

        #region INewBindingWhenInSyntax

        INewBindingWhenOrOnActivationSyntax<T> INewBindingWhenInSyntax<T>.InSingletonScope()
        {
            return this.InSingletonScope();
        }

        INewBindingWhenOrOnActivationSyntax<T> INewBindingWhenInSyntax<T>.InThreadScope()
        {
            return this.InThreadScope();
        }

        INewBindingWhenOrOnActivationSyntax<T> INewBindingWhenInSyntax<T>.InScope(Func<IContext, object> scope)
        {
            return this.InScope(scope);
        }

        INewBindingWhenSyntax<T> INewBindingWhenInSyntax<T>.InTransientScope()
        {
            return this.InTransientScope();
        }

        INewBindingWhenInSyntax<T> INewBindingWhenInSyntax<T>.When(Func<IRequest, bool> condition)
        {
            throw new NotImplementedException();
        }

        INewBindingWhenInSyntax<T> INewBindingWhenInSyntax<T>.WhenInjectedInto<TParent>()
        {
            throw new NotImplementedException();
        }

        INewBindingWhenInSyntax<T> INewBindingWhenInSyntax<T>.WhenInjectedInto(Type parent)
        {
            throw new NotImplementedException();
        }

        INewBindingWhenInSyntax<T> INewBindingWhenInSyntax<T>.WhenInjectedInto(params Type[] parents)
        {
            throw new NotImplementedException();
        }

        INewBindingWhenInSyntax<T> INewBindingWhenInSyntax<T>.WhenInjectedExactlyInto<TParent>()
        {
            throw new NotImplementedException();
        }

        INewBindingWhenInSyntax<T> INewBindingWhenInSyntax<T>.WhenInjectedExactlyInto(Type parent)
        {
            throw new NotImplementedException();
        }

        INewBindingWhenInSyntax<T> INewBindingWhenInSyntax<T>.WhenInjectedExactlyInto(params Type[] parents)
        {
            throw new NotImplementedException();
        }

        INewBindingWhenInSyntax<T> INewBindingWhenInSyntax<T>.WhenClassHas<TAttribute>()
        {
            throw new NotImplementedException();
        }

        INewBindingWhenInSyntax<T> INewBindingWhenInSyntax<T>.WhenMemberHas<TAttribute>()
        {
            throw new NotImplementedException();
        }

        INewBindingWhenInSyntax<T> INewBindingWhenInSyntax<T>.WhenTargetHas<TAttribute>()
        {
            throw new NotImplementedException();
        }

        INewBindingWhenInSyntax<T> INewBindingWhenInSyntax<T>.WhenClassHas(Type attributeType)
        {
            throw new NotImplementedException();
        }

        INewBindingWhenInSyntax<T> INewBindingWhenInSyntax<T>.WhenMemberHas(Type attributeType)
        {
            throw new NotImplementedException();
        }

        INewBindingWhenInSyntax<T> INewBindingWhenInSyntax<T>.WhenTargetHas(Type attributeType)
        {
            throw new NotImplementedException();
        }

        INewBindingWhenInSyntax<T> INewBindingWhenInSyntax<T>.WhenParentNamed(string name)
        {
            throw new NotImplementedException();
        }

        INewBindingWhenInSyntax<T> INewBindingWhenInSyntax<T>.WhenAnyAnchestorNamed(string name)
        {
            throw new NotImplementedException();
        }

        INewBindingWhenInSyntax<T> INewBindingWhenInSyntax<T>.WhenAnyAncestorNamed(string name)
        {
            throw new NotImplementedException();
        }

        INewBindingWhenInSyntax<T> INewBindingWhenInSyntax<T>.WhenNoAncestorNamed(string name)
        {
            throw new NotImplementedException();
        }

        INewBindingWhenInSyntax<T> INewBindingWhenInSyntax<T>.WhenAnyAncestorMatches(Predicate<IContext> predicate)
        {
            throw new NotImplementedException();
        }

        INewBindingWhenInSyntax<T> INewBindingWhenInSyntax<T>.WhenNoAncestorMatches(Predicate<IContext> predicate)
        {
            throw new NotImplementedException();
        }

        #endregion INewBindingWhenInSyntax

        #region INewBindingWhenOrOnActivationSyntax

        INewBindingWhenOrOnActivationSyntax<T> INewBindingWhenOrOnActivationSyntax<T>.OnActivation(Action<T> action)
        {
            return this.OnActivation(action);
        }

        INewBindingWhenOrOnActivationSyntax<T> INewBindingWhenOrOnActivationSyntax<T>.OnActivation<TImplementation>(Action<TImplementation> action)
        {
            return this.OnActivation(action);
        }

        INewBindingWhenOrOnActivationSyntax<T> INewBindingWhenOrOnActivationSyntax<T>.OnActivation(Action<IContext, T> action)
        {
            return this.OnActivation(action);
        }

        INewBindingWhenOrOnActivationSyntax<T> INewBindingWhenOrOnActivationSyntax<T>.OnActivation<TImplementation>(Action<IContext, TImplementation> action)
        {
            return this.OnActivation(action);
        }

        INewBindingWhenOrOnActivationSyntax<T> INewBindingWhenOrOnActivationSyntax<T>.OnDeactivation(Action<T> action)
        {
            return this.OnDeactivation(action);
        }

        INewBindingWhenOrOnActivationSyntax<T> INewBindingWhenOrOnActivationSyntax<T>.OnDeactivation<TImplementation>(Action<TImplementation> action)
        {
            return this.OnDeactivation(action);
        }

        INewBindingWhenOrOnActivationSyntax<T> INewBindingWhenOrOnActivationSyntax<T>.OnDeactivation(Action<IContext, T> action)
        {
            return this.OnDeactivation(action);
        }

        INewBindingWhenOrOnActivationSyntax<T> INewBindingWhenOrOnActivationSyntax<T>.OnDeactivation<TImplementation>(Action<IContext, TImplementation> action)
        {
            return this.OnDeactivation(action);
        }

        INewBindingWhenOrOnActivationSyntax<T> INewBindingWhenOrOnActivationSyntax<T>.When(Func<IRequest, bool> condition)
        {
            throw new NotImplementedException();
        }

        INewBindingWhenOrOnActivationSyntax<T> INewBindingWhenOrOnActivationSyntax<T>.WhenInjectedInto<TParent>()
        {
            throw new NotImplementedException();
        }

        INewBindingWhenOrOnActivationSyntax<T> INewBindingWhenOrOnActivationSyntax<T>.WhenInjectedInto(Type parent)
        {
            throw new NotImplementedException();
        }

        INewBindingWhenOrOnActivationSyntax<T> INewBindingWhenOrOnActivationSyntax<T>.WhenInjectedInto(params Type[] parents)
        {
            throw new NotImplementedException();
        }

        INewBindingWhenOrOnActivationSyntax<T> INewBindingWhenOrOnActivationSyntax<T>.WhenInjectedExactlyInto<TParent>()
        {
            throw new NotImplementedException();
        }

        INewBindingWhenOrOnActivationSyntax<T> INewBindingWhenOrOnActivationSyntax<T>.WhenInjectedExactlyInto(Type parent)
        {
            throw new NotImplementedException();
        }

        INewBindingWhenOrOnActivationSyntax<T> INewBindingWhenOrOnActivationSyntax<T>.WhenInjectedExactlyInto(params Type[] parents)
        {
            throw new NotImplementedException();
        }

        INewBindingWhenOrOnActivationSyntax<T> INewBindingWhenOrOnActivationSyntax<T>.WhenClassHas<TAttribute>()
        {
            throw new NotImplementedException();
        }

        INewBindingWhenOrOnActivationSyntax<T> INewBindingWhenOrOnActivationSyntax<T>.WhenMemberHas<TAttribute>()
        {
            throw new NotImplementedException();
        }

        INewBindingWhenOrOnActivationSyntax<T> INewBindingWhenOrOnActivationSyntax<T>.WhenTargetHas<TAttribute>()
        {
            throw new NotImplementedException();
        }

        INewBindingWhenOrOnActivationSyntax<T> INewBindingWhenOrOnActivationSyntax<T>.WhenClassHas(Type attributeType)
        {
            throw new NotImplementedException();
        }

        INewBindingWhenOrOnActivationSyntax<T> INewBindingWhenOrOnActivationSyntax<T>.WhenMemberHas(Type attributeType)
        {
            throw new NotImplementedException();
        }

        INewBindingWhenOrOnActivationSyntax<T> INewBindingWhenOrOnActivationSyntax<T>.WhenTargetHas(Type attributeType)
        {
            throw new NotImplementedException();
        }

        INewBindingWhenOrOnActivationSyntax<T> INewBindingWhenOrOnActivationSyntax<T>.WhenParentNamed(string name)
        {
            throw new NotImplementedException();
        }

        INewBindingWhenOrOnActivationSyntax<T> INewBindingWhenOrOnActivationSyntax<T>.WhenAnyAnchestorNamed(string name)
        {
            throw new NotImplementedException();
        }

        INewBindingWhenOrOnActivationSyntax<T> INewBindingWhenOrOnActivationSyntax<T>.WhenAnyAncestorNamed(string name)
        {
            throw new NotImplementedException();
        }

        INewBindingWhenOrOnActivationSyntax<T> INewBindingWhenOrOnActivationSyntax<T>.WhenNoAncestorNamed(string name)
        {
            throw new NotImplementedException();
        }

        INewBindingWhenOrOnActivationSyntax<T> INewBindingWhenOrOnActivationSyntax<T>.WhenAnyAncestorMatches(Predicate<IContext> predicate)
        {
            throw new NotImplementedException();
        }

        INewBindingWhenOrOnActivationSyntax<T> INewBindingWhenOrOnActivationSyntax<T>.WhenNoAncestorMatches(Predicate<IContext> predicate)
        {
            throw new NotImplementedException();
        }

        #endregion INewBindingWhenOrOnActivationSyntax

        #region INewBindingWhenWithOrOnInitializationSyntax

        INewBindingWhenWithOrOnInitializationSyntax<T> INewBindingWhenWithOrOnInitializationSyntax<T>.OnInitialization(Action<T> action)
        {
            return this.OnInitialization(action);
        }

        INewBindingWhenWithOrOnInitializationSyntax<T> INewBindingWhenWithOrOnInitializationSyntax<T>.OnInitialization<TImplementation>(Action<TImplementation> action)
        {
            return this.OnInitialization(action);
        }

        INewBindingWhenWithOrOnInitializationSyntax<T> INewBindingWhenWithOrOnInitializationSyntax<T>.OnInitialization(Action<IContext, T> action)
        {
            return this.OnInitialization(action);
        }

        INewBindingWhenWithOrOnInitializationSyntax<T> INewBindingWhenWithOrOnInitializationSyntax<T>.OnInitialization<TImplementation>(Action<IContext, TImplementation> action)
        {
            return this.OnInitialization(action);
        }

        INewBindingWhenWithOrOnInitializationSyntax<T> INewBindingWhenWithOrOnInitializationSyntax<T>.OnInitialization<TImplementation>(Func<IContext, TImplementation, TImplementation> action)
        {
            return this.OnInitialization(action);
        }

        INewBindingWhenWithOrOnInitializationSyntax<T> INewBindingWhenWithOrOnInitializationSyntax<T>.WithConstructorArgument(string name, object value)
        {
            throw new NotImplementedException();
        }

        INewBindingWhenWithOrOnInitializationSyntax<T> INewBindingWhenWithOrOnInitializationSyntax<T>.WithConstructorArgument(string name, Func<IContext, object> callback)
        {
            throw new NotImplementedException();
        }

        INewBindingWhenWithOrOnInitializationSyntax<T> INewBindingWhenWithOrOnInitializationSyntax<T>.WithConstructorArgument(string name, Func<IContext, ITarget<IConstructorArgument>, object> callback)
        {
            throw new NotImplementedException();
        }

        INewBindingWhenWithOrOnInitializationSyntax<T> INewBindingWhenWithOrOnInitializationSyntax<T>.WithConstructorArgument<TValue>(TValue value)
        {
            throw new NotImplementedException();
        }

        INewBindingWhenWithOrOnInitializationSyntax<T> INewBindingWhenWithOrOnInitializationSyntax<T>.WithConstructorArgument(Type type, object value)
        {
            throw new NotImplementedException();
        }

        INewBindingWhenWithOrOnInitializationSyntax<T> INewBindingWhenWithOrOnInitializationSyntax<T>.WithConstructorArgument<TValue>(Func<IContext, TValue> callback)
        {
            throw new NotImplementedException();
        }

        INewBindingWhenWithOrOnInitializationSyntax<T> INewBindingWhenWithOrOnInitializationSyntax<T>.WithConstructorArgument(Type type, Func<IContext, object> callback)
        {
            throw new NotImplementedException();
        }

        INewBindingWhenWithOrOnInitializationSyntax<T> INewBindingWhenWithOrOnInitializationSyntax<T>.WithConstructorArgument<TValue>(Func<IContext, ITarget<IConstructorArgument>, TValue> callback)
        {
            throw new NotImplementedException();
        }

        INewBindingWhenWithOrOnInitializationSyntax<T> INewBindingWhenWithOrOnInitializationSyntax<T>.WithConstructorArgument(Type type, Func<IContext, ITarget<IConstructorArgument>, object> callback)
        {
            throw new NotImplementedException();
        }

        INewBindingWhenWithOrOnInitializationSyntax<T> INewBindingWhenWithOrOnInitializationSyntax<T>.WithPropertyValue(string name, object value)
        {
            throw new NotImplementedException();
        }

        INewBindingWhenWithOrOnInitializationSyntax<T> INewBindingWhenWithOrOnInitializationSyntax<T>.WithPropertyValue(string name, Func<IContext, object> callback)
        {
            throw new NotImplementedException();
        }

        INewBindingWhenWithOrOnInitializationSyntax<T> INewBindingWhenWithOrOnInitializationSyntax<T>.WithPropertyValue(string name, Func<IContext, ITarget<IPropertyValue>, object> callback)
        {
            throw new NotImplementedException();
        }

        INewBindingWhenWithOrOnInitializationSyntax<T> INewBindingWhenWithOrOnInitializationSyntax<T>.WithParameter(IParameter parameter)
        {
            throw new NotImplementedException();
        }

        INewBindingWhenWithOrOnInitializationSyntax<T> INewBindingWhenWithOrOnInitializationSyntax<T>.WithMetadata(string key, object value)
        {
            throw new NotImplementedException();
        }

        INewBindingWhenWithOrOnInitializationSyntax<T> INewBindingWhenWithOrOnInitializationSyntax<T>.When(Func<IRequest, bool> condition)
        {
            throw new NotImplementedException();
        }

        INewBindingWhenWithOrOnInitializationSyntax<T> INewBindingWhenWithOrOnInitializationSyntax<T>.WhenInjectedInto<TParent>()
        {
            throw new NotImplementedException();
        }

        INewBindingWhenWithOrOnInitializationSyntax<T> INewBindingWhenWithOrOnInitializationSyntax<T>.WhenInjectedInto(Type parent)
        {
            throw new NotImplementedException();
        }

        INewBindingWhenWithOrOnInitializationSyntax<T> INewBindingWhenWithOrOnInitializationSyntax<T>.WhenInjectedInto(params Type[] parents)
        {
            throw new NotImplementedException();
        }

        INewBindingWhenWithOrOnInitializationSyntax<T> INewBindingWhenWithOrOnInitializationSyntax<T>.WhenInjectedExactlyInto<TParent>()
        {
            throw new NotImplementedException();
        }

        INewBindingWhenWithOrOnInitializationSyntax<T> INewBindingWhenWithOrOnInitializationSyntax<T>.WhenInjectedExactlyInto(Type parent)
        {
            throw new NotImplementedException();
        }

        INewBindingWhenWithOrOnInitializationSyntax<T> INewBindingWhenWithOrOnInitializationSyntax<T>.WhenInjectedExactlyInto(params Type[] parents)
        {
            throw new NotImplementedException();
        }

        INewBindingWhenWithOrOnInitializationSyntax<T> INewBindingWhenWithOrOnInitializationSyntax<T>.WhenClassHas<TAttribute>()
        {
            throw new NotImplementedException();
        }

        INewBindingWhenWithOrOnInitializationSyntax<T> INewBindingWhenWithOrOnInitializationSyntax<T>.WhenMemberHas<TAttribute>()
        {
            throw new NotImplementedException();
        }

        INewBindingWhenWithOrOnInitializationSyntax<T> INewBindingWhenWithOrOnInitializationSyntax<T>.WhenTargetHas<TAttribute>()
        {
            throw new NotImplementedException();
        }

        INewBindingWhenWithOrOnInitializationSyntax<T> INewBindingWhenWithOrOnInitializationSyntax<T>.WhenClassHas(Type attributeType)
        {
            throw new NotImplementedException();
        }

        INewBindingWhenWithOrOnInitializationSyntax<T> INewBindingWhenWithOrOnInitializationSyntax<T>.WhenMemberHas(Type attributeType)
        {
            throw new NotImplementedException();
        }

        INewBindingWhenWithOrOnInitializationSyntax<T> INewBindingWhenWithOrOnInitializationSyntax<T>.WhenTargetHas(Type attributeType)
        {
            throw new NotImplementedException();
        }

        INewBindingWhenWithOrOnInitializationSyntax<T> INewBindingWhenWithOrOnInitializationSyntax<T>.WhenParentNamed(string name)
        {
            throw new NotImplementedException();
        }

        INewBindingWhenWithOrOnInitializationSyntax<T> INewBindingWhenWithOrOnInitializationSyntax<T>.WhenAnyAnchestorNamed(string name)
        {
            throw new NotImplementedException();
        }

        INewBindingWhenWithOrOnInitializationSyntax<T> INewBindingWhenWithOrOnInitializationSyntax<T>.WhenAnyAncestorNamed(string name)
        {
            throw new NotImplementedException();
        }

        INewBindingWhenWithOrOnInitializationSyntax<T> INewBindingWhenWithOrOnInitializationSyntax<T>.WhenNoAncestorNamed(string name)
        {
            throw new NotImplementedException();
        }

        INewBindingWhenWithOrOnInitializationSyntax<T> INewBindingWhenWithOrOnInitializationSyntax<T>.WhenAnyAncestorMatches(Predicate<IContext> predicate)
        {
            throw new NotImplementedException();
        }

        INewBindingWhenWithOrOnInitializationSyntax<T> INewBindingWhenWithOrOnInitializationSyntax<T>.WhenNoAncestorMatches(Predicate<IContext> predicate)
        {
            throw new NotImplementedException();
        }

        #endregion INewBindingWhenWithOrOnInitializationSyntax

        #region INewBindingWhenNamedSyntax

        INewBindingWhenNamedSyntax<T> INewBindingWhenNamedSyntax<T>.When(Func<IRequest, bool> condition)
        {
            throw new NotImplementedException();
        }

        INewBindingWhenNamedSyntax<T> INewBindingWhenNamedSyntax<T>.WhenInjectedInto<TParent>()
        {
            throw new NotImplementedException();
        }

        INewBindingWhenNamedSyntax<T> INewBindingWhenNamedSyntax<T>.WhenInjectedInto(Type parent)
        {
            throw new NotImplementedException();
        }

        INewBindingWhenNamedSyntax<T> INewBindingWhenNamedSyntax<T>.WhenInjectedInto(params Type[] parents)
        {
            throw new NotImplementedException();
        }

        INewBindingWhenNamedSyntax<T> INewBindingWhenNamedSyntax<T>.WhenInjectedExactlyInto<TParent>()
        {
            throw new NotImplementedException();
        }

        INewBindingWhenNamedSyntax<T> INewBindingWhenNamedSyntax<T>.WhenInjectedExactlyInto(Type parent)
        {
            throw new NotImplementedException();
        }

        INewBindingWhenNamedSyntax<T> INewBindingWhenNamedSyntax<T>.WhenInjectedExactlyInto(params Type[] parents)
        {
            throw new NotImplementedException();
        }

        INewBindingWhenNamedSyntax<T> INewBindingWhenNamedSyntax<T>.WhenClassHas<TAttribute>()
        {
            throw new NotImplementedException();
        }

        INewBindingWhenNamedSyntax<T> INewBindingWhenNamedSyntax<T>.WhenMemberHas<TAttribute>()
        {
            throw new NotImplementedException();
        }

        INewBindingWhenNamedSyntax<T> INewBindingWhenNamedSyntax<T>.WhenTargetHas<TAttribute>()
        {
            throw new NotImplementedException();
        }

        INewBindingWhenNamedSyntax<T> INewBindingWhenNamedSyntax<T>.WhenClassHas(Type attributeType)
        {
            throw new NotImplementedException();
        }

        INewBindingWhenNamedSyntax<T> INewBindingWhenNamedSyntax<T>.WhenMemberHas(Type attributeType)
        {
            throw new NotImplementedException();
        }

        INewBindingWhenNamedSyntax<T> INewBindingWhenNamedSyntax<T>.WhenTargetHas(Type attributeType)
        {
            throw new NotImplementedException();
        }

        INewBindingWhenNamedSyntax<T> INewBindingWhenNamedSyntax<T>.WhenParentNamed(string name)
        {
            throw new NotImplementedException();
        }

        INewBindingWhenNamedSyntax<T> INewBindingWhenNamedSyntax<T>.WhenAnyAnchestorNamed(string name)
        {
            throw new NotImplementedException();
        }

        INewBindingWhenNamedSyntax<T> INewBindingWhenNamedSyntax<T>.WhenAnyAncestorNamed(string name)
        {
            throw new NotImplementedException();
        }

        INewBindingWhenNamedSyntax<T> INewBindingWhenNamedSyntax<T>.WhenNoAncestorNamed(string name)
        {
            throw new NotImplementedException();
        }

        INewBindingWhenNamedSyntax<T> INewBindingWhenNamedSyntax<T>.WhenAnyAncestorMatches(Predicate<IContext> predicate)
        {
            throw new NotImplementedException();
        }

        INewBindingWhenNamedSyntax<T> INewBindingWhenNamedSyntax<T>.WhenNoAncestorMatches(Predicate<IContext> predicate)
        {
            throw new NotImplementedException();
        }

        INewBindingWhenSyntax<T> INewBindingWhenNamedSyntax<T>.Named(string name)
        {
            throw new NotImplementedException();
        }

        #endregion INewBindingWhenNamedSyntax

        #region INewBindingWhenSyntax

        INewBindingWhenSyntax<T> INewBindingWhenSyntax<T>.When(Func<IRequest, bool> condition)
        {
            throw new NotImplementedException();
        }

        INewBindingWhenSyntax<T> INewBindingWhenSyntax<T>.WhenInjectedInto<TParent>()
        {
            throw new NotImplementedException();
        }

        INewBindingWhenSyntax<T> INewBindingWhenSyntax<T>.WhenInjectedInto(Type parent)
        {
            throw new NotImplementedException();
        }

        INewBindingWhenSyntax<T> INewBindingWhenSyntax<T>.WhenInjectedInto(params Type[] parents)
        {
            throw new NotImplementedException();
        }

        INewBindingWhenSyntax<T> INewBindingWhenSyntax<T>.WhenInjectedExactlyInto<TParent>()
        {
            throw new NotImplementedException();
        }

        INewBindingWhenSyntax<T> INewBindingWhenSyntax<T>.WhenInjectedExactlyInto(Type parent)
        {
            throw new NotImplementedException();
        }

        INewBindingWhenSyntax<T> INewBindingWhenSyntax<T>.WhenInjectedExactlyInto(params Type[] parents)
        {
            throw new NotImplementedException();
        }

        INewBindingWhenSyntax<T> INewBindingWhenSyntax<T>.WhenClassHas<TAttribute>()
        {
            throw new NotImplementedException();
        }

        INewBindingWhenSyntax<T> INewBindingWhenSyntax<T>.WhenMemberHas<TAttribute>()
        {
            throw new NotImplementedException();
        }

        INewBindingWhenSyntax<T> INewBindingWhenSyntax<T>.WhenTargetHas<TAttribute>()
        {
            throw new NotImplementedException();
        }

        INewBindingWhenSyntax<T> INewBindingWhenSyntax<T>.WhenClassHas(Type attributeType)
        {
            throw new NotImplementedException();
        }

        INewBindingWhenSyntax<T> INewBindingWhenSyntax<T>.WhenMemberHas(Type attributeType)
        {
            throw new NotImplementedException();
        }

        INewBindingWhenSyntax<T> INewBindingWhenSyntax<T>.WhenTargetHas(Type attributeType)
        {
            throw new NotImplementedException();
        }

        INewBindingWhenSyntax<T> INewBindingWhenSyntax<T>.WhenParentNamed(string name)
        {
            throw new NotImplementedException();
        }

        INewBindingWhenSyntax<T> INewBindingWhenSyntax<T>.WhenAnyAnchestorNamed(string name)
        {
            throw new NotImplementedException();
        }

        INewBindingWhenSyntax<T> INewBindingWhenSyntax<T>.WhenAnyAncestorNamed(string name)
        {
            throw new NotImplementedException();
        }

        INewBindingWhenSyntax<T> INewBindingWhenSyntax<T>.WhenNoAncestorNamed(string name)
        {
            throw new NotImplementedException();
        }

        INewBindingWhenSyntax<T> INewBindingWhenSyntax<T>.WhenAnyAncestorMatches(Predicate<IContext> predicate)
        {
            throw new NotImplementedException();
        }

        INewBindingWhenSyntax<T> INewBindingWhenSyntax<T>.WhenNoAncestorMatches(Predicate<IContext> predicate)
        {
            throw new NotImplementedException();
        }

        #endregion INewBindingWhenSyntax

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
        /// Indicates that the binding should be registered with the specified name. Names are not
        /// necessarily unique; multiple bindings for a given service may be registered with the same name.
        /// </summary>
        /// <param name="name">The name to give the binding.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        private BindingConfigurationBuilder<T> Named(string name)
        {
            throw new NotImplementedException();
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

        private BindingConfigurationBuilder<T> OnDeactivation(Action<T> action)
        {
            this.deactivationActions.Add((context, instance) => action((T)instance));
            return this;
        }

        private BindingConfigurationBuilder<T> OnDeactivation<TImplementation>(Action<TImplementation> action)
        {
            this.deactivationActions.Add((context, instance) => action((TImplementation)instance));
            return this;
        }

        private BindingConfigurationBuilder<T> OnDeactivation(Action<IContext, T> action)
        {
            this.deactivationActions.Add((context, instance) => action(context, (T)instance));
            return this;
        }

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
        private BindingConfigurationBuilder<T> WithPropertyValue(string name, Func<IContext, ITarget, object> callback)
        {
            this.parameters.Add(new PropertyValue(name, callback));
            return this;
        }
    }
}