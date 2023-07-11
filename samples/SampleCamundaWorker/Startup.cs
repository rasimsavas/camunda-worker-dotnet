using System;
using System.IO;
using System.Reflection.Metadata.Ecma335;
using Camunda.Worker;
using Camunda.Worker.Client;
using Camunda.Worker.Endpoints;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SampleCamundaWorker.Handlers;
using SampleCamundaWorker.Providers;

namespace SampleCamundaWorker;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddExternalTaskClient(client =>
        {
            client.BaseAddress = new Uri(Configuration.GetSection("Config").GetSection("ClientBaseAdress").Value);
        });
        
        services.AddCamundaWorker("sampleWorker")
            .AddHandler<SayHelloHandler>(a=> new EndpointMetadata(new string[] {"HalkHandler,BankHandler"},3333))
            .AddHandler<SayHelloGuestHandler>(a => new EndpointMetadata(new string[] { "BankHandler,BankHandler" }, 3333))
            .AddFetchAndLockRequestProvider((a,b) => new CustomFetchAndLockProvider(Configuration))
            .ConfigurePipeline(pipeline =>
            {
                pipeline.Use(next => async context =>
                {
                    var logger = context.ServiceProvider.GetRequiredService<ILogger<Startup>>();
                    logger.LogInformation("Started processing of task {Id}", context.Task.Id);
                    await next(context);
                    logger.LogInformation("Finished processing of task {Id}", context.Task.Id);
                });
            });


        /*
        services.AddCamundaWorker("sampleWorker")
            .AddHandler<SayHelloHandler>()
            .AddHandler<SayHelloGuestHandler>()
            .ConfigurePipeline(pipeline =>
            {
                pipeline.Use(next => async context =>
                {
                    var logger = context.ServiceProvider.GetRequiredService<ILogger<Startup>>();
                    logger.LogInformation("Started processing of task {Id}", context.Task.Id);
                    await next(context);
                    logger.LogInformation("Finished processing of task {Id}", context.Task.Id);
                });
            });

        */
        services.AddHealthChecks();

    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseHealthChecks("/health");
    }
}
