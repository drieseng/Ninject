// -------------------------------------------------------------------------------------------------
// <copyright file="PropertyReflectionStrategy.cs" company="Ninject Project Contributors">
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

namespace Ninject.Planning.Strategies
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    using Ninject.Components;
    using Ninject.Infrastructure;
    using Ninject.Injection;
    using Ninject.Planning.Directives;
    using Ninject.Selection;

    /// <summary>
    /// Adds directives to plans indicating which properties should be injected during activation.
    /// </summary>
    public class PropertyPlanningStrategy : NinjectComponent, IPlanningStrategy
    {
        private List<IPropertyInjectionHeuristic> injectionHeuristics;

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyPlanningStrategy"/> class.
        /// </summary>
        /// <param name="selector">The <see cref="IPropertyReflectionSelector"/> component.</param>
        /// <param name="injectionHeuristics">The injection heuristics.</param>
        /// <param name="injectorFactory">The injector factory component.</param>
        /// <exception cref="ArgumentNullException"><paramref name="selector"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="injectionHeuristics"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="injectorFactory"/> is <see langword="null"/>.</exception>
        public PropertyPlanningStrategy(IPropertyReflectionSelector selector, IEnumerable<IPropertyInjectionHeuristic> injectionHeuristics, IInjectorFactory injectorFactory)
        {
            Ensure.ArgumentNotNull(selector, nameof(selector));
            Ensure.ArgumentNotNull(injectionHeuristics, nameof(injectionHeuristics));
            Ensure.ArgumentNotNull(injectorFactory, nameof(injectorFactory));

            this.Selector = selector;
            this.injectionHeuristics = new List<IPropertyInjectionHeuristic>(injectionHeuristics);
            this.InjectorFactory = injectorFactory;
        }

        /// <summary>
        /// Gets the <see cref="IPropertyReflectionSelector"/> component.
        /// </summary>
        public IPropertyReflectionSelector Selector { get; }

        /// <summary>
        /// Gets the injection heuristics.
        /// </summary>
        public IReadOnlyList<IPropertyInjectionHeuristic> InjectionHeuristics
        {
            get
            {
                return this.injectionHeuristics;
            }
        }

        /// <summary>
        /// Gets the injector factory component.
        /// </summary>
        public IInjectorFactory InjectorFactory { get; }

        /// <summary>
        /// Adds a <see cref="PropertyInjectionDirective"/> to the plan for each property
        /// that should be injected.
        /// </summary>
        /// <param name="plan">The plan that is being generated.</param>
        /// <exception cref="ArgumentNullException"><paramref name="plan"/> is <see langword="null"/>.</exception>
        public void Execute(IPlan plan)
        {
            Ensure.ArgumentNotNull(plan, nameof(plan));

            foreach (var property in this.Selector.Select(plan.Type))
            {
                if (!ShouldInject(this.injectionHeuristics, property))
                {
                    continue;
                }

                plan.Add(new PropertyInjectionDirective(property, this.InjectorFactory.Create(property)));
            }
        }

        private static bool ShouldInject(List<IPropertyInjectionHeuristic> injectionHeuristics, PropertyInfo property)
        {
            var shouldInject = false;

            for (var i = 0; i < injectionHeuristics.Count; i++)
            {
                if (injectionHeuristics[i].ShouldInject(property))
                {
                    shouldInject = true;
                    break;
                }
            }

            return shouldInject;
        }
    }
}