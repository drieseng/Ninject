// -------------------------------------------------------------------------------------------------
// <copyright file="IResolutionBuilder.cs" company="Ninject Project Contributors">
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
    using System.Collections.Generic;
    using System.ComponentModel;

    public interface IResolutionBuilder : IFluentSyntax
    {
        /// <summary>
        /// Gets the root of the component bindings.
        /// </summary>
        /// <value>
        /// The root of the component binding.
        /// </value>
        [EditorBrowsable(EditorBrowsableState.Never)]
        IComponentBindingRoot Components { get; }

        /// <summary>
        /// Gets a key/value collection that can be used to share data between components.
        /// </summary>
        /// <value>
        /// A key/value collection that can be used to share data between components.
        /// </value>
        [EditorBrowsable(EditorBrowsableState.Never)]
        IDictionary<string, object> Properties { get; }

        /// <summary>
        /// Configures an <see cref="IKernelBuilder"/> to support open generic binding.
        /// </summary>
        /// <returns>
        /// The <see cref="IResolutionBuilder"/> instance.
        /// </returns>
        IResolutionBuilder OpenGenericBinding();

        /// <summary>
        /// Configures an <see cref="IKernelBuilder"/> to use the default value for a given target when no
        /// explicit binding is available.
        /// </summary>
        /// <returns>
        /// The <see cref="IResolutionBuilder"/> instance.
        /// </returns>
        IResolutionBuilder DefaultValueBinding();

        /// <summary>
        /// Configures an <see cref="IKernelBuilder"/> to automatically create a self-binding for a given
        /// service when no explicit binding is available.
        /// </summary>
        /// <returns>
        /// The <see cref="IResolutionBuilder"/> instance.
        /// </returns>
        IResolutionBuilder SelfBinding();

        /// <summary>
        /// Allow <see langword="null"/> as a valid value for injection.
        /// </summary>
        /// <returns>
        /// The <see cref="IResolutionBuilder"/> instance.
        /// </returns>
        /// <remarks>
        /// When not set, an <see cref="ActivationException"/> is thrown whenever a provider returns <see langword="null"/>.
        /// This is the default.
        /// </remarks>
        IResolutionBuilder AllowNull();

        /// <summary>
        /// Enables detection of cyclic dependencies.
        /// </summary>
        /// <returns>
        /// The <see cref="IResolutionBuilder"/> instance.
        /// </returns>
        /// <remarks>
        /// <para>
        /// If enabled, an <see cref="ActivationException"/> is thrown whenever a cyclic dependency is detected.
        /// </para>
        /// <para>
        /// When not enabled, the CLR throws a <see cref="StackOverflowException"/> and terminates the process in case
        /// of cyclic dependencies. This is the default.
        /// </para>
        /// </remarks>
        IResolutionBuilder DetectCyclicDependencies();

        /// <summary>
        /// Configures the <see cref="IKernelBuilder"/> to use expression-based injection.
        /// </summary>
        /// <returns>
        /// The <see cref="IResolutionBuilder"/> instance.
        /// </returns>
        IResolutionBuilder ExpressionInjector();

        /// <summary>
        /// Configures an <see cref="IKernelBuilder"/> to use reflection-based injection.
        /// </summary>
        /// <returns>
        /// The <see cref="IResolutionBuilder"/> instance.
        /// </returns>
        IResolutionBuilder ReflectionInjector();
    }
}
