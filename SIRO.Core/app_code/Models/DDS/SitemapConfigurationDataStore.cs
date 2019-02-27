
namespace SIRO.Core.Models.DDS
{
    using System.Collections.Generic;
    using EPiServer.Data;
    using EPiServer.Data.Dynamic;
    using Properties;

    /// <summary>
    /// The sitemap configuration data store.
    /// </summary>
    [EPiServerDataStore(AutomaticallyCreateStore = true, AutomaticallyRemapStore = true)]
    public class SitemapConfigurationDataStore
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SitemapConfigurationDataStore"/> class.
        /// </summary>
        public SitemapConfigurationDataStore()
        {
            this.Id = null;
            this.SearchEngines = new List<SearchEngine>();
            this.RestrictedTypes = new List<RestrictedType>();
            this.RestrictedSites = new List<RestrictedSite>();
        }

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        public Identity Id { get; set; }

        /// <summary>
        /// Gets or sets the search engines.
        /// </summary>
        public IList<SearchEngine> SearchEngines { get; set; }

        /// <summary>
        /// Gets or sets the restricted types.
        /// </summary>
        public IList<RestrictedType> RestrictedTypes { get; set; }

        /// <summary>
        /// Gets or sets the restricted sites.
        /// </summary>
        public IList<RestrictedSite> RestrictedSites { get; set; }
    }
}