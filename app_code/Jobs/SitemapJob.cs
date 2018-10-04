
namespace SIRO.Jobs
{
    using Core.Sitemap;
    using EPiServer.PlugIn;
    using EPiServer.Scheduler;

    /// <summary>
    /// The sitemap job.
    /// </summary>
    [ScheduledPlugIn(DisplayName = "Sitemap Generation", Description = "Generate sitemap files for every defined site and update robots.txt file")]
    public class SitemapJob : ScheduledJobBase
    {
        /// <summary>
        /// The sitemap generator.
        /// </summary>
        private readonly ISitemapGenerator sitemapGenerator;

        /// <summary>
        /// The stop signaled.
        /// </summary>
        private bool stopSignaled;

        /// <summary>
        /// Initializes a new instance of the <see cref="SitemapJob"/> class.
        /// </summary>
        /// <param name="sitemapGenerator">
        /// The sitemap generator.
        /// </param>
        public SitemapJob(ISitemapGenerator sitemapGenerator)
        {
            this.sitemapGenerator = sitemapGenerator;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SitemapJob"/> class.
        /// </summary>
        public SitemapJob()
        {
            this.IsStoppable = true;
        }

        /// <summary>
        /// Called when a user clicks on Stop for a manually started job, or when ASP.NET shuts down.
        /// </summary>
        public override void Stop()
        {
            this.stopSignaled = true;
        }

        /// <summary>
        /// Called when a scheduled job executes
        /// </summary>
        /// <returns>A status message to be stored in the database log and visible from admin mode</returns>
        public override string Execute()
        {
            // Call OnStatusChanged to periodically notify progress of job for manually started jobs
            this.OnStatusChanged($"Starting execution of {this.GetType()}");

            // Add implementation
            this.sitemapGenerator.RefreshSitemapTask();

            // For long running jobs periodically check if stop is signaled and if so stop execution
            return this.stopSignaled ? "Stop of job was called" : "Change to message that describes outcome of execution";
        }
    }
}