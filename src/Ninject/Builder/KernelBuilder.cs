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
    using System.Linq;

    using Ninject.Activation;
    using Ninject.Activation.Caching;
    using Ninject.Activation.Strategies;
    using Ninject.Components;
    using Ninject.Planning;
    using Ninject.Planning.Bindings;
    using Ninject.Planning.Bindings.Resolvers;
    using Ninject.Planning.Strategies;
    using Ninject.Syntax;

    /// <summary>
    /// Provides the mechanisms to build a kernel.
    /// </summary>
    public sealed partial class KernelBuilder : IKernelBuilder, IKernelConfiguration, IFluentSyntax
    {
        private IExceptionFormatter exceptionFormatter;
        private readonly NewBindingRoot bindingRoot;
        private readonly FeatureBuilder featureBuilder;

        /// <summary>
        /// Initializes a new instance of the <see cref="KernelBuilder"/> class.
        /// </summary>
        public KernelBuilder()
        {
            this.exceptionFormatter = new ExceptionFormatter();
            this.bindingRoot = new NewBindingRoot();
            this.ModuleBuilder = new ModuleLoader(this, this.exceptionFormatter);
            this.Components = new ComponentBindingRoot();
            this.Components.Bind<IPlanningStrategy>().To<ConstructorReflectionStrategy>();
            this.Components.Bind<IBindingResolver>().To<StandardBindingResolver>();
            this.Components.Bind<ActivationCacheStrategy>().ToSelf();
            this.Components.Bind<DeactivationCacheStrategy>().ToSelf();
            this.featureBuilder = new FeatureBuilder(this.Components);
        }

        /// <summary>
        /// Gets the root of the component bindings.
        /// </summary>
        /// <value>
        /// The root of the component binding.
        /// </value>
        internal ComponentBindingRoot Components { get; private set; }

        /// <summary>
        /// Gets the module builder.
        /// </summary>
        /// <value>
        /// The module builder.
        /// </value>
        internal ModuleLoader ModuleBuilder { get; private set; }

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
            configureModules(this.ModuleBuilder);
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
            this.ModuleBuilder.Complete();

            var componentContainer = this.Components.GetOrCreate();

            // Builds the bindings for the user-facing kernel
            var bindingBuilderVisitor = new BindingBuilderVisitor();
            this.bindingRoot.Build(componentContainer, bindingBuilderVisitor);
            var bindingsByType = bindingBuilderVisitor.Bindings;

            // Build the user-facing kernel
            return new ReadOnlyKernel(bindingsByType,
                                      componentContainer.Get<ICache>(),
                                      componentContainer.Get<IPlanner>(),
                                      componentContainer.Get<IPipeline>(),
                                      componentContainer.Get<IExceptionFormatter>(),
                                      componentContainer.Get<IContextFactory>(),
                                      componentContainer.Get<IBindingPrecedenceComparer>(),
                                      componentContainer.GetAll<IBindingResolver>().ToList(),
                                      componentContainer.GetAll<IMissingBindingResolver>().ToList());
        }

        #region IKernelConfiguration implementation

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
    }
}