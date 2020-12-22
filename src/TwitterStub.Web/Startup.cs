using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TwitterStub.Core.Metrics;
using TwitterStub.Core.Statistics;
using TwitterStub.Core.Stream;
using TwitterStub.Interfaces.Data;
using TwitterStub.Interfaces.Metrics;
using TwitterStub.Interfaces.Statistics;
using TwitterStub.Interfaces.Stream;
using TwitterStub.Models.Configuration;
using TwitterStub.Repository;
using TwitterStub.Service;

namespace TwitterStub.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            HostEnvironment = env;
        }

        public IConfiguration Configuration { get; }

        public IWebHostEnvironment HostEnvironment { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();

            services.Configure<MetricsSettings>(
                options => Configuration.GetSection("MetricsSettings").Bind(options));

            if (HostEnvironment.IsDevelopment())
            {
                //Uncomment this lines to run Service and Web App on the same Host 
                services.AddHostedService<Worker>();
                services.Configure<TwitterStubSettings>(
                options => Configuration.GetSection("TwitterStubSettings").Bind(options));
                services.AddTransient<IStreamTask, StreamTask>();

                services.AddDbContext<TwitterStubContext>(options =>
                {
                    string connectionStringBuilder = new SqliteConnectionStringBuilder()
                    {
                        DataSource = Configuration.GetConnectionString("TwitterStubSqlite")
                    }.ToString();

                    var connection = new SqliteConnection(connectionStringBuilder);
                    options.UseSqlite(connectionStringBuilder);
                });

            }
            else
            {
                services.AddDbContext<TwitterStubContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("TwitterStubDb")));
            }
            services.AddSingleton<IRepository, BaseRepository>();
            services.AddSingleton<IStatisticsService, StatisticsService>();
            

            services.AddSingleton<IMetricSource, MetricSource>();

            services.AddSingleton<IMetricTracker, TopEmojiMetric>();
            services.AddSingleton<IMetricTracker, TopHashtagMetric>();
            services.AddSingleton<IMetricTracker, TopDomainMetric>();
            services.AddSingleton<IMetricTracker, TotalTweetsMetric>();
            services.AddSingleton<IMetricTracker, TweetsPerHourMetric>();
            services.AddSingleton<IMetricTracker, TweetsPerMinuteMetric>();
            services.AddSingleton<IMetricTracker, TweetsPerSecondMetric>();
            services.AddSingleton<IMetricTracker, PhotoPercentageMetric>();
            services.AddSingleton<IMetricTracker, UrlsPercentageMetric>();
            services.AddSingleton<IMetricTracker, EmojiPercentageMetric>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                UpdateDatabase(app);
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }

        private static void UpdateDatabase(IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices
                .GetRequiredService<IServiceScopeFactory>()
                .CreateScope())
            {
                using (var context = serviceScope.ServiceProvider.GetService<TwitterStubContext>())
                {
                    context.Database.EnsureCreated();
                }
            }
        }
    }
}
