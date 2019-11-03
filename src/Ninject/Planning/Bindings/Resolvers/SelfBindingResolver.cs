// -------------------------------------------------------------------------------------------------
// <copyright file="SelfBindingResolver.cs" company="Ninject Project Contributors">
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

namespace Ninject.Planning.Bindings.Resolvers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Ninject.Activation;
    using Ninject.Activation.Providers;
    using Ninject.Builder;
    using Ninject.Components;
    using Ninject.Infrastructure;
    using Ninject.Selection;
    using Ninject.Selection.Heuristics;

    /// <summary>
    /// Represents a binding resolver that uses the service in question itself as the target to activate.
    /// </summary>
    public class SelfBindingResolver : NinjectComponent, IMissingBindingResolver
    {
        private readonly IPlanner planner;
        private readonly IPipeline pipeline;
        private readonly IConstructorInjectionSelector constructorSelector;
        private readonly IConstructorParameterValueProvider constructorParameterValueProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="SelfBindingResolver"/> class.
        /// </summary>
        /// <param name="planner">The <see cref="IPlanner"/> component.</param>
        /// <param name="pipeline">The <see cref="IPipeline"/> component.</param>
        /// <param name="constructorSelector">The <see cref="IConstructorInjectionSelector"/> component.</param>
        /// <param name="constructorParameterValueProvider">The value provider.</param>
        public SelfBindingResolver(IPlanner planner, IPipeline pipeline, IConstructorInjectionSelector constructorSelector, IConstructorParameterValueProvider constructorParameterValueProvider)
        {
            Ensure.ArgumentNotNull(planner, nameof(planner));
            Ensure.ArgumentNotNull(pipeline, nameof(pipeline));
            Ensure.ArgumentNotNull(constructorSelector, nameof(constructorSelector));
            Ensure.ArgumentNotNull(constructorParameterValueProvider, nameof(constructorParameterValueProvider));

            this.planner = planner;
            this.pipeline = pipeline;
            this.constructorSelector = constructorSelector;
            this.constructorParameterValueProvider = constructorParameterValueProvider;
        }

        /// <summary>
        /// Returns any bindings from the specified collection that match the specified service.
        /// </summary>
        /// <param name="bindings">The dictionary of all registered bindings.</param>
        /// <param name="request">The service in question.</param>
        /// <returns>
        /// The series of matching bindings.
        /// </returns>
        public IEnumerable<IBinding> Resolve(IDictionary<Type, ICollection<IBinding>> bindings, IRequest request)
        {
            var service = request.Service;

            if (!this.TypeIsSelfBindable(service))
            {
                return Enumerable.Empty<IBinding>();
            }

            return new[]
            {
                new Binding(service)
                {
                    Provider = new SelfBindingProvider(this.planner.GetPlan(service),
                                                       this.pipeline,
                                                       this.constructorSelector,
                                                       this.constructorParameterValueProvider),
                },
            };
        }

        /// <summary>
        /// Returns a value indicating whether the specified service is self-bindable.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <returns>
        /// <see langword="true"/> if the type is self-bindable; otherwise, <see langword="false"/>.
        /// </returns>
        protected virtual bool TypeIsSelfBindable(Type service)
        {
            return !service.IsInterface &&
                   !service.IsAbstract &&
                   !service.IsValueType &&
                   service != typeof(string) &&
                   !service.ContainsGenericParameters;
        }

        private sealed class SelfBindingProvider : StandardProviderBase
        {
            private readonly IConstructorInjectionSelector constructorSelector;
            private readonly IConstructorParameterValueProvider constructorParameterValueProvider;

            public SelfBindingProvider(IPlan plan,
                                       IPipeline pipeline,
                                       IConstructorInjectionSelector constructorSelector,
                                       IConstructorParameterValueProvider constructorParameterValueProvider)
                : base(plan, pipeline)
            {
                this.constructorSelector = constructorSelector;
                this.constructorParameterValueProvider = constructorParameterValueProvider;
            }

            /// <summary>
            /// Gets a value indicating whether the provider uses Ninject to resolve services when creating an instance.
            /// </summary>
            /// <value>
            /// <see langword="true"/> if the provider uses Ninject to resolve service when creating an instance; otherwise,
            /// <see langword="false"/>.
            /// </value>
            public override bool ResolvesServices => true;

            public override Type Type => this.Plan.Type;

            protected override object CreateInstance(IContext context, out bool isInitialized)
            {
                isInitialized = false;

                var directive = constructorSelector.Select(context.Plan, context);
                var values = constructorParameterValueProvider.GetValues(directive, context);
                return directive.Injector(values);
            }
        }
    }
}