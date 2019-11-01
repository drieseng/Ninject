// -------------------------------------------------------------------------------------------------
// <copyright file="ModuleLoadExtensions.cs" company="Ninject Project Contributors">
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

namespace Ninject
{
    using System;
    using System.Reflection;
    using Ninject.Builder;
    using Ninject.Infrastructure;
    using Ninject.Modules;

    /// <summary>
    /// Extension methods that enhance module loading.
    /// </summary>
    public static class ModuleLoadExtensions
    {
        /// <summary>
        /// Creates a new instance of the module and loads it into a kernel.
        /// </summary>
        /// <typeparam name="TModule">The type of the module.</typeparam>
        /// <param name="moduleLoader">The module loader into which the module is loaded.</param>
        /// <exception cref="ArgumentNullException"><paramref name="moduleLoader"/> is <see langword="null"/>.</exception>
        public static void Load<TModule>(this IModuleLoader moduleLoader)
            where TModule : INinjectModule, new()
        {
            Ensure.ArgumentNotNull(moduleLoader, nameof(moduleLoader));

            moduleLoader.Load(new TModule());
        }
    }
}