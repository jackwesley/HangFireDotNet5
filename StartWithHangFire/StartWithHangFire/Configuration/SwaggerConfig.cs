using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using System;

namespace StartWithHangFire.Configuration
{
    public static class SwaggerConfig
    {
        public static void AddSwaggerConfiguration(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc(name: "v1", new OpenApiInfo()
                {
                    Title = "Api Hang Fire",
                    Description = "Esta API tem por objetivo apresentar a biblioteca HangFire de Background Jobs",
                    Contact = new OpenApiContact() { Name = "Jack Wesley Moreira", Email = "jackwesley@hotmail.com" },
                    License = new OpenApiLicense() { Name = "MIT", Url = new Uri(uriString: "https://opensourse.org/licences") }
                });
            });
        }

        public static void UseSwaggerConfiguration(this IApplicationBuilder app)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint(url: "/swagger/v1/swagger.json", name: "v1");
            });
        }
    }
}
