// -------------------------------------------------------------------------------------------------
// <copyright file="BindingRoot.cs" company="Ninject Project Contributors">
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

using System;
using System.Collections.Generic;
using Ninject.Builder;
using Ninject.Builder.Bindings;
using Ninject.Infrastructure;
using Ninject.Infrastructure.Disposal;

namespace Ninject.Syntax
{
    /// <summary>
    /// Provides a path to register bindings.
    /// </summary>
    public sealed class NewBindingRoot : INewBindingRoot
    {
        private readonly List<INewBindingBuilder> bindingBuilders;

        public NewBindingRoot()
        {
            this.bindingBuilders = new List<INewBindingBuilder>();
        }

        /// <summary>
        /// Returns a value indicating whether a binding exists for the specified service.
        /// </summary>
        /// <typeparam name="T">The service to check.</typeparam>
        /// <returns>
        /// <see langword="true"/> if a binding exists for <typeparamref name="T"/>; otherwise,
        /// <see langword="false"/>.
        /// </returns>
        public bool IsBound<T>()
        {
            for (var i = 0; i < this.bindingBuilders.Count; i++)
            {
                if (this.bindingBuilders[i].Service == typeof(T))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Declares a binding for the specified service.
        /// </summary>
        /// <typeparam name="T">The service to bind.</typeparam>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        public INewBindingToSyntax<T> Bind<T>()
        {
            var bindingBuilder = new BindingBuilder<T>();
            this.AddBinding(bindingBuilder);
            return bindingBuilder;
        }

        /// <summary>
        /// Declares a binding for the specified service.
        /// </summary>
        /// <typeparam name="T1">The first service to bind.</typeparam>
        /// <typeparam name="T2">The second service to bind.</typeparam>
        /// <returns>The fluent syntax.</returns>
        public INewBindingToSyntax<T1, T2> Bind<T1, T2>()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Declares a binding for the specified service.
        /// </summary>
        /// <typeparam name="T1">The first service to bind.</typeparam>
        /// <typeparam name="T2">The second service to bind.</typeparam>
        /// <typeparam name="T3">The third service to bind.</typeparam>
        /// <returns>The fluent syntax.</returns>
        public INewBindingToSyntax<T1, T2, T3> Bind<T1, T2, T3>()
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
        public INewBindingToSyntax<T1, T2, T3, T4> Bind<T1, T2, T3, T4>()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Declares a binding for the specified service.
        /// </summary>
        /// <param name="services">The services to bind.</param>
        /// <returns>The fluent syntax.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="services"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="services"/> contains zero types to bind.</exception>
        public INewBindingToSyntax<object> Bind(params Type[] services)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Unregisters all bindings for the specified service.
        /// </summary>
        /// <typeparam name="T">The service to unbind.</typeparam>
        public void Unbind<T>()
        {
            this.Unbind(typeof(T));
        }

        /// <summary>
        /// Unregisters all bindings for the specified service.
        /// </summary>
        /// <param name="service">The service to unbind.</param>
        public void Unbind(Type service)
        {
            for (var i = (this.bindingBuilders.Count - 1); i >= 0; i--)
            {
                if (this.bindingBuilders[i].Service == service)
                {
                    this.bindingBuilders.RemoveAt(i);
                }
            }
        }

        /// <summary>
        /// Removes any existing bindings for the specified service, and declares a new one.
        /// </summary>
        /// <typeparam name="T1">The first service to re-bind.</typeparam>
        /// <returns>The fluent syntax.</returns>
        public INewBindingToSyntax<T1> Rebind<T1>()
        {
            this.Unbind<T1>();
            return this.Bind<T1>();
        }

        /// <summary>
        /// Removes any existing bindings for the specified services, and declares a new one.
        /// </summary>
        /// <typeparam name="T1">The first service to re-bind.</typeparam>
        /// <typeparam name="T2">The second service to re-bind.</typeparam>
        /// <returns>The fluent syntax.</returns>
        public INewBindingToSyntax<T1, T2> Rebind<T1, T2>()
        {
            this.Unbind<T1>();
            this.Unbind<T2>();
            return this.Bind<T1, T2>();
        }

        /// <summary>
        /// Removes any existing bindings for the specified services, and declares a new one.
        /// </summary>
        /// <typeparam name="T1">The first service to re-bind.</typeparam>
        /// <typeparam name="T2">The second service to re-bind.</typeparam>
        /// <typeparam name="T3">The third service to re-bind.</typeparam>
        /// <returns>The fluent syntax.</returns>
        public INewBindingToSyntax<T1, T2, T3> Rebind<T1, T2, T3>()
        {
            this.Unbind<T1>();
            this.Unbind<T2>();
            this.Unbind<T3>();
            return this.Bind<T1, T2, T3>();
        }

        /// <summary>
        /// Removes any existing bindings for the specified services, and declares a new one.
        /// </summary>
        /// <typeparam name="T1">The first service to re-bind.</typeparam>
        /// <typeparam name="T2">The second service to re-bind.</typeparam>
        /// <typeparam name="T3">The third service to re-bind.</typeparam>
        /// <typeparam name="T4">The fourth service to re-bind.</typeparam>
        /// <returns>The fluent syntax.</returns>
        public INewBindingToSyntax<T1, T2, T3, T4> Rebind<T1, T2, T3, T4>()
        {
            this.Unbind<T1>();
            this.Unbind<T2>();
            this.Unbind<T3>();
            this.Unbind<T4>();
            return this.Bind<T1, T2, T3, T4>();
        }

        /// <summary>
        /// Removes any existing bindings for the specified service, and declares a new one.
        /// </summary>
        /// <param name="services">The services to re-bind.</param>
        /// <returns>The fluent syntax.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="services"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="services"/> contains zero items.</exception>
        public INewBindingToSyntax<object> Rebind(params Type[] services)
        {
            Ensure.ArgumentNotNull(services, nameof(services));

            foreach (var service in services)
            {
                this.Unbind(service);
            }

            return this.Bind(services);
        }

        /// <summary>
        /// Registers the specified binding.
        /// </summary>
        /// <param name="binding">The binding to add.</param>
        private void AddBinding(INewBindingBuilder binding)
        {
            this.bindingBuilders.Add(binding);
        }

        /// <summary>
        /// Builds the bindings.
        /// </summary>
        /// <param name="root">The resolution root.</param>
        /// <param name="bindingVisitor">Gathers built bindings.</param>
        internal void Build(IResolutionRoot root, IVisitor<Planning.Bindings.IBinding> bindingVisitor)
        {
            foreach (var bindingBuilder in this.bindingBuilders)
            {
                bindingBuilder.Build(root, bindingVisitor);
            }
        }
    }
}