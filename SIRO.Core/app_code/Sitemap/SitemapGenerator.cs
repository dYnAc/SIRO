
/* *********************************************************************** *
 * File   : SitemapGenerator.cs                           Part of Sitecore *
 * Version: 1.0.0                                         www.sitecore.net *
 *                                                                         *
 *                                                                         *
 * Purpose: Contains logic which fires when event submitted                *
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
    using System.Configuration;
    using Helpers;
    using Models.DDS;

    /// <summary>
    /// The sitemap handler.
    /// </summary>
    public class SitemapGenerator : ISitemapGenerator
    {
        /// <summary>
        /// The sitemap manager.
        /// </summary>
        private readonly ISitemapManager sitemapManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="SitemapGenerator"/> class.
        /// </summary>
        /// <param name="sitemapManager">
        /// The sitemap manager.
        /// </param>
        public SitemapGenerator(ISitemapManager sitemapManager)
        {
            this.sitemapManager = sitemapManager;
        }

        /// <summary>
        /// The refresh sitemap task.
        /// </summary>
        /// <param name="dataBase">
        /// The data base.
        /// </param>
        public void RefreshSitemapTask(SitemapConfigurationDataStore dataBase = null)
        {
            LogHelper.Information("START: SitemapHandler.RefreshSitemap", typeof(SitemapGenerator));

            this.sitemapManager.GenerateSiteMaps(dataBase);
            this.sitemapManager.SubmitSitemapToSearchEnginesByHttp();

            var outPutRobots = ConfigurationManager.AppSettings["SiteMap_OutPutRobots_txt"].Trim();
            if (outPutRobots == "1")
            {
                this.sitemapManager.RegisterToRobotsFields();
            }

            LogHelper.Information("END: SitemapHandler.RefreshSitemap", typeof(SitemapGenerator));
        }
    }
}
