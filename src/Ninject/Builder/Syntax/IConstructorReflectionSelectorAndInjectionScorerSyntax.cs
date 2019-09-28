using System;
using System.Collections.Generic;
using System.Text;

namespace Ninject.Builder.Syntax
{
    public interface IConstructorReflectionSelectorAndScorerSyntax
    {
        IConstructorInjectionScorerSyntax Selector(Action<IConstructorReflectionSelectorBuilder> selectorBuilder);

        IConstructorReflectionSelectorSyntax Scorer(Action<IConstructorScorerBuilder> scorerBuilder);
    }
}
