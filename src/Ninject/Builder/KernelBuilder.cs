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
    using Ninject.Activation.Providers;
    using Ninject.Activation.Strategies;
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
        private readonly ComponentBindingRoot componentRoot;

        /// <summary>
        /// Initializes a new instance of the <see cref="KernelBuilder"/> class.
        /// </summary>
        public KernelBuilder()
        {
            this.bindingsBuilder = new BindingsBuilder();
            this.componentRoot = new ComponentBindingRoot();

            this.componentRoot.Bind<IPlanningStrategy>().To<ConstructorReflectionStrategy>();
            this.componentRoot.Bind<IBindingResolver>().To<StandardBindingResolver>();
        }

        /// <summary>
        /// Gets the root of the component bindings.
        /// </summary>
        /// <value>
        /// The root of the component binding.
        /// </value>
        public IComponentBindingRoot Components
        {
            get { return this.componentRoot; }
        }

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

            if (!this.Components.IsBound<IPlanner>())
            {
                this.Components.Bind<IPlanner>().To<Planner>().InSingletonScope();
            }

            if (!this.Components.IsBound<IActivationCache>())
            {
                this.Components.Bind<IActivationCache>().To<ActivationCache>().InSingletonScope();
            }

            if (!this.Components.IsBound<ICachePruner>())
            {
                this.Components.Bind<ICachePruner>()
                               .To<GarbageCollectionCachePruner>()
                               .InSingletonScope()
                               .WithPropertyValue(nameof(GarbageCollectionCachePruner.PruningInterval), GarbageCollectionCachePruner.DefaultPruningInterval);
            }

            if (!this.Components.IsBound<ICache>())
            {
                this.Components.Bind<ICache>().To<Cache>().InSingletonScope();
            }

            if (!this.Components.IsBound<IExceptionFormatter>())
            {
                this.Components.Bind<IExceptionFormatter>().To<ExceptionFormatter>();
            }

            if (!this.Components.IsBound<IBindingPrecedenceComparer>())
            {
                this.Components.Bind<IBindingPrecedenceComparer>().To<BindingPrecedenceComparer>();
            }

            if (!this.Components.IsBound<IConstructorInjectionSelector>())
            {
                this.Components.Bind<IConstructorInjectionSelector>().To<DefaultConstructorInjectionSelector>();
            }

            if (!this.Components.IsBound<IConstructorParameterValueProvider>())
            {
                this.Components.Bind<IConstructorParameterValueProvider>().To<ConstructorParameterValueProvider>();
            }

            if (!this.Components.IsBound<IPipeline>())
            {
                this.Components.Bind<IPipeline>().ToMethod(c => new PipelineFactory().Create(c.Kernel)).InSingletonScope();
            }

            var bindingActionAggregate = BindingActionAggregate.Create(this.bindingsBuilder.Bindings);
            if (bindingActionAggregate.HasActivationActions)
            {
                this.Components.Bind<IActivationStrategy>().To<BindingActionStrategy>();
            }

            if (bindingActionAggregate.HasDeactivationActions)
            {
                this.Components.Bind<IDeactivationStrategy>().To<BindingActionStrategy>();
            }

            if (bindingActionAggregate.HasInitializationActions)
            {
                this.Components.Bind<IInitializationStrategy>().To<BindingActionStrategy>();
            }

            var resolveComponentsKernel = new BuilderKernelFactory().CreateResolveComponentBindingsKernel();

            var componentBindingVisitor = new BindingBuilderVisitor();
            this.componentRoot.Build(resolveComponentsKernel, componentBindingVisitor);
            var componentBindings = componentBindingVisitor.Bindings;

            var componentContainer = new BuilderKernelFactory().CreateComponentsKernel(resolveComponentsKernel, componentBindings);

            var bindingBuilderVisitor = new BindingBuilderVisitor();
            this.bindingsBuilder.Build(componentContainer, bindingBuilderVisitor);
            var bindingsByType = bindingBuilderVisitor.Bindings;

            return new ReadOnlyKernel5(
                    bindingsByType,
                    componentContainer.Get<ICache>(),
                    componentContainer.Get<IPlanner>(),
                    componentContainer.Get<IPipeline>(),
                    componentContainer.Get<IExceptionFormatter>(),
                    componentContainer.Get<IBindingPrecedenceComparer>(),
                    componentContainer.GetAll<IBindingResolver>().ToList(),
                    componentContainer.GetAll<IMissingBindingResolver>().ToList());
        }

        private class BindingActionAggregate
        {
            private BindingActionAggregate()
            {
            }

            public bool HasActivationActions { get; private set; }

            public bool HasDeactivationActions { get; private set; }

            public bool HasInitializationActions { get; private set; }

            public static BindingActionAggregate Create(IReadOnlyList<BindingBuilder> bindings)
            {
                var aggregate = new BindingActionAggregate();

                foreach (var binding in bindings)
                {
                    var bindingConfiguration = binding.BindingConfigurationBuilder;
                    if (bindingConfiguration == null)
                    {
                        continue;
                    }

                    if (bindingConfiguration.HasActivationActions)
                    {
                        aggregate.HasActivationActions = true;
                    }

                    if (bindingConfiguration.HasDeactivationActions)
                    {
                        aggregate.HasDeactivationActions = true;
                    }

                    if (bindingConfiguration.HasInitializationActions)
                    {
                        aggregate.HasInitializationActions = true;
                    }
                }

                return aggregate;
            }
        }
    }
}