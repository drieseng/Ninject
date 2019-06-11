// -------------------------------------------------------------------------------------------------
// <copyright file="ConstructorParameterValueProvider.cs" company="Ninject Project Contributors">
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

namespace Ninject.Activation.Providers
{
    using System;
    using System.Collections.Generic;

    using Ninject.Parameters;
    using Ninject.Planning.Directives;
    using Ninject.Planning.Targets;

    /// <summary>
    /// Provides values that can be injected into a given constructor.
    /// </summary>
    internal sealed class ConstructorParameterValueProvider : IConstructorParameterValueProvider
    {
        /// <summary>
        /// Gets values to inject in the specified constructor in a given context.
        /// </summary>
        /// <param name="constructor">The constructor to provide values for.</param>
        /// <param name="context">The context.</param>
        /// <returns>
        /// The values.
        /// </returns>
        public object[] GetValues(IConstructorInjectionDirective constructor, IContext context)
        {
            var targets = constructor.Targets;

            if (targets.Length == 0)
            {
                return Array.Empty<object>();
            }

            var constructorArguments = GetConstructorArguments(context);
            var values = new object[targets.Length];

            for (var i = 0; i < targets.Length; i++)
            {
                values[i] = GetConstructorValue(context, targets[i], constructorArguments);
            }

            return values;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        void IDisposable.Dispose()
        {
        }

        private static object GetConstructorValue(IContext context, ITarget target, List<IConstructorArgument> constructorArguments)
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

            return target.ResolveWithin(context);
        }

        private static List<IConstructorArgument> GetConstructorArguments(IContext context)
        {
            var parameters = context.Parameters;
            if (parameters.Count == 0)
            {
                return new List<IConstructorArgument>();
            }

            var constructorArguments = new List<IConstructorArgument>();

            foreach (var parameter in parameters)
            {
                var constructorArgument = parameter as IConstructorArgument;
                if (constructorArgument != null)
                {
                    constructorArguments.Add(constructorArgument);
                }
            }

            return constructorArguments;
        }
    }
}