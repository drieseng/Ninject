// -------------------------------------------------------------------------------------------------
// <copyright file="KernelBuilderExtensions.cs" company="Ninject Project Contributors">
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
    using System;

    using Ninject.Activation.Providers;
    using Ninject.Activation.Strategies;
    using Ninject.Injection;
    using Ninject.Selection;

    /// <summary>
    /// Extension methods for configure an <see cref="IKernelBuilder"/>.
    /// </summary>
    public static class KernelBuilderExtensions
    {
        /// <summary>
        /// Enables and configures constructor injection.
        /// </summary>
        /// <param name="kernelBuilder">An <see cref="IKernelBuilder"/> instance.</param>
        /// <param name="ctor">A callback to configure constructor injection.</param>
        /// <returns>
        /// The <see cref="IKernelBuilder"/> instance.
        /// </returns>
        public static IKernelBuilder ConstructorInjection(this IKernelBuilder kernelBuilder, Action<IConstructorInjectionBuilder> ctor)
        {
            var constructorInjectionBuilder = new ConstructorInjectionBuilder();
            ctor(constructorInjectionBuilder);
            constructorInjectionBuilder.Build(kernelBuilder.Components);
            return kernelBuilder;
        }

        /// <summary>
        /// Enables and configures property injection.
        /// </summary>
        /// <param name="kernelBuilder">An <see cref="IKernelBuilder"/> instance.</param>
        /// <returns>
        /// The <see cref="IKernelBuilder"/> instance.
        /// </returns>
        public static IKernelBuilder PropertyInjection(this IKernelBuilder kernelBuilder)
        {
            kernelBuilder.Components.Bind<IPropertyValueProvider>().To<PropertyValueProvider>();
            kernelBuilder.Components.Bind<IPropertyReflectionSelector>().To<PropertyReflectionSelector>();
            kernelBuilder.Components.Bind<IInitializationStrategy>().To<PropertyInjectionStrategy>();
            return kernelBuilder;
        }

        /// <summary>
        /// Enables and configures property injection.
        /// </summary>
        /// <param name="kernelBuilder">An <see cref="IKernelBuilder"/> instance.</param>
        /// <param name="property">A builder to configure property injection.</param>
        /// <returns>
        /// The <see cref="IKernelBuilder"/> instance.
        /// </returns>
        public static IKernelBuilder PropertyInjection(this IKernelBuilder kernelBuilder, Action<IPropertyInjectionBuilder> property)
        {
            return kernelBuilder;
        }

        /// <summary>
        /// Enables and configures method injection.
        /// </summary>
        /// <param name="kernelBuilder">An <see cref="IKernelBuilder"/> instance.</param>
        /// <returns>
        /// The <see cref="IKernelBuilder"/> instance.
        /// </returns>
        public static IKernelBuilder MethodInjection(this IKernelBuilder kernelBuilder)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Configures an <see cref="IKernelBuilder"/> to use expression-based injection.
        /// </summary>
        /// <param name="kernelBuilder">An <see cref="IKernelBuilder"/> instance.</param>
        /// <returns>
        /// The <see cref="IKernelBuilder"/> instance.
        /// </returns>
        public static IKernelBuilder ExpressionBasedInjection(this IKernelBuilder kernelBuilder)
        {
            kernelBuilder.Components.Bind<IInjectorFactory>().To<ExpressionInjectorFactory>();
            return kernelBuilder;
        }

        /// <summary>
        /// Configures an <see cref="IKernelBuilder"/> to use expression-based injection.
        /// </summary>
        /// <param name="kernelBuilder">An <see cref="IKernelBuilder"/> instance.</param>
        /// <returns>
        /// The <see cref="IKernelBuilder"/> instance.
        /// </returns>
        public static IKernelBuilder ReflectionBasedInjection(this IKernelBuilder kernelBuilder)
        {
            kernelBuilder.Components.Bind<IInjectorFactory>().To<ReflectionInjectorFactory>();
            return kernelBuilder;
        }

        /// <summary>
        /// Configures an <see cref="IKernelBuilder"/> to automatically create a self-binding for a given
        /// service when no explicit binding is available.
        /// </summary>
        /// <param name="kernelBuilder">An <see cref="IKernelBuilder"/> instance.</param>
        /// <returns>
        /// The <see cref="IKernelBuilder"/> instance.
        /// </returns>
        public static IKernelBuilder SelfBinding(this IKernelBuilder kernelBuilder)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Configures an <see cref="IKernelBuilder"/> to use the default value for a given target when no
        /// explicit binding is available.
        /// </summary>
        /// <param name="kernelBuilder">An <see cref="IKernelBuilder"/> instance.</param>
        /// <returns>
        /// The <see cref="IKernelBuilder"/> instance.
        /// </returns>
        public static IKernelBuilder DefaultValueBinding(this IKernelBuilder kernelBuilder)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Configures an <see cref="IKernelBuilder"/> to support open generic binding.
        /// </summary>
        /// <param name="kernelBuilder">An <see cref="IKernelBuilder"/> instance.</param>
        /// <returns>
        /// The <see cref="IKernelBuilder"/> instance.
        /// </returns>
        public static IKernelBuilder OpenGenericBinding(this IKernelBuilder kernelBuilder)
        {
            throw new NotImplementedException();
        }
    }
}