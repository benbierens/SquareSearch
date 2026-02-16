using Logging;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace DbUrls
{
    public class UrlsDb
    {
        private readonly ILogger logger;
        private UrlsContext context = null!;

        public UrlsDb(ILogger logger)
        {
            this.logger = logger;
        }

        public async Task Initialize()
        {
            try
            {
                var host = GetEnv("DBHOST");
                context = new UrlsContext(
                    host: host,
                    login: GetEnv("DBLOGIN"),
                    password: GetEnv("DBPASSWORD"),
                    database: "dburls"
                );
                await context.Database.EnsureCreatedAsync();
                logger.Info($"Initialized database at host {host}");
            }
            catch (Exception ex)
            {
                logger.Error("Failed to initialize db", ex);
                throw;
            }
        }

        public async Task LearnNewUrls(IEnumerable<string> urls)
        {
            try
            {
                var newUrls = urls.Where(u => !context.Urls.Any(d => d.Url == u));

                foreach (var url in newUrls)
                {
                    var entry = new DbUrlDto { Url = url };
                    context.Urls.Add(entry);
                }

                await context.SaveChangesAsync();
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
                    var entry = context.Urls.SingleOrDefault(d => d.Url == url);
                    if (entry == null)
                    {
                        entry = new DbUrlDto { Url = url };
                        context.Urls.Add(entry);
                    }
                    entry.LastQueuedUtc = now;
                }

                await context.SaveChangesAsync();
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
                return context.Urls
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

        private string GetEnv(string v)
        {
            var value = Environment.GetEnvironmentVariable(v);
            if (string.IsNullOrEmpty(value)) throw new Exception("Missing envvar: " + v);
            return value;
        }
    }

    public class UrlsContext : DbContext
    {
        private readonly string host;
        private readonly string login;
        private readonly string password;
        private readonly string database;

        public UrlsContext(string host, string login, string password, string database)
        {
            this.host = host;
            this.login = login;
            this.password = password;
            this.database = database;
        }

        public DbSet<DbUrlDto> Urls { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseNpgsql($"Host={host};Username={login};Password={password};Database={database}");

        //protected override void OnModelCreating(ModelBuilder builder)
        //{
        //    builder.Entity<DbUrlDto>()
        //        .HasIndex(u => u.Url)
        //        .IsUnique();
        //}
    }

    [Index(nameof(Url), IsUnique = true)]
    public class DbUrlDto
    {
        [Key]
        public string Url { get; set; } = string.Empty;

        public DateTime LastQueuedUtc { get; set; } = DateTime.MinValue;
    }
}
