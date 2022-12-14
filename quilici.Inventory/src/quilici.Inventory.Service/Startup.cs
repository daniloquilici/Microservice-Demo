using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Polly;
using Polly.Timeout;
using quilici.Common.MassTransit;
using quilici.Common.MongoDB;
using quilici.Inventory.Service.Clients;
using quilici.Inventory.Service.Entities;
using System;
using System.Net.Http;

namespace quilici.Inventory.Service
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
            services.AddMongo()
                .AddMongoRepository<InventoryItem>("InventoryItems")
                .AddMongoRepository<CatalogItem>("catalogitem")
                .AddMassTransitWithRabbitMQ();

            AddCatalogClient(services);

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "MicroserviceDemo - Inventory", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "quilici.Inventory.Service v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        private static void AddCatalogClient(IServiceCollection services)
        {
            services.AddHttpClient<CatalogClient>(client =>
            {
                client.BaseAddress = new Uri("https://localhost:5001");
            })
                        .AddTransientHttpErrorPolicy(builder => builder.Or<TimeoutRejectedException>().WaitAndRetryAsync(
                            5,
                            retryAttempy => TimeSpan.FromSeconds(Math.Pow(2, retryAttempy)),
                            onRetry: (outcome, timespan, retryAttempt) =>
                            {
                                var serviceProvider = services.BuildServiceProvider();
                                serviceProvider.GetService<ILogger<CatalogClient>>()?.LogWarning($"Delaying for {timespan.TotalSeconds} seconds, then making retry {retryAttempt}.");
                            }
                        ))
                        .AddTransientHttpErrorPolicy(builder => builder.Or<TimeoutRejectedException>().CircuitBreakerAsync(
                            3,
                            TimeSpan.FromSeconds(15),
                            onBreak: (outcome, timespan) =>
                            {
                                var serviceProvider = services.BuildServiceProvider();
                                serviceProvider.GetService<ILogger<CatalogClient>>()?.LogWarning($"Opening the circuit for {timespan.TotalSeconds} seconds...");
                            },
                            onReset: () =>
                            {
                                var serviceProvider = services.BuildServiceProvider();
                                serviceProvider.GetService<ILogger<CatalogClient>>()?.LogWarning($"Closing the circuit...");
                            }
                        ))
                        .AddPolicyHandler(Policy.TimeoutAsync<HttpResponseMessage>(1));
        }
    }
}
