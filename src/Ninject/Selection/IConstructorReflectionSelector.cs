﻿// -------------------------------------------------------------------------------------------------
// <copyright file="IConstructorReflectionSelector.cs" company="Ninject Project Contributors">
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
    using System.Reflection;

    using Ninject.Components;

    /// <summary>
    /// Selects constructors to inject services into.
    /// </summary>
    public interface IConstructorReflectionSelector : INinjectComponent
    {
        /// <summary>
        /// Selects the constructors that could be injected into.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>
        /// A series of the selected constructors.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="type"/> is <see langword="null"/>.</exception>
        ConstructorInfo[] Select(Type type);
    }
}