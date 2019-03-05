using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Pubquiz.Domain.Models;
using Pubquiz.Logic.Hubs;
using Pubquiz.Logic.Messages;
using Pubquiz.Logic.Tools;
using Pubquiz.Persistence;
using Pubquiz.Persistence.Extensions;
using Pubquiz.Persistence.Helpers;
using Pubquiz.WebApi.Helpers;
using Rebus.Persistence.InMem;
using Rebus.Routing.TypeBased;
using Rebus.ServiceProvider;
using Rebus.Transport.InMem;
using Swashbuckle.AspNetCore.Swagger;

namespace Pubquiz.WebApi
{
    public class Startup
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly ILoggerFactory _loggerFactory;

        public Startup(IConfiguration configuration, IHostingEnvironment hostingEnvironment,
            ILoggerFactory loggerFactory)
        {
            _hostingEnvironment = hostingEnvironment;
            _loggerFactory = loggerFactory;
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors();
            services.AutoRegisterHandlersFromAssembly("Pubquiz.Logic");
            // needed so the inmemory subscription store will be centralized
            var inMemorySubscriberStore = new InMemorySubscriberStore();
            services.AddSingleton(inMemorySubscriberStore);
            services.AddRebus(configure =>
                configure.Logging(l => l.Use(new MsLoggerFactoryAdapter(_loggerFactory)))
                    .Transport(t => t.UseInMemoryTransport(new InMemNetwork(true), "Messages"))
                    .Subscriptions(s => s.StoreInMemory(inMemorySubscriberStore))
                    .Routing(r => r.TypeBased().MapAssemblyOf<InteractionResponseAdded>("Messages")));

            services.AddResponseCompression();
            services.AddMemoryCache();
            services.AddLogging(builder =>
            {
                builder.AddConfiguration(Configuration.GetSection("Logging"));
                builder.AddConsole();
                builder.AddDebug();
            });
            switch (Configuration.GetValue<string>("AppSettings:Database"))
            {
                case "Memory":
                    services.AddInMemoryPersistence();
                    break;
                case "MongoDB":
                    services.AddMongoDbPersistence("Quizr", Configuration.GetConnectionString("MongoDB"));
                    break;
            }

            services.AddRequests(Assembly.Load("Pubquiz.Logic"));
            services.AddMvcCore(options =>
                {
                    options.Filters.Add(typeof(DomainExceptionFilter));
                    options.Filters.Add(typeof(UnitOfWorkActionFilter));
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

            services.AddSingleton<IConfigureOptions<MvcJsonOptions>, JsonOptionsSetup>();

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(
                options =>
                {
                    options.LoginPath = new PathString("/swagger");
                    options.Events.OnRedirectToLogin = context =>
                    {
                        context.Response.StatusCode = StatusCodes.Status404NotFound;
                        return Task.CompletedTask;
                    };
                });

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
            app.UseCors(builder =>
            {
                builder.WithOrigins("http://localhost:8080", "*")
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
            });
            app.UseAuthentication();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                //app.UseHsts();
            }

            app.UseRebus(bus => bus.SubscribeByScanningForHandlers(Assembly.Load("Pubquiz.Logic")));
            //            app.UseRebus().Run(async context =>
            //            {
            //                var bus = app.ApplicationServices.GetRequiredService<IBus>();
            //            });
            app.UseDefaultFiles();
            app.UseStaticFiles();
            //app.UseHttpsRedirection();
            app.UseMvc();
            app.UseSwagger();
            app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "Pubquiz backend V1"); });

            app.UseSignalR(route => { route.MapHub<GameHub>("/gamehub"); });

            var unitOfWork = app.ApplicationServices.GetService<IUnitOfWork>();
            var mongoDbIsEmpty = Configuration.GetValue<string>("AppSettings:Database") == "MongoDB" &&
                                 unitOfWork.GetCollection<Team>().GetCountAsync().Result == 0;
            // Seed the test data when using in-memory-database
            if (mongoDbIsEmpty || Configuration.GetValue<string>("AppSettings:Database") == "Memory")
            {
                //var unitOfWork = app.ApplicationServices.GetService<IUnitOfWork>();
                var loggerFactory = app.ApplicationServices.GetService<ILoggerFactory>();
                var seeder = new TestSeeder(unitOfWork, loggerFactory);
                seeder.SeedTestSet();
            }
        }
    }
}