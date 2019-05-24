// -------------------------------------------------------------------------------------------------
// <copyright file="ComponentContext.cs" company="Ninject Project Contributors">
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

    using Ninject.Activation;
    using Ninject.Activation.Caching;
    using Ninject.Infrastructure.Language;
    using Ninject.Parameters;
    using Ninject.Planning;
    using Ninject.Planning.Bindings;

    /// <summary>
    /// Context for resolving an <see cref="INinjectComponent"/>.
    /// </summary>
    internal class ComponentContext : IContext
    {
        /// <summary>
        /// The <see cref="IExceptionFormatter"/> component.
        /// </summary>
        private readonly IExceptionFormatter exceptionFormatter;

        /// <summary>
        /// Initializes a new instance of the <see cref="ComponentContext"/> class.
        /// </summary>
        /// <param name="plan">The <see cref="IPlan"/> component.</param>
        /// <param name="request">The request.</param>
        /// <param name="binding">The binding.</param>
        /// <param name="cache">The <see cref="ICache"/> component.</param>
        /// <param name="exceptionFormatter">The <see cref="IExceptionFormatter"/> component.</param>
        public ComponentContext(IPlan plan, IRequest request, IBinding binding, ICache cache, IExceptionFormatter exceptionFormatter)
        {
            this.Plan = plan;
            this.Request = request;
            this.Binding = binding;
            this.Cache = cache;
            this.exceptionFormatter = exceptionFormatter;
            this.Parameters = request.Parameters.Concat(binding.Parameters);
        }

        /// <summary>
        /// Gets the kernel that is driving the activation.
        /// </summary>
        public IReadOnlyKernel Kernel => throw new NotImplementedException();

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
        /// Gets the cache component.
        /// </summary>
        public ICache Cache { get; }

        /// <summary>
        /// Gets the parameters that were passed to manipulate the activation process.
        /// </summary>
        public IReadOnlyList<IParameter> Parameters { get; }

        /// <summary>
        /// Gets the generic arguments for the request, if any.
        /// </summary>
        public Type[] GenericArguments => throw new NotImplementedException();

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
                return this.Request.Service.IsGenericTypeDefinition;
            }
        }

        /// <summary>
        /// Gets the provider that should be used to create the instance for this context.
        /// </summary>
        /// <value>
        /// The provider that should be used.
        /// </value>
        public IProvider Provider
        {
            get
            {
                return this.Binding.Provider;
            }
        }

        /// <summary>
        /// Gets the scope for the context that "owns" the instance activated therein.
        /// </summary>
        /// <returns>
        /// The object that acts as the scope.
        /// </returns>
        public object GetScope()
        {
            return this.Binding.GetScope(this);
        }

        /// <summary>
        /// Resolves this instance for this context.
        /// </summary>
        /// <returns>
        /// The resolved instance.
        /// </returns>
        public object Resolve()
        {
            var scope = this.GetScope();

            if (scope != null)
            {
                return this.ResolveInScope(scope);
            }

            return this.ResolveWithoutScope();
        }

        private object ResolveWithoutScope()
        {
            return this.Provider.Create(this);
        }

        private object ResolveInScope(object scope)
        {
            var cachedInstance = this.Cache.TryGet(this, scope);
            if (cachedInstance != null)
            {
                return cachedInstance;
            }

            var reference = new InstanceReference { Instance = this.Provider.Create(this) };
            this.Cache.Remember(this, scope, reference);
            return reference.Instance;
        }
    }
}