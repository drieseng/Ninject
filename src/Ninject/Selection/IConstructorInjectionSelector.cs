// -------------------------------------------------------------------------------------------------
// <copyright file="IConstructorInjectionSelector.cs" company="Ninject Project Contributors">
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
    using Ninject.Activation;
    using Ninject.Components;
    using Ninject.Planning;
    using Ninject.Planning.Directives;

    /// <summary>
    /// Selects the constructor to use for creating an instance of a service.
    /// </summary>
    public interface IConstructorInjectionSelector : INinjectComponent
    {
        /// <summary>
        /// Selects the constructor to create an instance of <see cref="IPlan.Type"/> in the specified
        /// context.
        /// </summary>
        /// <param name="plan">The plan.</param>
        /// <param name="context">The context.</param>
        /// <returns>
        /// The selected constructor.
        /// </returns>
        IConstructorInjectionDirective Select(IPlan plan, IContext context);
    }
}