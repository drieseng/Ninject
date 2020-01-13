﻿// -------------------------------------------------------------------------------------------------
// <copyright file="BestMatchConstructorInjectionSelectorBuilder.cs" company="Ninject Project Contributors">
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
    using Ninject.Selection;
    using Ninject.Selection.Heuristics;

    /// <summary>
    /// Configures the <see cref="IReadOnlyKernel"/> with a planning strategy that selects candidate constructors using
    /// a given <see cref="IConstructorReflectionSelector"/>, and an <see cref="IConstructorInjectionScorer"/> to select
    /// the best matching constructor from these candidates.
    /// </summary>
    internal sealed class BestMatchConstructorInjectionBuilder : IComponentBuilder,
                                                                 IConstructorReflectionSelectorSyntax,
                                                                 IConstructorInjectionScorerSyntax,
                                                                 IConstructorReflectionSelectorAndScorerSyntax
    {
        private ConstructorReflectionSelectorBuilder selectorBuilder;
        private ConstructorScorerBuilder scorerBuilder;

        /// <summary>
        /// Builds the constructor injection components.
        /// </summary>
        public void Build(IComponentBindingRoot root)
        {
            if (this.selectorBuilder == null)
            {
                throw new Exception("TODO");
            }

            if (this.scorerBuilder == null)
            {
                throw new Exception("TODO");
            }


            this.selectorBuilder.Build(root);
            this.scorerBuilder.Build(root);

            root.Bind<IConstructorInjectionSelector>().To<BestMatchConstructorInjectionSelector>();
        }

        IConstructorInjectionScorerSyntax IConstructorReflectionSelectorAndScorerSyntax.Selector(Action<IConstructorReflectionSelectorBuilder> selectorBuilder)
        {
            return this.Selector(selectorBuilder);
        }

        void IConstructorReflectionSelectorSyntax.Selector(Action<IConstructorReflectionSelectorBuilder> selectorBuilder)
        {
            this.Selector(selectorBuilder);
        }

        IConstructorReflectionSelectorSyntax IConstructorReflectionSelectorAndScorerSyntax.Scorer(Action<IConstructorScorerBuilder> scorerBuilder)
        {
            return this.Scorer(scorerBuilder);
        }

        void IConstructorInjectionScorerSyntax.Scorer(Action<IConstructorScorerBuilder> scorerBuilder)
        {
            this.Scorer(scorerBuilder);
        }

        /// <summary>
        /// Configures an <see cref="IConstructorReflectionSelector"/> to use for composing a list of constructors that
        /// can be used to instantiate a given service.
        /// </summary>
        /// <param name="selectorBuilder">A callback to configure an <see cref="IConstructorReflectionSelector"/>.</param>
        private BestMatchConstructorInjectionBuilder Selector(Action<IConstructorReflectionSelectorBuilder> selectorBuilder)
        {
            if (this.selectorBuilder != null)
            {
                throw new Exception("TODO");
            }

            this.selectorBuilder = new ConstructorReflectionSelectorBuilder();
            selectorBuilder(this.selectorBuilder);
            return this;
        }

        /// <summary>
        /// Configures an <see cref="IConstructorInjectionScorer"/> to use for selecting the best matching constructor.
        /// </summary>
        /// <param name="scorerBuilder">A callback to configure an <see cref="IConstructorInjectionScorer"/>.</param>
        private BestMatchConstructorInjectionBuilder Scorer(Action<IConstructorScorerBuilder> scorerBuilder)
        {
            if (this.scorerBuilder != null)
            {
                throw new Exception("TODO");
            }

            this.scorerBuilder = new ConstructorScorerBuilder();
            scorerBuilder(this.scorerBuilder);
            return this;
        }
    }
}