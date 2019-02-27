
namespace SIRO.Core.Models.ViewModels
{
    using System.Collections.Generic;
    using System.Linq;
    using DDS;
    using EPiServer.Core;
    using EPiServer.DataAbstraction;
    using EPiServer.ServiceLocation;
    using EPiServer.Web;
    using Properties;

    /// <summary>
    /// The sitemap configuration view model.
    /// </summary>
    public class SitemapConfigurationViewModel
    {
        /// <summary>
        /// The sitemap config data.
        /// </summary>
        private readonly SitemapConfigurationDataStore sitemapConfigData;

        /// <summary>
        /// The sitemap config manager.
        /// </summary>
        private readonly Injected<ISitemapConfigurationDataStoreManager> sitemapConfigManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="SitemapConfigurationViewModel"/> class.
        /// </summary>
        /// <param name="sitemapConfigManager">
        /// The _sitemap config manager.
        /// </param>
        public SitemapConfigurationViewModel()
        {
            this.sitemapConfigData = this.sitemapConfigManager.Service.GetFirstData() ?? new SitemapConfigurationDataStore();

            // Load data from DB
            this.RestrictedTypes = this.LoadCurrentPageTypes();
            this.RestrictedSites = this.LoadCurrentSites();
            this.SearchEngines = this.sitemapConfigData.SearchEngines?.ToList() ?? new List<SearchEngine>();
        }

        /// <summary>
        /// Gets or sets the restricted types.
        /// </summary>
        public List<RestrictedType> RestrictedTypes { get; set; }

        /// <summary>
        /// Gets or sets the restricted sites.
        /// </summary>
        public List<RestrictedSite> RestrictedSites { get; set; }

        /// <summary>
        /// Gets or sets the search engines.
        /// </summary>
        public List<SearchEngine> SearchEngines { get; set; }

        /// <summary>
        /// The save data.
        /// </summary>
        public void SaveData()
        {
            this.sitemapConfigData.RestrictedTypes = this.RestrictedTypes;
            this.sitemapConfigData.RestrictedSites = this.RestrictedSites;
            this.sitemapConfigData.SearchEngines = this.SearchEngines.Count == 0 ? null : this.SearchEngines;

            this.sitemapConfigManager.Service.SaveData(this.sitemapConfigData);
        }

        /// <summary>
        /// The load current page types.
        /// </summary>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        private List<RestrictedType> LoadCurrentPageTypes()
        {
            var restrictedTypes = new List<RestrictedType>();
            var previousRestrictedTypes = this.sitemapConfigData.RestrictedTypes;
            var repository = ServiceLocator.Current.GetInstance<IContentTypeRepository>();
            var baseType = typeof(PageData);
            var pageTypes = repository.List().Where(
                p => p.ModelType != null &&
                (p.ModelType.IsSubclassOf(baseType) || p.ModelType == baseType || baseType.IsInterface && baseType.IsAssignableFrom(p.ModelType)) &&
                p.IsAvailable).ToList();

            foreach (var pageType in pageTypes)
            {
                var newRestrictedType = new RestrictedType() { ContentType = pageType.Name, Restricted = false };

                if (previousRestrictedTypes != null)
                {
                    foreach (var previousRestricted in previousRestrictedTypes)
                    {
                        if (previousRestricted.ContentType != pageType.Name)
                        {
                            continue;
                        }

                        newRestrictedType.Restricted = previousRestricted.Restricted;
                        break;
                    }
                }

                restrictedTypes.Add(newRestrictedType);
            }

            return restrictedTypes;
        }

        /// <summary>
        /// The load current sites.
        /// </summary>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        private List<RestrictedSite> LoadCurrentSites()
        {
            var restrictedSites = new List<RestrictedSite>();
            var previousRestrictedSites = this.sitemapConfigData.RestrictedSites;

            var repository = ServiceLocator.Current.GetInstance<ISiteDefinitionRepository>();
            var sites = repository.List().ToList();

            foreach (var site in sites)
            {
                var newRestrictedSite = new RestrictedSite() { SiteName = site.Name, SiteMapFileName = site.Name + "Map.xml", Restricted = false };

                if (previousRestrictedSites != null)
                {
                    foreach (var previousRestricted in previousRestrictedSites)
                    {
                        if (previousRestricted.SiteName != site.Name)
                        {
                            continue;
                        }

                        newRestrictedSite.SiteMapFileName = previousRestricted.SiteMapFileName;
                        newRestrictedSite.Restricted = previousRestricted.Restricted;
                        break;
                    }
                }

                restrictedSites.Add(newRestrictedSite);
            }

            return restrictedSites;
        }
    }
}
