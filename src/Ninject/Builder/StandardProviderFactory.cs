// -------------------------------------------------------------------------------------------------
// <copyright file="StandardProviderFactory.cs" company="Ninject Project Contributors">
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

namespace Ninject.Builder
{
    using System;
    using System.Collections.Generic;

    using Ninject.Activation;
    using Ninject.Activation.Providers;
    using Ninject.Components;
    using Ninject.Parameters;
    using Ninject.Planning;
    using Ninject.Selection;

    /// <summary>
    /// Factory for a creating a built-in <see cref="IProvider"/>.
    /// </summary>
    internal sealed class StandardProviderFactory : IProviderFactory
    {
        private readonly Type implementation;
        private readonly ComponentContainer components;

        /// <summary>
        /// Initializes a new instance of the <see cref="StandardProviderFactory"/> class.
        /// </summary>
        /// <param name="implementation">The type of instances the factory creates <see cref="IProvider"/> instances for.</param>
        /// <param name="components">The components.</param>
        internal StandardProviderFactory(Type implementation, ComponentContainer components)
        {
            this.implementation = implementation;
            this.components = components;
        }

        /// <summary>
        /// Creates an appropiate <see cref="IProvider"/> taking into account the configured <see cref="IConstructorInjectionSelector"/>
        /// and the parameters.
        /// </summary>
        /// <param name="parameters">The parameters of the binding.</param>
        /// <returns>
        /// An <see cref="IProvider"/>.
        /// </returns>
        public IProvider Create(IReadOnlyList<IParameter> parameters)
        {
            var constructorSelector = this.components.Get<IConstructorInjectionSelector>();
            var pipeline = this.components.Get<IPipeline>();

            if (!this.components.TryGet<IPropertyReflectionSelector>(out _))
            {
                EnsureNoPropertyValuesAreConfigured(parameters);
            }

            var planner = this.components.Get<IPlanner>();
            var plan = planner.GetPlan(this.implementation);

            /* TODO: FAIL if method argument, but method injection is not active */

            if (constructorSelector is DefaultConstructorInjectionSelector || constructorSelector is UniqueConstructorInjectionSelector)
            {
                var constructor = constructorSelector.Select(plan, null);
                if (constructor.Targets.Length == 0)
                {
                    foreach (var parameter in parameters)
                    {
                        if (parameter is IConstructorArgument)
                        {
                            throw new Exception("Cannot define constructor arguments for a parameterless constructor.");
                        }
                    }

                    return new ParameterlessConstructorProvider(this.implementation, constructor, plan, pipeline);
                }

                return new FixedConstructorProvider(
                    this.implementation,
                    constructor,
                    plan,
                    pipeline,
                    this.components.Get<IConstructorParameterValueProvider>());
            }

            return new ContextAwareConstructorProvider(plan, constructorSelector, pipeline, this.components.Get<IConstructorParameterValueProvider>());
        }

        private static void EnsureNoPropertyValuesAreConfigured(IReadOnlyList<IParameter> parameters)
        {
            foreach (var parameter in parameters)
            {
                if (parameter is IPropertyValue)
                {
                    throw new Exception("Cannot define property values when property injection is not enabled.");
                }
            }
        }
    }
}