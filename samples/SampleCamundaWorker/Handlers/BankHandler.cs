using Camunda.Worker;
using SampleCamundaWorker.Providers;
using System.Threading;
using System.Threading.Tasks;

namespace SampleCamundaWorker.Handlers
{
    [TopicName("BankHandler")]
    public class BankHandler : IExternalTaskHandler
    {
        public Task<IExecutionResult> HandleAsync(ExternalTask externalTask, CancellationToken cancellationToken)
        {
            var aa = "asd";
            throw new System.NotImplementedException();
        }
    }
}
