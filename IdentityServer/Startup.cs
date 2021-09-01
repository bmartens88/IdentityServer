using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using IdentityServer.Entities;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace IdentityServer
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            // AutoMapper
            services.AddAutoMapper(typeof(Startup));

            services.AddControllersWithViews();

            // EF Core Setup
            var migrationAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;

            // AspNetCore Identity Setup
            services.AddDbContext<UserContext>(options =>
                options.UseSqlite(_configuration.GetConnectionString("identitySqlConnection")));

            services.AddIdentity<User, IdentityRole>(opt =>
                {
                    opt.Password.RequireDigit = false;
                    opt.Password.RequiredLength = 7;
                    opt.Password.RequireUppercase = false;
                })
                .AddEntityFrameworkStores<UserContext>()
                .AddDefaultTokenProviders();

            // In-memory configuration of Identity Server
            // services.AddIdentityServer()
            //     .AddInMemoryApiResources(Config.ApiResources)
            //     .AddInMemoryIdentityResources(Config.IdentityResources)
            //     .AddInMemoryClients(Config.Clients)
            //     .AddInMemoryApiScopes(Config.Scopes)
            //     .AddTestUsers(Config.TestUsers)
            //     .AddDeveloperSigningCredential();

            var builder = services.AddIdentityServer()
                // .AddTestUsers(Config.TestUsers)
                .AddConfigurationStore(opt =>
                {
                    opt.ConfigureDbContext = c => c.UseSqlite(_configuration.GetConnectionString("sqlConnection"),
                        sql => sql.MigrationsAssembly(migrationAssembly));
                })
                .AddOperationalStore(opt =>
                {
                    opt.ConfigureDbContext = o =>
                        o.UseSqlite(_configuration.GetConnectionString("sqlConnection"),
                            sql => sql.MigrationsAssembly(migrationAssembly));
                })
                // Add AspNetCore Identity
                .AddAspNetIdentity<User>();
            builder.AddDeveloperSigningCredential();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();

            app.UseRouting();

            app.UseIdentityServer();
            app.UseAuthorization();

            app.UseEndpoints(endpoints => endpoints.MapDefaultControllerRoute());
        }
    }
}