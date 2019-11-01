// -------------------------------------------------------------------------------------------------
// <copyright file="PipelineActivator.cs" company="Ninject Project Contributors">
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
    using System.Collections.Generic;
    using Ninject.Activation.Caching;
    using Ninject.Activation.Strategies;
    using Ninject.Components;

    /// <summary>
    /// Activates instances in a given context.
    /// </summary>
    internal sealed class PipelineActivator : NinjectComponent, IPipelineActivator
    {
        private readonly List<IActivationStrategy> activationStrategies;
        private readonly IActivationCache activationCache;

        /// <summary>
        /// Initializes a new instance of the <see cref="PipelineActivator"/> class.
        /// </summary>
        /// <param name="activationStrategies">The strategies to execute upon activation.</param>
        /// <param name="activationCache">The activation cache.</param>
        public PipelineActivator(List<IActivationStrategy> activationStrategies, IActivationCache activationCache)
        {
            this.activationStrategies = activationStrategies;
            this.activationCache = activationCache;
        }

        /// <summary>
        /// Activates the instance in the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="reference">The instance reference.</param>
        public void Activate(IContext context, InstanceReference reference)
        {
            if (!activationCache.IsActivated(reference.Instance))
            {
                this.activationStrategies.ForEach(a => a.Activate(context, reference));
            }
        }

        /// <summary>
        /// Releases resources held by the object.
        /// </summary>
        /// <param name="disposing"><see langword="true"/> if called manually, otherwise by GC.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.activationStrategies.Clear();
            }

            base.Dispose(disposing);
        }
    }
}