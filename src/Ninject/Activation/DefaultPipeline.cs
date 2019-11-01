// -------------------------------------------------------------------------------------------------
// <copyright file="DefaultPipeline.cs" company="Ninject Project Contributors">
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
    using Ninject.Components;
    using System;

    /// <summary>
    /// The default pipeline.
    /// </summary>
    internal sealed class DefaultPipeline : NinjectComponent, IPipeline
    {
        private readonly IPipelineInitializer initializer;
        private readonly IPipelineActivator activator;
        private readonly IPipelineDeactivator deactivator;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultPipeline"/> class.
        /// </summary>
        /// <param name="initializer">The pipeline initialzer.</param>
        /// <param name="activator">The pipeline activator.</param>
        /// <param name="deactivator">The pipeline deactivator.</param>
        internal DefaultPipeline(IPipelineInitializer initializer, IPipelineActivator activator, IPipelineDeactivator deactivator)
        {
            this.initializer = initializer;
            this.activator = activator;
            this.deactivator = deactivator;
        }

        /// <summary>
        /// Initializes an instance using the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="instance">The instance being initialized.</param>
        /// <returns>
        /// The initialized instance.
        /// </returns>
        public object Initialize(IContext context, object instance)
        {
            return this.initializer.Initialize(context, instance);
        }

        /// <summary>
        /// Activates the instance in the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="reference">The instance reference.</param>
        public void Activate(IContext context, InstanceReference reference)
        {
            this.activator.Activate(context, reference);
        }

        /// <summary>
        /// Deactivates the instance in the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="reference">The instance reference.</param>
        public void Deactivate(IContext context, InstanceReference reference)
        {
            this.deactivator.Deactivate(context, reference);
        }

        /// <summary>
        /// Releases resources held by the object.
        /// </summary>
        /// <param name="disposing"><see langword="true"/> if called manually, otherwise by GC.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                initializer?.Dispose();
                activator?.Dispose();
                deactivator?.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}