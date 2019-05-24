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
    using Ninject.Components;
    using Ninject.Planning.Bindings;
    using Ninject.Syntax;

    /// <summary>
    /// Provides a root for the fluent syntax associated with an <see cref="Binding"/>.
    /// </summary>
    /// <typeparam name="T">The service type.</typeparam>
    internal sealed class BindingBuilder<T> : BindingBuilder, IBindingToSyntax<T>
    {
        private readonly ComponentContainer components;
        private BindingConfigurationBuilder bindingConfigurationBuilder;

        /// <summary>
        /// Initializes a new instance of the <see cref="BindingBuilder{T}"/> class.
        /// </summary>
        /// <param name="components">The components.</param>
        public BindingBuilder(ComponentContainer components)
        {
            this.components = components;
        }

        /// <summary>
        /// Gets the binding configuration.
        /// </summary>
        /// <value>
        /// The binding configuration.
        /// </value>
        public IBindingConfiguration BindingConfiguration => throw new NotImplementedException();

        /// <summary>
        /// Builds the <see cref="Binding"/>.
        /// </summary>
        /// <returns>
        /// The <see cref="Binding"/>.
        /// </returns>
        public override Binding Build()
        {
            var service = typeof(T);
            var bindingConfiguration = this.bindingConfigurationBuilder.Build();

            return new Binding(service, bindingConfiguration);
        }

        /// <summary>
        /// Indicates that the service should be bound to the specified implementation type.
        /// </summary>
        /// <typeparam name="TImplementation">The implementation type.</typeparam>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        public IBindingWhenInNamedWithOrOnSyntax<TImplementation> To<TImplementation>()
            where TImplementation : T
        {
            var providerBuilder = new StandardProviderFactory(typeof(TImplementation), this.components);
            var bindingConfigurationBuilder = new BindingConfigurationBuilder<TImplementation>(this.components, providerBuilder, BindingTarget.Type);
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
        public IBindingWhenInNamedWithOrOnSyntax<T> To(Type implementation)
        {
            var providerBuilder = new StandardProviderFactory(implementation, this.components);
            var bindingConfigurationBuilder = new BindingConfigurationBuilder<T>(this.components, providerBuilder, BindingTarget.Type);
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
        public IBindingWhenInNamedWithOrOnSyntax<TImplementation> ToConstant<TImplementation>(TImplementation value)
            where TImplementation : T
        {
            var providerBuilder = new ConstantProviderFactory<TImplementation>(value);
            var bindingConfigurationBuilder = new BindingConfigurationBuilder<TImplementation>(this.components, providerBuilder, BindingTarget.Constant);
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
        public IBindingWhenInNamedWithOrOnSyntax<TImplementation> ToConstructor<TImplementation>(Expression<Func<IConstructorArgumentSyntax, TImplementation>> newExpression)
            where TImplementation : T
        {
            var providerBuilder = new ConstructorProviderFactory<TImplementation>(newExpression, this.components);
            var bindingConfigurationBuilder = new BindingConfigurationBuilder<TImplementation>(this.components, providerBuilder, BindingTarget.Type);
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
        public IBindingWhenInNamedWithOrOnSyntax<T> ToMethod(Func<IContext, T> method)
        {
            var providerBuilder = new ProviderBuilderAdapter(new CallbackProvider<T>(method));
            var bindingConfigurationBuilder = new BindingConfigurationBuilder<T>(this.components, providerBuilder, BindingTarget.Method);
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
        public IBindingWhenInNamedWithOrOnSyntax<TImplementation> ToMethod<TImplementation>(Func<IContext, TImplementation> method)
            where TImplementation : T
        {
            var providerBuilder = new ProviderBuilderAdapter(new CallbackProvider<TImplementation>(method));
            var bindingConfigurationBuilder = new BindingConfigurationBuilder<TImplementation>(this.components, providerBuilder, BindingTarget.Method);
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
        public IBindingWhenInNamedWithOrOnSyntax<T> ToProvider<TProvider>()
            where TProvider : IProvider
        {
            var providerBuilder = new ProviderBuilderAdapter(new CallbackProvider<TProvider>(ctx => ctx.Kernel.Get<TProvider>()));
            var bindingConfigurationBuilder = new BindingConfigurationBuilder<T>(this.components, providerBuilder, BindingTarget.Provider);
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
        public IBindingWhenInNamedWithOrOnSyntax<T> ToProvider(Type providerType)
        {
            var providerBuilder = new ProviderBuilderAdapter(new CallbackProvider<IProvider>(ctx => ctx.Kernel.Get(providerType) as IProvider));
            var bindingConfigurationBuilder = new BindingConfigurationBuilder<T>(this.components, providerBuilder, BindingTarget.Provider);
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
        public IBindingWhenInNamedWithOrOnSyntax<TImplementation> ToProvider<TImplementation>(IProvider<TImplementation> provider)
            where TImplementation : T
        {
            var providerBuilder = new ProviderBuilderAdapter(provider);
            var bindingConfigurationBuilder = new BindingConfigurationBuilder<TImplementation>(this.components, providerBuilder, BindingTarget.Provider);
            this.bindingConfigurationBuilder = bindingConfigurationBuilder;
            return bindingConfigurationBuilder;
        }

        /// <summary>
        /// Indicates that the service should be self-bound.
        /// </summary>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        public IBindingWhenInNamedWithOrOnSyntax<T> ToSelf()
        {
            var providerBuilder = new StandardProviderFactory(typeof(T), this.components);
            var bindingConfigurationBuilder = new BindingConfigurationBuilder<T>(this.components, providerBuilder, BindingTarget.Self);
            this.bindingConfigurationBuilder = bindingConfigurationBuilder;
            return bindingConfigurationBuilder;
        }
    }
}