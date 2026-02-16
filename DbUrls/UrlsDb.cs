using Logging;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace DbUrls
{
    public class UrlsDb : DbCommon<UrlsContext>
    {
        private readonly ILogger logger;

        public UrlsDb(ILogger logger)
            : base(logger, "dburls")
        {
            this.logger = logger;
        }

        public async Task LearnNewUrls(IEnumerable<string> urls)
        {
            try
            {
                var newUrls = urls.Where(u => !Context.Urls.Any(d => d.Url == u));

                foreach (var url in newUrls)
                {
                    var entry = new DbUrlDto { Url = url };
                    Context.Urls.Add(entry);
                }

                await Context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                logger.Error($"{nameof(LearnNewUrls)} failed: ", ex);
                throw;
            }
        }

        public async Task UpdateLastQueued(string[] urls)
        {
            try
            {
                var now = DateTime.UtcNow;
                foreach (var url in urls)
                {
                    var entry = Context.Urls.SingleOrDefault(d => d.Url == url);
                    if (entry == null)
                    {
                        entry = new DbUrlDto { Url = url };
                        Context.Urls.Add(entry);
                    }
                    entry.LastQueuedUtc = now;
                }

                await Context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                logger.Error($"{nameof(UpdateLastQueued)} failed: ", ex);
                throw;
            }
        }

        public async Task<string[]> QueryUrlsBefore(DateTime utc)
        {
            try
            {
                return Context.Urls
                    .Where(u => u.LastQueuedUtc < utc)
                    .Select(u => u.Url)
                    .ToArray();
            }
            catch (Exception ex)
            {
                logger.Error($"{nameof(QueryUrlsBefore)} failed: ", ex);
                throw;
            }
        }
    }

    public class UrlsContext : BaseContext
    {
        public DbSet<DbUrlDto> Urls { get; set; }
    }

    [Index(nameof(Url), IsUnique = true)]
    public class DbUrlDto
    {
        [Key]
        public string Url { get; set; } = string.Empty;

        public DateTime LastQueuedUtc { get; set; } = DateTime.MinValue;
    }
}
