// -------------------------------------------------------------------------------------------------
// <copyright file="FeatureBuilderExtensions.cs" company="Ninject Project Contributors">
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
    using Ninject.Injection;
    using Ninject.Planning.Bindings.Resolvers;
    using System;

    public static class FeatureBuilderExtensions
    {
        /// <summary>
        /// Enables and configures constructor injection.
        /// </summary>
        /// <param name="features">An <see cref="IFeatureBuilder"/> instance.</param>
        /// <param name="ctor">A callback to configure constructor injection.</param>
        /// <returns>
        /// The <see cref="IFeatureBuilder"/> instance.
        /// </returns>
        public static IFeatureBuilder ConstructorInjection(this IFeatureBuilder features, Action<IConstructorInjectionBuilder> ctor)
        {
            var constructorInjectionBuilder = new ConstructorInjectionBuilder();
            ctor(constructorInjectionBuilder);
            constructorInjectionBuilder.Build(features.Components);
            return features;
        }

        /// <summary>
        /// Enables and configures constructor injection to expect only a single public constructor to
        /// inject services into.
        /// </summary>
        /// <param name="features">An <see cref="IFeatureBuilder"/> instance.</param>
        /// <returns>
        /// The <see cref="IFeatureBuilder"/> instance.
        /// </returns>
        public static IFeatureBuilder ConstructorInjection(this IFeatureBuilder features)
        {
            return features.ConstructorInjection(c => c.Unique());
        }

        /// <summary>
        /// Configures an <see cref="IKernelBuilder"/> to use expression-based injection.
        /// </summary>
        /// <param name="features">An <see cref="IFeatureBuilder"/> instance.</param>
        /// <returns>
        /// The <see cref="IFeatureBuilder"/> instance.
        /// </returns>
        public static IFeatureBuilder ExpressionBasedInjection(this IFeatureBuilder features)
        {
            features.Components.Bind<IInjectorFactory>().To<ExpressionInjectorFactory>();
            return features;
        }

        /// <summary>
        /// Configures an <see cref="IKernelBuilder"/> to use expression-based injection.
        /// </summary>
        /// <param name="features">An <see cref="IFeatureBuilder"/> instance.</param>
        /// <returns>
        /// The <see cref="IFeatureBuilder"/> instance.
        /// </returns>
        public static IFeatureBuilder ReflectionBasedInjection(this IFeatureBuilder features)
        {
            features.Components.Bind<IInjectorFactory>().To<ReflectionInjectorFactory>();
            return features;
        }

        /// <summary>
        /// Configures an <see cref="IKernelBuilder"/> to automatically create a self-binding for a given
        /// service when no explicit binding is available.
        /// </summary>
        /// <param name="features">An <see cref="IFeatureBuilder"/> instance.</param>
        /// <returns>
        /// The <see cref="IFeatureBuilder"/> instance.
        /// </returns>
        public static IFeatureBuilder SelfBinding(this IFeatureBuilder features)
        {
            features.Components.Bind<IMissingBindingResolver>().To<SelfBindingResolver>();
            return features;
        }

        /// <summary>
        /// Configures an <see cref="IKernelBuilder"/> to use the default value for a given target when no
        /// explicit binding is available.
        /// </summary>
        /// <param name="features">An <see cref="IFeatureBuilder"/> instance.</param>
        /// <returns>
        /// The <see cref="IFeatureBuilder"/> instance.
        /// </returns>
        public static IFeatureBuilder DefaultValueBinding(this IFeatureBuilder features)
        {
            features.Components.Bind<IMissingBindingResolver>().To<DefaultValueBindingResolver>();
            return features;
        }

        /// <summary>
        /// Configures an <see cref="IKernelBuilder"/> to support open generic binding.
        /// </summary>
        /// <param name="features">An <see cref="IFeatureBuilder"/> instance.</param>
        /// <returns>
        /// The <see cref="IFeatureBuilder"/> instance.
        /// </returns>
        public static IFeatureBuilder OpenGenericBinding(this IFeatureBuilder features)
        {
            features.Components.Bind<IBindingResolver>().To<OpenGenericBindingResolver>();
            return features;
        }

        public static IFeatureBuilder Initialization(this IFeatureBuilder features, Action<IInitializationPipelineBuilder> pipeline)
        {
            var builder = new InitializationPipelineBuilder(features.Components, features.Properties);
            pipeline(builder);
            return features;
        }

        public static IFeatureBuilder Activation(this IFeatureBuilder features, Action<IActivationPipelineBuilder> pipeline)
        {
            var builder = new ActivationPipelineBuilder(features.Components, features.Properties);
            pipeline(builder);
            return features;
        }

        public static IFeatureBuilder Deactivation(this IFeatureBuilder features, Action<IDeactivationPipelineBuilder> pipeline)
        {
            var builder = new DeactivationPipelineBuilder(features.Components, features.Properties);
            pipeline(builder);
            return features;
        }
    }
}
