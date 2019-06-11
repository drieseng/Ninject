// -------------------------------------------------------------------------------------------------
// <copyright file="BestMatchConstructorInjectionSelector.cs" company="Ninject Project Contributors">
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

namespace Ninject.Selection
{
    using System;
    using System.Linq;

    using Ninject.Activation;
    using Ninject.Components;
    using Ninject.Infrastructure;
    using Ninject.Planning;
    using Ninject.Planning.Directives;
    using Ninject.Selection.Heuristics;

    /// <summary>
    /// Select the constructor with the highest score.
    /// </summary>
    internal sealed class BestMatchConstructorInjectionSelector : IConstructorInjectionSelector
    {
        private readonly IExceptionFormatter exceptionFormatter;

        /// <summary>
        /// Initializes a new instance of the <see cref="BestMatchConstructorInjectionSelector"/> class.
        /// </summary>
        /// <param name="constructorScorer">The <see cref="IConstructorInjectionScorer"/> to score constructors.</param>
        /// <param name="exceptionFormatter">The <see cref="IExceptionFormatter"/> component.</param>
        /// <exception cref="ArgumentNullException"><paramref name="constructorScorer"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="exceptionFormatter"/> is <see langword="null"/>.</exception>
        public BestMatchConstructorInjectionSelector(IConstructorInjectionScorer constructorScorer, IExceptionFormatter exceptionFormatter)
        {
            Ensure.ArgumentNotNull(constructorScorer, nameof(constructorScorer));
            Ensure.ArgumentNotNull(exceptionFormatter, nameof(exceptionFormatter));

            this.ConstructorScorer = constructorScorer;
            this.exceptionFormatter = exceptionFormatter;
        }

        /// <summary>
        /// Gets the constructor scorer component.
        /// </summary>
        public IConstructorInjectionScorer ConstructorScorer { get; }

        /// <summary>
        /// Selects the constructor to create an instance of <see cref="IPlan.Type"/> in the specified
        /// context.
        /// </summary>
        /// <param name="plan">The plan.</param>
        /// <param name="context">The context.</param>
        /// <returns>
        /// The selected constructor.
        /// </returns>
        /// <exception cref="ActivationException">No constructors is available to create an instance of the implementation type, or more than one constructor has the highest score.</exception>
        public IConstructorInjectionDirective Select(IPlan plan, IContext context)
        {
            var constructors = plan.GetConstructors();
            if (constructors.Count == 0)
            {
                throw new ActivationException(this.exceptionFormatter.NoConstructorsAvailable(context));
            }

            if (constructors.Count == 1)
            {
                return constructors[0];
            }

            var bestDirectives = constructors.GroupBy(directive => this.ConstructorScorer.Score(context, directive))
                                             .OrderByDescending(g => g.Key)
                                             .First()
                                             .ToArray();

            if (bestDirectives.Length > 1)
            {
                throw new ActivationException(ExceptionFormatter.ConstructorsAmbiguous(context, bestDirectives));
            }

            return bestDirectives[0];
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        void IDisposable.Dispose()
        {
        }
    }
}