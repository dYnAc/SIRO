
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
    using Models.DDS;

    /// <summary>
    /// The sitemap manager.
    /// </summary>
    public interface ISitemapManager
    {
        /// <summary>
        /// The generate site maps.
        /// </summary>
        /// <param name="currentDataBase">
        /// The current data base.
        /// </param>
        void GenerateSiteMaps(SitemapConfigurationDataStore currentDataBase = null);

        /// <summary>
        /// The submit sitemap to search engines by http.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        bool SubmitSitemapToSearchEnginesByHttp();

        /// <summary>
        /// The register to robots file.
        /// </summary>
        void RegisterToRobotsFile();

        /// <summary>
        /// The register to robots fields.
        /// </summary>
        void RegisterToRobotsFields();
    }
}
