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
    using Ninject.Builder.Bindings;
    using Ninject.Components;
    using Ninject.Infrastructure;
    using Ninject.Infrastructure.Disposal;
    using Ninject.Injection;
    using Ninject.Planning;
    using Ninject.Planning.Bindings;
    using Ninject.Planning.Bindings.Resolvers;
    using Ninject.Planning.Strategies;
    using Ninject.Selection;
    using Ninject.Syntax;

    /// <summary>
    /// Provides the mechanisms to build a kernel.
    /// </summary>
    public sealed class KernelBuilder : DisposableObject, IKernelBuilder, IKernelConfiguration
    {
        private IExceptionFormatter exceptionFormatter;
        private readonly NewBindingRoot bindingRoot;
        private readonly FeatureBuilder featureBuilder;
        private readonly ModuleLoader moduleBuilder;
        private readonly ComponentBindingRoot componentRoot;

        /// <summary>
        /// Initializes a new instance of the <see cref="KernelBuilder"/> class.
        /// </summary>
        public KernelBuilder()
        {
            this.exceptionFormatter = new ExceptionFormatter();
            this.bindingRoot = new NewBindingRoot();
            this.featureBuilder = new FeatureBuilder();
            this.moduleBuilder = new ModuleLoader(this, this.exceptionFormatter);

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
        internal ComponentBindingRoot Components
        {
            get { return this.componentRoot; }
        }

        /// <summary>
        /// Gets the module builder.
        /// </summary>
        /// <value>
        /// The module builder.
        /// </value>
        internal ModuleLoader ModuleBuilder
        {
            get { return this.moduleBuilder; }
        }

        /// <summary>
        /// Configures the features of the <see cref="IKernelBuilder"/>.
        /// </summary>
        /// <param name="features">A callback to configure features.</param>
        /// <returns>
        /// A reference to this instance after the operation has completed.
        /// </returns>
        public IKernelBuilder Features(Action<IFeatureBuilder> features)
        {
            features(this.featureBuilder);
            return this;
        }

        /// <summary>
        /// Adds bindings to the <see cref="IKernelBuilder"/>.
        /// </summary>
        /// <param name="configureBindings">A callback to configure bindings.</param>
        /// <returns>
        /// A reference to this instance after the operation has completed.
        /// </returns>
        public IKernelBuilder Bindings(Action<INewBindingRoot> configureBindings)
        {
            configureBindings(this.bindingRoot);
            return this;
        }

        /// <summary>
        /// Adds modules to the <see cref="IKernelBuilder"/>.
        /// </summary>
        /// <param name="configureModules">A callback to configure modules.</param>
        /// <returns>
        /// A reference to this instance after the operation has completed.
        /// </returns>
        public IKernelBuilder Modules(Action<IModuleLoader> configureModules)
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
            // Signal that all modules have been loaded
            this.moduleBuilder.Complete();

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

            if (!this.Components.IsBound<IConstructorReflectionSelector>())
            {
                this.Components.Bind<IConstructorReflectionSelector>().ToConstant(new ConstructorReflectionSelector());
            }

            if (!this.Components.IsBound<IConstructorParameterValueProvider>())
            {
                this.Components.Bind<IConstructorParameterValueProvider>().To<ConstructorParameterValueProvider>();
            }

            if (!this.Components.IsBound<IInjectorFactory>())
            {
                this.Components.Bind<IInjectorFactory>().To<ExpressionInjectorFactory>();
            }

            if (!this.Components.IsBound<IPipeline>())
            {
                this.Components.Bind<IPipeline>().ToMethod(c => new PipelineFactory().Create(c.Kernel)).InSingletonScope();
            }

            // Build a kernel for the components
            var resolveComponentsKernel = new BuilderKernelFactory().CreateResolveComponentBindingsKernel();
            var componentBindingVisitor = new BindingBuilderVisitor();
            this.componentRoot.Build(resolveComponentsKernel, componentBindingVisitor);
            var componentContainer = new BuilderKernelFactory().CreateComponentsKernel(resolveComponentsKernel, componentBindingVisitor.Bindings);

            // Validate the kernel
            Validate(componentContainer);

            // Builds the bindings for the user-facing kernel
            var bindingBuilderVisitor = new BindingBuilderVisitor();
            this.bindingRoot.Build(componentContainer, bindingBuilderVisitor);
            var bindingsByType = bindingBuilderVisitor.Bindings;

            // Build the user-facing kernel
            return new ReadOnlyKernel5(
                    new NinjectSettings(),
                    bindingsByType,
                    componentContainer.Get<ICache>(),
                    componentContainer.Get<IPlanner>(),
                    componentContainer.Get<IPipeline>(),
                    componentContainer.Get<IExceptionFormatter>(),
                    componentContainer.Get<IBindingPrecedenceComparer>(),
                    componentContainer.GetAll<IBindingResolver>().ToList(),
                    componentContainer.GetAll<IMissingBindingResolver>().ToList());
        }

        #region IKernelConfiguration implementation

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.exceptionFormatter.Dispose();
            }

            base.Dispose(disposing);
        }


        /// <summary>
        /// Adds bindings to the <see cref="IKernelBuilder"/>.
        /// </summary>
        /// <param name="configureBindings">A callback to configure bindings.</param>
        /// <returns>
        /// A reference to this instance after the operation has completed.
        /// </returns>
        IKernelConfiguration IKernelConfiguration.Bindings(Action<INewBindingRoot> configureBindings)
        {
            Bindings(configureBindings);
            return this;
        }

        /// <summary>
        /// Configures the features of the <see cref="IKernelBuilder"/>.
        /// </summary>
        /// <param name="features">A callback to configure features.</param>
        /// <returns>
        /// A reference to this instance after the operation has completed.
        /// </returns>
        IKernelConfiguration IKernelConfiguration.Features(Action<IFeatureBuilder> features)
        {
            Features(features);
            return this;
        }


        /// <summary>
        /// Adds modules to the <see cref="IKernelBuilder"/>.
        /// </summary>
        /// <param name="configureModules">A callback to configure modules.</param>
        /// <returns>
        /// A reference to this instance after the operation has completed.
        /// </returns>
        IKernelConfiguration IKernelConfiguration.Modules(Action<IModuleLoader> configureModules)
        {
            Modules(configureModules);
            return this;
        }

        #endregion IKernelConfiguration implementation

        private void Validate(IReadOnlyKernel componentContainer)
        {
            bool activationActionsSupported = Contains<IActivationStrategy, BindingActionStrategy>(componentContainer);
            bool deactivationActionsSupported = Contains<IDeactivationStrategy, BindingActionStrategy>(componentContainer);
            bool initializationActionsSupported = Contains<IInitializationStrategy, BindingActionStrategy>(componentContainer);

            if (activationActionsSupported && deactivationActionsSupported && initializationActionsSupported)
            {
                return;
            }

            BindingActionVisitor bindingActionVisitor = new BindingActionVisitor();
            this.Components.Accept(bindingActionVisitor);

            if (!activationActionsSupported && bindingActionVisitor.ActivationActions)
            {
                throw new Exception("TODO");
            }

            if (!deactivationActionsSupported && bindingActionVisitor.DeactivationActions)
            {
                throw new Exception("TODO");
            }

            if (!initializationActionsSupported && bindingActionVisitor.InitializationActions)
            {
                throw new Exception("TODO");
            }
        }

        private bool Contains<T, TImplementation>(IReadOnlyKernel componentContainer)
            where TImplementation : class, T
        {
            var implementations = componentContainer.GetAll<T>();
            foreach (var implementation in implementations)
            {
                if (implementation is TImplementation)
                {
                    return true;
                }
            }

            return false;
        }

        internal class BindingActionVisitor : IVisitor<NewBindingBuilder>
        {
            public bool ActivationActions { get; private set; }
            public bool DeactivationActions { get; private set; }
            public bool InitializationActions { get; private set; }

            public void Visit(NewBindingBuilder element)
            {
                ActivationActions |= element.BindingConfigurationBuilder.HasActivationActions;
                DeactivationActions |= element.BindingConfigurationBuilder.HasDeactivationActions;
                InitializationActions |= element.BindingConfigurationBuilder.HasInitializationActions;
            }
        }

        private class BindingActionAggregate
        {
            private BindingActionAggregate()
            {
            }

            public bool HasActivationActions { get; private set; }

            public bool HasDeactivationActions { get; private set; }

            public bool HasInitializationActions { get; private set; }

            public static BindingActionAggregate Create(IReadOnlyList<INewBindingBuilder> bindings)
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