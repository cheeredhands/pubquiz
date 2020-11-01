using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Pubquiz.Domain.Models;
using Pubquiz.Logic.Hubs;
using Pubquiz.Logic.Messages;
using Pubquiz.Logic.Tools;
using Pubquiz.Persistence;
using Pubquiz.Persistence.Extensions;
using Pubquiz.Persistence.Helpers;
using Pubquiz.WebApi.Helpers;
using Pubquiz.WebApi.Models;
using Rebus.Persistence.InMem;
using Rebus.Routing.TypeBased;
using Rebus.ServiceProvider;
using Rebus.Transport.InMem;

namespace Pubquiz.WebApi
{
    public class Startup
    {
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly IConfiguration _configuration;
        private readonly string _secretKey;
        private readonly SymmetricSecurityKey _signingKey;

        public Startup(IConfiguration configuration, IWebHostEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
            _configuration = configuration;
            _secretKey = _configuration.GetValue<string>("AppSettings:JwtSecret");
            _signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_secretKey));
        }


        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            AddDefaultWebApiStuff(services);
            AddQuizrSpecificStuff(services);
            AddSwagger(services);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseRouting();
            //app.UseCors();
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

            app.ApplicationServices.UseRebus(bus => bus.SubscribeByScanningForHandlers(Assembly.Load("Pubquiz.Logic")));

            app.UseSwagger();
            app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "Pubquiz backend V1"); });

            SeedStuff(app, env);
        }

        private void AddDefaultWebApiStuff(IServiceCollection services)
        {
            // controllers, authentication, authorization and such


            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            services.AddControllers()
                .AddMvcOptions(options =>
                {
                    options.Filters.Add(typeof(DomainExceptionFilter));
                    options.Filters.Add(typeof(UnitOfWorkActionFilter));
                })
                .AddNewtonsoftJson()
                .SetCompatibilityVersion(CompatibilityVersion.Version_3_0);
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    //options.RequireHttpsMetadata = false;
                    //options.SaveToken = true;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = _signingKey,
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
            services.AddSingleton<IConfigureOptions<MvcNewtonsoftJsonOptions>, JsonOptionsSetup>();
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
            services.AutoRegisterHandlersFromAssembly("Pubquiz.Logic");
            // needed so the in memory subscription store will be centralized
            var inMemorySubscriberStore = new InMemorySubscriberStore();
            services.AddSingleton(inMemorySubscriberStore);
            services.AddRebus(configure =>
                configure //.Logging(l => l.Use(new MsLoggerFactoryAdapter(_loggerFactory)))
                    .Transport(t => t.UseInMemoryTransport(new InMemNetwork(true), "Messages"))
                    .Subscriptions(s => s.StoreInMemory(inMemorySubscriberStore))
                    .Routing(r => r.TypeBased().MapAssemblyOf<InteractionResponseAdded>("Messages")));

            switch (_configuration.GetValue<string>("AppSettings:Database"))
            {
                case "Memory":
                    services.AddInMemoryPersistence();
                    break;
                case "MongoDB":
                    services.AddMongoDbPersistence("Quizr", _configuration.GetConnectionString("MongoDB"));
                    break;
            }

            services.AddRequests(Assembly.Load("Pubquiz.Logic"));
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
                var commentsFileName = Assembly.GetEntryAssembly().GetName().Name + ".XML";
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

        private void SeedStuff(IApplicationBuilder app, IWebHostEnvironment env)
        {
            var unitOfWork = app.ApplicationServices.GetService<IUnitOfWork>();
            var mongoDbIsEmpty = _configuration.GetValue<string>("AppSettings:Database") == "MongoDB" &&
                                 unitOfWork.GetCollection<Team>().GetCountAsync().Result == 0;
            // Seed the test data when using in-memory-database
            if (mongoDbIsEmpty || _configuration.GetValue<string>("AppSettings:Database") == "Memory")
            {
                //var unitOfWork = app.ApplicationServices.GetService<IUnitOfWork>();
                var loggerFactory = app.ApplicationServices.GetService<ILoggerFactory>();
                var seeder = new TestSeeder(unitOfWork, loggerFactory);
                if (env.IsDevelopment())
                {
                    seeder.SeedSeedSet("https://localhost:5001");    
                }
                else
                {
                    seeder.SeedSeedSet("https://decemberist.nl");
                }
                
            }
        }
    }
}