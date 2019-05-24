// -------------------------------------------------------------------------------------------------
// <copyright file="KernelBuilder.cs" company="Ninject Project Contributors">
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
    using System.Linq;

    using Ninject.Activation;
    using Ninject.Activation.Caching;
    using Ninject.Components;
    using Ninject.Planning;
    using Ninject.Planning.Bindings;
    using Ninject.Planning.Bindings.Resolvers;
    using Ninject.Planning.Strategies;
    using Ninject.Selection;

    /// <summary>
    /// Provides the mechanisms to build a kernel.
    /// </summary>
    public sealed class KernelBuilder : IKernelBuilder
    {
        private readonly BindingsBuilder bindingsBuilder;
        private readonly PipelineFactory pipelineFactory;
        private readonly Dictionary<Type, ICollection<IBinding>> bindingsByType;

        /// <summary>
        /// Initializes a new instance of the <see cref="KernelBuilder"/> class.
        /// </summary>
        public KernelBuilder()
        {
            this.Components = new ComponentContainer();
            this.bindingsBuilder = new BindingsBuilder(this);
            this.pipelineFactory = new PipelineFactory(this.Components);
            this.bindingsByType = new Dictionary<Type, ICollection<IBinding>>();
        }

        /// <summary>
        /// Gets the components.
        /// </summary>
        /// <value>
        /// The components.
        /// </value>
        public ComponentContainer Components { get; }

        /// <summary>
        /// Adds bindings to the <see cref="IKernelBuilder"/>.
        /// </summary>
        /// <param name="configureBindings">A callback to configure bindings.</param>
        /// <returns>
        /// A reference to this instance after the operation has completed.
        /// </returns>
        public IKernelBuilder Bindings(Action<IBindingsBuilder> configureBindings)
        {
            configureBindings(this.bindingsBuilder);
            return this;
        }

        /// <summary>
        /// Adds modules to the <see cref="IKernelBuilder"/>.
        /// </summary>
        /// <param name="configureModules">A callback to configure modules.</param>
        /// <returns>
        /// A reference to this instance after the operation has completed.
        /// </returns>
        public IKernelBuilder Modules(Action<IModuleBuilder> configureModules)
        {
            return this;
        }

        /// <summary>
        /// Builds the kernel.
        /// </summary>
        /// <returns>
        /// An <see cref="IReadOnlyKernel"/>.
        /// </returns>
        public IReadOnlyKernel Build()
        {
            /*
                TODO:
                fail if neither constructor injection, nor method or property injection is configured
                if no constructorselector is configured, then use DefaultConstructorSelector
                fail if no IInjectorFactory is configured
                fail if no bindings are configured, and neither SelfBinding nor DefaultValueBinding are configured?
            */

            if (!this.Components.TryGet<IPlanner>(out _))
            {
                this.Components.Add<IPlanner, Planner>();
            }

            if (!this.Components.TryGet<IActivationCache>(out _))
            {
                this.Components.Add<IActivationCache, ActivationCache>();
            }

            if (!this.Components.TryGet<ICachePruner>(out _))
            {
                this.Components.Add<ICachePruner, GarbageCollectionCachePruner>();
            }

            if (!this.Components.TryGet<IPipeline>(out var pipeline))
            {
                pipeline = this.pipelineFactory.Create();
                this.Components.Add(pipeline);
            }

            if (!this.Components.TryGet<ICache>(out var cache))
            {
                this.Components.Add<ICache, Cache>();
                cache = this.Components.Get<ICache>();
            }

            if (!this.Components.TryGet<IExceptionFormatter>(out var exceptionFormatter))
            {
                this.Components.Add<IExceptionFormatter, ExceptionFormatter>();
                exceptionFormatter = this.Components.Get<IExceptionFormatter>();
            }

            if (!this.Components.TryGet<IBindingPrecedenceComparer>(out var bindingPrecedenceComparer))
            {
                this.Components.Add<IBindingPrecedenceComparer, BindingPrecedenceComparer>();
                bindingPrecedenceComparer = this.Components.Get<IBindingPrecedenceComparer>();
            }

            this.Components.Add<IPlanningStrategy, ConstructorReflectionStrategy>();

            if (!this.Components.TryGet<IConstructorInjectionSelector>(out _))
            {
                this.Components.Add<IConstructorInjectionSelector, DefaultConstructorInjectionSelector>();
            }

            this.Components.Add<IBindingResolver, StandardBindingResolver>();

            var bindingsByType = this.bindingsBuilder.Build();

            foreach (var bindingTypeEntry in bindingsByType)
            {
                var bindings = bindingTypeEntry.Value;
                foreach (var binding in bindings)
                {
                    if (binding.ActivationActions.Count > 0)
                    {
                    }

                    if (binding.DeactivationActions.Count > 0)
                    {
                    }
                }
            }

            var kernel = new ReadOnlyKernel5(
                    bindingsByType,
                    cache,
                    pipeline,
                    exceptionFormatter,
                    bindingPrecedenceComparer,
                    this.Components.GetAll<IBindingResolver>().ToList(),
                    this.Components.GetAll<IMissingBindingResolver>().ToList());

            return kernel;
        }
    }
}