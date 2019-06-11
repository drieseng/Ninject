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

    internal class PropertyInjectionBuilder : IPropertyInjectionBuilder
    {
        private IDefaultPropertyInjectionHeuristicBuilder injectionHeuristicBuilder;

        public IPropertyInjectionBuilder InjectionHeuristic(Action<IDefaultPropertyInjectionHeuristicBuilder> heuristic)
        {
            this.injectionHeuristicBuilder = new DefaultPropertyInjectHeuristicBuilder();
            heuristic(this.injectionHeuristicBuilder);
            return this;
        }

        public IPropertyInjectionBuilder InjectionHeuristic()
        {
            this.injectionHeuristicBuilder = new DefaultPropertyInjectHeuristicBuilder();
            return this;
        }

        public IPropertyInjectionBuilder Selector(Action<IPropertyReflectionSelectorBuilder> selector)
        {
            throw new NotImplementedException();
        }

        public IPropertyInjectionBuilder Selector(Func<IKernelBuilder, IPropertyReflectionSelectorBuilder> selector)
        {
            throw new NotImplementedException();
        }
    }
}
