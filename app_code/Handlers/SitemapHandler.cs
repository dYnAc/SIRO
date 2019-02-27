
namespace SIRO.Handlers
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Web;
    using Core.Models.DDS;
    using EPiServer.ServiceLocation;
    using EPiServer.Web;

    /// <summary>
    /// The sitemap handler.
    /// </summary>
    public class SitemapHandler : IHttpHandler
    {
        /// <summary>
        /// The sitemap sub folder.
        /// </summary>
        private const string SitemapSubFolder = "sitemaps";

        /// <summary>
        /// The site rep.
        /// </summary>
        private readonly ISiteDefinitionRepository siteRep;

        /// <summary>
        /// The sitemap config manager.
        /// </summary>
        private readonly ISitemapConfigurationDataStoreManager sitemapConfigManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="SitemapHandler"/> class.
        /// </summary>
        public SitemapHandler()
        {
            this.siteRep = ServiceLocator.Current.GetInstance<ISiteDefinitionRepository>();
            this.sitemapConfigManager = new SitemapConfigurationDataStoreManager();
        }

        /// <summary>
        /// The is reusable.
        /// </summary>
        public bool IsReusable => false;

        /// <summary>
        /// The process request.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        public void ProcessRequest(HttpContext context)
        {
            var domainUri = context.Request.Url;
            var currentSite = this.siteRep.List().FirstOrDefault(x => verifyInHost(x.Hosts, domainUri.Authority) == true);

            // Set the response code, content type and appropriate sitemap file here
            // also think about handling caching, sending error codes etc.
            context.Response.Clear();
            context.Response.StatusCode = 200;
            context.Response.ContentType = "text/xml";

            var sitemapXmlContent = string.Empty;

            if (currentSite != null)
            {
                var sitemapConfigData = this.sitemapConfigManager.GetFirstData();

                var restrictedSite = sitemapConfigData?.RestrictedSites?.FirstOrDefault(x => x.SiteName == currentSite.Name);

                // Generate sitemap.xml file
                if (restrictedSite != null && !restrictedSite.Restricted)
                {
                    var fileName = string.Concat("/", SitemapSubFolder, "/", restrictedSite.SiteMapFileName);
                    var content = File.ReadAllText(System.Web.Hosting.HostingEnvironment.MapPath(fileName));
                    sitemapXmlContent += content;
                }
            }

            // Return the site map content
            context.Response.Write(sitemapXmlContent);
            context.Response.End();
        }

        private bool verifyInHost(IList<HostDefinition> hosts, string siteName)
        {
            foreach (var host in hosts)
            {
                if (host.Authority.Hostname == siteName)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
