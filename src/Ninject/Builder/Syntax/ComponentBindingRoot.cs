// -------------------------------------------------------------------------------------------------
// <copyright file="ComponentRoot.cs" company="Ninject Project Contributors">
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
    using Ninject.Activation.Caching;
    using Ninject.Activation.Providers;
    using Ninject.Activation.Strategies;
    using Ninject.Components;
    using Ninject.Infrastructure;
    using Ninject.Injection;
    using Ninject.Planning;
    using Ninject.Planning.Bindings;
    using Ninject.Selection;
    using Ninject.Syntax;

    internal class ComponentBindingRoot : IComponentBindingRoot, IComponentContainer
    {
        private readonly Action<ComponentBindingRoot> initializer;
        private readonly List<NewBindingBuilder> bindingBuilders;
        private readonly ComponentKernelFactory kernelFactory;
        private readonly Dictionary<string, object> properties;
        private readonly IExceptionFormatter exceptionFormatter;
        private IReadOnlyKernel componentContainer;

        /// <summary>
        /// Initializes a new instance of the <see cref="ComponentBindingRoot"/> class.
        /// </summary>
        /// <param name="properties">A key/value collection that can be used to share data between components.</param>
        /// <param name="exceptionFormatter">The <see cref="IExceptionFormatter"/> component.</param>
        /// <param name="kernelFactory">A factory to create the component kernel.</param>
        /// <param name="initializer">A delegate to perform additional initialization when the <see cref="ComponentBindingRoot"/> is being built, or <see langword="null"/> when no additional initialization is required.</param>
        public ComponentBindingRoot(Dictionary<string, object> properties,
                                    IExceptionFormatter exceptionFormatter,
                                    ComponentKernelFactory kernelFactory,
                                    Action<ComponentBindingRoot> initializer)
        {
            this.initializer = initializer;
            this.bindingBuilders = new List<NewBindingBuilder>();
            this.kernelFactory = kernelFactory;
            this.properties = properties;
            this.exceptionFormatter = exceptionFormatter;
        }

        /// <summary>
        /// Gets a key/value collection that can be used to share data between components.
        /// </summary>
        /// <value>
        /// A key/value collection that can be used to share data between components.
        /// </value>
        public IDictionary<string, object> Properties
        {
            get { return properties; }
        }

        /// <inheritdoc/>
        public INewBindingToSyntax<T> Bind<T>()
        {
            EnsureComponentContainerNotBuilt();

            var bindingBuilder = new BindingBuilder<T>(exceptionFormatter);
            this.bindingBuilders.Add(bindingBuilder);
            return bindingBuilder;
        }

        /// <inheritdoc/>
        public void Unbind<T>()
        {
            Unbind(typeof(T));
        }

        /// <inheritdoc/>
        public bool IsBound<T>()
        {
            var service = typeof(T);

            foreach (var binding in this.bindingBuilders)
            {
                if (binding.Service == service)
                {
                    return true;
                }
            }

            return false;
        }

        public IReadOnlyKernel GetOrBuild()
        {
            if (componentContainer == null)
            {
                componentContainer = BuildComponentContainer();
            }

            return componentContainer;
        }

        #region IComponentContainer implementation

        void IComponentContainer.Add<TComponent, TImplementation>()
        {
            Bind<TComponent>().To<TImplementation>().InSingletonScope();
        }

        public void RemoveAll<T>()
            where T : INinjectComponent
        {
            Unbind(typeof(T));
        }

        public void RemoveAll(Type component)
        {
            Unbind(component);
        }

        public void Remove<T, TImplementation>()
            where T : INinjectComponent
            where TImplementation : T
        {
            EnsureComponentContainerNotBuilt();

            var service = typeof(T);
            var implementation = typeof(TImplementation);

            for (var i = this.bindingBuilders.Count - 1; i >= 0; i--)
            {
                var bindingBuilder = this.bindingBuilders[i];
                if (bindingBuilder.Service == service && IsBoundTo(bindingBuilder, implementation))
                {
                    this.bindingBuilders.RemoveAt(i);
                }
            }
        }

        T IComponentContainer.Get<T>()
        {
            return GetOrBuild().Get<T>();
        }

        IEnumerable<T> IComponentContainer.GetAll<T>()
        {
            return GetOrBuild().GetAll<T>();
        }

        object IComponentContainer.Get(Type component)
        {
            return GetOrBuild().Get(component);
        }

        IEnumerable<object> IComponentContainer.GetAll(Type component)
        {
            return GetOrBuild().GetAll(component);
        }

        void IComponentContainer.AddTransient<TComponent, TImplementation>()
        {
            Bind<TComponent>().To<TImplementation>().InTransientScope();
        }

        #endregion IComponentContainer implementation

        void IDisposable.Dispose()
        {
            throw new NotImplementedException();
        }

        private IReadOnlyKernel BuildComponentContainer()
        {
            this.initializer?.Invoke(this);

            /*
                TODO:
                fail if neither constructor injection, nor method or property injection is configured
                if no constructorselector is configured, then use DefaultConstructorSelector
                fail if no IInjectorFactory is configured
                fail if no bindings are configured, and neither SelfBinding nor DefaultValueBinding are configured?
            */

            if (!IsBound<IPlanner>())
            {
                Bind<IPlanner>().To<Planner>().InSingletonScope();
            }

            if (!IsBound<IActivationCache>())
            {
                Bind<IActivationCache>().To<ActivationCache>().InSingletonScope();
            }

            if (!IsBound<ICachePruner>())
            {
                Bind<ICachePruner>().To<GarbageCollectionCachePruner>()
                                    .InSingletonScope()
                                    .WithPropertyValue(nameof(GarbageCollectionCachePruner.PruningInterval), GarbageCollectionCachePruner.DefaultPruningInterval);
            }

            if (!IsBound<ICache>())
            {
                Bind<ICache>().To<Cache>().InSingletonScope();
            }

            if (!IsBound<IExceptionFormatter>())
            {
                Bind<IExceptionFormatter>().To<ExceptionFormatter>();
            }

            if (!IsBound<IBindingPrecedenceComparer>())
            {
                Bind<IBindingPrecedenceComparer>().To<BindingPrecedenceComparer>();
            }

            if (!IsBound<IConstructorInjectionSelector>())
            {
                Bind<IConstructorInjectionSelector>().To<DefaultConstructorInjectionSelector>();
            }

            if (!IsBound<IConstructorReflectionSelector>())
            {
                Bind<IConstructorReflectionSelector>().ToConstant(new ConstructorReflectionSelector());
            }

            if (!IsBound<IConstructorParameterValueProvider>())
            {
                Bind<IConstructorParameterValueProvider>().To<ConstructorParameterValueProvider>();
            }

            if (!IsBound<IInjectorFactory>())
            {
                Bind<IInjectorFactory>().To<ExpressionInjectorFactory>();
            }

            if (!IsBound<IPipeline>())
            {
                Bind<IPipeline>().ToMethod(c => new PipelineFactory().Create(c.Kernel)).InSingletonScope();
            }

            if (!IsBound<IContextFactory>())
            {
                Bind<IContextFactory>().ToConstructor(c => new ContextFactory(c.Inject<ICache>(), c.Inject<IPlanner>(), c.Inject<IPipeline>(), c.Inject<IExceptionFormatter>(), false, true));
            }

            // Build the bindings for the components
            var resolveComponentsKernel = kernelFactory.CreateResolveComponentBindingsKernel();
            var componentBindingVisitor = new BindingBuilderVisitor();
            foreach (var bindingBuilder in this.bindingBuilders)
            {
                bindingBuilder.Build(resolveComponentsKernel, componentBindingVisitor);
            }

            // Build the kernel
            var componentContainer = kernelFactory.CreateComponentsKernel(resolveComponentsKernel, componentBindingVisitor.Bindings);

            // Validate the kernel
            Validate(componentContainer);

            return componentContainer;
        }

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
            foreach (var bindingBuilder in this.bindingBuilders)
            {
                bindingBuilder.Accept(bindingActionVisitor);
            }

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

        private static bool Contains<T, TImplementation>(IReadOnlyKernel componentContainer)
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

        private void Unbind(Type service)
        {
            EnsureComponentContainerNotBuilt();

            for (var i = this.bindingBuilders.Count - 1; i >= 0; i--)
            {
                if (this.bindingBuilders[i].Service == service)
                {
                    this.bindingBuilders.RemoveAt(i);
                }
            }
        }

        private void EnsureComponentContainerNotBuilt()
        {
            if (this.componentContainer != null)
            {
                throw new ActivationException(exceptionFormatter.InvalidOperationOnceComponentContainerIsBuilt());
            }
        }

        private static bool IsBoundTo(NewBindingBuilder bindingBuilder, Type implementation)
        {
            var standardProvider = bindingBuilder.BindingConfigurationBuilder.ProviderFactory as StandardProviderFactory;
            return standardProvider != null && standardProvider.Implementation == implementation;
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
    }
}