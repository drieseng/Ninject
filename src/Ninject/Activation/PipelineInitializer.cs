// -------------------------------------------------------------------------------------------------
// <copyright file="PipelineInitializer.cs" company="Ninject Project Contributors">
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

namespace Ninject.Activation
{
    using System.Collections.Generic;

    using Ninject.Activation.Strategies;

    /// <summary>
    /// Executes the initialization strategies for an instance in a given context.
    /// </summary>
    internal sealed class PipelineInitializer : IPipelineInitializer
    {
        private readonly List<IInitializationStrategy> initializationStrategies;

        /// <summary>
        /// Initializes a new instance of the <see cref="PipelineInitializer"/> class.
        /// </summary>
        /// <param name="initializationStrategies">The initialization strategies to execute.</param>
        public PipelineInitializer(List<IInitializationStrategy> initializationStrategies)
        {
            this.initializationStrategies = initializationStrategies;
        }

        /// <summary>
        /// Initializes an instance using the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="instance">The instance being initialized.</param>
        /// <returns>
        /// The initialized instance.
        /// </returns>
        public object Initialize(IContext context, object instance)
        {
            this.initializationStrategies.ForEach(i => instance = i.Initialize(context, instance));
            return instance;
        }
    }
}