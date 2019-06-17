// -------------------------------------------------------------------------------------------------
// <copyright file="BindingBuilder{T}.cs" company="Ninject Project Contributors">
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
    using System.Linq.Expressions;

    using Ninject.Activation;
    using Ninject.Activation.Providers;
    using Ninject.Infrastructure;
    using Ninject.Infrastructure.Introspection;
    using Ninject.Planning.Bindings;
    using Ninject.Syntax;

    /// <summary>
    /// Provides a root for the fluent syntax associated with an <see cref="Binding"/>.
    /// </summary>
    /// <typeparam name="T">The service type.</typeparam>
    internal sealed class BindingBuilder<T> : BindingBuilder, INewBindingToSyntax<T>
    {
        private BindingConfigurationBuilder bindingConfigurationBuilder;
        private string serviceNames;

        /// <summary>
        /// Gets the binding being built.
        /// </summary>
        public override BindingConfigurationBuilder BindingConfigurationBuilder => this.bindingConfigurationBuilder;

        /// <summary>
        /// Gets the names of the services that this instance builds a binding for.
        /// </summary>
        /// <value>
        /// The names of the services that this instance builds a binding for.
        /// </value>
        public override string ServiceNames
        {
            get
            {
                if (this.serviceNames == null)
                {
                    this.serviceNames = typeof(T).Format();
                }

                return this.serviceNames;
            }
        }

        /// <summary>
        /// Gets the <see cref="Type"/> of the service to bind.
        /// </summary>
        /// <value>
        /// The <see cref="Type"/> of the service to bind.
        /// </value>
        public override Type Service => typeof(T);

        /// <summary>
        /// Builds the binding(s) of this instance.
        /// </summary>
        /// <param name="root">The resolution root.</param>
        /// <param name="bindingVisitor">Accepts the built binding(s).</param>
        public override void Build(IResolutionRoot root, IVisitor<IBinding> bindingVisitor)
        {
            bindingVisitor.Visit(new Binding(typeof(T), this.bindingConfigurationBuilder.Build(root)));
        }

        /// <summary>
        /// Indicates that the service should be bound to the specified implementation type.
        /// </summary>
        /// <typeparam name="TImplementation">The implementation type.</typeparam>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        public INewBindingWhenInWithOrOnInitializationSyntax<TImplementation> To<TImplementation>()
            where TImplementation : T
        {
            var providerBuilder = new StandardProviderFactory(typeof(TImplementation));
            var bindingConfigurationBuilder = new BindingConfigurationBuilder<TImplementation>(providerBuilder, BindingTarget.Type, this);
            this.bindingConfigurationBuilder = bindingConfigurationBuilder;
            return bindingConfigurationBuilder;
        }

        /// <summary>
        /// Indicates that the service should be bound to the specified implementation type.
        /// </summary>
        /// <param name="implementation">The implementation type.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        public INewBindingWhenInWithOrOnInitializationSyntax<T> To(Type implementation)
        {
            var providerBuilder = new StandardProviderFactory(implementation);
            var bindingConfigurationBuilder = new BindingConfigurationBuilder<T>(providerBuilder, BindingTarget.Type, this);
            this.bindingConfigurationBuilder = bindingConfigurationBuilder;
            return bindingConfigurationBuilder;
        }

        /// <summary>
        /// Indicates that the service should be bound to the specified constant value.
        /// </summary>
        /// <typeparam name="TImplementation">The type of the implementation.</typeparam>
        /// <param name="value">The constant value.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        public INewBindingWhenNamedOrOnActivationSyntax<TImplementation> ToConstant<TImplementation>(TImplementation value)
            where TImplementation : T
        {
            var providerBuilder = new ConstantProviderFactory<TImplementation>(value);
            var bindingConfigurationBuilder = new BindingConfigurationBuilder<TImplementation>(providerBuilder, BindingTarget.Constant, this);
            this.bindingConfigurationBuilder = bindingConfigurationBuilder;
            return bindingConfigurationBuilder;
        }

        /// <summary>
        /// Indicates that the service should be bound to the specified constructor.
        /// </summary>
        /// <typeparam name="TImplementation">The type of the implementation.</typeparam>
        /// <param name="newExpression">The expression that specifies the constructor.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        public INewBindingWhenInWithOrOnInitializationSyntax<TImplementation> ToConstructor<TImplementation>(Expression<Func<IConstructorArgumentSyntax, TImplementation>> newExpression)
            where TImplementation : T
        {
            var providerBuilder = new ConstructorProviderFactory<TImplementation>(newExpression);
            var bindingConfigurationBuilder = new BindingConfigurationBuilder<TImplementation>(providerBuilder, BindingTarget.Type, this);
            this.bindingConfigurationBuilder = bindingConfigurationBuilder;

            foreach (var constructorArgument in providerBuilder.GetConstructorArguments())
            {
                bindingConfigurationBuilder.WithParameter(constructorArgument);
            }

            return bindingConfigurationBuilder;
        }

        /// <summary>
        /// Indicates that the service should be bound to the specified callback method.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        public INewBindingWhenInNamedSyntax<T> ToMethod(Func<IContext, T> method)
        {
            var providerBuilder = new ProviderBuilderAdapter(new CallbackProvider<T>(method));
            var bindingConfigurationBuilder = new BindingConfigurationBuilder<T>(providerBuilder, BindingTarget.Method, this);
            this.bindingConfigurationBuilder = bindingConfigurationBuilder;
            return bindingConfigurationBuilder;
        }

        /// <summary>
        /// Indicates that the service should be bound to the specified callback method.
        /// </summary>
        /// <typeparam name="TImplementation">The type of the implementation.</typeparam>
        /// <param name="method">The method.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        public INewBindingWhenInNamedSyntax<TImplementation> ToMethod<TImplementation>(Func<IContext, TImplementation> method)
            where TImplementation : T
        {
            var providerBuilder = new ProviderBuilderAdapter(new CallbackProvider<TImplementation>(method));
            var bindingConfigurationBuilder = new BindingConfigurationBuilder<TImplementation>(providerBuilder, BindingTarget.Method, this);
            this.bindingConfigurationBuilder = bindingConfigurationBuilder;
            return bindingConfigurationBuilder;
        }

        /// <summary>
        /// Indicates that the service should be bound to an instance of the specified provider type.
        /// The instance will be activated via the kernel when an instance of the service is activated.
        /// </summary>
        /// <typeparam name="TProvider">The type of provider to activate.</typeparam>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        public INewBindingWhenInWithOrOnInitializationSyntax<T> ToProvider<TProvider>()
            where TProvider : IProvider
        {
            var providerBuilder = new ProviderBuilderAdapter(new CallbackProvider<TProvider>(ctx => ctx.Kernel.Get<TProvider>()));
            var bindingConfigurationBuilder = new BindingConfigurationBuilder<T>(providerBuilder, BindingTarget.Provider, this);
            this.bindingConfigurationBuilder = bindingConfigurationBuilder;
            return bindingConfigurationBuilder;
        }

        /// <summary>
        /// Indicates that the service should be bound to an instance of the specified provider type.
        /// The instance will be activated via the kernel when an instance of the service is activated.
        /// </summary>
        /// <param name="providerType">The type of provider to activate.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        public INewBindingWhenInWithOrOnInitializationSyntax<T> ToProvider(Type providerType)
        {
            var providerBuilder = new ProviderBuilderAdapter(new CallbackProvider<IProvider>(ctx => ctx.Kernel.Get(providerType) as IProvider));
            var bindingConfigurationBuilder = new BindingConfigurationBuilder<T>(providerBuilder, BindingTarget.Provider, this);
            this.bindingConfigurationBuilder = bindingConfigurationBuilder;
            return bindingConfigurationBuilder;
        }

        /// <summary>
        /// Indicates that the service should be bound to the specified provider.
        /// </summary>
        /// <typeparam name="TImplementation">The type of the implementation.</typeparam>
        /// <param name="provider">The provider.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        public INewBindingWhenInWithOrOnInitializationSyntax<TImplementation> ToProvider<TImplementation>(IProvider<TImplementation> provider)
             where TImplementation : T
        {
            var providerBuilder = new ProviderBuilderAdapter(provider);
            var bindingConfigurationBuilder = new BindingConfigurationBuilder<TImplementation>(providerBuilder, BindingTarget.Provider, this);
            this.bindingConfigurationBuilder = bindingConfigurationBuilder;
            return bindingConfigurationBuilder;
        }

        /// <summary>
        /// Indicates that the service should be self-bound.
        /// </summary>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        public INewBindingWhenInWithOrOnInitializationSyntax<T> ToSelf()
        {
            var providerBuilder = new StandardProviderFactory(typeof(T));
            var bindingConfigurationBuilder = new BindingConfigurationBuilder<T>(providerBuilder, BindingTarget.Self, this);
            this.bindingConfigurationBuilder = bindingConfigurationBuilder;
            return bindingConfigurationBuilder;
        }
    }
}