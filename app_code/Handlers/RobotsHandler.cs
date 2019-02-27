
namespace SIRO.Handlers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using EPiServer;
    using EPiServer.Core;
    using EPiServer.ServiceLocation;
    using EPiServer.Web;

    /// <summary>
    /// The robots handler.
    /// </summary>
    public class RobotsHandler : IHttpHandler
    {
        /// <summary>
        /// The rep.
        /// </summary>
        private readonly IContentLoader rep;

        /// <summary>
        /// The site rep.
        /// </summary>
        private readonly ISiteDefinitionRepository siteRep;

        /// <summary>
        /// Initializes a new instance of the <see cref="RobotsHandler"/> class.
        /// </summary>
        public RobotsHandler()
        {
            this.rep = ServiceLocator.Current.GetInstance<IContentLoader>();
            this.siteRep = ServiceLocator.Current.GetInstance<ISiteDefinitionRepository>();
        }

        /// <summary>
        /// The is reusable.
        /// </summary>
        public bool IsReusable => false;

        /// <summary>
        /// The process request.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        public void ProcessRequest(HttpContext context)
        {
            var domainUri = context.Request.Url;
            var currentSite = this.siteRep.List().FirstOrDefault(x => verifyInHost(x.Hosts, domainUri.Authority) == true);

            var robotsTxtContent = @"User-agent: *"
                                   + Environment.NewLine +
                                   "Disallow: /episerver";

            if (currentSite != null)
            {
                var startPage = this.rep.Get<PageData>(currentSite.StartPage);

                var robotsFromField = string.Empty;
                var robotsProperty = startPage.GetType().GetProperty("RobotsContent");
                if (robotsProperty?.GetValue(startPage) is string)
                {
                    robotsFromField = robotsProperty.GetValue(startPage) as string;
                }

                // Generate robots.txt file
                if (!string.IsNullOrEmpty(robotsFromField))
                {
                    robotsTxtContent += Environment.NewLine + robotsFromField;
                }
            }

            // Set the response code, content type and appropriate robots file here
            // also think about handling caching, sending error codes etc.
            context.Response.StatusCode = 200;
            context.Response.ContentType = "text/plain";

            // Return the robots content
            context.Response.Write(robotsTxtContent);
            context.Response.End();
        }

        private bool verifyInHost(IList<HostDefinition> hosts, string siteName)
        {
            foreach (var host in hosts)
            {
                if (host.Authority.Hostname == siteName)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
