using Ninject.Activation;
using Ninject.Activation.Caching;
using Ninject.Components;
using Ninject.Injection;
using Ninject.Planning;
using Ninject.Planning.Bindings.Resolvers;
using System;
using System.Collections.Generic;

namespace Ninject.Builder
{
    internal class ResolutionBuilder : IResolutionBuilder
    {
        private bool allowNull;
        private bool detectCyclicDependencies;

        public ResolutionBuilder(IComponentBindingRoot componentBindingRoot, IDictionary<string, object> properties)
        {
            this.Components = componentBindingRoot;
            this.Properties = properties;
        }

        /// <summary>
        /// Gets the component bindings that make up the activation pipeline.
        /// </summary>
        /// <value>
        /// The component bindings that make up the activation pipeline.
        /// </value>
        public IComponentBindingRoot Components { get; }

        /// <summary>
        /// Gets a key/value collection that can be used to share data between components.
        /// </summary>
        /// <value>
        /// A key/value collection that can be used to share data between components.
        /// </value>
        public IDictionary<string, object> Properties { get; }

        public void Build()
        {
            Components.Bind<IContextFactory>()
                      .ToConstructor(c => new ContextFactory(c.Inject<ICache>(), c.Inject<IPlanner>(), c.Inject<IPipeline>(), c.Inject<IExceptionFormatter>(), this.allowNull, this.detectCyclicDependencies));
        }

        /// <summary>
        /// Allow <see langword="null"/> as a valid value for injection.
        /// </summary>
        /// <remarks>
        /// When not set, an <see cref="ActivationException"/> is thrown whenever a provider returns <see langword="null"/>.
        /// This is the default.
        /// </remarks>
        public IResolutionBuilder AllowNull()
        {
            this.allowNull = true;
            return this;
        }

        /// <summary>
        /// Enables detection of cyclic dependencies.
        /// </summary>
        /// <remarks>
        /// <para>
        /// If enabled, an <see cref="ActivationException"/> is thrown whenever a cyclic dependency is detected.
        /// </para>
        /// <para>
        /// When not enabled, the CLR throws a <see cref="StackOverflowException"/> and terminates the process in case
        /// of cyclic dependencies. This is the default.
        /// </para>
        /// </remarks>
        public IResolutionBuilder DetectCyclicDependencies()
        {
            this.detectCyclicDependencies = true;
            return this;
        }

        /// <summary>
        /// Configures the <see cref="IKernelBuilder"/> to use expression-based injection.
        /// </summary>
        /// <returns>
        /// The <see cref="IResolutionBuilder"/> instance.
        /// </returns>
        public IResolutionBuilder ExpressionInjector()
        {
            Components.Unbind<IInjectorFactory>();
            Components.Bind<IInjectorFactory>().To<ExpressionInjectorFactory>();
            return this;
        }

        /// <summary>
        /// Configures the <see cref="IKernelBuilder"/> to use reflection-based injection.
        /// </summary>
        /// <returns>
        /// The <see cref="IResolutionBuilder"/> instance.
        /// </returns>
        public IResolutionBuilder ReflectionInjector()
        {
            Components.Unbind<IInjectorFactory>();
            Components.Bind<IInjectorFactory>().To<ReflectionInjectorFactory>();
            return this;
        }

        /// <summary>
        /// Configures an <see cref="IKernelBuilder"/> to use the default value for a given target when no
        /// explicit binding is available.
        /// </summary>
        /// <returns>
        /// The <see cref="IResolutionBuilder"/> instance.
        /// </returns>
        public IResolutionBuilder DefaultValueBinding()
        {
            Components.Bind<IMissingBindingResolver>().To<DefaultValueBindingResolver>();
            return this;
        }

        /// <summary>
        /// Configures an <see cref="IKernelBuilder"/> to support open generic binding.
        /// </summary>
        /// <returns>
        /// The <see cref="IResolutionBuilder"/> instance.
        /// </returns>
        public IResolutionBuilder OpenGenericBinding()
        {
            Components.Bind<IBindingResolver>().To<OpenGenericBindingResolver>();
            return this;
        }

        /// <summary>
        /// Configures an <see cref="IKernelBuilder"/> to automatically create a self-binding for a given
        /// service when no explicit binding is available.
        /// </summary>
        /// <returns>
        /// The <see cref="IResolutionBuilder"/> instance.
        /// </returns>
        public IResolutionBuilder SelfBinding()
        {
            Components.Bind<IMissingBindingResolver>().To<SelfBindingResolver>();
            return this;
        }
    }
}
