using Camunda.Worker;
using Camunda.Worker.Client;
using Camunda.Worker.Variables;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using SampleCamundaWorker.Providers;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;

namespace SampleCamundaWorker.Handlers
{
    /// <summary>
    /// Sample External Task Client
    /// Custom handler class (for instance) <see cref="IExternalTaskHandler"/see> interface.
    /// </summary>
    [TopicName("HalkHandler")]
    public class HalkHandler : IExternalTaskHandler
    {
        private IConfiguration _config { get; set; }
        private readonly IExternalTaskClient _client;
        private string _globalBpmnError = "BPMN_ERROR";
        private string _workerId = "GlobalOptions:WorkerId";
        /// <summary>
        /// itializes a new instance of the <see cref="HalkHandler"/> class.
        /// </summary>
        /// <param name="config">The configuration object.</param>
        /// <param name="client">The external task client.</param>
        public HalkHandler(
            IConfiguration config,
            IExternalTaskClient client
        )
        {
            _config = config;
            _client = client;
            SetFileds();
        }

        /// <summary>
        /// SetFiledles the processing of the external task.
        /// </summary>
        /// <param name="externalTask">The external task to handle.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The execution result of the handler.</returns>
        public async Task<IExecutionResult> HandleAsync(ExternalTask externalTask, CancellationToken cancellationToken)
        {
            try
            {
                var response = await CallExternalApiAsync();

                if (response.Response.IsSuccessStatusCode)
                {
                    var processVariables = CreateProcessVariables(response); /// API yanıtına göre işlem değişkenlerini oluşturur.

                    return new CompleteResult { Variables = processVariables };
                }
                else if (!response.Response.IsSuccessStatusCode)
                {
                    return new BpmnErrorResult(_globalBpmnError, response.Response.ReasonPhrase, new Dictionary<string, VariableBase>
                    {
                        ["response.field"] = new StringVariable("Error Variable")
                    });
                }
                else
                {
                    await _client.ExtendLockAsync(externalTask.Id, new ExtendLockRequest(_workerId, 3000));
                    return new NoneResult();
                }
            }
            catch (Exception e)
            {
                LogException(e); /// Hata durumunda istisnayı kaydeder.

                return new FailureResult("Error Message", "Error Detail") /// IExecutionResult response for | POST /external-task/{id}/failure | 
                {
                    Retries = 1,
                    RetryTimeout = 60000
                };
            }
        }

        private async Task<ApiResponse> CallExternalApiAsync()
        {
            try
            {
                /// Burada gerçek bir API çağrısı yapılabilir, örnek olarak HttpClient kullanıldı.
                using (var httpClient = new HttpClient())
                {
                    var response = await httpClient.GetAsync("https://jsonplaceholder.typicode.com/users");
                    var responseBody = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<List<ApiResponse>>(responseBody);
                    result[0].Response = response;

                    return result[0];
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

        }

        private Dictionary<string, VariableBase> CreateProcessVariables(ApiResponse response)
        {
            var processVariables = new Dictionary<string, VariableBase>();
            // Yanıttan gelen verilere göre işlem değişkenlerini oluşturma işlemi.
            processVariables.Add("user", new StringVariable(JsonConvert.SerializeObject(response)));

            return processVariables;
        }

        private void SetFileds()
        {
            _globalBpmnError = _config.GetSection(_globalBpmnError).Value;  /// 'application.json'.
            _workerId = _config.GetSection(_workerId).Value;    /// etc
        }

        private static void LogException(Exception e)
        {
            // Hata durumunda istisnayı kaydetmek için bir loglama işlemi yapılabilir.
            Console.WriteLine($"An exception occurred: {e.Message}");
        }

        // Gerçek bir API yanıtını temsil eden sınıf.
        public class ApiResponse
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Username { get; set; }
            public string Email { get; set; }
            public Address Address { get; set; }
            public string Phone { get; set; }
            public string Website { get; set; }
            public Company Company { get; set; }
            public HttpResponseMessage Response { get; set; }
        }

        public class Address
        {
            public string Street { get; set; }
            public string Suite { get; set; }
            public string City { get; set; }
            public string Zipcode { get; set; }
            public Geo Geo { get; set; }
        }

        public class Geo
        {
            public string Lat { get; set; }
            public string Lng { get; set; }
        }

        public class Company
        {
            public string Name { get; set; }
            public string CatchPhrase { get; set; }
            public string Bs { get; set; }
        }

    }
}
/*
return new CompleteResult
{
    Variables = new Dictionary<string, VariableBase>
    {
        ["MESSAGE"] = new StringVariable($"Hello, rasim savas"),
        ["USER_INFO"] = JsonVariable.Create(new UserInfo("rasim", new List<string>
{
    "Admin"
}))
    }
};
*/
