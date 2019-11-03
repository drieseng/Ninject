// -------------------------------------------------------------------------------------------------
// <copyright file="Provider.cs" company="Ninject Project Contributors">
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

namespace Ninject.Activation
{
    using System;

    using Ninject.Infrastructure;

    /// <summary>
    /// A simple abstract provider for instances of a specific type.
    /// </summary>
    /// <typeparam name="T">The type of instances the provider creates.</typeparam>
    public abstract class Provider<T> : IProvider<T>
    {
        /// <summary>
        /// Gets a value indicating whether the provider uses Ninject to resolve services when creating an instance.
        /// </summary>
        /// <value>
        /// <see langword="true"/> if the provider uses Ninject to resolve service when creating an instance; otherwise,
        /// <see langword="false"/>.
        /// </value>
        public abstract bool ResolvesServices {get;}

        /// <summary>
        /// Gets the type of instances the provider creates.
        /// </summary>
        /// <value>
        /// The type of instances the provider creates.
        /// </value>
        public virtual Type Type
        {
            get { return typeof(T); }
        }

        /// <summary>
        /// Creates an instance within the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="isInitialized"><see langword="true"/> if the created instance is fully initialized; otherwise, <see langword="false"/></param>
        /// <returns>
        /// The created instance.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="context"/> is <see langword="null"/>.</exception>
        public object Create(IContext context, out bool isInitialized)
        {
            Ensure.ArgumentNotNull(context, nameof(context));

            return this.CreateInstance(context, out isInitialized);
        }

        /// <summary>
        /// Creates an instance within the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="isInitialized"><see langword="true"/> if the created instance is fully initialized; otherwise, <see langword="false"/></param>
        /// <returns>
        /// The created instance.
        /// </returns>
        protected abstract T CreateInstance(IContext context, out bool isInitialized);
    }
}