// -------------------------------------------------------------------------------------------------
// <copyright file="IProviderFactory.cs" company="Ninject Project Contributors">
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
    using System.Collections.Generic;

    using Ninject.Activation;
    using Ninject.Parameters;
    using Ninject.Syntax;

    /// <summary>
    /// Factory for a creating an <see cref="IProvider"/>.
    /// </summary>
    internal interface IProviderFactory
    {
        /// <summary>
        /// Creates an appropiate <see cref="IProvider"/>.
        /// </summary>
        /// <param name="root">The resolution root.</param>
        /// <param name="parameters">The parameters of the binding.</param>
        /// <returns>
        /// An <see cref="IProvider"/>.
        /// </returns>
        IProvider Create(IResolutionRoot root, IReadOnlyList<IParameter> parameters);
    }
}