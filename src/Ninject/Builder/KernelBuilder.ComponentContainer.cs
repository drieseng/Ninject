// -------------------------------------------------------------------------------------------------
// <copyright file="KernelBuilder.ComponentContainer.cs" company="Ninject Project Contributors">
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
    using Ninject.Components;
    using System;
    using System.Collections.Generic;

    partial class KernelBuilder
    {
        internal IComponentContainer AsComponentContainer()
        {
            return new ComponentContainerAdapter(this);
        }

        private class ComponentContainerAdapter : IComponentContainer
        {
            private readonly KernelBuilder kernelBuilder;

            public ComponentContainerAdapter(KernelBuilder kernelBuilder)
            {
                this.kernelBuilder = kernelBuilder;
            }

            /// <summary>
            /// Registers a component in the container.
            /// </summary>
            /// <typeparam name="TComponent">The component type.</typeparam>
            /// <typeparam name="TImplementation">The component's implementation type.</typeparam>
            void IComponentContainer.Add<TComponent, TImplementation>()
            {
                this.kernelBuilder.Components.Bind<TComponent>().To<TImplementation>().InSingletonScope();
            }

            /// <summary>
            /// Removes all registrations for the specified component.
            /// </summary>
            /// <typeparam name="T">The component type.</typeparam>
            void IComponentContainer.RemoveAll<T>()
            {
                this.kernelBuilder.Components.Unbind<T>();
            }

            /// <summary>
            /// Removes all registrations for the specified component.
            /// </summary>
            /// <param name="component">The component's type.</param>
            void IComponentContainer.RemoveAll(Type component)
            {
                throw new NotImplementedException();
            }

            /// <summary>
            /// Removes the specified registration.
            /// </summary>
            /// <typeparam name="T">The component type.</typeparam>
            /// <typeparam name="TImplementation">The implementation type.</typeparam>
            void IComponentContainer.Remove<T, TImplementation>()
            {
                throw new NotImplementedException();
            }

            /// <summary>
            /// Gets one instance of the specified component.
            /// </summary>
            /// <typeparam name="T">The component type.</typeparam>
            /// <returns>The instance of the component.</returns>
            T IComponentContainer.Get<T>()
            {
                throw new NotImplementedException();
            }

            /// <summary>
            /// Gets all available instances of the specified component.
            /// </summary>
            /// <typeparam name="T">The component type.</typeparam>
            /// <returns>A series of instances of the specified component.</returns>
            IEnumerable<T> IComponentContainer.GetAll<T>()
            {
                throw new NotImplementedException();
            }

            /// <summary>
            /// Gets one instance of the specified component.
            /// </summary>
            /// <param name="component">The component type.</param>
            /// <returns>The instance of the component.</returns>
            object IComponentContainer.Get(Type component)
            {
                throw new NotImplementedException();
            }

            /// <summary>
            /// Gets all available instances of the specified component.
            /// </summary>
            /// <param name="component">The component type.</param>
            /// <returns>A series of instances of the specified component.</returns>
            IEnumerable<object> IComponentContainer.GetAll(Type component)
            {
                throw new NotImplementedException();
            }

            /// <summary>
            /// Registers a transient component in the container.
            /// </summary>
            /// <typeparam name="TComponent">The component type.</typeparam>
            /// <typeparam name="TImplementation">The component's implementation type.</typeparam>
            void IComponentContainer.AddTransient<TComponent, TImplementation>()
            {
                this.kernelBuilder.Components.Bind<TComponent>().To<TImplementation>();
            }

            void IDisposable.Dispose()
            {
            }
        }
    }
}
