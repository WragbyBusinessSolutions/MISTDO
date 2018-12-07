using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MISTDO.Web.Data;
using MISTDO.Web.Models;
using MISTDO.Web.Services;
using MISTDO.Web.Extensions;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;

namespace MISTDO.Web
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
            services.AddRouting(options => { options.LowercaseUrls = true; });
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("TrainerConnection")) );
            services.AddDbContext<TraineeApplicationDbContext>(options =>
               options.UseSqlServer(Configuration.GetConnectionString("TraineeConnection")));
            services.AddDbContext<AdminApplicationDbContext>(options =>
              options.UseSqlServer(Configuration.GetConnectionString("AdminConnection")));

            services.AddSecondIdentity<TraineeApplicationUser, IdentityRole>(null)
             .AddEntityFrameworkStores<TraineeApplicationDbContext>()
             .AddDefaultTokenProviders();

            
            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();
            services.Configure<RouteOptions>(options => options.LowercaseUrls = true);

            services.ConfigureApplicationCookie(options =>
            {
              
                options.Cookie.HttpOnly = true;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(15);
                options.LoginPath = "/Home/Index";

                options.LogoutPath = "/Home/Index";
                options.AccessDeniedPath = "/Home/Index";
                options.SlidingExpiration = true;
            });
            services.AddDistributedMemoryCache();

    //        services
    //.AddAuthentication(o =>
    //{
    //    o.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    //})
    //.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, o =>
    //{
    //    o.LoginPath = new PathString("/account/login");
    //})
    //.AddCookie("TrainerAuth", o =>
    //{
    //    o.LoginPath = new PathString("/trainer/login/");
    //    o.LogoutPath = new PathString("/trainer/logout/");

    //});


         //   services.AddAuthentication()
         //     .AddCookie("TrainerAuth", options =>
         //     {
         //         options.LoginPath = "/trainer/login";
         //         options.AccessDeniedPath = "/trainer/AccessDenied";
         //         options.LogoutPath = "/trainer/logout";

         //     })
         //.AddCookie("TraineeAuth", options =>
         //{
         //    options.LoginPath = "/trainees/login";
         //    options.AccessDeniedPath = "/trainees/AccessDenied/";

         //});

         
            // Add application services.
            services.AddTransient<IEmailSender, EmailSender>();
            services.AddTransient<ITrainerService, TrainerService>();
            services.AddTransient<IExcelToTrainingService, ExcelToTrainingService>();
            services.AddTransient<IExcelToTraineeService, ExcelToTraineeService>();


            services.AddSession(options =>
            {
                // Set a short timeout for easy testing.
                options.IdleTimeout = TimeSpan.FromMinutes(10);
                options.Cookie.HttpOnly = true;
            });
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IServiceProvider provider)
        {
            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();

                //   app.UseExceptionHandler("/Home/Error");
            }
            app.UseSession();

            app.UseStaticFiles();
            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
         CreateUserRoles(provider).Wait();

        }
        private async Task CreateUserRoles(IServiceProvider serviceProvider)
        {
            try
            {
                var RoleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                var UserManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

                IdentityResult roleResult;
                //Adding Admin Role 
                var PAProleCheck = await RoleManager.RoleExistsAsync("PAP");
                var AMCProleCheck = await RoleManager.RoleExistsAsync("AMCP");
                var AdminroleCheck = await RoleManager.RoleExistsAsync("Admin");
                var TrainerroleCheck = await RoleManager.RoleExistsAsync("Trainer");
                var TraineeroleCheck = await RoleManager.RoleExistsAsync("Trainee");


                if (!PAProleCheck)
                {
                    roleResult = await RoleManager.CreateAsync(new IdentityRole("PAP"));
                }
                if (!AMCProleCheck)
                {
                    roleResult = await RoleManager.CreateAsync(new IdentityRole("AMCP"));
                }
                if (!AdminroleCheck)
                {
                    roleResult = await RoleManager.CreateAsync(new IdentityRole("Admin"));
                }
                if (!TrainerroleCheck)
                {
                    roleResult = await RoleManager.CreateAsync(new IdentityRole("Trainer"));
                }
                if (!TraineeroleCheck)
                {
                    roleResult = await RoleManager.CreateAsync(new IdentityRole("Trainee"));
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
