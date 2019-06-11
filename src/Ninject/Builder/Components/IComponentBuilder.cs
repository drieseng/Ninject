namespace Ninject.Builder.Components
{
    using System.ComponentModel;

    using Ninject.Builder.Syntax;
    using Ninject.Syntax;

    public interface IComponentBuilder : IFluentSyntax
    {
        /// <summary>
        /// Builds the component.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        void Build(IComponentBindingRoot root);
    }
}