
namespace SIRO.Gadgets
{
    using EPiServer.Shell.ViewComposition;

    /// <summary>
    /// The sitemap gadget.
    /// </summary>
    [Component(
        Title = "Sitemap Gadget",
        Categories = "dashboard",
        WidgetType = "nucleus/components/SitemapGadget",
        Description = "Sitemap generator gadget",
        PlugInAreas = EPiServer.Shell.ShellPlugInArea.DashboardDefaultTab)]
    public class SitemapGadget
    {
    }
}