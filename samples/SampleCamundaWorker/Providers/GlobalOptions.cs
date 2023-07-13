namespace SampleCamundaWorker.Providers
{
    public class GlobalOptions
    {
        public int AsyncResponseTimeout { get; set; }
        public string ClientBaseAdress { get; set; }
        public int MaxTasks { get; set; }
        public bool UsePriority { get; set; }
        public string WorkerId { get; set; }
        public string GlobalBpmnError { get; set; }
    }
}
