// -------------------------------------------------------------------------------------------------
// <copyright file="IComponentBindingToSyntax.cs" company="Ninject Project Contributors">
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

namespace Ninject.Builder.Syntax
{
    using Ninject.Activation;
    using System;

    /// <summary>
    /// Used to define the target of a binding.
    /// </summary>
    /// <typeparam name="T">The service being bound.</typeparam>
    public interface IComponentBindingToSyntax<T>
    {
        /// <summary>
        /// Indicates that the service should be self-bound.
        /// </summary>
        /// <returns>The fluent syntax.</returns>
        IComponentBindingInScopeSyntax<T> ToSelf();

        /// <summary>
        /// Indicates that the service should be bound to the specified implementation type.
        /// </summary>
        /// <typeparam name="TImplementation">The implementation type.</typeparam>
        /// <returns>The fluent syntax.</returns>
        IComponentBindingInScopeSyntax<TImplementation> To<TImplementation>()
            where TImplementation : T;

        /// <summary>
        /// Indicates that the service should be bound to the specified implementation type.
        /// </summary>
        /// <param name="implementation">The implementation type.</param>
        /// <returns>The fluent syntax.</returns>
        IComponentBindingInScopeSyntax<T> To(Type implementation);

        /// <summary>
        /// Indicates that the service should be bound to the specified constant value.
        /// </summary>
        /// <typeparam name="TImplementation">The type of the implementation.</typeparam>
        /// <param name="value">The constant value.</param>
        /// <returns>The fluent syntax.</returns>
        IComponentBindingInScopeSyntax<TImplementation> ToConstant<TImplementation>(TImplementation value)
            where TImplementation : T;

        /// <summary>
        /// Indicates that the service should be bound to the specified callback method.
        /// </summary>
        /// <typeparam name="TImplementation">The type of the implementation.</typeparam>
        /// <param name="method">The method.</param>
        /// <returns>
        /// The fluent syntax.
        /// </returns>
        IComponentBindingInScopeSyntax<TImplementation> ToMethod<TImplementation>(Func<IContext, TImplementation> method);
    }
}