// -------------------------------------------------------------------------------------------------
// <copyright file="BindingConfiguration.cs" company="Ninject Project Contributors">
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

namespace Ninject.Planning.Bindings
{
    using System;
    using System.Collections.Generic;

    using Ninject.Activation;
    using Ninject.Infrastructure;
    using Ninject.Parameters;

    /// <summary>
    /// The configuration of a binding.
    /// </summary>
    public sealed class BindingConfiguration : IBindingConfiguration
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BindingConfiguration"/> class.
        /// </summary>
        public BindingConfiguration()
        {
            this.Metadata = new BindingMetadata();
            this.Parameters = new List<IParameter>();
            this.InitializationActions = new List<Func<IContext, object, object>>();
            this.ActivationActions = new List<Action<IContext, object>>();
            this.DeactivationActions = new List<Action<IContext, object>>();
            this.ScopeCallback = StandardScopeCallbacks.Transient;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BindingConfiguration"/> class.
        /// </summary>
        /// <param name="parameters">The parameters defined for the binding.</param>
        /// <param name="metadata">The binding's metadata.</param>
        /// <param name="condition">The condition defined for the binding.</param>
        /// <param name="provider">The provider that should be used by the binding.</param>
        /// <param name="scopeCallback">The callback that returns the object that will act as the binding's scope.</param>
        /// <param name="initializationActions">The actions that contribute to the initialization of instances that are initialized via the binding.</param>
        /// <param name="activationActions">The actions that should be called after instances are activated via the binding.</param>
        /// <param name="deactivationActions">The actions that should be called before instances are deactivated via the binding.</param>
        /// <param name="target">The type of target for the binding.</param>
        internal BindingConfiguration(IList<IParameter> parameters,
                                      IBindingMetadata metadata,
                                      Func<IRequest, bool> condition,
                                      IProvider provider,
                                      Func<IContext, object> scopeCallback,
                                      List<Func<IContext, object, object>> initializationActions,
                                      List<Action<IContext, object>> activationActions,
                                      List<Action<IContext, object>> deactivationActions,
                                      BindingTarget target)
        {
            this.Metadata = metadata;
            this.Parameters = parameters;
            this.Condition = condition;
            this.Provider = provider;
            this.InitializationActions = initializationActions;
            this.ActivationActions = activationActions;
            this.DeactivationActions = deactivationActions;
            this.ScopeCallback = scopeCallback;
            this.Target = target;
        }

        /// <summary>
        /// Gets the binding's metadata.
        /// </summary>
        public IBindingMetadata Metadata { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether the binding was implicitly registered.
        /// </summary>
        public bool IsImplicit { get; set; }

        /// <summary>
        /// Gets a value indicating whether the binding has a condition associated with it.
        /// </summary>
        public bool IsConditional
        {
            get { return this.Condition != null; }
        }

        /// <summary>
        /// Gets or sets the type of target for the binding.
        /// </summary>
        public BindingTarget Target { get; set; }

        /// <summary>
        /// Gets or sets the condition defined for the binding.
        /// </summary>
        public Func<IRequest, bool> Condition { get; set; }

        /// <summary>
        /// Gets or sets the provider that should be used by the binding.
        /// </summary>
        public IProvider Provider { get; set; }

        /// <summary>
        /// Gets or sets the callback that returns the object that will act as the binding's scope.
        /// </summary>
        public Func<IContext, object> ScopeCallback { get; set; }

        /// <summary>
        /// Gets the parameters defined for the binding.
        /// </summary>
        public IList<IParameter> Parameters { get; private set; }

        /// <summary>
        /// Gets the actions that contribute to the initialization of instances that are initialized via the binding.
        /// </summary>
        public ICollection<Func<IContext, object, object>> InitializationActions { get; private set; }

        /// <summary>
        /// Gets the actions that should be called after instances are activated via the binding.
        /// </summary>
        public ICollection<Action<IContext, object>> ActivationActions { get; private set; }

        /// <summary>
        /// Gets the actions that should be called before instances are deactivated via the binding.
        /// </summary>
        public ICollection<Action<IContext, object>> DeactivationActions { get; private set; }

        /// <summary>
        /// Gets the scope for the binding, if any.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>
        /// The object that will act as the scope, or <see langword="null"/> if the service is transient.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="context"/> is <see langword="null"/>.</exception>
        public object GetScope(IContext context)
        {
            if (context == null)
            {
                Ensure.ThrowArgumentNotNull(nameof(context));
            }

            return this.ScopeCallback(context);
        }

        /// <summary>
        /// Determines whether the specified request satisfies the conditions defined on this binding.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>
        /// <see langword="true"/> if the request satisfies the conditions; otherwise, <see langword="false"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="request"/> is <see langword="null"/>.</exception>
        public bool Matches(IRequest request)
        {
            Ensure.ArgumentNotNull(request, nameof(request));

            return this.Condition == null || this.Condition(request);
        }
    }
}