﻿// -------------------------------------------------------------------------------------------------
// <copyright file="UniqueConstructorInjectionSelectorBuilder.cs" company="Ninject Project Contributors">
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
    using Ninject.Builder.Syntax;
    using Ninject.Selection;
    using System;

    /// <summary>
    /// Configures the <see cref="IReadOnlyKernel"/> with a planning strategy that selects candidate constructors using
    /// a given <see cref="IConstructorReflectionSelector"/>, and an <see cref="IConstructorInjectionSelector"/> that
    /// expects only a single candidate and returns this candidate.
    /// </summary>
    internal sealed class UniqueConstructorInjectionBuilder : IComponentBuilder, IConstructorReflectionSelectorSyntax
    {
        private ConstructorReflectionSelectorBuilder selectorBuilder;

        public UniqueConstructorInjectionBuilder()
        {
            this.selectorBuilder = new ConstructorReflectionSelectorBuilder();
        }

        /// <summary>
        /// Builds the constructor injection components.
        /// </summary>
        public void Build(IComponentBindingRoot root)
        {
            this.selectorBuilder.Build(root);
            root.Bind<IConstructorInjectionSelector>().To<UniqueConstructorInjectionSelector>();
        }

        /// <summary>
        /// Configures an <see cref="IConstructorReflectionSelector"/> to use for composing a list of constructors that
        /// can be used to instantiate a given service.
        /// </summary>
        /// <param name="selectorBuilder">A callback to configure an <see cref="IConstructorReflectionSelector"/>.</param>
        void IConstructorReflectionSelectorSyntax.Selector(Action<IConstructorReflectionSelectorBuilder> selectorBuilder)
        {
            selectorBuilder(this.selectorBuilder);
        }
    }
}