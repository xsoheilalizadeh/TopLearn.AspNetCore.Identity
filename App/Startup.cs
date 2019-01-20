using App.Data;
using App.DataProtection;
using App.Domain;
using App.Domain.Identity;
using App.Services;
using App.Services.Identity;
using App.Services.Identity.Managers;
using App.Services.Identity.Stores;
using App.Services.Identity.Validators;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace App
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });


            services.AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);


            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(
                    connectionString: Configuration.GetConnectionString("DefaultConnection")
                );
            });

            services.AddScoped<IUserValidator<User>, AppUserValidator>();
            services.AddScoped<UserValidator<User>, AppUserValidator>();

            services.AddScoped<IRoleValidator<Role>, AppRoleValidator>();
            services.AddScoped<RoleValidator<Role>, AppRoleValidator>();


            services.AddIdentity<User, Role>(option =>
                {
                    option.Stores.ProtectPersonalData = true;

                    option.User.RequireUniqueEmail = true;

                    option.Password.RequireDigit = true;
                    option.Password.RequireLowercase = false;
                    option.Password.RequireUppercase = false;
                    option.Password.RequireNonAlphanumeric = false;

                    option.SignIn.RequireConfirmedEmail = true;

                })
                .AddUserStore<AppUserStore>()
                .AddRoleStore<AppRoleStore>()
//                .AddUserValidator<AppUserValidator>()
//                .AddRoleValidator<AppRoleValidator>()
                .AddUserManager<AppUserManager>()
                .AddRoleManager<AppRoleManager>()
                .AddSignInManager<AppSignInManager>()
                .AddErrorDescriber<AppErrorDescriber>()
                .AddClaimsPrincipalFactory<AppUserClaimsPrincipalFactory>()
                .AddDefaultTokenProviders();

            services.AddScoped<ILookupProtectorKeyRing, KeyRing>();

            services.AddScoped<ILookupProtector, LookupProtector>();

            services.AddScoped<IPersonalDataProtector, PersonalDataProtector>();

            services.AddScoped<IEmailSender, EmailSender>();
            services.AddScoped<ISmsSender, SmsSender>();

            services.AddAuthentication()
                .AddGoogle(options =>
                {
                    options.ClientId = Configuration["GoogleAuth:ClientId"];
                    options.ClientSecret = Configuration["GoogleAuth:ClientSecret"];
                });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("RequiredAdminRoleAndManager", policy =>
                {
                    policy.RequireRole("Admin","Manager");
                });

                options.AddPolicy("Plan1", policy =>
                {
                    policy.RequireClaim("UserPlan","1");
                });

                options.AddPolicy("Plan18", policy =>
                {
                    policy.Requirements.Add(new Plan18Requirement("Saeed"));
                });

                options.AddPolicy(ConstantPolicies.DynamicPermission, policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.Requirements.Add(new DynamicPermissionRequirement());
                });
            });

            services.AddScoped<IAuthorizationHandler, Plan18AuthorizationHandler>();

            services.AddScoped<IAuthorizationHandler, DynamicPermissionHandler>();

            services.ConfigureApplicationCookie(options =>
            {
                options.Cookie.Name = "App.Cookie";
                options.LoginPath = "/account/sign-in";
                options.AccessDeniedPath = "/account/access-denied";
                options.LogoutPath = "/account/sign-out";
                options.ReturnUrlParameter = "returnTo";
            });
        }


        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}