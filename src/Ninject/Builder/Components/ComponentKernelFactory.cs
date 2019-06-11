﻿// -------------------------------------------------------------------------------------------------
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

namespace Ninject.Builder.Components
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
    using Ninject.Selection.Heuristics;

    internal class BuilderKernelFactory
    {
        public IReadOnlyKernel CreateResolveComponentBindingsKernel()
        {
            var exceptionFormatter = new ExceptionFormatter();
            var planner = CreatePlanner();
            var pipeline = CreatePipeline(exceptionFormatter);
            var cache = new Cache(pipeline, new NoOpCachePruner());

            return new ReadOnlyKernel5(
                    CreateBindings(planner, pipeline, cache, exceptionFormatter),
                    cache,
                    planner,
                    pipeline,
                    exceptionFormatter,
                    new BindingPrecedenceComparer(),
                    new List<IBindingResolver> { new StandardBindingResolver() },
                    new List<IMissingBindingResolver>());
        }

        public IReadOnlyKernel CreateComponentsKernel(IReadOnlyKernel resolveComponentBindingsKernel, Dictionary<Type, ICollection<IBinding>> bindings)
        {
            return new ReadOnlyKernel5(
                    bindings,
                    resolveComponentBindingsKernel.Get<ICache>(),
                    resolveComponentBindingsKernel.Get<IPlanner>(),
                    resolveComponentBindingsKernel.Get<IPipeline>(),
                    resolveComponentBindingsKernel.Get<IExceptionFormatter>(),
                    new BindingPrecedenceComparer(),
                    new List<IBindingResolver> { new StandardBindingResolver() },
                    new List<IMissingBindingResolver>());
        }

        private static BindingsBuilder CreateBindings(IPlanner planner, IPipeline pipeline, ICache cache, IExceptionFormatter exceptionFormatter)
        {
            BindingsBuilder bindings = new BindingsBuilder();

            bindings.Bind<IPlanner>().ToConstant(planner);
            bindings.Bind<IPipeline>().ToConstant(pipeline);
            bindings.Bind<ICache>().ToConstant(cache);
            bindings.Bind<IExceptionFormatter>().ToConstant(exceptionFormatter);

            bindings.Bind<IConstructorInjectionSelector>().ToConstant(new UniqueConstructorInjectionSelector());
            bindings.Bind<IConstructorParameterValueProvider>().To<ConstructorParameterValueProvider>();

            /*
            bindings.Bind<IPlanningStrategy>().To<ConstructorReflectionStrategy>();

            bindings.Bind<IInjectorFactory>().To<ExpressionInjectorFactory>();
            */

            return bindings;
        }

        private static IPlanner CreatePlanner()
        {
            var injectorFactory = new ExpressionInjectorFactory();

            return new Planner(new IPlanningStrategy[]
                {
                    new ConstructorReflectionStrategy(new ConstructorReflectionSelector(), injectorFactory),
                    new PropertyReflectionStrategy(new PropertyReflectionSelector(new IPropertyInjectionHeuristic[] { new AnyPropertyInjectionHeuristic() }), injectorFactory),
                });
        }

        private static IPipeline CreatePipeline(IExceptionFormatter exceptionFormatter)
        {
            var propertyInjectionStrategy = new PropertyInjectionStrategy(new PropertyValueProvider(), exceptionFormatter);
            var pipelineInitializer = new PipelineInitializer(new List<IInitializationStrategy> { propertyInjectionStrategy });
            var pipelineDeactivator = new PipelineDeactivator(new List<IDeactivationStrategy> { new DisposableStrategy() });

            return new DefaultPipeline(pipelineInitializer, new NoOpPipelineActivator(), pipelineDeactivator);
        }

        private class NoOpCachePruner : ICachePruner
        {
            void IDisposable.Dispose()
            {
            }

            public void Start(IPruneable cache)
            {
            }

            public void Stop()
            {
            }
        }

        private class AnyPropertyInjectionHeuristic : IPropertyInjectionHeuristic
        {
            void IDisposable.Dispose()
            {
            }

            public bool ShouldInject(Type type, PropertyInfo property)
            {
                return true;
            }
        }
    }
}