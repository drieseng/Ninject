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
    using Ninject.Builder.Syntax;

    /// <summary>
    /// Extension methods for configure an <see cref="IKernelBuilder"/>.
    /// </summary>
    public static class KernelBuilderExtensions
    {
        /// <summary>
        /// Enables and configures property injection.
        /// </summary>
        /// <param name="pipeline">An <see cref="IKernelBuilder"/> instance.</param>
        /// <param name="property">A callback to configure property injection.</param>
        /// <returns>
        /// The <see cref="IKernelBuilder"/> instance.
        /// </returns>
        public static IInitializationPipelineBuilder PropertyInjection(this IInitializationPipelineBuilder pipeline, Action<IPropertySelectorSyntax> property)
        {
            var propertyInjectionBuilder = new PropertyInjectionBuilder(pipeline.Components, pipeline.Properties);
            property(propertyInjectionBuilder);
            propertyInjectionBuilder.Build();
            return pipeline;
        }

        /// <summary>
        /// Enables and configures property injection.
        /// </summary>
        /// <param name="pipeline">An <see cref="IKernelBuilder"/> instance.</param>
        /// <returns>
        /// The <see cref="IKernelBuilder"/> instance.
        /// </returns>
        public static IInitializationPipelineBuilder PropertyInjection(this IInitializationPipelineBuilder pipeline)
        {
            return pipeline.PropertyInjection(property =>
                property.Selector(s => s.InjectNonPublic(false))
                        .InjectionHeuristic(a => a.InjectAttribute<InjectAttribute>()));
        }

        /// <summary>
        /// Enables and configures method injection.
        /// </summary>
        /// <param name="pipeline">An <see cref="IKernelBuilder"/> instance.</param>
        /// <param name="method">A callback to configure method injection.</param>
        /// <returns>
        /// The <see cref="IKernelBuilder"/> instance.
        /// </returns>
        public static IInitializationPipelineBuilder MethodInjection(this IInitializationPipelineBuilder pipeline, Action<IMethodSelectorSyntax> method)
        {
            var methodInjectionBuilder = new MethodInjectionBuilder(pipeline.Components, pipeline.Properties);
            method(methodInjectionBuilder);
            methodInjectionBuilder.Build();
            return pipeline;
        }

        /// <summary>
        /// Enables and configures method injection.
        /// </summary>
        /// <param name="pipeline">An <see cref="IKernelBuilder"/> instance.</param>
        /// <returns>
        /// The <see cref="IKernelBuilder"/> instance.
        /// </returns>
        public static IInitializationPipelineBuilder MethodInjection(this IInitializationPipelineBuilder pipeline)
        {
            return pipeline.MethodInjection(method =>
                method.Selector(s => s.InjectNonPublic(false))
                      .InjectionHeuristic(a => a.InjectAttribute<InjectAttribute>()));
        }
    }
}