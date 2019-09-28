// -------------------------------------------------------------------------------------------------
// <copyright file="InitializationPipelineBuilder.cs" company="Ninject Project Contributors">
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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using Ninject.Activation.Strategies;
using Ninject.Builder.Components;
using Ninject.Syntax;

namespace Ninject.Builder
{
    public class InitializationPipelineBuilder : IInitializationPipelineBuilder, IComponentBuilder
    {
        private List<IComponentBuilder> components;

        public InitializationPipelineBuilder()
        {
            this.components = new List<IComponentBuilder>();
        }

        public IInitializationPipelineBuilder BindingAction()
        {
            components.Add(new ComponentBuilder<IInitializationStrategy, BindingActionStrategy>());
            return this;
        }

        public IInitializationPipelineBuilder Initializable()
        {
            components.Add(new ComponentBuilder<IInitializationStrategy, InitializableStrategy>());
            return this;
        }

        public void Build(IComponentBindingRoot root)
        {
            foreach (var component in this.components)
            {
                component.Build(root);
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        IInitializationPipelineBuilder IInitializationPipelineBuilder.AddStage(Func<IComponentBuilder> componentDelegate)
        {
            this.components.Add(componentDelegate());
            return this;
        }
    }
}
