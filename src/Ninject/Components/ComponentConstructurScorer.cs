// -------------------------------------------------------------------------------------------------
// <copyright file="ComponentConstructurScorer.cs" company="Ninject Project Contributors">
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

namespace Ninject.Components
{
    using System;

    using Ninject.Activation;
    using Ninject.Parameters;
    using Ninject.Planning.Directives;
    using Ninject.Planning.Targets;
    using Ninject.Selection.Heuristics;

    /// <summary>
    /// Scores a constructor for a <see cref="INinjectComponent"/>.
    /// </summary>
    internal class ComponentConstructurScorer : IConstructorInjectionScorer
    {
        /// <summary>
        /// Calculates a score that specified directive based on the number of arguments for which
        /// a <see cref="IParameter"/> or binding exists.
        /// </summary>
        /// <param name="context">The context in which the <see cref="INinjectComponent"/> is created.</param>
        /// <param name="directive">The constructor to calculate a score for.</param>
        /// <returns>
        /// A score.
        /// </returns>
        public int Score(IContext context, ConstructorInjectionDirective directive)
        {
            var score = 1;

            foreach (ITarget target in directive.Targets)
            {
                if (this.ParameterExists(context, target))
                {
                    score++;
                    continue;
                }

                if (this.BindingExists(context, target))
                {
                    score++;
                    continue;
                }

                score++;

                if (score > 0)
                {
                    score += int.MinValue;
                }
            }

            return score;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        void IDisposable.Dispose()
        {
        }

        /// <summary>
        /// Checks whether any parameters exist for the given target..
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="target">The target.</param>
        /// <returns>
        /// <see langword="true"/> if a parameter exists for the target in the given context;
        /// otherwise, <see langword="false"/>.
        /// </returns>
        protected virtual bool ParameterExists(IContext context, ITarget target)
        {
            foreach (var parameter in context.Parameters)
            {
                if (parameter is IConstructorArgument ctorArgument && ctorArgument.AppliesToTarget(context, target))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Checks whether a binding exists for a given target.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="target">The target.</param>
        /// <returns>
        /// <see langword="true"/> if a binding exists for the target in the given context; otherwise,
        /// <see langword="false"/>.
        /// </returns>
        protected virtual bool BindingExists(IContext context, ITarget target)
        {
            // TODO
            return false;
        }
    }
}