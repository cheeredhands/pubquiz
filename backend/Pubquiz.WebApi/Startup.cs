using System.IO;
using System.Reflection;
using System.Text.Json;
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
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
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

namespace Pubquiz.WebApi
{
    public class Startup
    {
        private readonly IWebHostEnvironment _hostingEnvironment;
     
        public Startup(IConfiguration configuration, IWebHostEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            AddDefaultWebApiStuff(services);
            AddQuizrSpecificStuff(services);
            AddSwagger(services);
        }

        private void AddSwagger(IServiceCollection services)
        {
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo {Title = "Pubquiz backend", Version = "v1"});
            });

            services.ConfigureSwaggerGen(options =>
            {
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
        }

        private void AddQuizrSpecificStuff(IServiceCollection services)
        {
            services.AddMemoryCache();
            services.AutoRegisterHandlersFromAssembly("Pubquiz.Logic");
            // needed so the in memory subscription store will be centralized
            var inMemorySubscriberStore = new InMemorySubscriberStore();
            services.AddSingleton(inMemorySubscriberStore);
            services.AddRebus(configure =>
                configure //.Logging(l => l.Use(new MsLoggerFactoryAdapter(_loggerFactory)))
                    .Transport(t => t.UseInMemoryTransport(new InMemNetwork(true), "Messages"))
                    .Subscriptions(s => s.StoreInMemory(inMemorySubscriberStore))
                    .Routing(r => r.TypeBased().MapAssemblyOf<InteractionResponseAdded>("Messages")));

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
            services.AddSignalR();
        }

        private void AddDefaultWebApiStuff(IServiceCollection services)
        {
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            services.AddControllers()
                .AddJsonOptions(options =>
                    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase)
                .AddMvcOptions(options =>
                {
                    options.Filters.Add(typeof(DomainExceptionFilter));
                    options.Filters.Add(typeof(UnitOfWorkActionFilter));
                    var policy = new AuthorizationPolicyBuilder(CookieAuthenticationDefaults.AuthenticationScheme)
                        .RequireAuthenticatedUser()
                        .Build();
                    options.Filters.Add(new AuthorizeFilter(policy));
                })
                .AddNewtonsoftJson()
                .SetCompatibilityVersion(CompatibilityVersion.Version_3_0);
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
            services.AddSingleton<IConfigureOptions<MvcNewtonsoftJsonOptions>, JsonOptionsSetup>();
            services.AddCors();
            services.AddResponseCompression();
            services.AddLogging(builder =>
            {
                builder.AddConfiguration(Configuration.GetSection("Logging"));
                builder.AddConsole();
                builder.AddDebug();
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseCors(builder =>
            {
                builder.WithOrigins("http://localhost:8080", "*")
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
            });
            app.UseAuthentication();
            app.UseAuthorization();
            
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<GameHub>("/gamehub");
                endpoints.MapControllers();
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRebus(bus => bus.SubscribeByScanningForHandlers(Assembly.Load("Pubquiz.Logic")));
           
            app.UseSwagger();
            app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "Pubquiz backend V1"); });

            SeedStuff(app);
        }

        private void SeedStuff(IApplicationBuilder app)
        {
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