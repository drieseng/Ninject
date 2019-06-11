// -------------------------------------------------------------------------------------------------
// <copyright file="MethodInjectionStrategy.cs" company="Ninject Project Contributors">
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
    using System.Reflection;
    using Ninject.Infrastructure;
    using Ninject.Planning.Directives;
    using Ninject.Planning.Targets;

    /// <summary>
    /// Injects methods on an instance during activation.
    /// </summary>
    public class MethodInjectionStrategy : IInitializationStrategy
    {
        /// <summary>
        /// Injects values into the properties as described by <see cref="MethodInjectionDirective"/>s
        /// contained in the plan.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="instance">The instance being initialized.</param>
        /// <exception cref="ArgumentNullException"><paramref name="context"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="instance"/> is <see langword="null"/>.</exception>
        public object Initialize(IContext context, object instance)
        {
            Ensure.ArgumentNotNull(context, nameof(context));
            Ensure.ArgumentNotNull(instance, nameof(instance));

            foreach (var directive in context.Plan.GetMethods())
            {
                directive.Injector(instance, GetMethodArguments(directive.Targets, context));
            }

            return instance;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        void IDisposable.Dispose()
        {
        }

        private static object[] GetMethodArguments(ITarget<ParameterInfo>[] targets, IContext context)
        {
            var arguments = new object[targets.Length];

            for (var i = 0; i < targets.Length; i++)
            {
                arguments[i] = targets[i].ResolveWithin(context);
            }

            return arguments;
        }
    }
}