// -------------------------------------------------------------------------------------------------
// <copyright file="ConstructorScorerBuilder.cs" company="Ninject Project Contributors">
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

    using Ninject.Selection.Heuristics;

    /// <summary>
    /// Builds a component for assigning a score to a given constructor.
    /// </summary>
    internal sealed class ConstructorScorerBuilder : IConstructorScorerBuilder
    {
        private Type lowestScoreAttribute;
        private Type highestScoreAttribute;

        /// <summary>
        /// Builds the component for assigning a score to a given constructor.
        /// </summary>
        public void Build(IComponentBindingRoot root)
        {
            root.Bind<IConstructorInjectionScorer>()
                .To<StandardConstructorScorer>()
                .InSingletonScope()
                .WithPropertyValue(nameof(StandardConstructorScorer.HighestScoreAttribute), this.highestScoreAttribute)
                .WithPropertyValue(nameof(StandardConstructorScorer.LowestScoreAttribute), this.lowestScoreAttribute);
        }

        /// <summary>
        /// Specifies the type of a custom attribute that can be applied to a constructor to give it the lowest score.
        /// </summary>
        /// <param name="lowestScoreAttribute">The type of a custom attribute that can be applied to a constructor to give it the lowest score, or <see langword="null"/> to not reduce the score based on the presence of a custom attribute.</param>
        public void LowestScoreAttribute(Type lowestScoreAttribute)
        {
            this.lowestScoreAttribute = lowestScoreAttribute;
        }

        /// <summary>
        /// Specifies the type of a custom attribute that can be applied to a constructor to give it the highest score.
        /// </summary>
        /// <param name="highestScoreAttribute">The type of a custom attribute that can be applied to a constructor to give it the highest score, or <see langword="null"/> to not boost the score based on the presence of a custom attribute.</param>
        public void HighestScoreAttribute(Type highestScoreAttribute)
        {
            this.highestScoreAttribute = highestScoreAttribute;
        }
    }
}