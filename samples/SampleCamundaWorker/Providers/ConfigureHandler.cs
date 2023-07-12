using Camunda.Worker;
using Camunda.Worker.Endpoints;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace SampleCamundaWorker.Providers
{
    /// <summary>
    /// Helper class for configuring external task handlers dynamically.
    /// </summary>
    public static class ConfigureHandler
    {
        /// <summary>
        /// Configures external task handlers based on the exported types in the assembly.
        /// </summary>
        /// <param name="services">The service collection to configure.</param>
        /// <param name="config">The configuration object.</param>
        public static ICamundaWorkerBuilder Configure(IServiceCollection services, IConfiguration config)
        {
            try
            {
                Assembly assembly = Assembly.GetExecutingAssembly();

                Type handlerType = typeof(IExternalTaskHandler);
                Type endpointMetadataType = typeof(EndpointMetadata);
                MethodInfo addHandlerMethod = typeof(CamundaWorkerBuilderExtensions)
                    .GetMethods()
                    .Where(m => m.Name == "AddHandler" && m.GetParameters().Length == 2 &&
                                m.GetParameters()[1].ParameterType == endpointMetadataType)
                    .Single();

                var camundaWorkerBuilder = services.AddCamundaWorker(config.GetSection("workerId").Value, 1);

                foreach (var type in assembly.GetExportedTypes())
                {
                    if (typeof(IExternalTaskHandler).IsAssignableFrom(type) && type.IsClass && !type.IsAbstract)
                    {
                        var topicName = type.GetCustomAttribute<TopicNameAttribute>()?.TopicName;

                        if (topicName != null)
                        {
                            MethodInfo genericAddHandlerMethod = addHandlerMethod.MakeGenericMethod(type);
                            genericAddHandlerMethod.Invoke(null, new object[] { camundaWorkerBuilder, new EndpointMetadataBuilder(topicName, config).Build() });
                        }
                        else
                        {
                            // Log or handle the case when a handler is missing the TopicNameAttribute
                            throw new ArgumentNullException(nameof(topicName), $"Tanimlanan {type.Name} sinifina ait TopicName Attribute tanimlanmalidir");
                        }
                    }
                }
                return camundaWorkerBuilder;
            }
            catch
            {
                throw;
            }

        }
    }
}




/*
Assembly assembly = Assembly.GetExecutingAssembly();

Type handlerType = typeof(IExternalTaskHandler);
Type endpointMetadataType = typeof(EndpointMetadata);
MethodInfo addHandlerMethod = typeof(CamundaWorkerBuilderExtensions)
    .GetMethods()
    .Where(m => m.Name == "AddHandler" && m.GetParameters().Length ==2  &&
                m.GetParameters()[1].ParameterType == endpointMetadataType)
    .Single();

var camundaWorkerBuilder = services.AddCamundaWorker(Configuration.GetSection("workerId").Value, 1);

foreach (var type in assembly.GetExportedTypes())
{
    if (typeof(IExternalTaskHandler).IsAssignableFrom(type) && type.IsClass && !type.IsAbstract)
    {
        var topicName = (string) type.GetField("TopicName").GetValue(type);
        MethodInfo genericAddHandlerMethod = addHandlerMethod.MakeGenericMethod(type);
        genericAddHandlerMethod.Invoke(null, new object[] { camundaWorkerBuilder, new EndpointMetadataBuilder(topicName, this.Configuration).Build() });
        //genericAddHandlerMethod.Invoke(null, new object[] { camundaWorkerBuilder, new EndpointMetadataBuilder(type.Name, this.Configuration).Build() });
    }
}
*/


/*
services.AddCamundaWorker(Configuration.GetSection("workerId").Value, 1)
    //.AddHandler<SayHelloHandler>()
    //.AddHandler<SayHelloGuestHandler>()
    //.AddHandler<HalkHandler>(ankaTopic)
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
