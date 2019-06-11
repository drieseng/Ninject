// -------------------------------------------------------------------------------------------------
// <copyright file="ComponentContainer.cs" company="Ninject Project Contributors">
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
    using System.Linq;

    using Ninject.Activation;
    using Ninject.Activation.Caching;
    using Ninject.Activation.Providers;
    using Ninject.Activation.Strategies;
    using Ninject.Infrastructure;
    using Ninject.Infrastructure.Disposal;
    using Ninject.Infrastructure.Language;
    using Ninject.Parameters;
    using Ninject.Selection;

    /// <summary>
    /// An internal container that manages and resolves components that contribute to Ninject.
    /// </summary>
    public class ComponentContainer : DisposableObject, IComponentContainer
    {
        /// <summary>
        /// The mappings for ninject components.
        /// </summary>
        private readonly Multimap<Type, IContext> mappings;

        /// <summary>
        /// The factory to create <see cref="IContext"/> instances.
        /// </summary>
        private readonly ComponentContextFactory contextFactory;

        /// <summary>
        /// The ninject settings.
        /// </summary>
        private readonly INinjectSettings settings;

        /// <summary>
        /// The <see cref="IExceptionFormatter"/> component.
        /// </summary>
        private readonly IExceptionFormatter exceptionFormatter;

        private readonly ICache cache;

        /// <summary>
        /// Initializes a new instance of the <see cref="ComponentContainer"/> class.
        /// </summary>
        public ComponentContainer()
            : this(new NinjectSettings(), new ExceptionFormatter())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ComponentContainer"/> class.
        /// </summary>
        /// <param name="settings">The ninject settings.</param>
        public ComponentContainer(INinjectSettings settings)
            : this(settings, new ExceptionFormatter())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ComponentContainer"/> class.
        /// </summary>
        /// <param name="settings">The ninject settings.</param>
        /// <param name="exceptionFormatter">The <see cref="IExceptionFormatter"/> component.</param>
        public ComponentContainer(INinjectSettings settings, IExceptionFormatter exceptionFormatter)
        {
            var pipeline = CreatePipeline(exceptionFormatter);

            this.cache = new Cache(pipeline, new NoOpCachePruner());
            this.mappings = new Multimap<Type, IContext>(new ReferenceEqualityTypeComparer());
            this.settings = settings;
            this.exceptionFormatter = exceptionFormatter;
            /*this.contextFactory = this.CreateContextFactory(pipeline);*/
            this.Add(exceptionFormatter);
        }

        /// <summary>
        /// Releases resources held by the object.
        /// </summary>
        /// <param name="disposing"><see langword="true"/> if called manually, otherwise by GC.</param>
        public override void Dispose(bool disposing)
        {
            if (disposing && !this.IsDisposed)
            {
                this.mappings.Clear();
                this.cache.Dispose();
            }

            base.Dispose(disposing);
        }

        /// <summary>
        /// Registers a component in the container.
        /// </summary>
        /// <typeparam name="TComponent">The component type.</typeparam>
        /// <typeparam name="TImplementation">The component's implementation type.</typeparam>
        public void Add<TComponent, TImplementation>()
            where TComponent : INinjectComponent
            where TImplementation : TComponent, INinjectComponent
        {
            this.Add<TComponent, TImplementation>(Array.Empty<IParameter>());
        }

        /// <summary>
        /// Registers a component in the container.
        /// </summary>
        /// <typeparam name="TComponent">The component type.</typeparam>
        /// <typeparam name="TImplementation">The component's implementation type.</typeparam>
        /// <param name="parameters">The parameters to apply when creating the implementation.</param>
        public void Add<TComponent, TImplementation>(params IParameter[] parameters)
            where TComponent : INinjectComponent
            where TImplementation : TComponent, INinjectComponent
        {
            var componentType = typeof(TComponent);
            var implementationType = typeof(TImplementation);
            var context = this.contextFactory.Create(componentType, (ctx) => implementationType, implementationType, parameters);
            this.mappings.Add(componentType, context);
        }

        /// <summary>
        /// Registers a transient component in the container.
        /// </summary>
        /// <typeparam name="TComponent">The component type.</typeparam>
        /// <typeparam name="TImplementation">The component's implementation type.</typeparam>
        public void AddTransient<TComponent, TImplementation>()
            where TComponent : INinjectComponent
            where TImplementation : TComponent, INinjectComponent
        {
            this.Add<TComponent, TImplementation>();
        }

        /// <summary>
        /// Removes all registrations for the specified component.
        /// </summary>
        /// <typeparam name="T">The component type.</typeparam>
        public void RemoveAll<T>()
            where T : INinjectComponent
        {
            this.RemoveAll(typeof(T));
        }

        /// <summary>
        /// Removes the specified registration.
        /// </summary>
        /// <typeparam name="T">The component type.</typeparam>
        /// <typeparam name="TImplementation">The implementation type.</typeparam>
        public void Remove<T, TImplementation>()
            where T : INinjectComponent
            where TImplementation : T
        {
            var implementation = typeof(TImplementation);

            if (this.mappings.TryGetValue(typeof(T), out var contexts))
            {
                for (var i = contexts.Count - 1; i >= 0; i--)
                {
                    if (object.ReferenceEquals(contexts[i].Binding.Service, implementation))
                    {
                        contexts.RemoveAt(i);
                    }
                }

                if (contexts.Count == 0)
                {
                    this.mappings.RemoveAll(typeof(T));
                }
            }
        }

        /// <summary>
        /// Removes all registrations for the specified component.
        /// </summary>
        /// <param name="component">The component type.</param>
        /// <exception cref="ArgumentNullException"><paramref name="component"/> is <see langword="null"/>.</exception>
        public void RemoveAll(Type component)
        {
            Ensure.ArgumentNotNull(component, nameof(component));

            foreach (var context in this.mappings[component])
            {
                var scope = context.GetScope();
                if (scope != null)
                {
                    this.cache.Clear(scope);
                }
            }

            this.mappings.RemoveAll(component);
        }

        /// <summary>
        /// Gets one instance of the specified component.
        /// </summary>
        /// <typeparam name="T">The component type.</typeparam>
        /// <returns>The instance of the component.</returns>
        public T Get<T>()
            where T : INinjectComponent
        {
            var component = typeof(T);

            var implementations = this.mappings[component];
            if (implementations.Count == 0)
            {
                throw new InvalidOperationException(this.exceptionFormatter.NoSuchComponentRegistered(component));
            }

            return (T)this.ResolveInstance(implementations[0]);
        }

        /// <summary>
        /// Gets all available instances of the specified component.
        /// </summary>
        /// <typeparam name="T">The component type.</typeparam>
        /// <returns>
        /// A series of instances of the specified component.
        /// </returns>
        public IEnumerable<T> GetAll<T>()
            where T : INinjectComponent
        {
            return this.GetAll(typeof(T)).Cast<T>();
        }

        /// <summary>
        /// Gets one instance of the specified component.
        /// </summary>
        /// <param name="component">The component type.</param>
        /// <returns>
        /// The instance of the component.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="component"/> is <see langword="null"/>.</exception>
        public object Get(Type component)
        {
            Ensure.ArgumentNotNull(component, nameof(component));

            if (component.IsGenericType)
            {
                var gtd = component.GetGenericTypeDefinition();
                var argument = component.GenericTypeArguments[0];

                if (gtd.IsInterface)
                {
                    if (typeof(IEnumerable<>).IsAssignableFrom(gtd))
                    {
                        return this.GetAll(argument).CastSlow(argument);
                    }
                }
            }

            var implementations = this.mappings[component];
            if (implementations.Count == 0)
            {
                throw new InvalidOperationException(this.exceptionFormatter.NoSuchComponentRegistered(component));
            }

            return this.ResolveInstance(implementations[0]);
        }

        /// <summary>
        /// Gets all available instances of the specified component.
        /// </summary>
        /// <param name="component">The component type.</param>
        /// <returns>
        /// A series of instances of the specified component.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="component"/> is <see langword="null"/>.</exception>
        public IEnumerable<object> GetAll(Type component)
        {
            Ensure.ArgumentNotNull(component, nameof(component));

            return this.mappings[component]
                .Select(implementation => this.ResolveInstance(implementation));
        }

        /// <summary>
        /// Registers an instance of a component in the container.
        /// </summary>
        /// <typeparam name="TComponent">The component type.</typeparam>
        /// <param name="instance">THe instance of <typeparamref name="TComponent"/> to register.</param>
        public void Add<TComponent>(TComponent instance)
            where TComponent : INinjectComponent
        {
            var componentType = typeof(TComponent);
            var context = this.contextFactory.Create(componentType, (ctx) => null, instance);
            this.mappings.Add(componentType, context);
        }

        /// <summary>
        /// Attempts to xxxx.
        /// </summary>
        /// <typeparam name="T">xxxxxx.</typeparam>
        /// <param name="instance">yyyyy.</param>
        /// <returns>
        /// <see langword="true"/> if an instance of <typeparamref name="T"/> was found; otherwise, <see langword="false"/>.
        /// </returns>
        internal bool TryGet<T>(out T instance)
            where T : INinjectComponent
        {
            var component = typeof(T);

            var implementations = this.mappings[component];
            if (implementations.Count == 0)
            {
                instance = default(T);
                return false;
            }

            instance = (T)this.ResolveInstance(implementations[0]);
            return true;
        }

        private static IPipeline CreatePipeline(IExceptionFormatter exceptionFormatter)
        {
            var propertyInjectionStrategy = new PropertyInjectionStrategy(new PropertyValueProvider(), exceptionFormatter);
            var pipelineInitializer = new PipelineInitializer(new List<IInitializationStrategy> { propertyInjectionStrategy });
            var pipelineDeactivator = new PipelineDeactivator(new List<IDeactivationStrategy> { new DisposableStrategy() });

            return new DefaultPipeline(pipelineInitializer, new NoOpPipelineActivator(), pipelineDeactivator);
        }

        private object ResolveInstance(IContext context)
        {
            return context.Resolve();
        }

        private class NoOpCachePruner : ICachePruner
        {
            public void Dispose()
            {
            }

            public void Start(IPruneable cache)
            {
            }

            public void Stop()
            {
            }
        }
    }
}