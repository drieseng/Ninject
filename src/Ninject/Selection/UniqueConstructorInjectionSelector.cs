// -------------------------------------------------------------------------------------------------
// <copyright file="UniqueConstructorInjectionSelector.cs" company="Ninject Project Contributors">
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
    using System.Collections.Generic;

    using Ninject.Activation;
    using Ninject.Planning;
    using Ninject.Planning.Directives;

    /// <summary>
    /// Selects the only constructor from a given <see cref="IPlan"/>.
    /// </summary>
    public sealed class UniqueConstructorInjectionSelector : IConstructorInjectionSelector
    {
        /// <summary>
        /// Select the only constructor from the specified <see cref="IPlan"/>.
        /// </summary>
        /// <param name="plan">The plan to select the constructor from.</param>
        /// <param name="context">The context.</param>
        /// <returns>
        /// The selected constructor.
        /// </returns>
        /// <exception cref="ActivationException"><paramref name="plan"/> defines zero or more than one constructor.</exception>
        public IConstructorInjectionDirective Select(IPlan plan, IContext context)
        {
            var constructors = plan.GetConstructors();
            return CreateConstructorInjectionDirective(constructors);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        void IDisposable.Dispose()
        {
        }

        private static IConstructorInjectionDirective CreateConstructorInjectionDirective(IReadOnlyList<IConstructorInjectionDirective> constructors)
        {
            if (constructors.Count == 0)
            {
                throw new ActivationException("NO PUBLIC CONSTRUCTORS");
            }

            if (constructors.Count > 1)
            {
                throw new ActivationException("MORE THAN ONE PUBLIC CONSTRUCTORS");
            }

            return constructors[0];
        }
    }
}