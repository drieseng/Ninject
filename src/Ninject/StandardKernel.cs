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
    using Ninject.Builder;
    using Ninject.Modules;
    using System;

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
            : base(settings, modules)
        {
        }

        /// <summary>
        /// Adds components to the kernel during startup.
        /// </summary>
        protected override void AddComponents(IKernelBuilder kernelBuilder)
        {
            kernelBuilder.Features(features => features.Components.Bind<INinjectSettings>().ToConstant(this.Settings));
            kernelBuilder.Features(ConfigureFeatures);

            /*
            this.Components.Add<IAssemblyNameRetriever, AssemblyNameRetriever>();

            this.Components.Add<IModuleLoader, ModuleLoader>();
            this.Components.Add<IModuleLoaderPlugin, CompiledModuleLoaderPlugin>();
            */
        }

        private void ConfigureFeatures(IFeatureBuilder features)
        {
            features.ConstructorInjection(c => c.BestMatch(b => b.Selector(selector => selector.InjectNonPublic(this.Settings.InjectNonPublic))
                                       .Scorer(scorer => scorer.HighestScoreAttribute(this.Settings.InjectAttribute)
                                                               .LowestScoreAttribute(typeof(ObsoleteAttribute)))))
                    .Initialization(pipeline => ConfigureInitializationPipeline(pipeline))
                    .Activation(pipeline => ConfigureActivationPipeline(pipeline))
                    .Deactivation(pipeline => ConfigureDeactivationPipeline(pipeline))
                    .OpenGenericBinding()
                    .DefaultValueBinding()
                    .SelfBinding();

            if (this.Settings.UseReflectionBasedInjection)
            {
                features.ReflectionBasedInjection();
            }
            else
            {
                features.ExpressionBasedInjection();
            }
        }

        private void ConfigureInitializationPipeline(IInitializationPipelineBuilder pipeline)
        {
            if (this.Settings.PropertyInjection)
            {
                pipeline.PropertyInjection(property =>
                    property.Selector(selector => selector.InjectNonPublic(this.Settings.InjectNonPublic))
                            .InjectionHeuristic(h => h.InjectAttribute<InjectAttribute>()));
            }

            if (this.Settings.MethodInjection)
            {
                pipeline.MethodInjection(method =>
                    method.Selector(selector => selector.InjectNonPublic(this.Settings.InjectNonPublic))
                          .InjectionHeuristic(h => h.InjectAttribute<InjectAttribute>()));
            }


            pipeline.Initializable().BindingAction();
        }

        private static void ConfigureActivationPipeline(IActivationPipelineBuilder pipeline)
        {
            pipeline.Startable().BindingAction();
        }

        private static void ConfigureDeactivationPipeline(IDeactivationPipelineBuilder pipeline)
        {
            pipeline.BindingAction().Disposable().Stoppable();
        }
    }
}