using HelpersForCore;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NSwag.AspNetCore;

namespace CodeGenerator
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
            services.AddScoped<Pluralize.NET.Core.Pluralizer>();
            services.AddScoped(x => new SqlHelper(Configuration.GetConnectionString("DatabaseContext")));
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.AddMediatR();
            // In production, the Angular files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/dist";
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger(configure =>
                {
                    configure.PostProcess = (document, requset) =>
                    {
                        document.Info.Version = "v0";
                        document.Info.Title = "監察院公開查閱平台";
                        document.Info.Description = "API Endpoint documentation for 監察院公開查閱平台, which includes user management, groups management, boards and subboards management, threads management, tags management and more to come";
                        document.Info.TermsOfService = "None";
                        document.Info.Contact = new NSwag.SwaggerContact
                        {
                            Name = "Derrick Yen",
                            Email = "derrickyen@universal.com.tw",
                            Url = System.String.Empty
                        };
                        document.Info.License = new NSwag.SwaggerLicense
                        {
                            Name = "Commercial",
                            Url = "https://www.universal.com.tw/software-solutions/license"
                        };
                    };
                });
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseStaticFiles();
            app.UseSpaStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "swagger",
                    template: "swagger/{*path}"
                );
                routes.MapRoute(
                    name: "default",
                    template: "{controller}/{action=Index}/{id?}");
            });

            app.UseSpa(spa =>
            {
                // To learn more about options for serving an Angular SPA from ASP.NET Core,
                // see https://go.microsoft.com/fwlink/?linkid=864501

                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment())
                {
                    spa.Options.StartupTimeout = new System.TimeSpan(days: 0, hours: 0, minutes: 1, seconds: 30);
                    spa.UseAngularCliServer(npmScript: "start");
                }
            });
        }
    }
}
