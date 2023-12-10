using Amazon.S3;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Logging.Console;
using NSwag;
using shr.API.Services;

namespace shr.API
{
    public class Startup
    {
        public IConfigurationRoot Configuration { get; }

        public Startup(IWebHostEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers().ConfigureApiBehaviorOptions(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });

            services.AddSingleton<IShareService>(new ShareService(Configuration["Share:Bucket"], new AmazonS3Config
            {
                ServiceURL = "https://s3.yandexcloud.net/",
            }));

            services.AddSingleton<IMimeMappingService>(new MimeMappingService(new FileExtensionContentTypeProvider()));

            services.AddOpenApiDocument(options =>
            {
                options.Title = "shr.API";
                options.Version = Configuration["APIVersion"];
            });

            services.AddRouting(options => options.LowercaseUrls = true);
        }

        public void Configure(WebApplication app, IWebHostEnvironment env)
        {
            app.UseExceptionHandler("/error");
            app.MapControllers();
            app.UseOpenApi(options =>
            {
                options.PostProcess = (document, httpRequest) =>
                {
                    document.Servers.Clear();
                    document.Servers.Add(new OpenApiServer { Url = Configuration["URL"] });
                };
                options.Path = "/api/openapi.yaml";
            });
            app.Urls.Add($"http://0.0.0.0:{Configuration["PORT"]}");
        }
    }
}
