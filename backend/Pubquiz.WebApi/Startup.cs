using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Pubquiz.Domain.Models;
using Pubquiz.Persistence;
using Pubquiz.Persistence.Extensions;
using Pubquiz.WebApi.Helpers;
using Swashbuckle.AspNetCore.Swagger;

namespace Pubquiz.WebApi
{
    public class Startup
    {
        private IHostingEnvironment _hostingEnvironment;

        public Startup(IConfiguration configuration, IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddResponseCompression();
            services.AddMemoryCache();
            services.AddLogging(builder =>
            {
                builder.AddConfiguration(Configuration.GetSection("Logging"));
                builder.AddConsole();
                builder.AddDebug();
            });
            services.AddInMemoryPersistence();
            services.AddRequests(Assembly.Load("Pubquiz.Domain"));
            services.AddMvcCore(options =>
                {
                    options.Filters.Add(typeof(DomainExceptionFilter));
                    var policy = new AuthorizationPolicyBuilder(CookieAuthenticationDefaults.AuthenticationScheme)
                        .RequireAuthenticatedUser()
                        .Build();
                    options.Filters.Add(new AuthorizeFilter(policy));
                })
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
                .AddApiExplorer()
                .AddJsonFormatters()
                .AddCacheTagHelper()
                .AddAuthorization();
//                .AddAuthorization(options => //https://github.com/aspnet/Security/issues/1488#issuecomment-336634950
//                {
//                    options.DefaultPolicy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
//                });

            services.AddSingleton<IConfigureOptions<MvcJsonOptions>, JsonOptionsSetup>();

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(
                CookieAuthenticationDefaults.AuthenticationScheme, options =>
                {
//                    options.LoginPath = new PathString("/swagger");
//                    options.AccessDeniedPath = new PathString("/swagger");
//                    options.Events.OnRedirectToLogin = context =>
//                    {
//                        if (context.Request.Path.StartsWithSegments("/api")
//                            && context.Response.StatusCode == StatusCodes.Status200OK)
//                        {
//                            context.Response.Clear();
//                            context.Response.StatusCode = StatusCodes.Status404NotFound;
//                            return Task.CompletedTask;
//                        }
//
//                        context.Response.Redirect(context.RedirectUri);
//                        return Task.CompletedTask;
//                    };
                });
            services.AddIdentity<ApplicationUser, ApplicationRole>(o =>
                o.User.AllowedUserNameCharacters =
                    "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+%");
            services.AddTransient<IUserStore<ApplicationUser>, MyUserStore>();
            services.AddTransient<IRoleStore<ApplicationRole>, MyRoleStore>();

            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new Info {Title = "Pubquiz backend", Version = "v1"});
            });

            services.ConfigureSwaggerGen(options =>
            {
                options.DescribeAllEnumsAsStrings();
                var baseDirectory = _hostingEnvironment.ContentRootPath;
                var commentsFileName = Assembly.GetEntryAssembly().GetName().Name + ".XML";
                var commentsFile = Path.Combine(baseDirectory, commentsFileName);
                if (File.Exists(commentsFile))
                {
                    options.IncludeXmlComments(commentsFile);
                }

                var xmlDocPath = Configuration["Swagger:Path"];
                if (File.Exists(xmlDocPath))
                {
                    options.IncludeXmlComments(xmlDocPath);
                }

                options.CustomSchemaIds(x => x.FullName);
            });

            services.AddSignalR();
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            
           
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseAuthentication();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                //app.UseHsts();
            }

            app.UseDefaultFiles();
            app.UseStaticFiles();
            //app.UseHttpsRedirection();
            app.UseMvc();
            app.UseSwagger();
            app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "Pubquiz backend V1"); });
            
            // Seed the test data
            var unitOfWork =  app.ApplicationServices.GetService<IUnitOfWork>();
            var loggerFactory =  app.ApplicationServices.GetService<ILoggerFactory>();
            var seeder = new TestSeeder(unitOfWork, loggerFactory);
            seeder.SeedTestSet();
            
           
        }
    }
}