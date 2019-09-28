// -------------------------------------------------------------------------------------------------
// <copyright file="IKernelConfiguration.cs" company="Ninject Project Contributors">
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

namespace Ninject.Builder
{
    using Ninject.Syntax;
    using System;

    public interface IKernelConfiguration : IFluentSyntax
    {
        /// <summary>
        /// Adds bindings to the <see cref="IKernelBuilder"/>.
        /// </summary>
        /// <param name="configureBindings">A callback to configure bindings.</param>
        /// <returns>
        /// A reference to this instance after the operation has completed.
        /// </returns>
        IKernelBuilder Bindings(Action<INewBindingRoot> configureBindings);

        /// <summary>
        /// Configures the features of the <see cref="IKernelBuilder"/>.
        /// </summary>
        /// <param name="features">A callback to configure features.</param>
        /// <returns>
        /// A reference to this instance after the operation has completed.
        /// </returns>
        IKernelBuilder Features(Action<IFeatureBuilder> features);

        /// <summary>
        /// Adds modules to the <see cref="IKernelBuilder"/>.
        /// </summary>
        /// <param name="configureModules">A callback to configure modules.</param>
        /// <returns>
        /// A reference to this instance after the operation has completed.
        /// </returns>
        IKernelBuilder Modules(Action<IModuleBuilder> configureModules);
    }
}
