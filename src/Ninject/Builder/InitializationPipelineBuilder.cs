﻿// -------------------------------------------------------------------------------------------------
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

namespace Ninject.Builder
{
    using System;
    using System.Collections.Generic;
    using Ninject.Activation.Strategies;

    internal class InitializationPipelineBuilder : IInitializationPipelineBuilder
    {
        public InitializationPipelineBuilder(IComponentBindingRoot componentBindingRoot, IDictionary<string, object> properties)
        {
            this.Components = componentBindingRoot;
            this.Properties = properties;
        }

        /// <summary>
        /// Gets the component bindings that make up the activation pipeline.
        /// </summary>
        /// <value>
        /// The component bindings that make up the activation pipeline.
        /// </value>
        public IComponentBindingRoot Components { get; }

        /// <summary>
        /// Gets a key/value collection that can be used to share data between components.
        /// </summary>
        /// <value>
        /// A key/value collection that can be used to share data between components.
        /// </value>
        public IDictionary<string, object> Properties { get; }

        public IInitializationPipelineBuilder BindingAction()
        {
            this.Components.Bind<IInitializationStrategy>()
                           .To<BindingActionStrategy>()
                           .InSingletonScope();
            return this;
        }

        public IInitializationPipelineBuilder Initializable()
        {
            this.Components.Bind<IInitializationStrategy>()
                           .To<InitializableStrategy>()
                           .InSingletonScope();
            return this;
        }
    }
}