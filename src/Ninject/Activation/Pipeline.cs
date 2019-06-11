// -------------------------------------------------------------------------------------------------
// <copyright file="Pipeline.cs" company="Ninject Project Contributors">
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
    using System.Collections.Generic;
    using System.Linq;

    using Ninject.Activation.Caching;
    using Ninject.Activation.Strategies;
    using Ninject.Components;
    using Ninject.Infrastructure;

    /// <summary>
    /// Drives the activation (injection, etc.) of an instance.
    /// </summary>
    public class Pipeline : NinjectComponent, IPipeline
    {
        /// <summary>
        /// The activation cache.
        /// </summary>
        private readonly IActivationCache activationCache;

        /// <summary>
        /// The strategies that contribute to the activation and deactivation processes.
        /// </summary>
        private readonly List<IActivationStrategy> strategies;

        private readonly List<IDeactivationStrategy> deactivationStrategies;

        /// <summary>
        /// The strategies that contribute to the initialization process.
        /// </summary>
        private readonly List<IInitializationStrategy> initializationStrategies;

        /// <summary>
        /// Initializes a new instance of the <see cref="Pipeline"/> class.
        /// </summary>
        /// <param name="strategies">The strategies to execute during activation and deactivation.</param>
        /// <param name="initializationStrategies">The strategies to execute during initialization.</param>
        /// <param name="activationCache">The activation cache.</param>
        /// <exception cref="ArgumentNullException"><paramref name="activationStrategies"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="deactivationStrategies"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="initializationStrategies"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="activationCache"/> is <see langword="null"/>.</exception>
        public Pipeline(IEnumerable<IInitializationStrategy> initializationStrategies, IEnumerable<IActivationStrategy> activationStrategies, IEnumerable<IDeactivationStrategy> deactivationStrategies, IActivationCache activationCache)
        {
            Ensure.ArgumentNotNull(initializationStrategies, nameof(initializationStrategies));
            Ensure.ArgumentNotNull(activationStrategies, nameof(activationStrategies));
            Ensure.ArgumentNotNull(deactivationStrategies, nameof(deactivationStrategies));
            Ensure.ArgumentNotNull(activationCache, nameof(activationCache));

            this.initializationStrategies = initializationStrategies.ToList();
            this.strategies = activationStrategies.ToList();
            this.deactivationStrategies = deactivationStrategies.ToList();
            this.activationCache = activationCache;
        }

        /// <summary>
        /// Gets the strategies that contribute to the activation and deactivation processes.
        /// </summary>
        public IReadOnlyList<IActivationStrategy> Strategies
        {
            get { return this.strategies; }
        }

        /// <summary>
        /// Gets the strategies that contribute to the initialization process.
        /// </summary>
        public IReadOnlyList<IInitializationStrategy> InitializationStrategies
        {
            get { return this.initializationStrategies; }
        }

        /// <summary>
        /// Initializes an instance in the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="instance">The instance being initialized.</param>
        /// <exception cref="ArgumentNullException"><paramref name="context"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="instance"/> is <see langword="null"/>.</exception>
        public object Initialize(IContext context, object instance)
        {
            if (context == null)
            {
                Ensure.ThrowArgumentNotNull(nameof(context));
            }

            if (instance == null)
            {
                Ensure.ThrowArgumentNotNull(nameof(instance));
            }

            if (this.initializationStrategies.Count > 0)
            {
                var targetType = context.Request.Service;

                this.initializationStrategies.ForEach(s =>
                    {
                        instance = s.Initialize(context, instance);

                        // Protect against strategy returning null
                        if (instance == null)
                        {
                            throw new Exception("TODO");
                        }

                        // Protect against strategy returning an incompatible type
                        if (!targetType.IsAssignableFrom(instance.GetType()))
                        {
                            throw new Exception("TODO");
                        }

                        // TODO UPDATE PLAN!!
                    });
            }

            return instance;
        }

        /// <summary>
        /// Activates the instance in the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="reference">The instance reference.</param>
        /// <exception cref="ArgumentNullException"><paramref name="context"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="reference"/> is <see langword="null"/>.</exception>
        public void Activate(IContext context, InstanceReference reference)
        {
            Ensure.ArgumentNotNull(context, nameof(context));
            Ensure.ArgumentNotNull(reference, nameof(reference));

            if (!this.activationCache.IsActivated(reference.Instance))
            {
                Console.WriteLine("ACTIVATE " + this.strategies.Count);
                this.strategies.ForEach(s => s.Activate(context, reference));
            }
        }

        /// <summary>
        /// Deactivates the instance in the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="reference">The instance reference.</param>
        /// <exception cref="ArgumentNullException"><paramref name="context"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="reference"/> is <see langword="null"/>.</exception>
        public void Deactivate(IContext context, InstanceReference reference)
        {
            Ensure.ArgumentNotNull(context, nameof(context));
            Ensure.ArgumentNotNull(reference, nameof(reference));

            if (!this.activationCache.IsDeactivated(reference.Instance))
            {
                this.deactivationStrategies.ForEach(s => s.Deactivate(context, reference));
            }
        }
    }
}