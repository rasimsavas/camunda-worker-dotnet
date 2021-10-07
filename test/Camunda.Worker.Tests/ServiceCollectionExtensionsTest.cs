using Camunda.Worker.Client;
using Camunda.Worker.Execution;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Xunit;

namespace Camunda.Worker
{
    public class ServiceCollectionExtensionsTest
    {
        [Fact]
        public void TestAddCamundaWorker()
        {
            var services = new ServiceCollection();

            services.AddCamundaWorker("testWorker", 100);

            using var provider = services.BuildServiceProvider();

            var fetchAndLockOptions = provider.GetRequiredService<IOptions<FetchAndLockOptions>>().Value;
            Assert.Equal("testWorker", fetchAndLockOptions.WorkerId);

            Assert.Contains(services, d => d.Lifetime == ServiceLifetime.Singleton &&
                                           d.ServiceType == typeof(IEndpointProvider));

            Assert.Contains(services, d => d.Lifetime == ServiceLifetime.Transient &&
                                           d.ServiceType == typeof(ITopicsProvider));

            Assert.Contains(services, d => d.Lifetime == ServiceLifetime.Transient &&
                                           d.ServiceType == typeof(ICamundaWorker));

            Assert.Contains(services, d => d.Lifetime == ServiceLifetime.Singleton &&
                                           d.ServiceType == typeof(WorkerHandlerDescriptor));
        }
    }
}
