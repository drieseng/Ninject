// -------------------------------------------------------------------------------------------------
// <copyright file="INinjectModule.cs" company="Ninject Project Contributors">
//   Copyright (c) 2007-2010 Enkari, Ltd. All rights reserved.
//   Copyright (c) 2010-2017 Ninject Project Contributors. All rights reserved.
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

using Ninject.Builder;
using System;

namespace Ninject.Modules
{
    /// <summary>
    /// A pluggable unit that can be loaded into an <see cref="IKernelBuilder"/>.
    /// </summary>
    public interface INinjectModule
    {
        /// <summary>
        /// Gets the module's name.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Called when the module is loaded into a kernel.
        /// </summary>
        /// <param name="kernelConfiguration">The configuration of the kernel that is loading the module.</param>
        /// <exception cref="ArgumentNullException"><paramref name="kernelConfiguration"/> is <see langword="null"/>.</exception>
        void OnLoad(Builder.IKernelConfiguration kernelConfiguration);

        /// <summary>
        /// Called after all modules are loaded.
        /// </summary>
        /// <param name="kernelConfiguration">The configuration of the kernel that has loaded the modules.</param>
        /// <exception cref="ArgumentNullException"><paramref name="kernelConfiguration"/> is <see langword="null"/>.</exception>
        /// <remarks>
        /// A module can verify here if all other required bindings are available.
        /// </remarks>
        void OnLoadCompleted(Builder.IKernelConfiguration kernelConfiguration);
    }
}