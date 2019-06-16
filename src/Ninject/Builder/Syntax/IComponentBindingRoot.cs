// -------------------------------------------------------------------------------------------------
// <copyright file="IComponentBindingRoot.cs" company="Ninject Project Contributors">
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

    /// <summary>
    /// Provides a path to register bindings.
    /// </summary>
    public interface IComponentBindingRoot : IFluentSyntax
    {
        /// <summary>
        /// Declares a binding for the specified service.
        /// </summary>
        /// <typeparam name="T">The service to bind.</typeparam>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        INewBindingToSyntax<T> Bind<T>();

        /// <summary>
        /// Unregisters all bindings for the specified service.
        /// </summary>
        /// <typeparam name="T">The service to unbind.</typeparam>
        void Unbind<T>();

        /// <summary>
        /// Determines whether a binding is defined for specified service.
        /// </summary>
        /// <typeparam name="T">The service to check.</typeparam>
        /// <returns>
        /// <see langword="true"/> if at least one binding exists for <typeparamref name="T"/>; otherwise, <see langword="false"/>.
        /// </returns>
        bool IsBound<T>();
    }
}