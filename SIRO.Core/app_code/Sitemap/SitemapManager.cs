
/* *********************************************************************** *
 * File   : SitemapManager.cs                             Part of Sitecore *
 * Version: 1.0.0                                         www.sitecore.net *
 *                                                                         *
 *                                                                         *
 * Purpose: Manager class what contains all main logic                     *
 *                                                                         *
 * Copyright (C) 1999-2009 by Sitecore A/S. All rights reserved.           *
 *                                                                         *
 * This work is the property of:                                           *
 *                                                                         *
 *        Sitecore A/S                                                     *
 *        Meldahlsgade 5, 4.                                               *
 *        1613 Copenhagen V.                                               *
 *        Denmark                                                          *
 *                                                                         *
 * This is a Sitecore published work under Sitecore's                      *
 * shared source license.                                                  *
 *                                                                         *
 * *********************************************************************** */

namespace SIRO.Core.Sitemap
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Configuration;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Web;
    using System.Xml;
    using EPiServer;
    using EPiServer.Core;
    using EPiServer.DataAbstraction;
    using EPiServer.Filters;
    using EPiServer.ServiceLocation;
    using EPiServer.Web;
    using EPiServer.Web.Routing;
    using Helpers;
    using Models.DDS;
    using SIRO.Core.app_code.Helpers;

    /// <summary>
    /// The sitemap manager.
    /// </summary>
    public class SitemapManager : ISitemapManager
    {
        /// <summary>
        /// The sitemap sub folder.
        /// </summary>
        private const string SitemapSubFolder = "sitemaps";

        /// <summary>
        /// The sitemap manager configurator.
        /// </summary>
        private readonly ISitemapConfigurationDataStoreManager sitemapManagerConfigurator;

        /// <summary>
        /// The m sites.
        /// </summary>
        private StringDictionary sites;

        /// <summary>
        /// The max urls.
        /// </summary>
        private int maxUrls;

        /// <summary>
        /// The data from base.
        /// </summary>
        private SitemapConfigurationDataStore dataFromBase;

        /// <summary>
        /// The default language.
        /// </summary>
        private LanguageBranch defaultLanguage;

        /// <summary>
        /// Initializes a new instance of the <see cref="SitemapManager"/> class.
        /// </summary>
        /// <param name="sitemapManagerConfigurator">
        /// The sitemap manager configurator.
        /// </param>
        public SitemapManager(ISitemapConfigurationDataStoreManager sitemapManagerConfigurator)
        {
            this.sitemapManagerConfigurator = sitemapManagerConfigurator;
        }

        /// <summary>
        /// The generate site maps.
        /// </summary>
        /// <param name="currentDataBase">
        /// The current data base.
        /// </param>
        public void GenerateSiteMaps(SitemapConfigurationDataStore currentDataBase = null)
        {
            LogHelper.Information("START: SitemapManager.SitemapManager", typeof(SitemapGenerator));

            this.dataFromBase = currentDataBase ?? this.sitemapManagerConfigurator.GetFirstData() ?? new SitemapConfigurationDataStore();

            this.sites = SitemapManagerConfiguration.GetSites(this.dataFromBase);
            this.maxUrls = int.Parse(ConfigurationManager.AppSettings["SiteMap_MAX_URLS"]);

            var languageRep = ServiceLocator.Current.GetInstance<ILanguageBranchRepository>();
            this.defaultLanguage = languageRep.LoadFirstEnabledBranch();

            LogHelper.Information("SitemapManager.SitemapManager: # Sites: " + this.sites.Count, typeof(SitemapGenerator));
            foreach (DictionaryEntry site in this.sites)
            {
                this.BuildSiteMap(site.Key.ToString(), site.Value.ToString());
            }

            LogHelper.Information("END: SitemapManager.SitemapManager", typeof(SitemapGenerator));
        }

        /// <summary>
        /// The submit sitemap to search engines by http.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool SubmitSitemapToSearchEnginesByHttp()
        {
            if (!SitemapManagerConfiguration.IsProductionEnvironment)
            {
                return false;
            }

            if (this.dataFromBase == null)
            {
                return false;
            }

            var engines = this.dataFromBase.SearchEngines;
            foreach (var engine in engines)
            {
                if (engine == null)
                {
                    continue;
                }

                var engineHttpRequestString = engine.Url;
                foreach (string sitemapUrl in this.sites)
                {
                    this.SubmitEngine(engineHttpRequestString, sitemapUrl);
                }
            }

            return true;
        }

        /// <summary>
        /// The register to robots file.
        /// </summary>
        public void RegisterToRobotsFile()
        {
            var robotsPath = System.Web.Hosting.HostingEnvironment.MapPath(string.Concat("/", "robots.txt"));
            var sitemapContent = new StringBuilder(string.Empty);

            if (robotsPath == null)
            {
                return;
            }

            if (File.Exists(robotsPath))
            {
                StreamReader sr = new StreamReader(robotsPath);
                sitemapContent.Append(sr.ReadToEnd());
                sr.Close();
            }

            var sw = new StreamWriter(robotsPath, false);
            foreach (DictionaryEntry site in this.sites)
            {
                var siteName = site.Key.ToString();
                var filename = site.Value.ToString();
                var defRep = ServiceLocator.Current.GetInstance<ISiteDefinitionRepository>();
                var siteDef = defRep.Get(siteName);
                var serverUrl = SitemapManagerConfiguration.GetServerUrlBySite(siteDef);
                var sitemapLine = string.Concat("Sitemap: ", serverUrl, SitemapSubFolder, "/", filename);

                if (!sitemapContent.ToString().Contains(sitemapLine))
                {
                    sitemapContent.AppendLine(sitemapLine);
                }
            }

            sw.Write(sitemapContent.ToString());
            sw.Close();
        }

        /// <summary>
        /// The register to robots fields.
        /// </summary>
        public void RegisterToRobotsFields()
        {
            foreach (DictionaryEntry site in this.sites)
            {
                var siteName = site.Key.ToString();
                var filename = site.Value.ToString();
                var defRep = ServiceLocator.Current.GetInstance<ISiteDefinitionRepository>();
                var siteDef = defRep.Get(siteName);
                var serverUrl = SitemapManagerConfiguration.GetServerUrlBySite(siteDef);
                var sitemapLine = string.Concat("Sitemap: ", serverUrl, SitemapSubFolder, "/", filename);

                var rep = ServiceLocator.Current.GetInstance<IContentRepository>();
                var startPage = rep.Get<PageData>(siteDef.StartPage);

                // Save robots content from field
                if (startPage == null)
                {
                    continue;
                }

                var robotsFromField = string.Empty;
                var robotsFromFieldProperty = startPage.GetType().GetProperty("RobotsContent");
                if (robotsFromFieldProperty?.GetValue(startPage) is string)
                {
                    robotsFromField = robotsFromFieldProperty.GetValue(startPage) as string;
                }

                if (string.IsNullOrEmpty(robotsFromField) || !robotsFromField.Contains(sitemapLine))
                {
                    robotsFromField = sitemapLine + Environment.NewLine;
                    var startPageWritable = ((PageData)startPage).CreateWritableClone();

                    if (startPageWritable != null)
                    {
                        var robotsFromFieldPropertyWritable = startPageWritable.GetType().GetProperty("RobotsContent");
                        robotsFromFieldPropertyWritable?.SetValue(startPageWritable, robotsFromField);
                        rep.Save(
                            (PageData)startPageWritable,
                            EPiServer.DataAccess.SaveAction.Publish,
                            EPiServer.Security.AccessLevel.NoAccess);
                    }
                }
            }
        }

        /// <summary>
        /// The build site map.
        /// </summary>
        /// <param name="siteName">
        /// The site name.
        /// </param>
        /// <param name="sitemapUrlNew">
        /// The sitemap url new.
        /// </param>
        private void BuildSiteMap(string siteName, string sitemapUrlNew)
        {
            var siteDefinitionRepository = ServiceLocator.Current.GetInstance<ISiteDefinitionRepository>();
            var site = siteDefinitionRepository.Get(siteName);

            // Pass in all the enabled languages of the site, so it can limit the sitemap items those of the languages
            var items = this.GetSitemapItems(site, this.defaultLanguage);

            // Clear out old site maps
            var sitemapFolder = System.Web.Hosting.HostingEnvironment.MapPath(string.Concat("/", SitemapSubFolder));

            if (sitemapFolder != null)
            {
                if (Directory.Exists(sitemapFolder))
                {
                    foreach (var file in Directory.GetFiles(sitemapFolder))
                    {
                        if (file.Contains(sitemapUrlNew))
                        {
                            File.Delete(file);
                        }
                    }
                }
                else
                {
                    Directory.CreateDirectory(sitemapFolder);
                }
            }

            // if items.count > 50,000, slice it up into multiple files
            if (items.Count > this.maxUrls)
            {
                var fileNames = new List<string>();
                for (var x = 0; x < items.Count; x += this.maxUrls)
                {
                    // NOTE: assumes sitemap URL is at root of site!!!!!!!!!!!!!!
                    fileNames.Add("\\" + SitemapSubFolder + "\\" + x + sitemapUrlNew);
                }

                var fullPathIndex = System.Web.Hosting.HostingEnvironment.MapPath(string.Concat("/", sitemapUrlNew)) ?? "sitemapGeneric.xml";
                var xmlContentIndex = this.BuildSitemapIndex(fileNames, site);

                var strWriterIndex = new StreamWriter(fullPathIndex, false);
                strWriterIndex.Write(xmlContentIndex);
                strWriterIndex.Close();

                // Create each sitemap file
                while (items.Count > 0)
                {
                    var curItems = items.Take(this.maxUrls).ToList();
                    items.RemoveRange(0, curItems.Count());

                    var fullPath = System.Web.Hosting.HostingEnvironment.MapPath(string.Concat("/", fileNames[0])) ?? "sitemapGenericIndexed.xml";
                    fileNames.RemoveAt(0);
                    var xmlContent = this.BuildSitemapXml(curItems, site);

                    var strWriter = new StreamWriter(fullPath, false);
                    strWriter.Write(xmlContent);
                    strWriter.Close();
                }
            }
            else
            {
                var fullPath = System.Web.Hosting.HostingEnvironment.MapPath(string.Concat("/", SitemapSubFolder, "/", sitemapUrlNew)) ?? "sitemapGeneric.xml";
                var xmlContent = this.BuildSitemapXml(items, site);

                var strWriter = new StreamWriter(fullPath, false);
                strWriter.Write(xmlContent);
                strWriter.Close();
            }
        }

        /// <summary>
        /// The build sitemap index.
        /// </summary>
        /// <param name="fileNames">
        /// The file names.
        /// </param>
        /// <param name="site">
        /// The site.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string BuildSitemapIndex(IEnumerable<string> fileNames, SiteDefinition site)
        {
            var serverUrl = SitemapManagerConfiguration.GetServerUrlBySite(site);

            var doc = new XmlDocument();
            var declarationNode = doc.CreateXmlDeclaration("1.0", "UTF-8", null);
            doc.AppendChild(declarationNode);

            var siteMapIndexNode = doc.CreateElement("sitemapindex");
            var xmlNsAttr = doc.CreateAttribute("xmlns");
            xmlNsAttr.Value = SitemapManagerConfiguration.XmlNsTpl;
            var xmlNsLangAttr = doc.CreateAttribute("xmlns:xhtml");
            xmlNsLangAttr.Value = SitemapManagerConfiguration.XmlNsLangTpl;
            siteMapIndexNode.Attributes.Append(xmlNsAttr);
            siteMapIndexNode.Attributes.Append(xmlNsLangAttr);
            doc.AppendChild(siteMapIndexNode);

            foreach (var filename in fileNames)
            {
                var url = $"{serverUrl}/{ HttpUtility.HtmlEncode(filename.Replace("\\", "/"))}";
                var lastMod = HttpUtility.HtmlEncode(DateTime.Now.ToString("yyyy-MM-ddTHH:mm:sszzz"));

                var sitemapNode = doc.CreateElement("sitemap");
                siteMapIndexNode.AppendChild(sitemapNode);

                var locNode = doc.CreateElement("loc");
                sitemapNode.AppendChild(locNode);
                locNode.AppendChild(doc.CreateTextNode(url));

                var lastModNode = doc.CreateElement("lastmod");
                sitemapNode.AppendChild(lastModNode);
                lastModNode.AppendChild(doc.CreateTextNode(lastMod));
            }

            return doc.OuterXml;
        }

        /// <summary>
        /// The build sitemap xml.
        /// </summary>
        /// <param name="items">
        /// The items.
        /// </param>
        /// <param name="site">
        /// The site.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string BuildSitemapXml(List<PageData> items, SiteDefinition site)
        {
            var doc = new XmlDocument();
            var declarationNode = doc.CreateXmlDeclaration("1.0", "UTF-8", null);
            doc.AppendChild(declarationNode);
            var urlSetNode = doc.CreateElement("urlset");
            var xmlNsAttr = doc.CreateAttribute("xmlns");
            xmlNsAttr.Value = SitemapManagerConfiguration.XmlNsTpl;
            var xmlNsLangAttr = doc.CreateAttribute("xmlns:xhtml");
            xmlNsLangAttr.Value = SitemapManagerConfiguration.XmlNsLangTpl;
            urlSetNode.Attributes.Append(xmlNsAttr);
            urlSetNode.Attributes.Append(xmlNsLangAttr);

            doc.AppendChild(urlSetNode);

            foreach (var itm in items)
            {
                var seoProperty = itm.GetType().GetProperty("ExcludeFromSiteMap");
                if (seoProperty?.GetValue(itm) is bool)
                {
                    var isExcluded = seoProperty.GetValue(itm) as bool?;
                    if (isExcluded != null && isExcluded.Value)
                    {
                        continue;
                    }
                }

                doc = this.BuildSitemapItem(doc, itm, site);
            }

            return doc.OuterXml;
        }

        /// <summary>
        /// The build sitemap item.
        /// </summary>
        /// <param name="doc">
        /// The doc.
        /// </param>
        /// <param name="item">
        /// The item.
        /// </param>
        /// <param name="site">
        /// The site.
        /// </param>
        /// <returns>
        /// The <see cref="XmlDocument"/>.
        /// </returns>
        private XmlDocument BuildSitemapItem(XmlDocument doc, PageData item, SiteDefinition site)
        {
            var url = HttpUtility.HtmlEncode(this.GetItemUrl(item, this.defaultLanguage.Culture, site));

            var lastMod = HttpUtility.HtmlEncode(item.Saved.ToString("yyyy-MM-ddTHH:mm:sszzz"));

            var urlSetNode = doc.LastChild;

            var urlNode = doc.CreateElement("url");
            urlSetNode.AppendChild(urlNode);

            var locNode = doc.CreateElement("loc");
            urlNode.AppendChild(locNode);
            locNode.AppendChild(doc.CreateTextNode(url));

            // Add different languages to the sitemap as specified in https://support.google.com/webmasters/answer/2620865?hl=en
            var existingLanguages = item.ExistingLanguages;
            var rep = ServiceLocator.Current.GetInstance<IContentLoader>();

            foreach (var lang in existingLanguages)
            {
                var culturedPage = rep.Get<PageData>(item.ContentLink, lang);
                var culturedUrl = HttpUtility.HtmlEncode(this.GetItemUrl(culturedPage.ContentLink, lang, site));

                var languageNode = doc.CreateElement("xhtml", "link", SitemapManagerConfiguration.XmlNsLangTpl);
                languageNode.SetAttribute("rel", "alternate");
                languageNode.SetAttribute("hreflang", lang.Name);
                languageNode.SetAttribute("href", culturedUrl);
                urlNode.AppendChild(languageNode);
            }

            var seoProperty = item.GetType().GetProperty("Priority");
            if (seoProperty?.GetValue(item) is string)
            {
                var priority = seoProperty.GetValue(item) as string;
                if (!string.IsNullOrEmpty(priority))
                {
                    // possibly validate it's between 0.0 or 1.0?  or leave up to content author?
                    double priorityNumber;
                    if (double.TryParse(priority, out priorityNumber))
                    {
                        if (priorityNumber >= 0.0 && priorityNumber <= 1.0)
                        {
                            XmlNode priorityNode = doc.CreateElement("priority");
                            urlNode.AppendChild(priorityNode);
                            priorityNode.AppendChild(doc.CreateTextNode(priority));
                        }
                    }
                }
            }

            seoProperty = item.GetType().GetProperty("ChangeFrequency");
            if (seoProperty?.GetValue(item) is string)
            {
                var frecuency = seoProperty.GetValue(item) as string;
                if (!string.IsNullOrEmpty(frecuency))
                {
                    XmlNode chgNode = doc.CreateElement("changefreq");
                    urlNode.AppendChild(chgNode);
                    chgNode.AppendChild(doc.CreateTextNode(frecuency));
                }
            }

            XmlNode lastModNode = doc.CreateElement("lastmod");
            urlNode.AppendChild(lastModNode);
            lastModNode.AppendChild(doc.CreateTextNode(lastMod));

            return doc;
        }

        /// <summary>
        /// The get item url.
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        /// <param name="lang">
        /// The lang.
        /// </param>
        /// <param name="site">
        /// The site.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string GetItemUrl(PageData item, CultureInfo lang, SiteDefinition site)
        {
            return this.GetItemUrl(item.ContentLink, lang, site);
        }

        /// <summary>
        /// The get item url.
        /// </summary>
        /// <param name="contentLink">
        /// The content link.
        /// </param>
        /// <param name="lang">
        /// The lang.
        /// </param>
        /// <param name="site">
        /// The site.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string GetItemUrl(ContentReference contentLink, CultureInfo lang, SiteDefinition site)
        {
            // Get page url based on language
            var urlResolver = ServiceLocator.Current.GetInstance<UrlResolver>();
            var pageUrl = urlResolver.GetVirtualPath(contentLink, lang.Name);

            // Add last host name to the page url
            var uriBuilder = new UriBuilder(site.SiteUrl) { Path = pageUrl.VirtualPath };
            var url = uriBuilder.Uri.AbsoluteUri;

            // Will definitely make sure postfix is tripped out
            var fullUrl = url.WithoutPostfix("/");

            return fullUrl;
        }

        /// <summary>
        /// The submit engine.
        /// </summary>
        /// <param name="engine">
        /// The engine.
        /// </param>
        /// <param name="sitemapUrl">
        /// The sitemap url.
        /// </param>
        private void SubmitEngine(string engine, string sitemapUrl)
        {
            // Check if it is not localhost because search engines returns an error
            if (!sitemapUrl.Contains("http://localhost"))
            {
                var request = string.Concat(engine, HttpUtility.HtmlEncode(sitemapUrl));
                var httpRequest = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(request);
                try
                {
                    var webResponse = httpRequest.GetResponse();

                    System.Net.HttpWebResponse httpResponse = (System.Net.HttpWebResponse)webResponse;
                    if (httpResponse.StatusCode != System.Net.HttpStatusCode.OK)
                    {
                        LogHelper.Error($"Cannot submit sitemap to \"{engine}\"", this);
                    }
                }
                catch
                {
                    LogHelper.Warning($"The search engine \"{request}\" returns an 404 error", this);
                }
            }
        }

        /// <summary>
        /// The get sitemap items.
        /// </summary>
        /// <param name="root">
        /// The root.
        /// </param>
        /// <param name="language">
        /// The language.
        /// </param>
        /// <returns>
        /// The <see cref="IList"/>.
        /// </returns>
        private List<PageData> GetSitemapItems(SiteDefinition root, LanguageBranch language)
        {
            var thisLanguage = language.Culture.DisplayName;

            var repository = ServiceLocator.Current.GetInstance<IContentLoader>();
            var contentRoot = repository.Get<PageData>(root.StartPage);

            // Get all published pages which are visible and have not been excluded from the site map
            var descendants = new List<PageData>();
            PageDataHelper.GetChildrenRecursive(contentRoot.ContentLink, descendants);
            descendants = FilterForVisitor.Filter(descendants).OfType<PageData>().Where(x => x.Status == VersionStatus.Published &&
                            x.IsVisibleOnSite()).ToList();

            // TODO: Change current user to search for pages as in sitecore.
            var sitemapItems = descendants;

            // Limit the sitemap items to only those descendants that have a version in default language set in the site node configuration
            sitemapItems = sitemapItems.Where(x => x.ExistingLanguages.Any(l => l.DisplayName == thisLanguage)).ToList();

            // Add root
            sitemapItems.Insert(0, contentRoot);

            // Filter types which are not allowed
            var selected = from itm in sitemapItems
                           where this.IsRestricted(itm) == false
                           select itm;

            return selected.ToList();
        }

        /// <summary>
        /// The is restricted.
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private bool IsRestricted(PageData item)
        {
            var strContentType = item.GetType().FullName;
            strContentType = strContentType.Replace("Proxy", string.Empty);
            strContentType = strContentType.Replace("Castle.Proxies.", string.Empty);

            var restrictedContentTypes = this.dataFromBase.RestrictedTypes;

            var restrictedType = restrictedContentTypes?.FirstOrDefault(x => x.ContentType == strContentType);

            return restrictedType != null && restrictedType.Restricted;
        }
    }
}
