namespace Ninject.Builder
{
    internal class FeatureBuilder : IFeatureBuilder
    {
        private readonly ComponentBindingRoot componentRoot;

        public FeatureBuilder()
        {
            this.componentRoot = new ComponentBindingRoot();
        }

        /// <summary>
        /// Gets the root of the component bindings.
        /// </summary>
        /// <value>
        /// The root of the component binding.
        /// </value>
        public IComponentBindingRoot Components
        {
            get { return this.componentRoot; }
        }
    }
}
