using System;
using System.Collections.Generic;
using System.Reflection;
using Ninject.Parameters;
using Ninject.Planning.Directives;
using Ninject.Planning.Targets;

namespace Ninject.Activation.Providers
{
    public class MethodParameterValueProvider : IMethodParameterValueProvider
    {
        /// <summary>
        /// Gets the values to inject in the method from the specified context.
        /// </summary>
        /// <param name="method">The method to provide values for.</param>
        /// <param name="context">The context.</param>
        /// <returns>
        /// The values.
        /// </returns>
        public object[] GetValues(IMethodInjectionDirective method, IContext context)
        {
            var targets = method.Targets;

            if (targets.Length == 0)
            {
                return Array.Empty<object>();
            }

            var methodArguments = GetMethodArguments(context);
            var values = new object[targets.Length];

            for (var i = 0; i < targets.Length; i++)
            {
                values[i] = GetMethodArgumentValue(context, targets[i], methodArguments);
            }

            return values;
        }

        private static List<IMethodArgument> GetMethodArguments(IContext context)
        {
            var parameters = context.Parameters;
            if (parameters.Count == 0)
            {
                return new List<IMethodArgument>();
            }

            var methodArguments = new List<IMethodArgument>();

            foreach (var parameter in parameters)
            {
                var methodArgument = parameter as IMethodArgument;
                if (methodArgument != null)
                {
                    methodArguments.Add(methodArgument);
                }
            }

            return methodArguments;
        }

        private static object GetMethodArgumentValue(IContext context, ITarget<ParameterInfo> target, List<IMethodArgument> methodArguments)
        {
            IMethodArgument match = null;

            if (methodArguments.Count > 0)
            {
                foreach (var parameter in methodArguments)
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
    }
}
