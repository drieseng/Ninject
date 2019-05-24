// -------------------------------------------------------------------------------------------------
// <copyright file="ConstructorInjectionBuilder.cs" company="Ninject Project Contributors">
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
    using System.ComponentModel;

    using Ninject.Components;

    /// <summary>
    /// Enables and configures constructor injection.
    /// </summary>
    internal sealed class ConstructorInjectionBuilder : IConstructorInjectionBuilder
    {
        private readonly ComponentContainer components;
        private IConstructorInjectionSelectorBuilder constructorInjectionSelectorBuilder;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConstructorInjectionBuilder"/> class.
        /// </summary>
        /// <param name="components">The components.</param>
        public ConstructorInjectionBuilder(ComponentContainer components)
        {
            this.components = components;
        }

        /// <summary>
        /// Configure constructor injection to select the best matching constructor from the list of constructors
        /// a given service exposes.
        /// </summary>
        /// <param name="bestMatchingConstructorBuilder">A callback to configure the best matching constructor mechanism.</param>
        public void BestMatch(Action<IBestMatchConstructorInjectionSelectorBuilder> bestMatchingConstructorBuilder)
        {
            var builder = new BestMatchConstructorInjectionSelectorBuilder(this.components);
            bestMatchingConstructorBuilder(builder);
            this.constructorInjectionSelectorBuilder = builder;
        }

        /// <summary>
        /// Configure constructor injection to expect a given service to expose only a single constructor.
        /// </summary>
        public void Unique()
        {
            this.constructorInjectionSelectorBuilder = new UniqueConstructorInjectionSelectorBuilder(this.components);
        }

        /// <summary>
        /// Builds the constructor injection components.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void Build()
        {
            this.constructorInjectionSelectorBuilder.Build();
        }
    }
}