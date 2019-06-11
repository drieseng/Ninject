// -------------------------------------------------------------------------------------------------
// <copyright file="PipelineFactory.cs" company="Ninject Project Contributors">
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
    using System.Linq;

    using Ninject.Activation.Strategies;
    using Ninject.Syntax;

    /// <summary>
    /// Factory for creating a minimal <see cref="IPipeline"/> based on the configured strategies.
    /// </summary>
    internal sealed class PipelineFactory
    {
        /// <summary>
        /// Creates a minimal <see cref="IPipeline"/>.
        /// </summary>
        /// <param name="root">The resolution root.</param>
        /// <returns>
        /// An <see cref="IPipeline"/>.
        /// </returns>
        public IPipeline Create(IResolutionRoot root)
        {
            var initializationStrategies = root.GetAll<IInitializationStrategy>().ToList();
            var activationStrategies = root.GetAll<IActivationStrategy>().ToList();
            var deactivationStrategies = root.GetAll<IDeactivationStrategy>().ToList();

            if (initializationStrategies.Count == 0 && activationStrategies.Count == 0 && deactivationStrategies.Count == 0)
            {
                return new NoOpPipeline();
            }

            return new DefaultPipeline(
                CreatePipelineInitializer(initializationStrategies),
                CreatePipelineActivator(activationStrategies),
                CreatePipelineDeactivator(deactivationStrategies));
        }

        private static IPipelineInitializer CreatePipelineInitializer(List<IInitializationStrategy> initializationStrategies)
        {
            if (initializationStrategies.Count > 0)
            {
                return new PipelineInitializer(initializationStrategies);
            }

            return new NoOpPipelineInitializer();
        }

        private static IPipelineActivator CreatePipelineActivator(List<IActivationStrategy> activationStrategies)
        {
            if (activationStrategies.Count > 0)
            {
                return new PipelineActivator(activationStrategies);
            }

            return new NoOpPipelineActivator();
        }

        private static IPipelineDeactivator CreatePipelineDeactivator(List<IDeactivationStrategy> deactivationStrategies)
        {
            if (deactivationStrategies.Count > 0)
            {
                return new PipelineDeactivator(deactivationStrategies);
            }

            return new NoOpPipelineDeactivator();
        }
    }
}