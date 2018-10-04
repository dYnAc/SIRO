
namespace SIRO.Core.Sitemap
{
    using System;
    using System.Configuration;
    using System.Xml;

    /// <summary>
    /// The sitemap factory.
    /// </summary>
    public class SitemapFactory
    {
        /// <summary>
        /// The sitemap config file path.
        /// </summary>
        private static readonly string SitemapConfigFilePath = ConfigurationManager.AppSettings["siteMapConfig"];

        /// <summary>
        /// The doc.
        /// </summary>
        private static XmlDocument doc;

        /// <summary>
        /// Initializes a new instance of the <see cref="SitemapFactory"/> class.
        /// </summary>
        protected SitemapFactory()
        {
            LoadConfigDocument();
        }

        /// <summary>
        /// The get config nodes.
        /// </summary>
        /// <param name="node">
        /// The node.
        /// </param>
        /// <returns>
        /// The <see cref="XmlNodeList"/>.
        /// </returns>
        public static XmlNodeList GetConfigNodes(string node)
        {
            LoadConfigDocument();
            return doc.SelectNodes(node);
        }

        /// <summary>
        /// The load config document.
        /// </summary>
        /// <exception cref="Exception">
        /// If the file is not found
        /// </exception>
        private static void LoadConfigDocument()
        {
            try
            {
                doc = new XmlDocument();
                doc.Load(System.Web.Hosting.HostingEnvironment.MapPath(string.Concat("/", SitemapConfigFilePath)));
            }
            catch (System.IO.FileNotFoundException e)
            {
                throw new Exception("No configuration file found.", e);
            }
            catch (Exception ex)
            {
                throw new Exception("Other error found.", ex);
            }
        }
    }
}