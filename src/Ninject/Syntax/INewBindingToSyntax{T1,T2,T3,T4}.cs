﻿// -------------------------------------------------------------------------------------------------
// <copyright file="IBindingToSyntax{T1}.cs" company="Ninject Project Contributors">
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

namespace Ninject.Syntax
{
    using System;
    using System.Linq.Expressions;

    using Ninject.Activation;

    /// <summary>
    /// Used to define the target of a binding.
    /// </summary>
    /// <typeparam name="T1">The first service type to be bound.</typeparam>
    /// <typeparam name="T2">The second service type to be bound.</typeparam>
    /// <typeparam name="T3">The third service type to be bound.</typeparam>
    /// <typeparam name="T4">The fourth service type to be bound.</typeparam>
    public interface INewBindingToSyntax<T1, T2, T3, T4> : IFluentSyntax
    {
        /// <summary>
        /// Indicates that the service should be bound to the specified implementation type.
        /// </summary>
        /// <typeparam name="TImplementation">The implementation type.</typeparam>
        /// <returns>The fluent syntax.</returns>
        INewBindingWhenInWithOrOnInitializationSyntax<TImplementation> To<TImplementation>()
            where TImplementation : T1, T2, T3, T4;

        /// <summary>
        /// Indicates that the service should be bound to the specified implementation type.
        /// </summary>
        /// <param name="implementation">The implementation type.</param>
        /// <returns>The fluent syntax.</returns>
        INewBindingWhenInWithOrOnInitializationSyntax<object> To(Type implementation);

        /// <summary>
        /// Indicates that the service should be bound to an instance of the specified provider type.
        /// The instance will be activated via the kernel when an instance of the service is activated.
        /// </summary>
        /// <typeparam name="TProvider">The type of provider to activate.</typeparam>
        /// <returns>The fluent syntax.</returns>
        INewBindingWhenInWithOrOnInitializationSyntax<object> ToProvider<TProvider>()
            where TProvider : IProvider;

        /// <summary>
        /// Indicates that the service should be bound to an instance of the specified provider type.
        /// The instance will be activated via the kernel when an instance of the service is activated.
        /// </summary>
        /// <param name="providerType">The type of provider to activate.</param>
        /// <returns>The fluent syntax.</returns>
        INewBindingWhenInWithOrOnInitializationSyntax<object> ToProvider(Type providerType);

        /// <summary>
        /// Indicates that the service should be bound to the specified provider.
        /// </summary>
        /// <typeparam name="TImplementation">The type of the implementation.</typeparam>
        /// <param name="provider">The provider.</param>
        /// <returns>The fluent syntax.</returns>
        INewBindingWhenInWithOrOnInitializationSyntax<TImplementation> ToProvider<TImplementation>(IProvider<TImplementation> provider)
            where TImplementation : T1, T2, T3, T4;

        /// <summary>
        /// Indicates that the service should be bound to the specified callback method.
        /// </summary>
        /// <typeparam name="TImplementation">The type of the implementation.</typeparam>
        /// <param name="method">The method.</param>
        /// <returns>The fluent syntax.</returns>
        INewBindingWhenInNamedSyntax<TImplementation> ToMethod<TImplementation>(Func<IContext, TImplementation> method)
            where TImplementation : T1, T2, T3, T4;

        /// <summary>
        /// Indicates that the service should be bound to the specified constant value.
        /// </summary>
        /// <typeparam name="TImplementation">The type of the implementation.</typeparam>
        /// <param name="value">The constant value.</param>
        /// <returns>The fluent syntax.</returns>
        INewBindingWhenNamedOrOnActivationSyntax<TImplementation> ToConstant<TImplementation>(TImplementation value)
            where TImplementation : T1, T2, T3, T4;

        /// <summary>
        /// Indicates that the service should be bound to the specified constructor.
        /// </summary>
        /// <typeparam name="TImplementation">The type of the implementation.</typeparam>
        /// <param name="newExpression">The expression that specifies the constructor.</param>
        /// <returns>The fluent syntax.</returns>
        INewBindingWhenInWithOrOnInitializationSyntax<TImplementation> ToConstructor<TImplementation>(
            Expression<Func<IConstructorArgumentSyntax, TImplementation>> newExpression)
            where TImplementation : T1, T2, T3, T4;
    }
}