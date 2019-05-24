// -------------------------------------------------------------------------------------------------
// <copyright file="SpecificConstructorInjectionSelector.cs" company="Ninject Project Contributors">
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

namespace Ninject.Selection
{
    using System;

    using Ninject.Activation;
    using Ninject.Infrastructure;
    using Ninject.Planning;
    using Ninject.Planning.Directives;

    /// <summary>
    /// An <see cref="IConstructorInjectionSelector"/> that always returns a predefined <see cref="ConstructorInjectionDirective"/>.
    /// </summary>
    internal class SpecificConstructorInjectionSelector : IConstructorInjectionSelector
    {
        private readonly ConstructorInjectionDirective constructor;

        /// <summary>
        /// Initializes a new instance of the <see cref="SpecificConstructorInjectionSelector"/> class.
        /// </summary>
        /// <param name="constructor">The constructor.</param>
        /// <exception cref="ArgumentNullException"><paramref name="constructor"/> is <see langword="null"/>.</exception>
        public SpecificConstructorInjectionSelector(ConstructorInjectionDirective constructor)
        {
            Ensure.ArgumentNotNull(constructor, nameof(constructor));

            this.constructor = constructor;
        }

        /// <summary>
        /// Selects the constructor to create an instance of <see cref="IPlan.Type"/> in the specified
        /// context.
        /// </summary>
        /// <param name="plan">The plan.</param>
        /// <param name="context">The context.</param>
        /// <returns>
        /// The selected constructor.
        /// </returns>
        public ConstructorInjectionDirective Select(IPlan plan, IContext context)
        {
            return this.constructor;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        void IDisposable.Dispose()
        {
        }
    }
}