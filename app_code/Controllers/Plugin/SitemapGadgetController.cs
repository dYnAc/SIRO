
namespace SIRO.Controllers.Plugin
{
    using System.Web.Mvc;
    using Core.Models.DDS;
    using Core.Models.ViewModels;
    using Core.Sitemap;

    /// <summary>
    /// The sitemap gadget controller.
    /// </summary>
    [Authorize(Roles = "WebEditors, WebAdmins, Administrators")]
    public class SitemapGadgetController : Controller
    {
        /// <summary>
        /// The generator.
        /// </summary>
        private readonly ISitemapGenerator generator;

        /// <summary>
        /// Initializes a new instance of the <see cref="SitemapGadgetController"/> class.
        /// </summary>
        /// <param name="generator">
        /// The generator.
        /// </param>
        public SitemapGadgetController(ISitemapGenerator generator)
        {
            this.generator = generator;
        }

        /// <summary>
        /// The index.
        /// </summary>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        public ActionResult Index()
        {
            return this.View("~/Views/Plugins/SitemapGadget/Index.cshtml", new SitemapConfigurationViewModel());
        }

        /// <summary>
        /// The generate sitemap.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <returns>
        /// The <see cref="JsonResult"/>.
        /// </returns>
        public JsonResult GenerateSitemap(SitemapConfigurationViewModel model)
        {
            var data =
                new SitemapConfigurationDataStore()
                    {
                        RestrictedSites = model.RestrictedSites,
                        RestrictedTypes = model.RestrictedTypes,
                        SearchEngines = model.SearchEngines
                    };
            this.generator.RefreshSitemapTask(data);
            return this.Json("Sitemaps generated succesfully!!!", JsonRequestBehavior.AllowGet);
        }
    }
}