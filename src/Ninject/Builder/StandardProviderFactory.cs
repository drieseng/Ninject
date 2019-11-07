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
    using Ninject.Parameters;
    using Ninject.Planning;
    using Ninject.Selection;
    using Ninject.Syntax;

    /// <summary>
    /// Factory for a creating a built-in <see cref="IProvider"/>.
    /// </summary>
    internal sealed class StandardProviderFactory : IProviderFactory
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StandardProviderFactory"/> class.
        /// </summary>
        /// <param name="implementation">The type of instances the factory creates <see cref="IProvider"/> instances for.</param>
        public StandardProviderFactory(Type implementation)
        {
            this.Implementation = implementation;
        }

        public Type Implementation { get; }

        /// <summary>
        /// Creates an appropiate <see cref="IProvider"/> taking into account the configured <see cref="IConstructorInjectionSelector"/>
        /// and the parameters.
        /// </summary>
        /// <param name="root">The resolution root.</param>
        /// <param name="parameters">The parameters of the binding.</param>
        /// <returns>
        /// An <see cref="IProvider"/>.
        /// </returns>
        public IProvider Create(IResolutionRoot root, IReadOnlyList<IParameter> parameters)
        {
            var constructorSelector = root.Get<IConstructorInjectionSelector>();
            var pipeline = root.Get<IPipeline>();

            var planner = root.Get<IPlanner>();
            var plan = planner.GetPlan(this.Implementation);

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

                    return new ParameterlessConstructorProvider(this.Implementation, constructor, plan, pipeline);
                }

                return new FixedConstructorProvider(this.Implementation,
                                                    constructor,
                                                    plan,
                                                    pipeline,
                                                    root.Get<IConstructorParameterValueProvider>());
            }

            return new ContextAwareConstructorProvider(plan, constructorSelector, pipeline, root.Get<IConstructorParameterValueProvider>());
        }
    }
}