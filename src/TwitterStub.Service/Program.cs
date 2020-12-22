using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TwitterStub.Core.Stream;
using TwitterStub.Interfaces.Data;
using TwitterStub.Interfaces.Stream;
using TwitterStub.Models.Configuration;
using TwitterStub.Repository;

namespace TwitterStub.Service
{
    public class Program
    {

        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            using (var scope = host.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<TwitterStubContext>();
                db.Database.EnsureCreated();
            };

            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<Worker>();

                    services.Configure<TwitterStubSettings>(
                            options => hostContext.Configuration.GetSection("TwitterStubSettings").Bind(options));
                    
                    if (hostContext.HostingEnvironment.IsDevelopment())
                    {
                        services.AddDbContext<TwitterStubContext>(options =>
                        {
                            string connectionStringBuilder = new SqliteConnectionStringBuilder()
                            {
                                DataSource = hostContext.Configuration.GetConnectionString("TwitterStubSqlite")
                            }.ToString();

                            var connection = new SqliteConnection(connectionStringBuilder);
                            options.UseSqlite(connectionStringBuilder);
                        });

                    }
                    else
                    {
                        services.AddDbContext<TwitterStubContext>(options =>
                        options.UseSqlServer(hostContext.Configuration.GetConnectionString("TwitterStubDb")));
                    }
                    services.AddSingleton<IRepository, BaseRepository>();
                    services.AddSingleton<IStreamTask, StreamTask>();

                });
    }
}
