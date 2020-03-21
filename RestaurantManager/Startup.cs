using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using RestaurantManager.Managers;
using RestaurantManager.Middlewares;
using RestaurantManager.Models;

namespace RestaurantManager
{
    public class Startup
    {
        public Startup(
            IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(
            IServiceCollection services)
        {
            var tables = new List<Table>
            {
                new Table(6),
                new Table(2),
                new Table(2),
                new Table(3),
                new Table(4),
                //new Table(5),
                //new Table(5)
            };

            services
                .AddSingleton(
                    new RestManager(tables))
                .AddSwaggerGen(
                    _ =>
                    {
                        _.SwaggerDoc(
                            "v1",
                            new OpenApiInfo
                            {
                                Title = "RestaurantManager",
                                Version = "v1"
                            });
                    })
                .AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(
            IApplicationBuilder app,
            IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
                app
                    .UseDeveloperExceptionPage();

            app
                .UseRouting()
                .UseAuthorization()
                .UseMiddleware<ExceptionMiddleware>()
                .UseSwagger()
                .UseSwaggerUI(
                    _ =>
                    {
                        _.SwaggerEndpoint(
                            "/swagger/v1/swagger.json",
                            "RestaurantManager");

                        _.RoutePrefix = string.Empty;
                    })
                .UseEndpoints(
                    endpoints =>
                    {
                        endpoints.MapControllers();
                    });
        }
    }
}
