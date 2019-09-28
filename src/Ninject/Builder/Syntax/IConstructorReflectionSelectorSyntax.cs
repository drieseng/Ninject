using System;

namespace Ninject.Builder.Syntax
{
    public interface IConstructorReflectionSelectorSyntax
    {
        void Selector(Action<IConstructorReflectionSelectorBuilder> selectorBuilder);
    }
}
