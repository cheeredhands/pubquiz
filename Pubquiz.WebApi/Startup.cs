using System.IO;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Pubquiz.Repository.Extensions;
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
            services.AddInMemoryRepository();
            services.AddRequests(Assembly.Load("Pubquiz.Domain"));
            services.AddMvcCore(options => { options.Filters.Add(typeof(DomainExceptionFilter)); })
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
                .AddApiExplorer()
                .AddJsonFormatters()
                .AddCacheTagHelper();
            services.AddSingleton<IConfigureOptions<MvcJsonOptions>, JsonOptionsSetup>();

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
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Pubquiz backend V1");
            });
        }
    }
}