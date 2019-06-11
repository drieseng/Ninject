// -------------------------------------------------------------------------------------------------
// <copyright file="ComponentContextFactory.cs" company="Ninject Project Contributors">
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
    using System.Collections.Generic;
    using System.Text;

    using Ninject.Activation;
    using Ninject.Activation.Caching;
    using Ninject.Activation.Providers;
    using Ninject.Activation.Strategies;
    using Ninject.Injection;
    using Ninject.Parameters;
    using Ninject.Planning;
    using Ninject.Planning.Bindings;
    using Ninject.Planning.Strategies;
    using Ninject.Selection;
    using Ninject.Selection.Heuristics;

    /// <summary>
    /// Factory for creating a context to resolve <see cref="INinjectComponent"/> instances.
    /// </summary>
    internal class ComponentContextFactory
    {
        private readonly IPlanner planner;
        private readonly IPipeline pipeline;
        private readonly ICache cache;
        private readonly IConstructorInjectionSelector constructorInjectionSelector;
        private readonly IConstructorParameterValueProvider constructorParameterValueProvider;
        private readonly IExceptionFormatter exceptionFormatter;

        /// <summary>
        /// Initializes a new instance of the <see cref="ComponentContextFactory"/> class.
        /// </summary>
        /// <param name="pipeline">The pipeline component.</param>
        /// <param name="cache">A <see cref="ICache"/> component.</param>
        /// <param name="constructorInjectionSelector">A constructor selector.</param>
        /// <param name="constructorParameterValueProvider">A value provider for constructor parameters.</param>
        /// <param name="exceptionFormatter">An <see cref="IExceptionFormatter"/> component.</param>
        public ComponentContextFactory(IPipeline pipeline, ICache cache, IConstructorInjectionSelector constructorInjectionSelector, IConstructorParameterValueProvider constructorParameterValueProvider, IExceptionFormatter exceptionFormatter)
        {
            this.pipeline = pipeline;
            this.cache = cache;
            this.planner = CreatePlanner();
            this.constructorInjectionSelector = constructorInjectionSelector;
            this.constructorParameterValueProvider = constructorParameterValueProvider;
            this.exceptionFormatter = exceptionFormatter;
        }

        /// <summary>
        /// Creates an <see cref="IContext"/> to create an instance of a given type, and initialized using the specified
        /// parameter and/or with values obtain from the <see cref="IConstructorParameterValueProvider"/> that was used to
        /// construct this context.
        /// </summary>
        /// <param name="component">The type of the component.</param>
        /// <param name="scopeCallback">The scope for the context that "owns" the instance activated in this context.</param>
        /// <param name="implementation">The type of the implementation.</param>
        /// <param name="parameters">The parameter to use to instantiate the instance.</param>
        /// <returns>
        /// An <see cref="IContext"/>.
        /// </returns>
        public IContext Create(Type component, Func<IContext, object> scopeCallback, Type implementation, params IParameter[] parameters)
        {
            var request = new Request(component, true);
            var plan = this.planner.GetPlan(implementation);
            var bindingConfiguration = new BindingConfiguration(parameters)
                {
                    Provider = this.CreateProvider(plan),
                    ScopeCallback = scopeCallback,
                };
            var binding = new Binding(plan.Type, bindingConfiguration);
            return new ComponentContext(plan, request, binding, this.cache, this.exceptionFormatter);
        }

        /// <summary>
        /// Creates an <see cref="IContext"/> for the specified instance.
        /// </summary>
        /// <typeparam name="T">The type of the instance.</typeparam>
        /// <param name="component">The type of the component.</param>
        /// <param name="scopeCallback">The scope for the context that "owns" the instance.</param>
        /// <param name="instance">The instance.</param>
        /// <returns>
        /// An <see cref="IContext"/> for the specified instance.
        /// </returns>
        public IContext Create<T>(Type component, Func<IContext, object> scopeCallback, T instance)
        {
            var request = new Request(component, true);
            var plan = this.planner.GetPlan(instance.GetType());
            var bindingConfiguration = new BindingConfiguration
                {
                    Provider = new ConstantProvider<T>(instance),
                    ScopeCallback = scopeCallback,
                };
            var binding = new Binding(plan.Type, bindingConfiguration);
            return new ComponentContext(plan, request, binding, this.cache, this.exceptionFormatter);
        }

        private static IPlanner CreatePlanner()
        {
            var injectorFactory = new ExpressionInjectorFactory();

            return new Planner(new IPlanningStrategy[]
                {
                    new ConstructorReflectionStrategy(new ConstructorReflectionSelector(), injectorFactory),
                    new PropertyReflectionStrategy(new PropertyReflectionSelector(new IPropertyInjectionHeuristic[] { new DefaultPropertyInjectionHeuristic() }), injectorFactory),
                });
        }

        private static IPipeline CreatePipeline(IExceptionFormatter exceptionFormatter)
        {
            var propertyInjectionStrategy = new PropertyInjectionStrategy(new PropertyValueProvider(), exceptionFormatter);
            var pipelineInitializer = new PipelineInitializer(new List<IInitializationStrategy> { propertyInjectionStrategy });
            var pipelineDeactivator = new PipelineDeactivator(new List<IDeactivationStrategy> { new DisposableStrategy() });

            return new DefaultPipeline(pipelineInitializer, new NoOpPipelineActivator(), pipelineDeactivator);
        }

        private IProvider CreateProvider(IPlan plan)
        {
            return new ComponentProvider(plan, this.constructorInjectionSelector, this.pipeline, this.constructorParameterValueProvider);
        }
    }
}