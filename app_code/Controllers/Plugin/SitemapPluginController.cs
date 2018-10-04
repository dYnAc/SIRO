
namespace SIRO.Controllers.Plugin
{
    using System;
    using System.Web.Mvc;
    using Core.Models.Properties;
    using Core.Models.ViewModels;

    /// <summary>
    /// The sitemap plugin controller.
    /// </summary>
    [Authorize(Roles = "CmsAdmins")]
    [EPiServer.PlugIn.GuiPlugIn(
        Area = EPiServer.PlugIn.PlugInArea.AdminConfigMenu,
        Url = "/SitemapPlugin/Index",
        DisplayName = "Sitemap Administration Plugin")]
    public class SitemapPluginController : Controller
    {
        /// <summary>
        /// The index.
        /// </summary>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        public ActionResult Index()
        {
            return this.View("~/Views/Plugins/SitemapPlugin/Index.cshtml", new SitemapConfigurationViewModel());
        }

        /// <summary>
        /// The update restricted types.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        [HttpPost]
        public ActionResult UpdateRestrictedTypesAndSites(SitemapConfigurationViewModel model)
        {
            model.SaveData();
            this.ViewBag.UserMessage = "Restricted types and sites updated successfully";
            return this.View("~/Views/Plugins/SitemapPlugin/Index.cshtml", model);
        }

        /// <summary>
        /// The list search engines.
        /// </summary>
        /// <returns>
        /// The <see cref="JsonResult"/>.
        /// </returns>
        public JsonResult ListSearchEngines()
        {
            var model = new SitemapConfigurationViewModel();
            return this.Json(model.SearchEngines, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// The add search engine.
        /// </summary>
        /// <param name="engine">
        /// The engine.
        /// </param>
        /// <returns>
        /// The <see cref="JsonResult"/>.
        /// </returns>
        public JsonResult AddSearchEngine(SearchEngine engine)
        {
            var model = new SitemapConfigurationViewModel();
            engine.Code = Guid.NewGuid().ToString();
     
            model.SearchEngines.Add(engine);
            model.SaveData();

            return this.Json("OK", JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// The get search engine by id.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="JsonResult"/>.
        /// </returns>
        public JsonResult GetSearchEngineById(string id)
        {
            var model = new SitemapConfigurationViewModel();
            var engine = model.SearchEngines.Find(x => x.Code.Equals(id));

            return this.Json(engine, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// The update search engine.
        /// </summary>
        /// <param name="engine">
        /// The engine.
        /// </param>
        /// <returns>
        /// The <see cref="JsonResult"/>.
        /// </returns>
        public JsonResult UpdateSearchEngine(SearchEngine engine)
        {
            var model = new SitemapConfigurationViewModel();
            var index = model.SearchEngines.FindIndex(x => x.Code.Equals(engine.Code));
            model.SearchEngines[index] = engine;
            model.SaveData();

            return this.Json("OK", JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// The delete search engine.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="JsonResult"/>.
        /// </returns>
        public JsonResult DeleteSearchEngine(string id)
        {
            var model = new SitemapConfigurationViewModel();
            var engine = model.SearchEngines.Find(x => x.Code.Equals(id));
            model.SearchEngines.Remove(engine);
            model.SaveData();

            return this.Json("OK", JsonRequestBehavior.AllowGet);
        }
    }
}