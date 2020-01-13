// -------------------------------------------------------------------------------------------------
// <copyright file="DefaultValueBindingResolver.cs" company="Ninject Project Contributors">
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
    using Ninject.Components;
    using Ninject.Infrastructure;
    using Ninject.Planning.Targets;

    /// <summary>
    /// Represents a binding resolver that takes the target default value as the resolved object.
    /// </summary>
    public class DefaultValueBindingResolver : NinjectComponent, IMissingBindingResolver
    {
        /// <summary>
        /// Determines whether the specified request can be resolved.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>
        /// <see langword="true"/> if the request can be resolved; otherwise, <see langword="false"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="request"/> is <see langword="null"/>.</exception>
        public bool CanResolve(IRequest request)
        {
            Ensure.ArgumentNotNull(request, nameof(request));

            return HasDefaultValue(request.Target);
        }

        /// <summary>
        /// Returns any bindings from the specified collection that match the specified service.
        /// </summary>
        /// <param name="bindings">The dictionary of all registered bindings.</param>
        /// <param name="request">The service in question.</param>
        /// <returns>The series of matching bindings.</returns>
        public IEnumerable<IBinding> Resolve(IDictionary<Type, ICollection<IBinding>> bindings, IRequest request)
        {
            var service = request.Service;

            if (!HasDefaultValue(request.Target))
            {
                return Enumerable.Empty<IBinding>();
            }

            return new[]
                {
                    new Binding(service)
                    {
                        Condition = r => HasDefaultValue(r.Target),
                        Provider = new DefaultParameterValueProvider(service)
                    },
                };
        }

        private static bool HasDefaultValue(ITarget target)
        {
            return target != null && target.HasDefaultValue;
        }

        private class DefaultParameterValueProvider : IProvider
        {
            public DefaultParameterValueProvider(Type type)
            {
                this.Type = type;
            }

            /// <summary>
            /// Gets a value indicating whether the provider uses Ninject to resolve services when creating an instance.
            /// </summary>
            /// <value>
            /// <see langword="true"/> if the provider uses Ninject to resolve service when creating an instance; otherwise,
            /// <see langword="false"/>.
            /// </value>
            public bool ResolvesServices => true;

            public Type Type { get; private set; }

            public object Create(IContext context, out bool isInitialized)
            {
                isInitialized = true;

                var target = context.Request.Target;
                return target?.DefaultValue;
            }
        }
    }
}