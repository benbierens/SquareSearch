using Logging;
using Microsoft.EntityFrameworkCore;

namespace DbUrls
{
    public abstract class DbCommon<TContext> where TContext : BaseContext, new()
    {
        private readonly ILogger logger;
        private readonly string dbname;

        internal DbCommon(ILogger logger, string dbname)
        {
            this.logger = logger;
            this.dbname = dbname;
        }

        protected TContext Context { get; private set; } = null!;

        public async Task Initialize()
        {
            try
            {
                var host = GetEnv("DBHOST");
                Context = new TContext();
                Context.Decorate(
                    host: host,
                    login: GetEnv("DBLOGIN"),
                    password: GetEnv("DBPASSWORD"),
                    database: dbname
                );
                await Context.Database.EnsureCreatedAsync();
                logger.Info($"Initialized database at host {host}");
            }
            catch (Exception ex)
            {
                logger.Error("Failed to initialize db", ex);
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

    public abstract class BaseContext : DbContext
    {
        private string host = string.Empty;
        private string login = string.Empty;
        private string password = string.Empty;
        private string database = string.Empty;

        public void Decorate(string host, string login, string password, string database)
        {
            this.host = host;
            this.login = login;
            this.password = password;
            this.database = database;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseNpgsql($"Host={host};Username={login};Password={password};Database={database}");
    }
}
