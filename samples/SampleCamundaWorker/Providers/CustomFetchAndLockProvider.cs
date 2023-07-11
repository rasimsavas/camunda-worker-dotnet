using Camunda.Worker.Client;
using Camunda.Worker.Execution;
using Camunda.Worker.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static Camunda.Worker.Client.FetchAndLockRequest;

namespace SampleCamundaWorker.Providers
{
    public class CustomFetchAndLockProvider : IFetchAndLockRequestProvider
    {
        private IConfiguration _configuration;
        private Constants _const;
        private List<Topic> _topics;

        public CustomFetchAndLockProvider(IConfiguration configuration)
        {
            _configuration = configuration;
            _const = _configuration.GetSection("Config").Get<Constants>() ?? new Constants();
            _topics = _configuration.GetSection("Topics").Get<List<Topic>>();
        }
        public FetchAndLockRequest GetRequest()
        {
            
            var request = new FetchAndLockRequest(_const.WorkerId , _const.MaxTasks);
            foreach(var t in _topics)
            {
                t.ProcessDefinitionId = null;
                t.ProcessDefinitionKey = null;
                t.BusinessKey = null;
            }
            request.Topics = _topics;
            request.AsyncResponseTimeout = _const.AsyncResponseTimeout;
            request.UsePriority = _const.UsePriority;
            return request;
        }
        private class Constants
        {
            public int MaxTasks { get; set; }
            public bool UsePriority { get; set; }
            public int AsyncResponseTimeout { get; set; }
            public string WorkerId { get; set; }
        }
        public class ClientOptions
        {
            public int LockDuration { get; set; }
            public bool LocalVariables { get; set; }
            public bool DeserializeValues { get; set; }
            public bool IncludeExtensionProperties { get; set; }
            public List<string> Variables { get; set; }
            public List<string> ProcessDefinitionIds { get; set; }
            public List<string> ProcessDefinitionKeys { get; set; }
            public Dictionary<string, string> ProcessVariables { get; set; }
            public List<string> TenantIds { get; set; }
            public string BusinessKey { get; set; }
            public string ProcessDefinitionId { get; set; }
            public string ProcessDefinitionKey { get; set; }
            public bool WithoutTenantId { get; set; }
        }

    }
}
