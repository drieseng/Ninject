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

    using Ninject.Activation.Providers;
    using Ninject.Activation.Strategies;
    using Ninject.Builder.Syntax;
    using Ninject.Planning.Strategies;
    using Ninject.Selection;

    internal class PropertyInjectionBuilder : IComponentBuilder, IPropertyInjectionHeuristicsSyntax, IPropertySelectorSyntax
    {
        private IComponentBuilder selectorBuilder;
        private List<IComponentBuilder> injectionHeuristicBuilders;

        public PropertyInjectionBuilder()
        {
            injectionHeuristicBuilders = new List<IComponentBuilder>();
        }

        public void Build(IComponentBindingRoot root)
        {
            if (this.selectorBuilder == null)
            {
                throw new Exception("TODO specifiy at least one ...");
            }

            if (this.injectionHeuristicBuilders.Count == 0)
            {
                throw new Exception("TODO specifiy at least one ...");
            }

            this.selectorBuilder.Build(root);

            foreach (var injectionHeuristicBuilder in this.injectionHeuristicBuilders)
            {
                injectionHeuristicBuilder.Build(root);
            }

            root.Bind<IPlanningStrategy>().To<PropertyReflectionStrategy>();
            root.Bind<IInitializationStrategy>().To<PropertyInjectionStrategy>();
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
