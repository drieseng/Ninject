// -------------------------------------------------------------------------------------------------
// <copyright file="IPropertyInjectionSelector.cs" company="Ninject Project Contributors">
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
    using Ninject.Components;
    using System;
    using System.Reflection;

    /// <summary>
    /// Defines whether a given property should effectively be injected during the initialization of a type.
    /// </summary>
    public interface IPropertyInjectionHeuristic : INinjectComponent
    {
        /// <summary>
        /// Returns a value indicating whether a property should be injected during the initialization
        /// of a type.
        /// </summary>
        /// <param name="type">The type being initialized, and for which the property should be injected.</param>
        /// <param name="property">The property to take a decision for.</param>
        /// <returns>
        /// <see langword="true"/> if the property should be injected; otherwise, <see langword="false"/>.
        /// </returns>
        bool ShouldInject(Type type, PropertyInfo property);
    }
}