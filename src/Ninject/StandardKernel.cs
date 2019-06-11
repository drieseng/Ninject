// -------------------------------------------------------------------------------------------------
// <copyright file="StandardKernel.cs" company="Ninject Project Contributors">
//   Copyright (c) 2007-2010 Enkari, Ltd. All rights reserved.
//   Copyright (c) 2010-2017 Ninject Project Contributors. All rights reserved.
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

namespace Ninject
{
    using Ninject.Activation;
    using Ninject.Activation.Caching;
    using Ninject.Activation.Providers;
    using Ninject.Activation.Strategies;
    using Ninject.Components;
    using Ninject.Injection;
    using Ninject.Modules;
    using Ninject.Planning;
    using Ninject.Planning.Bindings;
    using Ninject.Planning.Bindings.Resolvers;
    using Ninject.Planning.Strategies;
    using Ninject.Selection;
    using Ninject.Selection.Heuristics;

    /// <summary>
    /// The standard implementation of a kernel.
    /// </summary>
    public class StandardKernel : KernelBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StandardKernel"/> class.
        /// </summary>
        /// <param name="modules">The modules to load into the kernel.</param>
        public StandardKernel(params INinjectModule[] modules)
            : this(new NinjectSettings(), modules)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StandardKernel"/> class.
        /// </summary>
        /// <param name="settings">The configuration to use.</param>
        /// <param name="modules">The modules to load into the kernel.</param>
        public StandardKernel(INinjectSettings settings, params INinjectModule[] modules)
            : base(CreateComponentContainer(settings), settings, modules)
        {
        }

        /// <summary>
        /// Adds components to the kernel during startup.
        /// </summary>
        private static IComponentContainer CreateComponentContainer(INinjectSettings settings)
        {
            var components = new ComponentContainer(settings, new ExceptionFormatter());

            components.Add<IPlanner, Planner>();
            components.Add<IPlanningStrategy, ConstructorReflectionStrategy>();

            // TODO: add constructor argument for HighestScoreAttribute
            components.Add<IConstructorInjectionScorer, StandardConstructorScorer>();
            components.Add<IConstructorReflectionSelector, ConstructorReflectionSelector>();

            /*
            components.Add<IInjectionHeuristic, StandardInjectionHeuristic>();
            */

            components.Add<IPipeline, Pipeline>();

            if (!settings.ActivationCacheDisabled)
            {
                components.Add<IActivationStrategy, ActivationCacheStrategy>();
            }

            if (settings.PropertyInjection)
            {
                components.Add<IPlanningStrategy, PropertyReflectionStrategy>();
                components.Add<IPropertyReflectionSelector, PropertyReflectionSelector>();
                components.Add<IInitializationStrategy, PropertyInjectionStrategy>();
                components.Add<IPropertyValueProvider, PropertyValueProvider>();
            }

            if (settings.MethodInjection)
            {
                components.Add<IPlanningStrategy, MethodReflectionStrategy>();
                components.Add<IMethodReflectionSelector, MethodReflectionSelector>();
                components.Add<IInitializationStrategy, MethodInjectionStrategy>();
            }

            components.Add<IInitializationStrategy, InitializableStrategy>();
            components.Add<IActivationStrategy, StartableStrategy>();
            components.Add<IDeactivationStrategy, StoppableStrategy>();
            components.Add<IActivationStrategy, BindingActionStrategy>();
            components.Add<IDeactivationStrategy, BindingActionStrategy>();

            // Should be added as last deactivation strategy
            components.Add<IDeactivationStrategy, DisposableStrategy>();

            components.Add<IBindingPrecedenceComparer, BindingPrecedenceComparer>();

            components.Add<IBindingResolver, StandardBindingResolver>();
            components.Add<IBindingResolver, OpenGenericBindingResolver>();

            components.Add<IMissingBindingResolver, DefaultValueBindingResolver>();
            components.Add<IMissingBindingResolver, SelfBindingResolver>();

            if (!settings.UseReflectionBasedInjection)
            {
                components.Add<IInjectorFactory, ExpressionInjectorFactory>();
            }
            else
            {
                components.Add<IInjectorFactory, ReflectionInjectorFactory>();
            }

            components.Add<ICache, Cache>();
            components.Add<IActivationCache, ActivationCache>();
            components.Add<ICachePruner, GarbageCollectionCachePruner>();

            components.Add<IAssemblyNameRetriever, AssemblyNameRetriever>();

            /*
            components.Add<IModuleLoader, ModuleLoader>();
            components.Add<IModuleLoaderPlugin, CompiledModuleLoaderPlugin>();
            */


            return components;
        }
    }
}