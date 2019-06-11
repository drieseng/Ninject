// -------------------------------------------------------------------------------------------------
// <copyright file="PropertyInjectionStrategy.cs" company="Ninject Project Contributors">
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

namespace Ninject.Activation.Strategies
{
    using System;
    using System.Collections.Generic;

    using Ninject.Activation.Providers;
    using Ninject.Components;
    using Ninject.Infrastructure;
    using Ninject.Parameters;
    using Ninject.Planning.Directives;

    /// <summary>
    /// Injects properties on an instance during activation.
    /// </summary>
    public class PropertyInjectionStrategy : IInitializationStrategy
    {
        /// <summary>
        /// The <see cref="IPropertyValueProvider"/> provider.
        /// </summary>
        private readonly IPropertyValueProvider propertyValueProvider;

        /// <summary>
        /// The <see cref="IExceptionFormatter"/> component.
        /// </summary>
        private readonly IExceptionFormatter exceptionFormatter;

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyInjectionStrategy"/> class.
        /// </summary>
        /// <param name="propertyValueProvider">The value provider.</param>
        /// <param name="exceptionFormatter">The <see cref="IExceptionFormatter"/> component.</param>
        /// <exception cref="ArgumentNullException"><paramref name="propertyValueProvider"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="exceptionFormatter"/> is <see langword="null"/>.</exception>
        public PropertyInjectionStrategy(IPropertyValueProvider propertyValueProvider, IExceptionFormatter exceptionFormatter)
        {
            Ensure.ArgumentNotNull(propertyValueProvider, nameof(propertyValueProvider));
            Ensure.ArgumentNotNull(exceptionFormatter, nameof(exceptionFormatter));

            this.propertyValueProvider = propertyValueProvider;
            this.exceptionFormatter = exceptionFormatter;
        }

        /// <summary>
        /// Injects values into the properties as described by <see cref="PropertyInjectionDirective"/>s
        /// contained in the plan.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="instance">The instance being initialized.</param>
        /// <returns>
        /// The initialized instance.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="context"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="instance"/> is <see langword="null"/>.</exception>
        public object Initialize(IContext context, object instance)
        {
            Ensure.ArgumentNotNull(context, nameof(context));
            Ensure.ArgumentNotNull(instance, nameof(instance));

            var properties = context.Plan.GetProperties();
            var propertyParameters = GetPropertyParameters(context);

            if (properties.Count > 0)
            {
                if (propertyParameters.Count > 0)
                {
                    foreach (var directive in properties)
                    {
                        IPropertyValue match = null;

                        foreach (var propertyParameter in propertyParameters)
                        {
                            if (propertyParameter.AppliesToTarget(context, directive.Target))
                            {
                                if (match != null)
                                {
                                    throw new ActivationException(this.exceptionFormatter.MoreThanOnePropertyValueForTarget(context, directive.Target));
                                }

                                match = propertyParameter;
                            }
                        }

                        object value;

                        if (match != null)
                        {
                            value = match.GetValue(context, directive.Target);
                            propertyParameters.Remove(match);
                        }
                        else
                        {
                            value = this.propertyValueProvider.GetValue(directive, context);
                        }

                        directive.Injector(instance, value);
                    }

                    // Check if there are any property parameters for which we have no found a corresponding property
                    if (propertyParameters.Count > 0)
                    {
                        throw new ActivationException(this.exceptionFormatter.CouldNotResolvePropertyForValueInjection(context.Request, propertyParameters[0].Name));
                    }
                }
                else
                {
                    foreach (var directive in properties)
                    {
                        var value = this.propertyValueProvider.GetValue(directive, context);
                        directive.Injector(instance, value);
                    }
                }
            }
            else
            {
                // Check if there are any property parameters (for which we have not found a corresponding property)
                if (propertyParameters.Count > 0)
                {
                    throw new ActivationException(this.exceptionFormatter.CouldNotResolvePropertyForValueInjection(context.Request, propertyParameters[0].Name));
                }
            }

            return instance;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        void IDisposable.Dispose()
        {
        }

        private static List<IPropertyValue> GetPropertyParameters(IContext context)
        {
            var parameters = context.Parameters;
            if (parameters.Count == 0)
            {
                return new List<IPropertyValue>();
            }

            var propertyParameters = new List<IPropertyValue>();

            foreach (var parameter in parameters)
            {
                var propertyValue = parameter as IPropertyValue;
                if (propertyValue != null)
                {
                    propertyParameters.Add(propertyValue);
                }
            }

            return propertyParameters;
        }
    }
}