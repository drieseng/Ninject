// -------------------------------------------------------------------------------------------------
// <copyright file="ConstructorProviderFactory.cs" company="Ninject Project Contributors">
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
    using System.Linq.Expressions;

    using Ninject.Activation;
    using Ninject.Activation.Providers;
    using Ninject.Components;
    using Ninject.Parameters;
    using Ninject.Planning;
    using Ninject.Selection;
    using Ninject.Selection.Heuristics;
    using Ninject.Syntax;

    public class ConstructorProviderFactory<T> : IProviderFactory
    {
        private Expression<Func<IConstructorArgumentSyntax, T>> newExpression;

        public ConstructorProviderFactory(Expression<Func<IConstructorArgumentSyntax, T>> newExpression)
        {
            if (!(newExpression.Body is NewExpression))
            {
                throw new ArgumentException("The expression must be a constructor call.", nameof(newExpression));
            }

            this.newExpression = newExpression;
        }

        public IProvider Create(IResolutionRoot root, IReadOnlyList<IParameter> parameters)
        {
            var ctorExpression = (NewExpression)this.newExpression.Body;

            var scorer = new SpecificConstructorSelector(ctorExpression.Constructor);
            var selector = new BestMatchConstructorInjectionSelector(scorer, root.Get<IExceptionFormatter>());
            var plan = root.Get<IPlanner>().GetPlan(typeof(T));

            return new ContextAwareConstructorProvider(plan, selector, root.Get<IPipeline>(), root.Get<IConstructorParameterValueProvider>());
        }

        public List<ConstructorArgument> GetConstructorArguments()
        {
            var ctorExpression = (NewExpression)this.newExpression.Body;

            var ctorParameters = ctorExpression.Constructor.GetParameters();

            var constructorArguments = new List<ConstructorArgument>();

            for (var i = 0; i < ctorExpression.Arguments.Count; i++)
            {
                var argument = ctorExpression.Arguments[i];
                var argumentName = ctorParameters[i].Name;

                var constructorArgument = CreateConstructorArgument(argument, argumentName, this.newExpression.Parameters[0]);
                if (constructorArgument != null)
                {
                    constructorArguments.Add(constructorArgument);
                }
            }

            return constructorArguments;
        }

        /// <summary>
        /// Adds a constructor argument for the specified argument expression.
        /// </summary>
        /// <param name="argument">The argument.</param>
        /// <param name="argumentName">Name of the argument.</param>
        /// <param name="constructorArgumentSyntaxParameterExpression">The constructor argument syntax parameter expression.</param>
        private static ConstructorArgument CreateConstructorArgument(Expression argument, string argumentName, ParameterExpression constructorArgumentSyntaxParameterExpression)
        {
            if (!(argument is MethodCallExpression methodCall) ||
                !methodCall.Method.IsGenericMethod ||
                methodCall.Method.GetGenericMethodDefinition().DeclaringType != typeof(IConstructorArgumentSyntax))
            {
                var compiledExpression = Expression.Lambda(argument, constructorArgumentSyntaxParameterExpression).Compile();
                return new ConstructorArgument(
                    argumentName,
                    ctx => compiledExpression.DynamicInvoke(new ConstructorArgumentSyntax(ctx)));
            }

            return null;
        }

        /// <summary>
        /// Passed to ToConstructor to specify that a constructor value is Injected.
        /// </summary>
        private class ConstructorArgumentSyntax : IConstructorArgumentSyntax
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="ConstructorArgumentSyntax"/> class.
            /// </summary>
            /// <param name="context">The context.</param>
            public ConstructorArgumentSyntax(IContext context)
            {
                this.Context = context;
            }

            /// <summary>
            /// Gets the context.
            /// </summary>
            /// <value>The context.</value>
            public IContext Context
            {
                get;
                private set;
            }

            /// <summary>
            /// Specifies that the argument is injected.
            /// </summary>
            /// <typeparam name="T1">The type of the parameter.</typeparam>
            /// <returns>Not used. This interface has no implementation.</returns>
            public T1 Inject<T1>()
            {
                throw new InvalidOperationException("This method is for declaration that a parameter shall be injected only! Never call it directly.");
            }
        }
    }
}