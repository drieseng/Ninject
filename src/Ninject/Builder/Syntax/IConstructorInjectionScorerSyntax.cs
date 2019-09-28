using System;
using System.Collections.Generic;
using System.Text;

namespace Ninject.Builder.Syntax
{
    public interface IConstructorInjectionScorerSyntax
    {
        void Scorer(Action<IConstructorScorerBuilder> scorerBuilder);
    }
}
