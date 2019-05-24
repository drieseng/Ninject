// -------------------------------------------------------------------------------------------------
// <copyright file="BindingsBuilder.cs" company="Ninject Project Contributors">
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

    using Ninject.Planning.Bindings;
    using Ninject.Syntax;

    /// <summary>
    /// Configures bindings for an <see cref="IKernelBuilder"/>.
    /// </summary>
    internal sealed class BindingsBuilder : IBindingsBuilder
    {
        private readonly List<BindingBuilder> bindingBuilders;
        private readonly KernelBuilder kernelBuilder;

        /// <summary>
        /// Initializes a new instance of the <see cref="BindingsBuilder"/> class.
        /// </summary>
        /// <param name="kernelBuilder">The kernel builder.</param>
        public BindingsBuilder(KernelBuilder kernelBuilder)
        {
            this.bindingBuilders = new List<BindingBuilder>();
            this.kernelBuilder = kernelBuilder;
        }

        /// <summary>
        /// Declares a binding for the specified service.
        /// </summary>
        /// <typeparam name="T">The service to bind.</typeparam>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        public IBindingToSyntax<T> Bind<T>()
        {
            var bindingBuilder = new BindingBuilder<T>(this.kernelBuilder.Components);
            this.bindingBuilders.Add(bindingBuilder);
            return bindingBuilder;
        }

        /// <summary>
        /// Declares a binding for the specified service.
        /// </summary>
        /// <typeparam name="T1">The first service to bind.</typeparam>
        /// <typeparam name="T2">The second service to bind.</typeparam>
        /// <returns>The fluent syntax.</returns>
        public IBindingToSyntax<T1, T2> Bind<T1, T2>()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Declares a binding for the specified service.
        /// </summary>
        /// <typeparam name="T1">The first service to bind.</typeparam>
        /// <typeparam name="T2">The second service to bind.</typeparam>
        /// <typeparam name="T3">The third service to bind.</typeparam>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        public IBindingToSyntax<T1, T2, T3> Bind<T1, T2, T3>()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Declares a binding for the specified service.
        /// </summary>
        /// <typeparam name="T1">The first service to bind.</typeparam>
        /// <typeparam name="T2">The second service to bind.</typeparam>
        /// <typeparam name="T3">The third service to bind.</typeparam>
        /// <typeparam name="T4">The fourth service to bind.</typeparam>
        /// <returns>The fluent syntax.</returns>
        public IBindingToSyntax<T1, T2, T3, T4> Bind<T1, T2, T3, T4>()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Declares a binding from the service to itself.
        /// </summary>
        /// <param name="services">The services to bind.</param>
        /// <returns>The fluent syntax.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="services"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="services"/> contains zero items.</exception>
        public IBindingToSyntax<object> Bind(params Type[] services)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Builds the bindings.
        /// </summary>
        /// <returns>
        /// The bindings.
        /// </returns>
        public Dictionary<Type, ICollection<IBinding>> Build()
        {
            Dictionary<Type, ICollection<IBinding>> bindingsByType = new Dictionary<Type, ICollection<IBinding>>();

            foreach (var bindingBuilder in this.bindingBuilders)
            {
                var binding = bindingBuilder.Build();

                if (!bindingsByType.TryGetValue(binding.Service, out var bindingsForType))
                {
                    bindingsForType = new List<IBinding>();
                    bindingsByType.Add(binding.Service, bindingsForType);
                }

                bindingsForType.Add(binding);
            }

            return bindingsByType;
        }
    }
}