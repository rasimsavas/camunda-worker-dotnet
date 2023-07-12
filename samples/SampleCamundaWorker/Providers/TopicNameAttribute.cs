using System;

namespace SampleCamundaWorker.Providers
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class TopicNameAttribute : Attribute
    {
        public string TopicName { get; }

        public TopicNameAttribute(string topicName)
        {
            if (topicName == null)
            {
                throw new ArgumentNullException(topicName.GetType().Name);
            }
            TopicName = topicName;
        }
    }
}
