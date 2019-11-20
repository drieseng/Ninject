// -------------------------------------------------------------------------------------------------
// <copyright file="Context.cs" company="Ninject Project Contributors">
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

#define CYCLIC

namespace Ninject.Activation
{
    using System;
    using System.Collections.Generic;

    using Ninject.Activation.Caching;
    using Ninject.Components;
    using Ninject.Infrastructure;
    using Ninject.Infrastructure.Language;
    using Ninject.Parameters;
    using Ninject.Planning;
    using Ninject.Planning.Bindings;
    using Ninject.Planning.Targets;

    /// <summary>
    /// Contains information about the activation of a single instance.
    /// </summary>
    public sealed class Context : IContext
    {
        /// <summary>
        /// The <see cref="IExceptionFormatter"/> component.
        /// </summary>
        private readonly IExceptionFormatter exceptionFormatter;

        /// <summary>
        /// The cached scope object.
        /// </summary>
        private object cachedScope;

        /// <summary>
        /// Initializes a new instance of the <see cref="Context"/> class.
        /// </summary>
        /// <param name="kernel">The kernel managing the resolution.</param>
        /// <param name="request">The context's request.</param>
        /// <param name="binding">The context's binding.</param>
        /// <param name="cache">The cache component.</param>
        /// <param name="planner">The planner component.</param>
        /// <param name="pipeline">The pipeline component.</param>
        /// <param name="exceptionFormatter">The <see cref="IExceptionFormatter"/> component.</param>
        /// <param name="allowNullInjection"><see langword="true"/> if <see langword="null"/> is allowed as injected value; otherwise, <see langword="false"/>.</param>
        /// <param name="detectCyclicDependencies"><see langword="true"/> if cyclic dependencies should be detected; otherwise, <see langword="false"/>.</param>
        /// <exception cref="ArgumentNullException"><paramref name="kernel"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="request"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="binding"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="cache"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="planner"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="pipeline"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="exceptionFormatter"/> is <see langword="null"/>.</exception>
        public Context(IReadOnlyKernel kernel,
                       IRequest request,
                       IBinding binding,
                       ICache cache,
                       IPlanner planner,
                       IPipeline pipeline,
                       IExceptionFormatter exceptionFormatter,
                       bool allowNullInjection,
                       bool detectCyclicDependencies)
        {
            Ensure.ArgumentNotNull(kernel, nameof(kernel));
            Ensure.ArgumentNotNull(cache, nameof(cache));
            Ensure.ArgumentNotNull(planner, nameof(planner));
            Ensure.ArgumentNotNull(pipeline, nameof(pipeline));
            Ensure.ArgumentNotNull(exceptionFormatter, nameof(exceptionFormatter));
            Ensure.ArgumentNotNull(request, nameof(request));
            Ensure.ArgumentNotNull(binding, nameof(binding));

            this.Kernel = kernel;
            this.Request = request;
            this.Binding = binding;
            this.Cache = cache;
            this.Planner = planner;
            this.Pipeline = pipeline;
            this.Parameters = request.Parameters.Concat(binding.Parameters);
            this.exceptionFormatter = exceptionFormatter;
            this.AllowNullInjection = allowNullInjection;
            this.DetectCyclicDependencies = detectCyclicDependencies;
        }

        /// <summary>
        /// Gets the kernel that is driving the activation.
        /// </summary>
        public IReadOnlyKernel Kernel { get; }

        /// <summary>
        /// Gets the request.
        /// </summary>
        public IRequest Request { get; }

        /// <summary>
        /// Gets the binding.
        /// </summary>
        public IBinding Binding { get; }

        /// <summary>
        /// Gets or sets the activation plan.
        /// </summary>
        public IPlan Plan { get; set; }

        /// <summary>
        /// Gets a value indicating whether <see langword="null"/> is a valid value for injection.
        /// </summary>
        /// <value>
        /// <see langword="true"/> if <see langword="null"/> is allowed as injected value; otherwise, <see langword="false"/>.
        /// The default is <see langword="false"/>.
        /// </value>
        /// <remarks>
        /// When <see langword="false"/>, an <see cref="ActivationException"/> is thrown whenever a provider returns <see langword="null"/>.
        /// </remarks>
        public bool AllowNullInjection { get; }

        /// <summary>
        /// Gets a value whether cyclic dependencies should be detected.
        /// </summary>
        /// <value>
        /// <see langword="true"/> if cyclic dependencies should be detected; otherwise, <see langword="false"/>.
        /// The default is <see langword="true"/>.
        /// </value>
        /// <remarks>
        /// <para>
        /// When <see langword="true"/>, an <see cref="ActivationException"/> is thrown whenever a cyclic dependency
        /// is detected.
        /// </para>
        /// <para>
        /// When <see cref="DetectCyclicDependencies"/> is <see langword="false"/>, the CLR may throw a
        /// <see cref="StackOverflowException"/> and terminate the process in case of cyclic dependencies.
        /// </para>
        /// </remarks>
        public bool DetectCyclicDependencies { get; }

        /// <summary>
        /// Gets the provider that should be used to create the instance for this context.
        /// </summary>
        /// <value>
        /// The provider that should be used.
        /// </value>
        public IProvider Provider
        {
            get { return this.Binding.Provider; }
        }

        /// <summary>
        /// Gets or sets the parameters that were passed to manipulate the activation process.
        /// </summary>
        public IReadOnlyList<IParameter> Parameters { get; set; }

        /// <summary>
        /// Gets the generic arguments for the request, if any.
        /// </summary>
        public Type[] GenericArguments
        {
            get
            {
                return this.Request.Service.GenericTypeArguments;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the request involves inferred generic arguments.
        /// </summary>
        /// <value>
        /// <see langword="true"/> if the request involves inferred generic arguments; otherwise, <see langword="false"/>.
        /// </value>
        public bool HasInferredGenericArguments
        {
            get
            {
                return this.Binding.Service.IsGenericTypeDefinition;
            }
        }

        /// <summary>
        /// Gets the cache component.
        /// </summary>
        public ICache Cache { get; }

        /// <summary>
        /// Gets the planner component.
        /// </summary>
        public IPlanner Planner { get; }

        /// <summary>
        /// Gets the pipeline component.
        /// </summary>
        public IPipeline Pipeline { get; }

        /// <summary>
        /// Gets the scope for the context that "owns" the instance activated therein.
        /// </summary>
        /// <returns>
        /// The object that acts as the scope.
        /// </returns>
        public object GetScope()
        {
            return this.cachedScope ?? this.Request.GetScope() ?? this.Binding.GetScope(this);
        }

        /// <summary>
        /// Resolves the instance associated with this hook.
        /// </summary>
        /// <returns>
        /// The resolved instance.
        /// </returns>
        public object Resolve()
        {
            try
            {
                this.cachedScope = this.Request.GetScope() ?? this.Binding.GetScope(this);
                if (this.cachedScope != null)
                {
                    return this.ResolveInScope(this.cachedScope);
                }
                else
                {
                    return this.ResolveWithoutScope();
                }
            }
            finally
            {
                this.cachedScope = null;
            }
        }

        private static bool IsCyclical(IRequest request, ITarget target)
        {
            if (request == null)
            {
                return false;
            }

            if (request.Target == target)
            {
                return true;
            }

            return IsCyclical(request.ParentRequest, target);
        }

        private object ResolveWithoutScope()
        {
            if (DetectCyclicDependencies && this.Provider.ResolvesServices)
            {
                EnsureNotCyclic();

                this.Request.ActiveBindings.Push(this.Binding);
            }

            var instance = this.Provider.Create(this, out var isInitialized);

            if (DetectCyclicDependencies && this.Provider.ResolvesServices)
            {
                this.Request.ActiveBindings.Pop();
            }

            if (instance == null && !AllowNullInjection)
            {
                throw new ActivationException(this.exceptionFormatter.ProviderReturnedNull(this));
            }

            // Ensure Plan is set, in case (custom) provider did not.
            if (this.Plan == null)
            {
                this.Plan = this.Planner.GetPlan(this.Request.Service);
            }

            // Only pass the instance through the initialization pipeline if the provider doesn't consider
            // the instance fully initialized.
            if (!isInitialized)
            {
                // Pass the instance through the initialization pipeline which may alter both the instance
                // and the plan in the context.
                instance = this.Pipeline.Initialize(this, instance);
            }

            return instance;
        }

        private object ResolveInScope(object scope)
        {
            lock (scope)
            {
                var cachedInstance = this.Cache.TryGet(this, scope);
                if (cachedInstance != null)
                {
                    return cachedInstance;
                }

                if (DetectCyclicDependencies && this.Provider.ResolvesServices)
                {
                    EnsureNotCyclic();

                    this.Request.ActiveBindings.Push(this.Binding);
                }

                var reference = new InstanceReference { Instance = this.Provider.Create(this, out var isInitialized) };

                if (DetectCyclicDependencies && this.Provider.ResolvesServices)
                {
                    this.Request.ActiveBindings.Pop();
                }

                if (reference.Instance == null)
                {
                    if (!AllowNullInjection)
                    {
                        throw new ActivationException(this.exceptionFormatter.ProviderReturnedNull(this));
                    }

                    // Ensure Plan is set, in case (custom) provider did not.
                    if (this.Plan == null)
                    {
                        this.Plan = this.Planner.GetPlan(this.Request.Service);
                    }

                    return null;
                }

                // Ensure Plan is set, in case (custom) provider did not.
                if (this.Plan == null)
                {
                    this.Plan = this.Planner.GetPlan(this.Request.Service);
                }

                // Add the instance to the cache before passing it in the initialization pipeline, to make the
                // instance available to be injected into properties or methods of dependent services.
                this.Cache.Remember(this, scope, reference);

                if (!isInitialized)
                {
                    // Pass the instance through the initialization pipeline which may alter both the instance
                    // and the plan in the context.
                    reference.Instance = this.Pipeline.Initialize(this, reference.Instance);
                }

                this.Pipeline.Activate(this, reference);

                return reference.Instance;
            }
        }

        private void EnsureNotCyclic()
        {
            if (this.Request.ActiveBindings.Contains(this.Binding) &&
                IsCyclical(this.Request.ParentRequest, this.Request.Target))
            {
                throw new ActivationException(this.exceptionFormatter.CyclicalDependenciesDetected(this));
            }
        }
    }
}