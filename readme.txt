---------------------------------------------------------------------
------ Sitemap and Robots dynamic generator - SIRO Readme -----------
---------------------------------------------------------------------
 
!!!!!!!! IMPORTANT  !!!!!!!!
Every inherit field must have their respectiva Display and/or 
UIHint (Optional) attributes in order to work correctly on EPiServer.
For instance:

-- Common Page -- 

[Display(GroupName = "Navigation",
		Name = "Exclude from Xml Site Map",
		Order = 400)]
bool ExcludeFromSiteMap { get; set; }
       
[Display(GroupName = "Navigation",
    Name = "Priority in Site Map",
    Order = 500)]
string Priority { get; set; }
     
[Display(GroupName = "Navigation",
    Name = "Change Frequency of Page in Site Map",
    Order = 600)]
string ChangeFrequency { get; set; }

-- Start Page --

[Display(Name = "Robots", Order = 1200, GroupName = "Metadata")]
[UIHint(UIHint.Textarea)]
string RobotsContent { get; set; }

This fields allow the editor to decide if a page can be individually included
in the sitemap even when the page type is not restricted.

-- FEATURES Sitemap--

* Multi site 
* Multi language (https://support.google.com/webmasters/answer/2620865?hl=en)
* Filter by page (Atribute ExcludeFromSiteMap)
* Sort by priority and frequency (Attributes Priority nad ChangeFrequency)
* Filter by page type
* Filter by site
* Notification to search engines in production environment
* Automatic update of robots.txt file
* Small tweaks configurations inside web.config and Sitemap.config files such as:
  ** Limit the number of entries per file (Default 50000) - web.config, it gnerates
     and index file and several sitemap files if the limit is reached
  ** Output to robots.txt file (Default true:1) - web.config
  ** Sitemap file required namespaces - Sitemap.config
  ** Production environment (Default false) - Sitemap.config
* Admin plugin which allows to configure sites, page types and search engines
* Adnin job to generate sitemaps using the admin sitemap plugin
* Web editor gadget which generates sitemaps with restricted sites and page types
* Http handler to intercept calls to sitemap.xml file per domain/site

-- FEATURES Robots --

* Multi site
* Customizable string field to edit robots.txt content inside Start page
* Http handler to intercept calls to robots.txt file per domain/site

*******************************************************
*******************  VERNDALE  ************************
*******************************************************
