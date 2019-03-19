using AutoMapper;
using CodeGenerator.Data;
using CodeGenerator.Data.Authentication;
using CodeGenerator.Data.Configs;
using CodeGenerator.Data.Models;
using HelpersForCore;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NSwag.AspNetCore;
using System;

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
            #region appsettings.json configs
            services.Configure<JwtConfig>(Configuration.GetSection("JwtConfig"));
            services.Configure<SecurityConfig>(Configuration.GetSection("SecurityConfig"));
            #endregion

            services.AddScoped<Pluralize.NET.Core.Pluralizer>();
            services.AddScoped<JwtBuilder>();
            services.AddScoped(x => new SqlHelper(Configuration.GetConnectionString("CodeGeneratorContext")));
            services.AddEntityFrameworkSqlServer().AddDbContext<CodeGeneratorContext>(options =>
            {
                options.UseSqlServer(
                    Configuration.GetConnectionString("CodeGeneratorContext"),
                    x => x.MigrationsHistoryTable("CodeGeneratorMigrationsHistory"));
            });

            services.AddAutoMapper();
            #region Identity
            services.AddDefaultIdentity<ApplicationUser>()
                .AddEntityFrameworkStores<CodeGeneratorContext>()
                .AddDefaultTokenProviders();
            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
                options.User.RequireUniqueEmail = true;
            });
            services.ConfigureApplicationCookie(options =>
            {
                options.Cookie.HttpOnly = false;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(5);
                options.LoginPath = "/users/login";
                options.AccessDeniedPath = "/users/accessDenied";
                options.SlidingExpiration = true;
            });
            #endregion

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.AddHttpContextAccessor();
            services.AddMediatR();
            services.AddSwaggerDocument(configure =>
            {
                configure.PostProcess = document =>
                {
                    document.Info.Version = "v1";
                    document.Info.Title = "CodeGenerator API";
                    document.Info.Description = "ASP.NET Core Web API";
                };
            });

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
                app.UseSwagger();
                app.UseSwaggerUi3();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }
            app.UseExceptionHandler("/api/Error");

            app.UseAuthentication();
            app.UseStaticFiles();
            app.UseSpaStaticFiles();

            app.UseMvc(routes =>
            {
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
