using Camunda.Worker;
using Camunda.Worker.Client;
using Camunda.Worker.Endpoints;
using Camunda.Worker.Execution;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SampleCamundaWorker.Handlers;
using SampleCamundaWorker.Providers;
using System;
using System.Collections.Generic;

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
        services.Configure<GlobalOptions>(Configuration.GetSection("GlobalOptions"));
        services.AddExternalTaskClient(client =>
        {
            client.BaseAddress = new Uri(Configuration.GetConnectionString("rest"));
        });


        ICamundaWorkerBuilder builder = ConfigureHandler.Configure(services, Configuration);

        builder.AddFetchAndLockRequestProvider((workerId, provider) => new CustomFetchAndLockProvider(
            provider.GetRequiredService<IConfiguration>(),
            provider.GetRequiredService<IOptionsMonitor<GlobalOptions>>()
            )
        );
        builder.ConfigurePipeline(pipeline =>
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
            //.AddHandler<SayHelloHandler>()
            //.AddHandler<SayHelloGuestHandler>()
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
