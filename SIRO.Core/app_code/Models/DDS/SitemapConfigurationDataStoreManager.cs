
namespace SIRO.Core.Models.DDS
{
    using System.Collections.Generic;
    using System.Linq;
    using EPiServer.Data;
    using EPiServer.Data.Dynamic;

    /// <summary>
    /// The sitemap configuration data store manager.
    /// </summary>
    public class SitemapConfigurationDataStoreManager : ISitemapConfigurationDataStoreManager
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
        public SitemapConfigurationDataStore GetDataById(Identity id)
        {
            using (var store = typeof(SitemapConfigurationDataStore).GetOrCreateStore())
            {
                return store.Load<SitemapConfigurationDataStore>(id);
            }
        }

        /// <summary>
        /// The get all data.
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerable"/>.
        /// </returns>
        public IEnumerable<SitemapConfigurationDataStore> GetAllData()
        {
            using (var store = typeof(SitemapConfigurationDataStore).GetOrCreateStore())
            {
                return store.LoadAll<SitemapConfigurationDataStore>();
            }
        }

        /// <summary>
        /// The get first data.
        /// </summary>
        /// <returns>
        /// The <see cref="SitemapConfigurationDataStore"/>.
        /// </returns>
        public SitemapConfigurationDataStore GetFirstData()
        {
            return this.GetAllData().FirstOrDefault();
        }

        /// <summary>
        /// The save data.
        /// </summary>
        /// <param name="data">
        /// The data.
        /// </param>
        /// <returns>
        /// The <see cref="Identity"/>.
        /// </returns>
        public Identity SaveData(SitemapConfigurationDataStore data)
        {
            using (var store = typeof(SitemapConfigurationDataStore).GetOrCreateStore())
            {
                return data.Id == null ? store.Save(data) : store.Save(data, data.Id);
            }
        }

        /// <summary>
        /// The delete data.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        public void DeleteData(Identity id)
        {
            using (var store = typeof(SitemapConfigurationDataStore).GetOrCreateStore())
            {
                store.Delete(id);
            }
        }
    }
}