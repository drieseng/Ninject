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
    using Ninject.Planning;

    /// <summary>
    /// Creates instances of services.
    /// </summary>
    public abstract class Provider : IProvider
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Provider"/> class.
        /// </summary>
        /// <param name="type">The type (or prototype) of instances the provider creates.</param>
        /// <param name="plan">The <see cref="IPlan"/> component.</param>
        /// <param name="pipeline">The <see cref="IPipeline"/> component.</param>
        protected Provider(Type type, IPlan plan, IPipeline pipeline)
        {
            this.Type = type;
            this.Plan = plan;
            this.Pipeline = pipeline;
        }

        /// <summary>
        /// Gets the type (or prototype) of instances the provider creates.
        /// </summary>
        public Type Type { get; }

        /// <summary>
        /// Gets the <see cref="IPlan"/> for <see cref="Type"/>.
        /// </summary>
        public IPlan Plan { get; }

        /// <summary>
        /// Gets the <see cref="IPipeline"/> component.
        /// </summary>
        public IPipeline Pipeline { get; }

        /// <summary>
        /// Gets a value indicating whether the provider uses Ninject to resolve services when creating an instance.
        /// </summary>
        /// <value>
        /// <see langword="true"/> if the provider uses Ninject to resolve service when creating an instance; otherwise,
        /// <see langword="false"/>.
        /// </value>
        public abstract bool ResolvesServices { get; }

        /// <summary>
        /// Creates an instance within the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="isInitialized"><see langword="true"/> if the created instance is fully initialized; otherwise, <see langword="false"/></param>
        /// <returns>
        /// The created instance.
        /// </returns>
        public object Create(IContext context, out bool isInitialized)
        {
            Ensure.ArgumentNotNull(context, nameof(context));

            // Assign the initial plan for the implementation type that we'll be creating an instance for.
            context.Plan = this.Plan;

            // Create an instance of the implementation type.
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
        protected abstract object CreateInstance(IContext context, out bool isInitialized);
    }
}