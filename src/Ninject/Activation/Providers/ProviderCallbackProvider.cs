// -------------------------------------------------------------------------------------------------
// <copyright file="CallbackProvider.cs" company="Ninject Project Contributors">
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

namespace Ninject.Activation.Providers
{
    using System;

    using Ninject.Infrastructure;

    /// <summary>
    /// A provider that delegates to a callback to obtain the provider that is used to create instances.
    /// </summary>
    /// <typeparam name="T">The type of instances the provider creates.</typeparam>
    internal class ProviderCallbackProvider<T> : Provider<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProviderCallbackProvider{T}"/> class.
        /// </summary>
        /// <param name="providerCallback">The callback that will be called to create the provider.</param>
        /// <exception cref="ArgumentNullException"><paramref name="providerCallback"/> is <see langword="null"/>.</exception>
        public ProviderCallbackProvider(Func<IContext, IProvider> providerCallback)
        {
            Ensure.ArgumentNotNull(providerCallback, nameof(providerCallback));

            this.ProviderCallback = providerCallback;
        }

        /// <summary>
        /// Gets a value indicating whether the provider uses Ninject to resolve services when creating an instance.
        /// </summary>
        /// <value>
        /// <see langword="true"/> if the provider uses Ninject to resolve service when creating an instance; otherwise,
        /// <see langword="false"/>.
        /// </value>
        public override bool ResolvesServices => true;

        /// <summary>
        /// Gets the callback method used by the provider.
        /// </summary>
        public Func<IContext, IProvider> ProviderCallback { get; }

        /// <summary>
        /// Invokes the callback method to create an instance.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="isInitialized"><see langword="true"/> if the created instance is fully initialized; otherwise, <see langword="false"/></param>
        /// <returns>
        /// The created instance.
        /// </returns>
        protected override T CreateInstance(IContext context, out bool isInitialized)
        {
            var provider = this.ProviderCallback(context);
            if (provider == null)
            {
                throw new ActivationException("TODO");
            }

            var instance = provider.Create(context, out isInitialized);
            if (instance != null)
            {
                if (!(instance is T))
                {
                    throw new ActivationException("TODO");
                }
            }

            return (T) instance;
        }
    }
}