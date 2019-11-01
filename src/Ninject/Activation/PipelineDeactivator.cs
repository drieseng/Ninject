// -------------------------------------------------------------------------------------------------
// <copyright file="PipelineDeactivator.cs" company="Ninject Project Contributors">
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
    /// Deactivates instances in a given context.
    /// </summary>
    internal sealed class PipelineDeactivator : NinjectComponent, IPipelineDeactivator
    {
        private readonly List<IDeactivationStrategy> deactivationStrategies;
        private readonly IActivationCache activationCache;

        /// <summary>
        /// Initializes a new instance of the <see cref="PipelineDeactivator"/> class.
        /// </summary>
        /// <param name="deactivationStrategies">The strategies to execute upon deactivation.</param>
        /// <param name="activationCache">The activation cache.</param>
        public PipelineDeactivator(List<IDeactivationStrategy> deactivationStrategies, IActivationCache activationCache)
        {
            this.deactivationStrategies = deactivationStrategies;
            this.activationCache = activationCache;
        }

        /// <summary>
        /// Deactivates the instance in the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="reference">The instance reference.</param>
        public void Deactivate(IContext context, InstanceReference reference)
        {
            if (!activationCache.IsDeactivated(reference.Instance))
            {
                this.deactivationStrategies.ForEach(a => a.Deactivate(context, reference));
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
                deactivationStrategies?.Clear();
            }

            base.Dispose(disposing);
        }
    }
}