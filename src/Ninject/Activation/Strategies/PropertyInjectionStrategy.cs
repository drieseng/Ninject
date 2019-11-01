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
    using System.Reflection;
    using Ninject.Components;
    using Ninject.Infrastructure;
    using Ninject.Injection;
    using Ninject.Parameters;
    using Ninject.Planning.Directives;
    using Ninject.Selection;

    /// <summary>
    /// Injects properties on an instance during activation.
    /// </summary>
    public class PropertyInjectionStrategy : IInitializationStrategy
    {
        /// <summary>
        /// Selects properties where <see cref="IPropertyValue"/> instances can be applied to.
        /// </summary>
        private readonly IPropertyReflectionSelector propertyReflectionSelector;

        /// <summary>
        /// The injector factory component.
        /// </summary>
        private readonly IInjectorFactory injectorFactory;

        /// <summary>
        /// The <see cref="IExceptionFormatter"/> component.
        /// </summary>
        private readonly IExceptionFormatter exceptionFormatter;

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyInjectionStrategy"/> class.
        /// </summary>
        /// <param name="propertyReflectionSelector">Selects properties where <see cref="IPropertyValue"/> instances can be applied to.</param>
        /// <param name="injectorFactory">The injector factory component.</param>
        /// <param name="exceptionFormatter">The <see cref="IExceptionFormatter"/> component.</param>
        /// <exception cref="ArgumentNullException"><paramref name="propertyReflectionSelector"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="injectorFactory"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="exceptionFormatter"/> is <see langword="null"/>.</exception>
        public PropertyInjectionStrategy(IPropertyReflectionSelector propertyReflectionSelector,
                                         IInjectorFactory injectorFactory,
                                         IExceptionFormatter exceptionFormatter)
        {
            Ensure.ArgumentNotNull(propertyReflectionSelector, nameof(propertyReflectionSelector));
            Ensure.ArgumentNotNull(injectorFactory, nameof(injectorFactory));
            Ensure.ArgumentNotNull(exceptionFormatter, nameof(exceptionFormatter));

            this.propertyReflectionSelector = propertyReflectionSelector;
            this.injectorFactory = injectorFactory;
            this.exceptionFormatter = exceptionFormatter;
        }

        /// <summary>
        /// Injects values into the properties as described by <see cref="PropertyInjectionDirective"/> instances
        /// in the <see cref="Context.Plan"/>.
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

            var propertyDirectives = context.Plan.GetProperties();
            var propertyValues = GetPropertyValues(context);

            if (propertyDirectives.Count > 0)
            {
                if (propertyValues.Count > 0)
                {
                    for (var d = 0; d < propertyDirectives.Count; d++)
                    {
                        var directive = propertyDirectives[d];

                        IPropertyValue match = null;

                        for (var v = (propertyValues.Count - 1); v >= 0; v--)
                        {
                            var propertyParameter = propertyValues[v];
                            if (propertyParameter.AppliesToTarget(context, directive.Target))
                            {
                                if (match != null)
                                {
                                    throw new ActivationException(this.exceptionFormatter.MoreThanOnePropertyValueForTarget(context, directive.Target));
                                }

                                match = propertyParameter;
                                propertyValues.RemoveAt(v);
                            }
                        }

                        object value;

                        if (match != null)
                        {
                            value = match.GetValue(context, directive.Target);
                        }
                        else
                        {
                            value = directive.Target.ResolveWithin(context);
                        }

                        directive.Injector(instance, value);
                    }

                    if (propertyValues.Count > 0)
                    {
                        AssignPropertyValueOverrides(context, instance, propertyValues);
                    }
                }
                else
                {
                    for (var d = 0; d < propertyDirectives.Count; d++)
                    {
                        var directive = propertyDirectives[d];
                        var value = directive.Target.ResolveWithin(context);
                        directive.Injector(instance, value);
                    }
                }
            }
            else
            {
                if (propertyValues.Count > 0)
                {
                    AssignPropertyValueOverrides(context, instance, propertyValues);
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

        /// <summary>
        /// Applies user supplied override values to instance properties.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="instance">The instance being activated.</param>
        /// <param name="propertyValues">The parameter override value accessors.</param>
        /// <exception cref="ActivationException">A given <see cref="IPropertyValue"/> cannot be resolved to a property of the specified instance.</exception>
        private void AssignPropertyValueOverrides(IContext context, object instance, List<IPropertyValue> propertyValues)
        {
            var properties = new List<PropertyInfo>(this.propertyReflectionSelector.Select(instance.GetType()));

            for (var v = 0; v < propertyValues.Count; v++)
            {
                var propertyValue = propertyValues[v];

                var propertyInfo = FindPropertyByName(properties, propertyValue.Name, StringComparison.Ordinal);
                if (propertyInfo == null)
                {
                    throw new ActivationException(this.exceptionFormatter.CouldNotResolvePropertyForValueInjection(context.Request, propertyValue.Name));
                }

                var target = new PropertyInjectionDirective(propertyInfo, this.injectorFactory.Create(propertyInfo));
                var value = propertyValue.GetValue(context, target.Target);
                target.Injector(instance, value);
            }
        }

        private static List<IPropertyValue> GetPropertyValues(IContext context)
        {
            var parameters = context.Parameters;
            if (parameters.Count == 0)
            {
                return new List<IPropertyValue>();
            }

            var propertyParameters = new List<IPropertyValue>();

            for (var p = 0; p < parameters.Count; p++)
            {
                var propertyValue = parameters[p] as IPropertyValue;
                if (propertyValue != null)
                {
                    propertyParameters.Add(propertyValue);
                }
            }

            return propertyParameters;
        }

        /// <summary>
        /// Locates a <see cref="PropertyInfo"/> by name using the specified <see cref="StringComparison"/>.
        /// </summary>
        /// <param name="properties">The list of properties to search.</param>
        /// <param name="name">The name to find.</param>
        /// <param name="stringComparison">The <see cref="StringComparison"/> to use when comparing the name.</param>
        /// <returns>
        /// The <see cref="PropertyInfo"/> with a matching <see cref="IParameter.Name"/>, if found; otherwise,
        /// <see langword="null"/>.
        /// </returns>
        private static PropertyInfo FindPropertyByName(List<PropertyInfo> properties, string name, StringComparison stringComparison)
        {
            PropertyInfo found = null;

            for (var p = 0; p < properties.Count; p++)
            {
                var property = properties[p];
                if (string.Equals(property.Name, name, stringComparison))
                {
                    found = property;
                    break;
                }
            }

            return found;
        }
    }
}