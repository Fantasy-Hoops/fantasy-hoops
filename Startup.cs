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
using fantasy_hoops;
using dotenv.net;
using Microsoft.AspNetCore.Http;
using fantasy_hoops.Services;
using Microsoft.Net.Http.Headers;
using WebPush;
using System.Threading.Tasks;

namespace fantasy_hoops
{
    public class Startup
    {
        private GameContext _context;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<IPushService, PushService>();

            services.AddMvc();

            DotEnv.Config(true, ".env");
            #if DEBUG
                DotEnv.Config(false, ".env.development");
            #endif

            ConfigureAuth(services);
            ConfigureDbContext(services);
            Task.Run(() => StartJobs());

            // In production, the React files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/build";
            });
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

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = "nekrosius.com",

                ValidateAudience = true,
                ValidAudience = "nekrosius.com",

                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("Tai yra raktas musu saugumo sistemai, kuo ilgesnis tuo geriau?")),

                RequireExpirationTime = false,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(configureOptions =>
            {
                configureOptions.ClaimsIssuer = "nekrosius.com";
                configureOptions.TokenValidationParameters = tokenValidationParameters;
                configureOptions.SaveToken = true;
            });
        }

        private void ConfigureDbContext(IServiceCollection services)
        {
            services.AddDbContext<GameContext>();
            // 'scoped' in ASP.NET means "per HTTP request"
            services.AddScoped<GameContext>();

            services.AddMvc()
             .AddJsonOptions(
                   options => options.SerializerSettings.ReferenceLoopHandling
                       = Newtonsoft.Json.ReferenceLoopHandling.Ignore);
        }

        private async Task StartJobs()
        {
            Environment.SetEnvironmentVariable("VapidSubject", "mailto:email@outlook.com");
            Environment.SetEnvironmentVariable("VapidPublicKey", "BBcHZh0MdAZjW7GTacGPphvPPXRyLAfB5UyC-t3ma_H0WI2KcH_W-31dE3XZqmb762FQCIG37GnsTnDwlN8Cg8s");
            Environment.SetEnvironmentVariable("VapidPrivateKey", "Y2q1KzR5YmV1e_EPxA1aR7ogn13qiNVp0eE6G7rt2zY");
            _context = new GameContext();
            _context.Database.Migrate();
            await Scheduler.Run(_context);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles(new StaticFileOptions
            {
                OnPrepareResponse = ctx =>
                {
                    const int cacheExpirationInSeconds = 60 * 60 * 24 * 30; //one month
                    ctx.Context.Response.Headers[HeaderNames.CacheControl] =
                        "public,max-age=" + cacheExpirationInSeconds;
                }
            });
            app.UseSpaStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller}/{action=Index}/{id?}");
            });

            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment())
                {
                    spa.UseReactDevelopmentServer(npmScript: "start");
                }
            });
        }
    }
}
