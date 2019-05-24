// -------------------------------------------------------------------------------------------------
// <copyright file="ComponentParameterValueProvider.cs" company="Ninject Project Contributors">
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

namespace Ninject.Components
{
    using System;
    using System.Collections.Generic;

    using Ninject.Activation;
    using Ninject.Activation.Providers;
    using Ninject.Infrastructure.Language;
    using Ninject.Parameters;
    using Ninject.Planning.Directives;
    using Ninject.Planning.Targets;

    /// <summary>
    /// Provides parameter values to instantiate an <see cref="INinjectComponent"/>.
    /// </summary>
    internal class ComponentParameterValueProvider : IConstructorParameterValueProvider
    {
        private readonly ComponentContainer components;

        /// <summary>
        /// Initializes a new instance of the <see cref="ComponentParameterValueProvider"/> class.
        /// </summary>
        /// <param name="components">The components.</param>
        public ComponentParameterValueProvider(ComponentContainer components)
        {
            this.components = components;
        }

        /// <summary>
        /// Gets the values to inject in the constructor from the specified context.
        /// </summary>
        /// <param name="constructor">The constructor to provide values for.</param>
        /// <param name="context">The context.</param>
        /// <returns>
        /// The values.
        /// </returns>
        public object[] GetValues(ConstructorInjectionDirective constructor, IContext context)
        {
            var targets = constructor.Targets;

            if (targets.Length == 0)
            {
                return Array.Empty<object>();
            }

            var constructorArguments = context.Parameters.GetConstructorArguments();
            var values = new object[targets.Length];

            for (var i = 0; i < targets.Length; i++)
            {
                values[i] = this.GetConstructorValue(context, targets[i], constructorArguments);
            }

            return values;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        void IDisposable.Dispose()
        {
        }

        private object GetConstructorValue(IContext context, ITarget target, List<IConstructorArgument> constructorArguments)
        {
            IConstructorArgument match = null;

            if (constructorArguments.Count > 0)
            {
                foreach (var parameter in constructorArguments)
                {
                    if (parameter.AppliesToTarget(context, target))
                    {
                        if (match != null)
                        {
                            throw new InvalidOperationException("Sequence contains more than one matching element");
                        }

                        match = parameter;
                    }
                }

                if (match != null)
                {
                    return match.GetValue(context, target);
                }
            }

            return this.components.Get(target.Type);
        }
    }
}