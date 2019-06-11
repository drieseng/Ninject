// -------------------------------------------------------------------------------------------------
// <copyright file="BindingConfigurationBuilder.cs" company="Ninject Project Contributors">
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
    using Ninject.Planning.Bindings;
    using Ninject.Syntax;

    /// <summary>
    /// Builds a binding configurating.
    /// </summary>
    internal abstract class BindingConfigurationBuilder
    {
        /// <summary>
        /// Builds the binding configuration.
        /// </summary>
        /// <param name="root">The resolution root.</param>
        /// <returns>
        /// The binding configuration.
        /// </returns>
        public abstract IBindingConfiguration Build(IResolutionRoot root);

        public abstract bool HasActivationActions { get; }

        public abstract bool HasDeactivationActions { get; }

        public abstract bool HasInitializationActions { get; }
    }
}