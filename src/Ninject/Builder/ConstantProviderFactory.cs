// -------------------------------------------------------------------------------------------------
// <copyright file="ConstantProviderFactory.cs" company="Ninject Project Contributors">
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
    using Ninject.Activation.Providers;
    using Ninject.Parameters;
    using Ninject.Syntax;

    /// <summary>
    /// An <see cref="IProvider"/> that returns a constant value.
    /// </summary>
    /// <typeparam name="T">The type of the constant value.</typeparam>
    internal class ConstantProviderFactory<T> : IProviderFactory
    {
        private readonly T value;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConstantProviderFactory{T}"/> class.
        /// </summary>
        /// <param name="value">A constant value.</param>
        public ConstantProviderFactory(T value)
        {
            this.value = value;
        }

        /// <summary>
        /// Creates a <see cref="ConstantProvider{T}"/> for the value that this instance was constructed with.
        /// </summary>
        /// <param name="root">The resolution root.</param>
        /// <param name="parameters">The parameters of the binding.</param>
        /// <returns>
        /// A <see cref="ConstantProvider{T}"/> for the value that this instance was constructed with.
        /// </returns>
        public IProvider Create(IResolutionRoot root, IReadOnlyList<IParameter> parameters)
        {
            return new ConstantProvider<T>(this.value);
        }
    }
}