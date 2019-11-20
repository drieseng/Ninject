// -------------------------------------------------------------------------------------------------
// <copyright file="ParameterlessConstructorProvider.cs" company="Ninject Project Contributors">
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

namespace Ninject.Activation.Providers
{
    using System;

    using Ninject.Planning;
    using Ninject.Planning.Directives;

    /// <summary>
    /// Provides instances using their respective parameterless constructor.
    /// </summary>
    internal sealed class ParameterlessConstructorProvider : StandardProviderBase
    {
        private readonly IConstructorInjectionDirective constructor;

        /// <summary>
        /// Initializes a new instance of the <see cref="ParameterlessConstructorProvider"/> class.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="constructor">The parameterless constructor.</param>
        /// <param name="plan">The plan.</param>
        public ParameterlessConstructorProvider(Type type, IConstructorInjectionDirective constructor, IPlan plan)
            : base(plan)
        {
            this.Type = type;
            this.constructor = constructor;
        }

        /// <summary>
        /// Gets a value indicating whether the provider uses Ninject to resolve services when creating an instance.
        /// </summary>
        /// <value>
        /// <see langword="true"/> if the provider uses Ninject to resolve service when creating an instance; otherwise,
        /// <see langword="false"/>.
        /// </value>
        public override bool ResolvesServices => false;

        /// <summary>
        /// Gets the type of instances the provider creates.
        /// </summary>
        /// <value>
        /// The type of instances the provider creates.
        /// </value>
        public override Type Type { get; }

        /// <summary>
        /// Creates an instance using the parameterless constructor that was specified when the current instance
        /// was initialized.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="isInitialized"><see langword="true"/> if the created instance is fully initialized; otherwise, <see langword="false"/></param>
        /// <returns>
        /// The created instance.
        /// </returns>
        protected override object CreateInstance(IContext context, out bool isInitialized)
        {
            isInitialized = false;

            return this.constructor.Injector(Array.Empty<object>());
        }
    }
}