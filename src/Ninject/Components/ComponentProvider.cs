// -------------------------------------------------------------------------------------------------
// <copyright file="ComponentProvider.cs" company="Ninject Project Contributors">
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

namespace Ninject.Components
{
    using System;

    using Ninject.Activation;
    using Ninject.Activation.Providers;
    using Ninject.Planning;
    using Ninject.Selection;

    /// <summary>
    /// Creates an instance of a <see cref="INinjectComponent"/>.
    /// </summary>
    internal class ComponentProvider : StandardProviderBase
    {
        private readonly IConstructorInjectionSelector constructorSelector;
        private readonly IConstructorParameterValueProvider constructorParameterValueProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="ComponentProvider"/> class.
        /// </summary>
        /// <param name="plan">The plan.</param>
        /// <param name="constructorSelector">The constructor selector.</param>
        /// <param name="pipeline">The pipeline.</param>
        /// <param name="constructorParameterValueProvider">The value provider.</param>
        public ComponentProvider(IPlan plan,
                                 IConstructorInjectionSelector constructorSelector,
                                 IPipeline pipeline,
                                 IConstructorParameterValueProvider constructorParameterValueProvider)
            : base(plan, pipeline)
        {
            this.constructorSelector = constructorSelector;
            this.constructorParameterValueProvider = constructorParameterValueProvider;
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
        /// Gets the type of instances the provider creates.
        /// </summary>
        /// <value>
        /// The type of instances the provider creates.
        /// </value>
        public override Type Type => this.Plan.Type;

        /// <summary>
        /// Creates an instance of the <see cref="INinjectComponent"/>.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="isInitialized"><see langword="true"/> if the created instance is fully initialized; otherwise, <see langword="false"/></param>
        /// <returns>
        /// An instance of the <see cref="INinjectComponent"/>.
        /// </returns>
        protected override object CreateInstance(IContext context, out bool isInitialized)
        {
            isInitialized = false;

            var constructor = this.constructorSelector.Select(this.Plan, context);
            var values = this.constructorParameterValueProvider.GetValues(constructor, context);
            return constructor.Injector(values);
        }
    }
}