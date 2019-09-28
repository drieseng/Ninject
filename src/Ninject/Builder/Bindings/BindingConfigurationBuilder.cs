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
    using Ninject.Builder.Bindings;
    using Ninject.Planning.Bindings;
    using Ninject.Syntax;

    /// <summary>
    /// Builds a <see cref="IBindingConfiguration"/>.
    /// </summary>
    internal abstract class BindingConfigurationBuilder : INewBindingConfigurationBuilder
    {
        /// <summary>
        /// Builds the binding configuration.
        /// </summary>
        /// <param name="root">The resolution root.</param>
        /// <returns>
        /// The binding configuration.
        /// </returns>
        public abstract IBindingConfiguration Build(IResolutionRoot root);

        /// <summary>
        /// Returns a value indicating whether any activation actions are defined for the current binding.
        /// </summary>
        /// <value>
        /// <see langword="true"/> if at least one activation action is defined for the current binding;
        /// otherwise, <see langword="false"/>.
        /// </value>
        public abstract bool HasActivationActions { get; }

        /// <summary>
        /// Returns a value indicating whether any deactivation actions are defined for the current binding.
        /// </summary>
        /// <value>
        /// <see langword="true"/> if at least one deactivation action is defined for the current binding;
        /// otherwise, <see langword="false"/>.
        /// </value>
        public abstract bool HasDeactivationActions { get; }

        /// <summary>
        /// Returns a value indicating whether any initialization actions are defined for the current binding.
        /// </summary>
        /// <value>
        /// <see langword="true"/> if at least one initialization action is defined for the current binding;
        /// otherwise, <see langword="false"/>.
        /// </value>
        public abstract bool HasInitializationActions { get; }
    }
}