﻿// -------------------------------------------------------------------------------------------------
// <copyright file="ActivationCacheStrategy.cs" company="Ninject Project Contributors">
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

namespace Ninject.Activation.Strategies
{
    using System;

    using Ninject.Activation.Caching;
    using Ninject.Components;
    using Ninject.Infrastructure;

    /// <summary>
    /// Adds all activated instances to the activation cache.
    /// </summary>
    internal class DeactivationCacheStrategy : NinjectComponent, IDeactivationStrategy
    {
        /// <summary>
        /// The activation cache.
        /// </summary>
        private readonly IActivationCache activationCache;

        /// <summary>
        /// Initializes a new instance of the <see cref="DeactivationCacheStrategy"/> class.
        /// </summary>
        /// <param name="activationCache">The activation cache.</param>
        /// <exception cref="ArgumentNullException"><paramref name="activationCache"/> is <see langword="null"/>.</exception>
        public DeactivationCacheStrategy(IActivationCache activationCache)
        {
            Ensure.ArgumentNotNull(activationCache, nameof(activationCache));

            this.activationCache = activationCache;
        }

        /// <summary>
        /// Registers an instance in the <see cref="IActivationCache"/> as being deactivated.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="reference">A reference to the instance being deactivated.</param>
        public void Deactivate(IContext context, InstanceReference reference)
        {
            this.activationCache.AddDeactivatedInstance(reference.Instance);
        }
    }
}