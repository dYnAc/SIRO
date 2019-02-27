
/* *********************************************************************** *
 * File   : SitemapManagerConfiguration.cs                Part of Sitecore *
 * Version: 1.0.0                                         www.sitecore.net *
 *                                                                         *
 *                                                                         *
 * Purpose: Class for getting config information from db and conf file     *
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
    using System.Collections.Specialized;
    using System.Xml;
    using EPiServer.Web;
    using Models.DDS;

    /// <summary>
    /// The sitemap manager configuration.
    /// </summary>
    public class SitemapManagerConfiguration
    {
        #region properties

        /// <summary>
        /// The xml ns tpl.
        /// </summary>
        public static string XmlNsTpl => GetValueByName("xmlNsTpl");

        /// <summary>
        /// The xml ns lang tpl.
        /// </summary>
        public static string XmlNsLangTpl => GetValueByName("xmlNsLangTpl");

        /// <summary>
        /// Gets a value indicating whether is production environment.
        /// </summary>
        public static bool IsProductionEnvironment
        {
            get
            {
                var production = GetValueByName("productionEnvironment");
                return !string.IsNullOrEmpty(production) && (production.ToLower() == "true" || production == "1");
            }
        }
        #endregion properties

        /// <summary>
        /// The get sites.
        /// </summary>
        /// <param name="dataBase">
        /// The data base.
        /// </param>
        /// <returns>
        /// The <see cref="StringDictionary"/>.
        /// </returns>
        public static StringDictionary GetSites(SitemapConfigurationDataStore dataBase)
        {
            var sites = new StringDictionary();

            if (dataBase.RestrictedSites != null)
            {
                foreach (var site in dataBase.RestrictedSites)
                {
                    if (!site.Restricted && !string.IsNullOrEmpty(site.SiteName)
                        && !string.IsNullOrEmpty(site.SiteMapFileName))
                    {
                        sites.Add(site.SiteName, site.SiteMapFileName);
                    }
                }
            }

            return sites;
        }

        /// <summary>
        /// The get server url by site.
        /// </summary>
        /// <param name="site">
        /// The site.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string GetServerUrlBySite(SiteDefinition site)
        {
            return site.SiteUrl.AbsoluteUri;
        }

        /// <summary>
        /// The get value by name.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private static string GetValueByName(string name)
        {
            var result = string.Empty;

            foreach (XmlElement node in SitemapFactory.GetConfigNodes("sitemapVariables/sitemapVariable"))
            {
                if (node.GetAttribute("name") == name)
                {
                    result = node.GetAttribute("value");
                    break;
                }
            }

            return result;
        }
    }
}
