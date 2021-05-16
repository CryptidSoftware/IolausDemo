using Iolaus;
using Iolaus.Utils;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NATS.Client;

namespace IolausDemo
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();
            services.AddHttpClient();

            var connectionFactory = new ConnectionFactory();
            var connection = connectionFactory.CreateConnection();
            services.AddSingleton(connection);

            var configReader = new Iolaus.Config.FileConfigurationProvider("config.json");
            services.AddSingleton<Iolaus.Config.IConfigurationProvider>(configReader);

            services.AddRouteRegistry(r => {
                r.Add("NATS", Iolaus.Nats.NatsRoute.Route);
                r.Add("HTTP", Iolaus.Http.HttpRoute.Route);
            });

            services.AddSingleton<Router>();


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
            });
        }
    }
}
