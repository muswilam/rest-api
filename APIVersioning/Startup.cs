using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace APIVersioning
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "APIVersioning", Version = "v1" });
            });

            // updating the middleware to use versioning.
            services.AddApiVersioning(options =>
            {
                // -incase that not specified the api version in url,
                // -it will automalically adding it to the url.
                options.AssumeDefaultVersionWhenUnspecified = true;

                // -the default api version applied to services that don't have explicit versions.
                options.DefaultApiVersion = ApiVersion.Default;

                // -return all available api versions.
                options.ReportApiVersions = true;

                // -add Media Type versioning.
                // -The API version reader is used to read the service API version specified by a
                // -client. The default value is the Microsoft.AspNetCore.Mvc.Versioning.QueryStringApiVersionReader,
                // -which only reads the service API version from the "api-version" query string
                // -parameter. Replace the default value with an alternate implementation, such as
                // -the Microsoft.AspNetCore.Mvc.Versioning.HeaderApiVersionReader, which can read
                // -the service API version from additional information like HTTP headers.
                // options.ApiVersionReader = new MediaTypeApiVersionReader("x-api-version");

                // -add custom header for specifying api version.
                //options.ApiVersionReader = new HeaderApiVersionReader("x-api-version");

                // -unioned API version readers.
                // -combine multiple readers like: Media type header reader and custom header reader.
                options.ApiVersionReader = ApiVersionReader.Combine(
                    new HeaderApiVersionReader("api-version"),
                    new MediaTypeApiVersionReader("api-version")
                );
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "APIVersioning v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
