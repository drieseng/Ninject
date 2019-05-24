// -------------------------------------------------------------------------------------------------
// <copyright file="StandardProviderBase.cs" company="Ninject Project Contributors">
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

    using Ninject.Planning;

    /// <summary>
    /// Provider base functionality for creating and initializing a service instance.
    /// </summary>
    internal abstract class StandardProviderBase : IProvider
    {
        private readonly IPipeline pipeline;

        /// <summary>
        /// Initializes a new instance of the <see cref="StandardProviderBase"/> class.
        /// </summary>
        /// <param name="plan">The plan.</param>
        /// <param name="pipeline">The pipeline.</param>
        protected StandardProviderBase(IPlan plan, IPipeline pipeline)
        {
            this.Plan = plan;
            this.pipeline = pipeline;
        }

        /// <summary>
        /// Gets the type of instances the provider creates.
        /// </summary>
        /// <value>
        /// The type of instances the provider creates.
        /// </value>
        public abstract Type Type { get; }

        /// <summary>
        /// Gets the activation plan.
        /// </summary>
        /// <value>
        /// The activation plan.
        /// </value>
        protected IPlan Plan { get; }

        /// <summary>
        /// Creates and initializes an instance within the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>
        /// The created and initialized instance.
        /// </returns>
        public object Create(IContext context)
        {
            // Assign the initial plan for the implementation type that we'll be creating an instance for.
            context.Plan = this.Plan;

            // Create an instance of the implementation type.
            var instance = this.CreateInstance(context);

            // Pass the instance through the initialization pipeline which may alter both the instance
            // and the plan in the context.
            //
            // Note that this will not be reflected in the Plan and Type of the standard provider.
            instance = this.pipeline.Initialize(context, instance);

            return instance;
        }

        /// <summary>
        /// Creates an instance within the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>
        /// The created instance.
        /// </returns>
        protected abstract object CreateInstance(IContext context);
    }
}