﻿// -------------------------------------------------------------------------------------------------
// <copyright file="MethodReflectionSelectorBuilder.cs" company="Ninject Project Contributors">
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

using Ninject.Builder.Syntax;
using Ninject.Selection;
using Ninject.Selection.Heuristics;
using System;
using System.Collections.Generic;

namespace Ninject.Builder
{
    internal class MethodReflectionSelectorBuilder : IMethodReflectionSelectorBuilder, IMethodInjectionHeuristicsSyntax
    {
        private bool _injectNonPublic;
        private readonly List<IComponentBuilder> injectionHeuristicBuilders = new List<IComponentBuilder>();

        public void Build(IComponentBindingRoot root)
        {
            if (injectionHeuristicBuilders.Count == 0)
            {
                throw new Exception("At least one injection heuristic must be configured.");
            }

            foreach (var injectionHeuristicBuilder in this.injectionHeuristicBuilders)
            {
                injectionHeuristicBuilder.Build(root);
            }

            root.Bind<IMethodReflectionSelector>()
                .ToConstructor((s) => new MethodReflectionSelector(s.Inject<IEnumerable<IMethodInjectionHeuristic>>(), _injectNonPublic));
        }

        public void InjectNonPublic(bool value)
        {
            _injectNonPublic = value;
        }

        IMethodInjectionHeuristicsSyntax IMethodInjectionHeuristicsSyntax.InjectionHeuristic(Action<IAttributeBasedMethodInjectionHeuristicBuilder> heuristic)
        {
            var injectionHeuristicBuilder = new AttributeBasedMethodInjectionHeuristicBuilder();
            heuristic(injectionHeuristicBuilder);
            this.injectionHeuristicBuilders.Add(injectionHeuristicBuilder);
            return this;
        }

        IMethodInjectionHeuristicsSyntax IMethodInjectionHeuristicsSyntax.InjectionHeuristic<T>()
        {
            injectionHeuristicBuilders.Add(new BindComponentBuilder<IMethodInjectionHeuristic, T>());
            return this;
        }
    }
}