
namespace SIRO.Core.Models.Properties
{
    /// <summary>
    /// The restricted site.
    /// </summary>
    public class RestrictedSite
    {
        /// <summary>
        /// Gets or sets the content type.
        /// </summary>
        public string SiteName { get; set; }

        /// <summary>
        /// Gets or sets the site map file name.
        /// </summary>
        public string SiteMapFileName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether restricted.
        /// </summary>
        public bool Restricted { get; set; }
    }
}