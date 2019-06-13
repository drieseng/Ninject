namespace Ninject.Builder
{
    using System.ComponentModel;

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