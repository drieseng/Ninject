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

namespace Ninject.Builder.Components
{
    using System;
    using Ninject.Activation;
    using Ninject.Activation.Providers;
    using Ninject.Builder.Syntax;
    using Ninject.Infrastructure;
    using Ninject.Syntax;

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
        public override void Build(IResolutionRoot root, IVisitor<Planning.Bindings.IBinding> bindingVisitor)
        {
            bindingVisitor.Visit(new Planning.Bindings.Binding(this.Service, this.bindingConfigurationBuilder.Build(root)));
        }

        public IComponentBindingInScopeSyntax<TImplementation> To<TImplementation>()
            where TImplementation : T
        {
            var providerBuilder = new StandardProviderFactory(typeof(TImplementation));
            var bindingConfigurationBuilder = new BindingConfigurationBuilder<TImplementation>(providerBuilder, Planning.Bindings.BindingTarget.Type);
            this.bindingConfigurationBuilder = bindingConfigurationBuilder;
            return bindingConfigurationBuilder;
        }

        public IComponentBindingInScopeSyntax<T> To(Type implementation)
        {
            throw new NotImplementedException();
        }

        public IComponentBindingInScopeSyntax<TImplementation> ToMethod<TImplementation>(Func<IContext, TImplementation> method)
        {
            var providerBuilder = new ProviderBuilderAdapter(new CallbackProvider<TImplementation>(method));
            var bindingConfigurationBuilder = new BindingConfigurationBuilder<TImplementation>(providerBuilder, Planning.Bindings.BindingTarget.Method);
            this.bindingConfigurationBuilder = bindingConfigurationBuilder;
            return bindingConfigurationBuilder;
        }

        public IComponentBindingInScopeSyntax<TImplementation> ToConstant<TImplementation>(TImplementation value)
            where TImplementation : T
        {
            throw new NotImplementedException();
        }

        public IComponentBindingInScopeSyntax<T> ToSelf()
        {
            var providerBuilder = new StandardProviderFactory(typeof(T));
            var bindingConfigurationBuilder = new BindingConfigurationBuilder<T>(providerBuilder, Planning.Bindings.BindingTarget.Type);
            this.bindingConfigurationBuilder = bindingConfigurationBuilder;
            return bindingConfigurationBuilder;
        }
    }
}