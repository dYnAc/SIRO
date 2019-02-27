using SIRO.Core.IoC;
using SIRO.Core.Models.DDS;
using SIRO.Core.Sitemap;

namespace SIRO.Core.Initialization
{
    using System.Web.Mvc;
    using EPiServer.Framework;
    using EPiServer.Framework.Initialization;
    using EPiServer.ServiceLocation;

    /// <summary>
    /// The dependency resolver initialization.
    /// </summary>
    [InitializableModule]
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
            //Implementations for custom interfaces can be registered here.
            context.ConfigurationComplete += (o, e) =>
            {
                //Register custom implementations that should be used in favour of the default implementations
                context.Services
                    .AddTransient<ISitemapConfigurationDataStoreManager, SitemapConfigurationDataStoreManager>()
                    .AddTransient<ISitemapGenerator, SitemapGenerator>()
                    .AddTransient<ISitemapManager, SitemapManager>();
            };
        }

        /// <summary>
        /// The initialize.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        public void Initialize(InitializationEngine context)
        {
            DependencyResolver.SetResolver(new ServiceLocatorDependencyResolver(context.Locate.Advanced));
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
    }
}
