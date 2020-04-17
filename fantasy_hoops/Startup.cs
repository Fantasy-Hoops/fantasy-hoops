using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using fantasy_hoops.Database;
using fantasy_hoops.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Text;
using Microsoft.AspNetCore.Http;
using fantasy_hoops.Services;
using System.Threading.Tasks;
using Microsoft.AspNetCore.ResponseCompression;
using System.IO.Compression;
using fantasy_hoops.Repositories;
using FluentScheduler;
using System.Collections.Generic;
using fantasy_hoops.Auth;
using fantasy_hoops.Repositories.Interfaces;
using fantasy_hoops.Services.Interfaces;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace fantasy_hoops
{
    public class Startup
    {
        public IWebHostEnvironment HostingEnvironment;
        public static IConfiguration Configuration { get; set; }
        
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            HostingEnvironment = env;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
            AddScopes(services);
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddMvc();

            // Add gzip compression
            services.Configure<GzipCompressionProviderOptions>(options => options.Level = CompressionLevel.Optimal);
            services.AddResponseCompression(options =>
            {
                options.Providers.Add<GzipCompressionProvider>();
                //options.EnableForHttps = true;
                options.MimeTypes = new[]
                {
                    // Default
                    "text/plain",
                    "text/css",
                    "application/javascript",
                    "text/html",
                    "application/xml",
                    "text/xml",
                    "application/json",
                    "text/json",

                    // Custom
                    "image/svg+xml",
                    "application/font-woff2"
                };
            });

#if DEBUG
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "FH API",
                    Description = "Fantasy Hoops API"
                });
            });
#endif
            services.AddDbContext<GameContext>(o => o.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            ConfigureAuth(services);

            // In production, the React files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/build";
            });
            
            services.AddControllers().AddNewtonsoftJson(options =>
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
            );

            services.AddDataProtection()
                .SetDefaultKeyLifetime(TimeSpan.FromDays(14));
        }

        public void AddScopes(IServiceCollection services)
        {
            // Services
            services.AddScoped<IBlogService, BlogService>();
            services.AddScoped<IFriendService, FriendService>();
            services.AddScoped<ILineupService, LineupService>();
            services.AddScoped<IPushService, PushService>();
            services.AddScoped<IScoreService, ScoreService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IAchievementsService, AchievementsService>();
            services.AddScoped<ITournamentsService, TournamentsService>();

            // Repositories
            services.AddScoped<IBlogRepository, BlogRepository>();
            services.AddScoped<IFriendRepository, FriendRepository>();
            services.AddScoped<IInjuryRepository, InjuryRepository>();
            services.AddScoped<ILeaderboardRepository, LeaderboardRepository>();
            services.AddScoped<ILineupRepository, LineupRepository>();
            services.AddScoped<INewsRepository, NewsRepository>();
            services.AddScoped<INotificationRepository, NotificationRepository>();
            services.AddScoped<IPlayerRepository, PlayerRepository>();
            services.AddScoped<IPushNotificationRepository, PushNotificationRepository>();
            services.AddScoped<IScoreRepository, ScoreRepository>();
            services.AddScoped<IStatsRepository, StatsRepository>();
            services.AddScoped<ITeamRepository, TeamRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IBestLineupsRepository, BestLineupsRepository>();
            services.AddScoped<IAchievementsRepository, AchievementsRepository>();
            services.AddScoped<ITournamentsRepository, TournamentsRepository>();
        }

        private void ConfigureAuth(IServiceCollection services)
        {
            services.AddIdentity<User, IdentityRole>(config =>
            {
                config.Password.RequireDigit = false;
                config.Password.RequireLowercase = false;
                config.Password.RequireUppercase = false;
                config.Password.RequireNonAlphanumeric = false;
                config.Password.RequiredLength = 8;
                config.SignIn.RequireConfirmedEmail = false;
            })
           .AddEntityFrameworkStores<GameContext>()
           .AddDefaultTokenProviders();

            List<string> issuers = new List<string>() { Configuration["CustomAuth:Issuer"], Configuration["Google:Issuer"] };
            List<string> audiences = new List<string>() { Configuration["CustomAuth:Audience"], Configuration["Google:Audience"] };

            services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultForbidScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(configureOptions =>
                {
                    //configureOptions.ClaimsIssuer = "nekrosius.com";
                    configureOptions.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuers = issuers,

                        ValidateAudience = true,
                        ValidAudiences = audiences,

                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["CustomAuth:IssuerSigningKey"])),

                        RequireExpirationTime = true,
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.Zero
                    };
                    configureOptions.SecurityTokenValidators.Add(new GoogleTokenValidator());
                    configureOptions.SaveToken = true;
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider serviceProvider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger(c => c.RouteTemplate = "swagger/{documentName}/swagger.json");
                app.UseSwaggerUI(c =>
                {
                    c.RoutePrefix = "swagger";
                    c.SwaggerEndpoint("v1/swagger.json", "FH API V1");
                });
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseRouting();

            app.UseSpaStaticFiles();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action=Index}/{id?}");
            });

            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment())
                {
                    spa.UseReactDevelopmentServer(npmScript: "start");
                }
            });

            using (var serviceScope = app.ApplicationServices.CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<GameContext>();
                context.Database.Migrate();

                var scoreService = serviceScope.ServiceProvider.GetService<IScoreService>();
                var pushService = serviceScope.ServiceProvider.GetService<IPushService>();
                JobManager.UseUtcTime();
                JobManager.Initialize(new ApplicationRegistry(context, scoreService, pushService));
            }

            Task.Run(() => CreateRoles(serviceProvider)).Wait();
        }

        private async Task CreateRoles(IServiceProvider serviceProvider)
        {
            //initializing custom roles 
            var RoleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var UserManager = serviceProvider.GetRequiredService<UserManager<User>>();
            string[] roleNames = { "Admin" };
            IdentityResult roleResult;

            foreach (var roleName in roleNames)
            {
                var roleExist = await RoleManager.RoleExistsAsync(roleName);
                // ensure that the role does not exist
                if (!roleExist)
                {
                    //create the roles and seed them to the database: 
                    roleResult = await RoleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            var naidze = await UserManager.FindByNameAsync("Naidze");
            if (naidze != null && !await UserManager.IsInRoleAsync(naidze, "Admin"))
                await UserManager.AddToRoleAsync(naidze, "Admin");

            var bennek = await UserManager.FindByNameAsync("bennek");
            if (bennek != null && !await UserManager.IsInRoleAsync(bennek, "Admin"))
                await UserManager.AddToRoleAsync(bennek, "Admin");
        }
    }
}
