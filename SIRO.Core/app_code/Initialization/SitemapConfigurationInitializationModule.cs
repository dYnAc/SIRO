
namespace SIRO.Core.Initialization
{
    using System.Web.Mvc;
    using System.Web.Routing;
    using EPiServer.Framework;
    using EPiServer.Framework.Initialization;

    /// <summary>
    /// The sitemap configuration initialization module.
    /// </summary>
    [InitializableModule]
    public class SitemapConfigurationInitializationModule : IInitializableModule
    {
        private bool initialized = false;

        /// <summary>
        /// The initialize.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        public void Initialize(InitializationEngine context)
        {
            if (!initialized)
            {
                RouteTable.Routes.MapRoute(
                    name: "SitemapConfiguration",
                    url: "SitemapPlugin/{action}/{id}",
                    defaults: new { controller = "SitemapPlugin", action = "Index", id="" });

                RouteTable.Routes.MapRoute(
                    name: "SitemapEditor",
                    url: "SitemapGadget/{action}/{id}",
                    defaults: new { controller = "SitemapGadget", action = "Index", id = "" });
            }
        }

        /// <summary>
        /// The uninitialize.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        public void Uninitialize(InitializationEngine context)
        {
            // Do nothing.
        }
    }
}
