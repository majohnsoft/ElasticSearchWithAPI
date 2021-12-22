using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Nest;
using Smart.Business.Implementation;
using Smart.Business.Interface;
using Smart.Business.utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Smart.API
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
            services.AddTransient<IManagementServices, ManagementServices>();
            services.AddTransient<IPropertyServices, PropertyServices>(); 
            services.AddTransient<IMarketRegionService, MarketRegionService>();
            services.AddSingleton(x => new ConnectionSettings(new Uri(Configuration.GetConnectionString("AWSURL")))
                            .BasicAuthentication("admin", "Lkjhgf#$321"));

            var corsURL = Configuration.GetValue<string>("AppSettings:AllowedCrossOrigins");
            services.AddCors(options =>
            {
                options.AddPolicy(name: "AllowOrigin",
                    builder =>
                    {
                        builder.WithOrigins(corsURL)
                                            .AllowAnyHeader()
                                            .AllowAnyMethod();
                    });
            });
            services.AddMvc(
                config =>
                {
                    config.Filters.Add(typeof(CustomExceptionHandler));
                }
            );

            services.AddSwaggerGen(swagger =>
            {
                swagger.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "Smart Apartment Data API",
                    Description = "Smart Apartment Data API"
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Smart Apartment Data API"));
                //app.UseExceptionHandler(logger);
            }

            app.UseHttpsRedirection();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("../swagger/v1/swagger.json", "Smart Apartment Data API");

            });

            app.UseRouting();

            app.UseAuthorization();

            app.UseCors(builder =>
            {
                builder
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader();
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
