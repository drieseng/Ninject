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

    using Ninject.Activation.Providers;
    using Ninject.Activation.Strategies;
    using Ninject.Builder.Syntax;
    using Ninject.Planning.Strategies;
    using Ninject.Selection;

    internal class MethodInjectionBuilder : IComponentBuilder, IMethodSelectorSyntax
    {
        private IComponentBuilder selectorBuilder;

        public void Build(IComponentBindingRoot root)
        {
            if (this.selectorBuilder == null)
            {
                throw new Exception("Please configure a selector.");
            }

            this.selectorBuilder.Build(root);

            root.Bind<IPlanningStrategy>().To<MethodReflectionStrategy>();
            root.Bind<IInitializationStrategy>().To<MethodInjectionStrategy>();
            root.Bind<IMethodParameterValueProvider>().To<MethodParameterValueProvider>();
        }

        public IMethodInjectionHeuristicsSyntax Selector(Action<IMethodReflectionSelectorBuilder> selector)
        {
            if (this.selectorBuilder != null)
            {
                throw new Exception("TODO");
            }

            var selectorBuilder = new MethodReflectionSelectorBuilder();
            selector(selectorBuilder);
            this.selectorBuilder = selectorBuilder;
            return selectorBuilder;
        }

        void IMethodSelectorSyntax.Selector<T>()
        {
            if (selectorBuilder != null)
            {
                throw new Exception("TODO");
            }

            selectorBuilder = new BindComponentBuilder<IMethodReflectionSelector, T>();
        }
    }
}
