// -------------------------------------------------------------------------------------------------
// <copyright file="ExtensionsForParameter.cs" company="Ninject Project Contributors">
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

namespace Ninject.Infrastructure.Language
{
    using System.Collections.Generic;

    using Ninject.Parameters;

    /// <summary>
    /// Provides extension methods for <see cref="IParameter"/> and <see cref="IReadOnlyList{IParameter}"/>.
    /// </summary>
    internal static class ExtensionsForParameter
    {
        /// <summary>
        /// Returns all <see cref="IConstructorArgument"/> instances from the specified parameters.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns>
        /// All <see cref="IConstructorArgument"/> instances from <paramref name="parameters"/>.
        /// </returns>
        public static List<IConstructorArgument> GetConstructorArguments(this IReadOnlyList<IParameter> parameters)
        {
            if (parameters.Count == 0)
            {
                return new List<IConstructorArgument>();
            }

            var constructorArguments = new List<IConstructorArgument>();

            foreach (var parameter in parameters)
            {
                var constructorArgument = parameter as IConstructorArgument;
                if (constructorArgument != null)
                {
                    constructorArguments.Add(constructorArgument);
                }
            }

            return constructorArguments;
        }
    }
}