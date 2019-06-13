// -------------------------------------------------------------------------------------------------
// <copyright file="ComponentBindingBuilder{T}.cs" company="Ninject Project Contributors">
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

    using Ninject.Activation;
    using Ninject.Activation.Providers;
    using Ninject.Infrastructure;
    using Ninject.Planning.Bindings;
    using Ninject.Syntax;

    /// <summary>
    /// Provides a root for the fluent syntax associated with an <see cref="Binding"/>.
    /// </summary>
    /// <typeparam name="T">The service type.</typeparam>
    internal class ComponentBindingBuilder<T> : ComponentBindingBuilder, IComponentBindingToSyntax<T>
    {
        private BindingConfigurationBuilder bindingConfigurationBuilder;

        /// <summary>
        /// Initializes a new instance of the <see cref="ComponentBindingBuilder{T}"/> class.
        /// </summary>
        public ComponentBindingBuilder()
            : base(typeof(T))
        {
        }

        /// <summary>
        /// Builds the binding.
        /// </summary>
        /// <param name="root">The resolution root.</param>
        /// <param name="bindingVisitor">Gathers built bindings.</param>
        public override void Build(IResolutionRoot root, IVisitor<IBinding> bindingVisitor)
        {
            bindingVisitor.Visit(new Binding(this.Service, this.bindingConfigurationBuilder.Build(root)));
        }

        /// <summary>
        /// Indicates that the service should be bound to the specified implementation type.
        /// </summary>
        /// <typeparam name="TImplementation">The implementation type.</typeparam>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        public IComponentBindingInScopeSyntax<TImplementation> To<TImplementation>()
            where TImplementation : T
        {
            var providerBuilder = new StandardProviderFactory(typeof(TImplementation));
            var bindingConfigurationBuilder = new BindingConfigurationBuilder<TImplementation>(providerBuilder, BindingTarget.Type);
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
        public IComponentBindingInScopeSyntax<T> To(Type implementation)
        {
            var providerBuilder = new StandardProviderFactory(implementation);
            var bindingConfigurationBuilder = new BindingConfigurationBuilder<T>(providerBuilder, BindingTarget.Type);
            this.bindingConfigurationBuilder = bindingConfigurationBuilder;
            return bindingConfigurationBuilder;
        }

        /// <summary>
        /// Indicates that the service should be bound to the specified callback method.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        public IComponentBindingInScopeSyntax<T> ToMethod(Func<IContext, T> method)
        {
            var providerBuilder = new ProviderBuilderAdapter(new CallbackProvider<T>(method));
            var bindingConfigurationBuilder = new BindingConfigurationBuilder<T>(providerBuilder, BindingTarget.Method);
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
        public IComponentBindingWithOrOnActivationSyntax<TImplementation> ToConstant<TImplementation>(TImplementation value)
            where TImplementation : T
        {
            var providerBuilder = new ConstantProviderFactory<TImplementation>(value);
            var bindingConfigurationBuilder = new BindingConfigurationBuilder<TImplementation>(providerBuilder, BindingTarget.Constant);
            this.bindingConfigurationBuilder = bindingConfigurationBuilder;
            return bindingConfigurationBuilder;
        }

        /// <summary>
        /// Indicates that the service should be self-bound.
        /// </summary>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        public IComponentBindingInScopeSyntax<T> ToSelf()
        {
            var providerBuilder = new StandardProviderFactory(typeof(T));
            var bindingConfigurationBuilder = new BindingConfigurationBuilder<T>(providerBuilder, BindingTarget.Type);
            this.bindingConfigurationBuilder = bindingConfigurationBuilder;
            return bindingConfigurationBuilder;
        }
    }
}