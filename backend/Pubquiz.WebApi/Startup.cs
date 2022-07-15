using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using MediatR;
using MediatR.Pipeline;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Pubquiz.Domain;
using Pubquiz.Domain.Models;
using Pubquiz.Logic.Handlers;
using Pubquiz.Logic.Messages;
using Pubquiz.Logic.Tools;
using Pubquiz.Persistence;
using Pubquiz.Persistence.Helpers;
using Pubquiz.Persistence.InMemory;
using Pubquiz.Persistence.MongoDb;
using Pubquiz.WebApi.Helpers;
using Pubquiz.WebApi.Hubs;
using Pubquiz.WebApi.Models;
using Serilog;
using SimpleInjector;
using SimpleInjector.Lifestyles;

namespace Pubquiz.WebApi
{
    public class Startup
    {
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly IConfiguration _configuration;
        private readonly Container _container = new SimpleInjector.Container();

        public Startup(IConfiguration configuration, IWebHostEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
            _configuration = configuration;

            // Set to false. This will be the default in v5.x and going forward.
            _container.Options.ResolveUnregisteredConcreteTypes = false;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            AddDefaultWebApiStuff(services);
            AddQuizrSpecificStuff(services);
            AddSwagger(services);
            AddDI(services);
        }

        private void AddDI(IServiceCollection services)
        {
            // Sets up the basic configuration that for integrating Simple Injector with
            // ASP.NET Core by setting the DefaultScopedLifestyle, and setting up auto
            // cross wiring.
            services.AddSimpleInjector(_container, options =>
            {
                // AddAspNetCore() wraps web requests in a Simple Injector scope and
                // allows request-scoped framework services to be resolved.
                options.AddAspNetCore()

                    // Ensure activation of a specific framework type to be created by
                    // Simple Injector instead of the built-in configuration system.
                    // All calls are optional. You can enable what you need. For instance,
                    // ViewComponents, PageModels, and TagHelpers are not needed when you
                    // build a Web API.
                    .AddControllerActivation();
                //.AddViewComponentActivation()
                //.AddPageModelActivation()
                //.AddTagHelperActivation();

                // Optionally, allow application components to depend on the non-generic
                // ILogger (Microsoft.Extensions.Logging) or IStringLocalizer
                // (Microsoft.Extensions.Localization) abstractions.
                options.AddLogging();
                //options.AddLocalization();
            });

            InitializeContainer(services);
        }

        private void InitializeContainer(IServiceCollection services)
        {
            // Add application services. For instance:
            AddMediatr(_container, typeof(Startup).Assembly, typeof(TeamRegistered).Assembly);
            _container.Collection.Register(typeof(IRequestPreProcessor<>),
                new[]
                {
                    typeof(RequestValidationPreProcessor<>)
                });

            // Find all Hub implementations and register them as scoped.
            var types = _container.GetTypesToRegister<Hub>(typeof(GameHub).Assembly);
            foreach (Type type in types) _container.Register(type, type, Lifestyle.Scoped);

            // NOTE: SimpleInjectorHubActivator<T> must be registered as Scoped
            services.AddScoped(typeof(IHubActivator<>), typeof(SimpleInjectorHubActivator<>));

            _container.Register<TestSeeder>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // UseSimpleInjector() finalizes the integration process.
            app.UseSimpleInjector(_container);

            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseSerilogRequestLogging();
            app.UseRouting();
            app.UseCors(builder =>
            {
                builder.WithOrigins("http://localhost:8080", "http://localhost:8081", "*")
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

            app.UseSwagger();
            app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "Pubquiz backend V1"); });

            SeedStuff(_container);
        }

        private void AddMediatr(Container container, params Assembly[] assemblies)
        {
            var allAssemblies = GetAssemblies(assemblies);

            container.RegisterSingleton<IMediator, Mediator>();
            container.Register(typeof(IRequestHandler<,>), assemblies);

            RegisterHandlers(container, typeof(INotificationHandler<>), allAssemblies);
            //RegisterHandlers(container, typeof(IRequestExceptionAction<,>), allAssemblies);
            //RegisterHandlers(container, typeof(IRequestExceptionHandler<,,>), allAssemblies);

            //Pipeline
            container.Collection.Register(typeof(IPipelineBehavior<,>), new[]
            {
                //typeof(RequestExceptionProcessorBehavior<,>),
                //typeof(RequestExceptionActionProcessorBehavior<,>),
                typeof(RequestPreProcessorBehavior<,>),
                //typeof(RequestPostProcessorBehavior<,>)
            });

            container.Register(() => new ServiceFactory(container.GetInstance), Lifestyle.Singleton);
        }

        private static void RegisterHandlers(Container container, Type collectionType, Assembly[] assemblies)
        {
            // we have to do this because by default, generic type definitions (such as the Constrained Notification Handler) won't be registered
            var handlerTypes = container.GetTypesToRegister(collectionType, assemblies, new TypesToRegisterOptions
            {
                IncludeGenericTypeDefinitions = true,
                IncludeComposites = false,
            });

            container.Collection.Register(collectionType, handlerTypes);
        }

        private static Assembly[] GetAssemblies(IEnumerable<Assembly> assemblies)
        {
            var allAssemblies = new List<Assembly> {typeof(IMediator).GetTypeInfo().Assembly};
            allAssemblies.AddRange(assemblies);

            return allAssemblies.ToArray();
        }

        private void AddDefaultWebApiStuff(IServiceCollection services)
        {
            // controllers, authentication, authorization and such

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            services.AddControllers()
                .AddMvcOptions(options => { options.Filters.Add(typeof(DomainExceptionFilter)); })
                .AddJsonOptions(opts => { opts.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()); })
                .SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

            var secretKey = _configuration.GetValue<string>("AppSettings:JwtSecret");
            var signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secretKey));

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    //options.RequireHttpsMetadata = false;
                    //options.SaveToken = true;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = signingKey,
                        ValidateIssuer = false,
                        ValidateAudience = false
                    };

                    // We have to hook the OnMessageReceived event in order to
                    // allow the JWT authentication handler to read the access
                    // token from the query string when a WebSocket or 
                    // Server-Sent Events request comes in.
                    options.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            var accessToken = context.Request.Query["access_token"];

                            // If the request is for our hub...
                            var path = context.HttpContext.Request.Path;
                            if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/gamehub"))
                            {
                                // Read the token out of the query string
                                context.Token = accessToken;
                            }

                            return Task.CompletedTask;
                        }
                    };
                });
            // api user claim policy
            services.AddAuthorization(options =>
            {
                options.AddPolicy(AuthPolicy.Team,
                    policy => policy.RequireClaim(ClaimTypes.Role, "Team", "QuizMaster")
                        .RequireAuthenticatedUser());
                options.AddPolicy(AuthPolicy.Admin,
                    policy => policy.RequireClaim(ClaimTypes.Role, "Admin")
                        .RequireAuthenticatedUser());
                options.AddPolicy(AuthPolicy.QuizMaster,
                    policy => policy.RequireClaim(ClaimTypes.Role, "QuizMaster", "Admin")
                        .RequireAuthenticatedUser());
            });
            services.AddResponseCompression();

            // CORS
            var corsAllowedOrigins = _configuration.GetValue<string>("AppSettings:corsAllowedOrigins")?.Split(',');
            var corsPolicy = new CorsPolicyBuilder(corsAllowedOrigins)
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials()
                .Build();
            services.AddCors(options => options.AddDefaultPolicy(corsPolicy));

            // logging
            services.AddLogging(builder =>
            {
                builder.AddConfiguration(_configuration.GetSection("Logging"));
                builder.AddConsole();
                builder.AddDebug();
            });
        }

        private void AddQuizrSpecificStuff(IServiceCollection services)
        {
            services.AddMemoryCache();

            var quizrSettings = new QuizrSettings();
            _configuration.Bind("QuizrSettings", quizrSettings);
            quizrSettings.WebRootPath = _hostingEnvironment.WebRootPath;
            services.AddSingleton(_ => quizrSettings);

            switch (_configuration.GetValue<string>("AppSettings:Database"))
            {
                case "Memory":
                    services.AddInMemoryPersistence();
                    break;
                case "MongoDB":
                    services.AddMongoDbPersistence("Quizr", _configuration.GetConnectionString("MongoDB"));
                    break;
            }

            services.AddSignalR().AddJsonProtocol(options =>
                options.PayloadSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
        }

        private void AddSwagger(IServiceCollection services)
        {
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo {Title = "Pubquiz backend", Version = "v1"});
                var securityScheme = new OpenApiSecurityScheme
                {
                    Description =
                        "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 12345abcdef\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                };
                options.AddSecurityDefinition("Bearer", securityScheme);

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Id = "Bearer", //The name of the previously defined security scheme.
                                Type = ReferenceType.SecurityScheme
                            }
                        },
                        new List<string>()
                    }
                });
            });

            services.ConfigureSwaggerGen(options =>
            {
                var baseDirectory = _hostingEnvironment.ContentRootPath;
                var commentsFileName = Assembly.GetEntryAssembly()?.GetName().Name + ".xml";
                var commentsFile = Path.Combine(baseDirectory, commentsFileName);
                if (File.Exists(commentsFile))
                {
                    options.IncludeXmlComments(commentsFile);
                }

                var xmlDocPath = _configuration["Swagger:Path"];
                if (File.Exists(xmlDocPath))
                {
                    options.IncludeXmlComments(xmlDocPath);
                }

                options.CustomSchemaIds(x => x.FullName);
            });
        }

        private static void SeedStuff(Container container)
        {
            // var unitOfWork = app.ApplicationServices.GetService<IUnitOfWork>();
            // var mediator = app.ApplicationServices.GetService<IMediator>();
            // var quizrSettings = app.ApplicationServices.GetService<QuizrSettings>();
            //
            // var loggerFactory = app.ApplicationServices.GetService<ILoggerFactory>();
            using var scope = AsyncScopedLifestyle.BeginScope(container);
            
            var unitOfWork = container.GetInstance<IUnitOfWork>();
            var quizrSettings = container.GetInstance<QuizrSettings>();
            var mediator = container.GetInstance<IMediator>();
            var seeder = container.GetInstance<TestSeeder>();

            var quizCollection = unitOfWork.GetCollection<Quiz>();
            var gameCollection = unitOfWork.GetCollection<Game>();
            var seedQuiz =
                quizCollection.GetAsync(Guid.Parse("DEF9AB47-DF1A-48AE-8946-D20DB7B6127F").ToShortGuidString()).Result;
            if (seedQuiz == null)
            {
                seeder.SeedSeedSet(quizrSettings.BaseUrl);
            }

            var okiKerstQuiz = gameCollection.AnyAsync(q => q.Title == "OKI-kerstquiz 2020").Result;
            if (!okiKerstQuiz)
            {
                seeder.SeedZippedExcelQuiz("uploads/OKI-Kerstquiz-2020.zip", "OKI-Kerstquiz-2020.zip", "OKI2020",
                    "OKI-kerstquiz 2020").Wait();
            }

            var krystkwis = gameCollection.AnyAsync(g => g.Title == "Krystkwis 2020").Result;
            if (!krystkwis)
            {
                seeder.SeedZippedExcelQuiz("uploads/Fryslan-Kerstquiz-2020.zip", "Fryslan-Kerstquiz-2020.zip", "JOEPIE",
                    "Krystkwis 2020").Wait();
            }
        }
    }
}