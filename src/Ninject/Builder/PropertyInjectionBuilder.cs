// -------------------------------------------------------------------------------------------------
// <copyright file="PropertyInjectionBuilder.cs" company="Ninject Project Contributors">
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
    using System.Collections.Generic;

    using Ninject.Activation.Strategies;
    using Ninject.Builder.Syntax;
    using Ninject.Planning.Strategies;
    using Ninject.Selection;

    internal class PropertyInjectionBuilder : IPropertyInjectionBuilder
    {
        private IComponentBuilder selectorBuilder;
        private readonly List<IComponentBuilder> injectionHeuristicBuilders;

        public PropertyInjectionBuilder(IComponentBindingRoot componentBindingRoot, IDictionary<string, object> properties)
        {
            this.Components = componentBindingRoot;
            this.Properties = properties;
            this.injectionHeuristicBuilders = new List<IComponentBuilder>();
        }

        /// <summary>
        /// Gets the root of the component bindings.
        /// </summary>
        /// <value>
        /// The root of the component binding.
        /// </value>
        public IComponentBindingRoot Components { get; }

        /// <summary>
        /// Gets a key/value collection that can be used to share data between components.
        /// </summary>
        /// <value>
        /// A key/value collection that can be used to share data between components.
        /// </value>
        public IDictionary<string, object> Properties { get; }

        public void Build()
        {
            foreach (var injectionHeuristicBuilder in this.injectionHeuristicBuilders)
            {
                injectionHeuristicBuilder.Build(this.Components);
            }

            this.selectorBuilder?.Build(this.Components);

            if (!this.Components.IsBound<IPropertyReflectionSelector>())
            {
                throw new Exception("TODO");
            }

            this.Components.Bind<IPlanningStrategy>().To<PropertyPlanningStrategy>();
            this.Components.Bind<IInitializationStrategy>().To<PropertyInjectionStrategy>();
        }

        public IPropertyInjectionHeuristicsSyntax InjectionHeuristic(Action<IAttributeBasedPropertyInjectionHeuristicBuilder> heuristic)
        {
            var injectionHeuristicBuilder = new AttributeBasedPropertyInjectionHeuristicBuilder();
            heuristic(injectionHeuristicBuilder);
            this.injectionHeuristicBuilders.Add(injectionHeuristicBuilder);
            return this;
        }

        IPropertyInjectionHeuristicsSyntax IPropertyInjectionHeuristicsSyntax.InjectionHeuristic<T>()
        {
            injectionHeuristicBuilders.Add(new BindComponentBuilder<IPropertyInjectionHeuristic, T>());
            return this;
        }

        public IPropertyInjectionHeuristicsSyntax Selector(Action<IPropertyReflectionSelectorBuilder> selector)
        {
            if (this.selectorBuilder != null)
            {
                throw new Exception("TODO");
            }

            var selectorBuilder = new PropertyReflectionSelectorBuilder();
            selector(selectorBuilder);
            this.selectorBuilder = selectorBuilder;
            return this;
        }

        void IPropertySelectorSyntax.Selector<T>()
        {
            if (selectorBuilder != null)
            {
                throw new Exception("TODO");
            }

            selectorBuilder = new BindComponentBuilder<IPropertyReflectionSelector, T>();
        }
    }
}
