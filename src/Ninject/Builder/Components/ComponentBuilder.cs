namespace Ninject.Builder.Components
{
    using System;
    using System.Collections.Generic;

    internal abstract class ComponentBuilder : IComponentBuilder
    {
        protected ComponentBuilder(IList<IComponentBuilder> components)
        {
            Components = components;
        }

        public IList<IComponentBuilder> Components { get; }

        public void Build(IComponentBindingRoot root)
        {
            throw new NotImplementedException();
        }
    }
}
