// -------------------------------------------------------------------------------------------------
// <copyright file="ComponentKernelFactory.cs" company="Ninject Project Contributors">
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
    using System.Reflection;
    using Ninject.Activation;
    using Ninject.Activation.Caching;
    using Ninject.Activation.Providers;
    using Ninject.Activation.Strategies;
    using Ninject.Components;
    using Ninject.Injection;
    using Ninject.Planning;
    using Ninject.Planning.Bindings;
    using Ninject.Planning.Bindings.Resolvers;
    using Ninject.Planning.Strategies;
    using Ninject.Selection;
    using Ninject.Syntax;

    internal class BuilderKernelFactory
    {
        public IReadOnlyKernel CreateResolveComponentBindingsKernel()
        {
            var exceptionFormatter = new ExceptionFormatter();
            var propertySelector = new PropertyReflectionSelector(false);
            var injectorFactory = new ExpressionInjectorFactory();
            var planner = CreatePlanner(propertySelector, injectorFactory);
            var pipeline = CreatePipeline(propertySelector, injectorFactory, exceptionFormatter);
            var cache = new Cache(pipeline, new GarbageCollectionCachePruner());
            var contextFactory = new ContextFactory(cache, exceptionFormatter, false, true);

            return new ReadOnlyKernel(CreateBindings(planner, pipeline, cache, exceptionFormatter, contextFactory),
                                      cache,
                                      planner,
                                      pipeline,
                                      exceptionFormatter,
                                      contextFactory,
                                      new BindingPrecedenceComparer(),
                                      new List<IBindingResolver> { new StandardBindingResolver() },
                                      new List<IMissingBindingResolver>());
        }

        public IReadOnlyKernel CreateComponentsKernel(IReadOnlyKernel resolveComponentBindingsKernel, Dictionary<Type, ICollection<IBinding>> bindings)
        {
            return new ReadOnlyKernel(bindings,
                                      resolveComponentBindingsKernel.Get<ICache>(),
                                      resolveComponentBindingsKernel.Get<IPlanner>(),
                                      resolveComponentBindingsKernel.Get<IPipeline>(),
                                      resolveComponentBindingsKernel.Get<IExceptionFormatter>(),
                                      resolveComponentBindingsKernel.Get<IContextFactory>(),
                                      new BindingPrecedenceComparer(),
                                      new List<IBindingResolver> { new StandardBindingResolver() },
                                      new List<IMissingBindingResolver>());
        }

        private static NewBindingRoot CreateBindings(IPlanner planner,
                                                     IPipeline pipeline,
                                                     ICache cache,
                                                     IExceptionFormatter exceptionFormatter,
                                                     IContextFactory contextFactory)
        {
            var bindings = new NewBindingRoot();

            bindings.Bind<IPlanner>().ToConstant(planner);
            bindings.Bind<IPipeline>().ToConstant(pipeline);
            bindings.Bind<ICache>().ToConstant(cache);
            bindings.Bind<IExceptionFormatter>().ToConstant(exceptionFormatter);
            bindings.Bind<IContextFactory>().ToConstant(contextFactory);
            bindings.Bind<IConstructorInjectionSelector>().ToConstant(new UniqueConstructorInjectionSelector());
            bindings.Bind<IConstructorParameterValueProvider>().To<ConstructorParameterValueProvider>();

            return bindings;
        }

        private static IPlanner CreatePlanner(PropertyReflectionSelector propertySelector, ExpressionInjectorFactory injectorFactory)
        {
            return new Planner(new IPlanningStrategy[]
                {
                    new ConstructorReflectionStrategy(new ConstructorReflectionSelector(), injectorFactory),
                    new PropertyPlanningStrategy(propertySelector, new IPropertyInjectionHeuristic[] { new AnyPropertyInjectionHeuristic() }, injectorFactory)
                });
        }

        private static IPipeline CreatePipeline(PropertyReflectionSelector propertySelector, ExpressionInjectorFactory injectorFactory, IExceptionFormatter exceptionFormatter)
        {
            var activationCache = new ActivationCache(new GarbageCollectionCachePruner());
            var propertyInjectionStrategy = new PropertyInjectionStrategy(propertySelector, injectorFactory, exceptionFormatter);
            var pipelineInitializer = new PipelineInitializer(new List<IInitializationStrategy> { propertyInjectionStrategy });
            var pipelineDeactivator = new PipelineDeactivator(new List<IDeactivationStrategy> { new DisposableStrategy() }, activationCache);

            return new DefaultPipeline(pipelineInitializer, new NoOpPipelineActivator(), pipelineDeactivator);
        }

        private class AnyPropertyInjectionHeuristic : IPropertyInjectionHeuristic
        {
            void IDisposable.Dispose()
            {
            }

            public bool ShouldInject(PropertyInfo property)
            {
                return true;
            }
        }
    }
}