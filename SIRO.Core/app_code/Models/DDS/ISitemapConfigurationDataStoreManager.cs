
namespace SIRO.Core.Models.DDS
{
    using System.Collections.Generic;
    using EPiServer.Data;

    /// <summary>
    /// The sitemap configuration data store manager.
    /// </summary>
    public interface ISitemapConfigurationDataStoreManager
    {
        /// <summary>
        /// The get data by id.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="SitemapConfigurationDataStore"/>.
        /// </returns>
        SitemapConfigurationDataStore GetDataById(Identity id);

        /// <summary>
        /// The get all data.
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerable"/>.
        /// </returns>
        IEnumerable<SitemapConfigurationDataStore> GetAllData();

        /// <summary>
        /// The get first data.
        /// </summary>
        /// <returns>
        /// The <see cref="SitemapConfigurationDataStore"/>.
        /// </returns>
        SitemapConfigurationDataStore GetFirstData();

        /// <summary>
        /// The save data.
        /// </summary>
        /// <param name="data">
        /// The data.
        /// </param>
        /// <returns>
        /// The <see cref="Identity"/>.
        /// </returns>
        Identity SaveData(SitemapConfigurationDataStore data);

        /// <summary>
        /// The delete data.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        void DeleteData(Identity id);
    }
}