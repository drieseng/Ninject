// -------------------------------------------------------------------------------------------------
// <copyright file="ComponentKernel.cs" company="Ninject Project Contributors">
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
    using Ninject.Parameters;
    using Ninject.Planning.Bindings;
    using Ninject.Syntax;

    /// <summary>
    /// A <see cref="IReadOnlyKernel"/> for resolving <see cref="INinjectComponent"/> instances.
    /// </summary>
    internal class ComponentKernel : IReadOnlyKernel
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
            throw new NotImplementedException();
        }

        /// <summary>
        /// Determines whether the specified request can be resolved.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="ignoreImplicitBindings">if set to <see langword="true"/> implicit bindings are ignored.</param>
        /// <returns>
        /// <see langword="true"/> if the request can be resolved; otherwise, <see langword="false"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="request"/> is <see langword="null"/>.</exception>
        public bool CanResolve(IRequest request, bool ignoreImplicitBindings)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates a request for the specified service.
        /// </summary>
        /// <param name="service">The service that is being requested.</param>
        /// <param name="constraint">The constraint to apply to the bindings to determine if they match the request.</param>
        /// <param name="parameters">The parameters to pass to the resolution.</param>
        /// <param name="isOptional"><see langword="true"/> if the request is optional; otherwise, <see langword="false"/>.</param>
        /// <param name="isUnique"><see langword="true"/> if the request should return a unique result; otherwise, <see langword="false"/>.</param>
        /// <returns>
        /// The request for the specified service.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="service"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="parameters"/> is <see langword="null"/>.</exception>
        public IRequest CreateRequest(Type service, Func<IBindingMetadata, bool> constraint, IReadOnlyList<IParameter> parameters, bool isOptional, bool isUnique)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets an instance of the specified service.
        /// </summary>
        /// <param name="service">The service to resolve.</param>
        /// <returns>
        /// An instance of the service.
        /// </returns>
        public object Get(Type service)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the bindings registered for the specified service.
        /// </summary>
        /// <param name="service">The service in question.</param>
        /// <returns>
        /// A series of bindings that are registered for the service.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="service"/> is <see langword="null"/>.</exception>
        public IBinding[] GetBindings(Type service)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the service object of the specified type.
        /// </summary>
        /// <param name="serviceType">The service type.</param>
        /// <returns>
        /// The service object.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="serviceType"/> is <see langword="null"/>.</exception>
        object IServiceProvider.GetService(Type serviceType)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Injects the specified existing instance, without managing its lifecycle.
        /// </summary>
        /// <param name="instance">The instance to inject.</param>
        /// <param name="parameters">The parameters to pass to the request.</param>
        /// <exception cref="ArgumentNullException"><paramref name="instance"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="parameters"/> is <see langword="null"/>.</exception>
        void IResolutionRoot.Inject(object instance, params IParameter[] parameters)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Deactivates and releases the specified instance if it is currently managed by Ninject.
        /// </summary>
        /// <param name="instance">The instance to release.</param>
        /// <returns>
        /// <see langword="true"/> if the instance was found and released; otherwise, <see langword="false"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="instance"/> is <see langword="null"/>.</exception>
        public bool Release(object instance)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Resolves instances for the specified request. The instances are not actually resolved
        /// until a consumer iterates over the enumerator.
        /// </summary>
        /// <param name="request">The request to resolve.</param>
        /// <returns>
        /// An enumerator of instances that match the request.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="request"/> is <see langword="null"/>.</exception>
        public IEnumerable<object> Resolve(IRequest request)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Resolves an instance for the specified request.
        /// </summary>
        /// <param name="request">The request to resolve.</param>
        /// <returns>
        /// An instance that matches the request.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="request"/> is <see langword="null"/>.</exception>
        /// <exception cref="ActivationException">More than one matching bindings is available for the request, and <see cref="IRequest.IsUnique"/> is <see langword="true"/>.</exception>
        public object ResolveSingle(IRequest request)
        {
            throw new NotImplementedException();
        }
    }
}