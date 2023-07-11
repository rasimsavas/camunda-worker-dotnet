using Camunda.Worker.Client;
using Camunda.Worker.Execution;
using Microsoft.Extensions.Configuration;

namespace SampleCamundaWorker.Providers
{
    public class CustomFetchAndLockProvider : IFetchAndLockRequestProvider
    {
        private IConfiguration _configuration;
        public CustomFetchAndLockProvider(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public FetchAndLockRequest GetRequest()
        {
            //_configuration.GetSection();
            return new FetchAndLockRequest("asd",10);
        }

    }
}
