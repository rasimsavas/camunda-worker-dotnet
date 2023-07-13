using Camunda.Worker;
using Camunda.Worker.Client;
using Camunda.Worker.Endpoints;
using Camunda.Worker.Execution;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
namespace SampleCamundaWorker.Providers
{
    public sealed class CustomFetchAndLockProvider : IFetchAndLockRequestProvider
    {
        private readonly GlobalOptions _options;
        private readonly IConfiguration _config;

        public CustomFetchAndLockProvider(
            IConfiguration config,
            IOptionsMonitor<GlobalOptions> gOptions
            )
        {
            _config = config;
            _options = gOptions.CurrentValue;
        }

        public FetchAndLockRequest GetRequest()
        {
            var topics1 = _config.GetSection("AllTopics").Get<FetchAndLockRequest.Topic[]>(); //.Get<List<FetchAndLockRequest.Topic[]>>().First();

            var fetchAndLockRequest = new FetchAndLockRequest(_options.WorkerId, _options.MaxTasks)
            {
                UsePriority = _options.UsePriority,
                AsyncResponseTimeout = _options.AsyncResponseTimeout,
                Topics = GetTopics(topics1)
            };

            return fetchAndLockRequest;
        }

        private static FetchAndLockRequest.Topic[] GetTopics(FetchAndLockRequest.Topic[] topics)
        {
            try
            {
                var result = topics.Where(x => x.ProcessDefinitionKey == "" || x.ProcessDefinitionId == "" || x.BusinessKey == "").ToArray();
                if (!result.Any())
                    return topics;

                else
                {
                    foreach (var topic in topics)
                    {
                        topic.ProcessDefinitionId = null;
                        topic.BusinessKey = null;
                        topic.ProcessDefinitionKey = null;
                    }
                }

                return topics;
            }
            catch(Exception e)
            {
                throw;
            }

        }
    }
}
