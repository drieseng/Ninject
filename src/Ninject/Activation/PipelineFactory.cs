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
    using Ninject.Activation.Caching;
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
                CreatePipelineActivator(root, activationStrategies),
                CreatePipelineDeactivator(root, deactivationStrategies));
        }

        private static IPipelineInitializer CreatePipelineInitializer(List<IInitializationStrategy> initializationStrategies)
        {
            if (initializationStrategies.Count > 0)
            {
                return new PipelineInitializer(initializationStrategies);
            }

            return new NoOpPipelineInitializer();
        }

        private static IPipelineActivator CreatePipelineActivator(IResolutionRoot root, List<IActivationStrategy> activationStrategies)
        {
            if (activationStrategies.Count > 0)
            {
                // If any activation strategy is defined, make sure to register the ActivationCacheStrategy as
                // first in the activation pipeline so that the activation is registered even in case one of the
                // strategies throw
                activationStrategies.Insert(0, root.Get<ActivationCacheStrategy>());

                return new PipelineActivator(activationStrategies, root.Get<IActivationCache>());
            }

            return new NoOpPipelineActivator();
        }

        private static IPipelineDeactivator CreatePipelineDeactivator(IResolutionRoot root, List<IDeactivationStrategy> deactivationStrategies)
        {
            if (deactivationStrategies.Count > 0)
            {
                // If any deactivation strategy is defined, make sure to register the DeactivationCacheStrategy as
                // first in the deactivation pipeline so that the deactivation is registered even in case one of the
                // strategies throw
                deactivationStrategies.Insert(0, root.Get<DeactivationCacheStrategy>());

                return new PipelineDeactivator(deactivationStrategies, root.Get<IActivationCache>());
            }

            return new NoOpPipelineDeactivator();
        }
    }
}