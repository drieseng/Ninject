using Ninject.Syntax;

namespace Ninject.Builder.Components
{
    public interface IComponentBuilder<T>
    {
        void Build(INewBindingToSyntax<T> bind);
    }
}
