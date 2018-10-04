
namespace SIRO.Core.Initialization
{
    using System.Web.Mvc;
    using EPiServer.Framework;
    using EPiServer.Framework.Initialization;
    using EPiServer.ServiceLocation;
    using IoC;
    using Models.DDS;
    using Sitemap;
    using StructureMap;

    /// <summary>
    /// The dependency resolver initialization.
    /// </summary>
    [InitializableModule]
    [ModuleDependency(typeof(EPiServer.Web.InitializationModule))]
    public class DependencyResolverInitialization : IConfigurableModule
    {
        /// <summary>
        /// The configure container.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        public void ConfigureContainer(ServiceConfigurationContext context)
        {
            context?.StructureMap().Configure(ConfigureContainer);
            DependencyResolver.SetResolver(new StructureMapDependencyResolver(context?.StructureMap()));
        }

      /// <summary>
        /// The initialize.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        public void Initialize(InitializationEngine context)
        {
        }

        /// <summary>
        /// The uninitialize.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        public void Uninitialize(InitializationEngine context)
        {
        }

        /// <summary>
        /// The preload.
        /// </summary>
        /// <param name="parameters">
        /// The parameters.
        /// </param>
        public void Preload(string[] parameters)
        {
        }

        /// <summary>
        /// The configure container.
        /// </summary>
        /// <param name="container">
        /// The container.
        /// </param>
        private static void ConfigureContainer(ConfigurationExpression container)
        {
            //Swap out the default ContentRenderer for our custom
            container.For<ISitemapConfigurationDataStoreManager>().Use<SitemapConfigurationDataStoreManager>();
            container.For<ISitemapGenerator>().Use<SitemapGenerator>();
            container.For<ISitemapManager>().Use<SitemapManager>();

            //Implementations for custom interfaces can be registered here.
        }
    }
}
