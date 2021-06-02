using IdentityModel;
using IdentityServer.Data;
using IdentityServer.Model;
using IdentityServer4.Models;
using IdentityServer4.Test;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace IdentityServer
{
    public class Startup
    {
        public IWebHostEnvironment Environment { get; }
        public IConfiguration Configuration { get; }

        public Startup(IWebHostEnvironment environment, IConfiguration configuration)
        {
            Environment = environment;
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
            services.AddDbContextPool<ApplicationContext>(
                options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 8;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequiredUniqueChars = 1;

            }).AddEntityFrameworkStores<ApplicationContext>()
             .AddDefaultTokenProviders();



            services.AddIdentityServer()
                .AddInMemoryClients(Config.Clients)
                .AddInMemoryIdentityResources(Config.IdentityResources)
                //.AddInMemoryApiResources(Config.ApiResources)
                .AddInMemoryApiScopes(Config.ApiScopes)
                 //.AddTestUsers(Config.TestUsers)
                 .AddAspNetIdentity<ApplicationUser>()
                .AddDeveloperSigningCredential();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            UpdateDatabase(app);
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();
            app.UseRouting();
            app.UseIdentityServer();
            app.UseAuthorization();

            app.UseEndpoints(a =>
            {
                a.MapDefaultControllerRoute();
            });
        }

        private static void UpdateDatabase(IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices
                .GetRequiredService<IServiceScopeFactory>()
                .CreateScope())
            {
                using (var context = serviceScope.ServiceProvider.GetService<ApplicationContext>())
                {
                    context.Database.Migrate();
                    var userMgr = serviceScope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                    var alice = userMgr.FindByEmailAsync("pankaj@email.com").Result;
                    if (alice == null)
                    {
                        alice = new ApplicationUser
                        {
                            UserName = "pankaj",
                            Email = "pankaj@email.com",
                            EmailConfirmed = true,
                        };
                        var result = userMgr.CreateAsync(alice, "Pankaj@123").Result;
                        if (!result.Succeeded)
                        {
                            throw new Exception(result.Errors.First().Description);
                        }

                        result = userMgr.AddClaimsAsync(alice, new Claim[]{
                            new Claim(JwtClaimTypes.Name, "Pankaj Dhami"),
                            new Claim(JwtClaimTypes.GivenName, "Pankaj"),
                            new Claim(JwtClaimTypes.FamilyName, "Dhami"),
                            new Claim(JwtClaimTypes.WebSite, "http://pankaj.com"),
                        }).Result;
                        if (!result.Succeeded)
                        {
                            throw new Exception(result.Errors.First().Description);
                        }
                        Log.Debug("pankaj created");
                    }
                    else
                    {
                        Log.Debug("pankaj already exists");
                    }

                    var bob = userMgr.FindByEmailAsync("BobSmith@email.com").Result;
                    if (bob == null)
                    {
                        bob = new ApplicationUser
                        {
                            UserName = "bob",
                            Email = "BobSmith@email.com",
                            EmailConfirmed = true
                        };
                        var result = userMgr.CreateAsync(bob, "Pass123$").Result;
                        if (!result.Succeeded)
                        {
                            throw new Exception(result.Errors.First().Description);
                        }

                        result = userMgr.AddClaimsAsync(bob, new Claim[]{
                            new Claim(JwtClaimTypes.Name, "Bob Smith"),
                            new Claim(JwtClaimTypes.GivenName, "Bob"),
                            new Claim(JwtClaimTypes.FamilyName, "Smith"),
                            new Claim(JwtClaimTypes.WebSite, "http://bob.com"),
                            new Claim("location", "somewhere")
                        }).Result;
                        if (!result.Succeeded)
                        {
                            throw new Exception(result.Errors.First().Description);
                        }
                        Log.Debug("bob created");
                    }
                    else
                    {
                        Log.Debug("bob already exists");
                    }
                }
            }
        }

        //for more detail on jwt auth by identityserver4 https://damienbod.com/2017/04/14/asp-net-core-identityserver4-resource-owner-password-flow-with-custom-userrepository/

        //        curl --location --request POST 'https://localhost:5005/connect/token' \
        //--header 'Content-Type: application/x-www-form-urlencoded' \
        //--data-urlencode 'grant_type=password' \
        //--data-urlencode 'scope=movieAPI' \
        //--data-urlencode 'client_id=resourceownerclient' \
        //--data-urlencode 'client_secret=secret' \
        //--data-urlencode 'username=pankaj' \
        //--data-urlencode 'password=Pankaj@123'
    }
}
