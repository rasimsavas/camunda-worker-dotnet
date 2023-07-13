using Camunda.Worker.Client;
using Camunda.Worker.Endpoints;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;

namespace SampleCamundaWorker.Providers;
/// <summary>
/// Builder class for creating an instance of <see cref="EndpointMetadata"/>.
/// </summary>
public class EndpointMetadataBuilder
{
    private readonly IConfiguration _config;
    private readonly List<string> _topicNames = new List<string>();
    private int _lockDuration;
    private bool _localVariables;
    private bool _deserializeValues;
    private bool _includeExtensionProperties;
    private IReadOnlyCollection<string> _variables;
    private IReadOnlyCollection<string> _processDefinitionIds;
    private IReadOnlyCollection<string> _processDefinitionKeys;
    private IReadOnlyDictionary<string, string>? _processVariables;
    private IReadOnlyCollection<string> _tenantIds;
    private string _businessKey;
    private string _processDefinitionId;
    private string _processDefinitionKey;
    //private string _processDefinitionVersionTag;
    private bool _withoutTenantId;
    private readonly string _topics = "AllTopics";

    /// <summary>
    /// Initializes a new instance of the <see cref="EndpointMetadataBuilder"/> class.
    /// </summary>
    /// <param name="topicName">The name of the topic.</param>
    /// <param name="config">The configuration object.</param>
    public EndpointMetadataBuilder(string topicName, IConfiguration config)
    {
        _config = config;
        Initialize(topicName);
    }

    private void Initialize(string topicName)
    {

        var topic = _config.GetSection(_topics).Get<FetchAndLockRequest.Topic[]>().Where(topic => topic.TopicName == topicName).First();

        _topicNames.Add(topicName);



        _lockDuration = topic.LockDuration; //_config.GetSection($"{_topics}:{topicName}:LockDuration").Get<int>();
        _localVariables = topic.LocalVariables; //_config.GetSection($"{_topics}:{topicName}:LocalVariables").Get<bool>();
        _deserializeValues = topic.DeserializeValues; //_config.GetSection($"{_topics}:{topicName}:DeserializeValues").Get<bool>();
        _includeExtensionProperties = topic.IncludeExtensionProperties; //_config.GetSection($"{_topics}:{topicName}:IncludeExtensionProperties").Get<bool>();
        _variables = topic.Variables; //_config.GetSection($"{_topics}:{topicName}:Variables").Get<List<string>>();
        _processDefinitionKeys = topic.ProcessDefinitionKeyIn; //_config.GetSection($"{_topics}:{topicName}:ProcessDefinitionKeys").Get<List<string>>();
        _processDefinitionIds = topic.ProcessDefinitionIdIn; // _config.GetSection($"{_topics}:{topicName}:ProcessDefinitionIds").Get<List<string>>();
        _processVariables = topic.ProcessVariables; // _config.GetSection($"{_topics}:{topicName}:ProcessVariables").Get<Dictionary<string, string>>();
        _tenantIds = topic.TenantIdIn; // _config.GetSection($"{_topics}:{topicName}:TenantIds").Get<List<string>>();
        _businessKey = topic.BusinessKey; // _config.GetSection($"{_topics}:{topicName}:BusinessKey").Get<string>();
        _processDefinitionId = topic.ProcessDefinitionId; // _config.GetSection($"{_topics}:{topicName}:ProcessDefinitionId").Get<string>();
        _processDefinitionKey = topic.ProcessDefinitionKey; // _config.GetSection($"{_topics}:{topicName}:ProcessDefinitionKey").Get<string>();
        //_processDefinitionVersionTag = _config.GetSection($"{_topics}:{topicName}:ProcessDefinitionVersionTag").Get<string>();
        _withoutTenantId = (bool)topic.WithoutTenantId; // _config.GetSection($"{_topics}:{topicName}:WithoutTenantId").Get<bool>();
    }

    /// <summary>
    /// Builds an instance of <see cref="EndpointMetadata"/>.
    /// </summary>
    /// <returns>The built <see cref="EndpointMetadata"/> instance.</returns>
    public EndpointMetadata Build()
    {
        return new EndpointMetadata(_topicNames, _lockDuration)
        {
            LocalVariables = _localVariables,
            DeserializeValues = _deserializeValues,
            IncludeExtensionProperties = _includeExtensionProperties,
            Variables = (IReadOnlyList<string>)_variables,
            ProcessDefinitionIds = (IReadOnlyList<string>)_processDefinitionIds,
            ProcessDefinitionKeys = (IReadOnlyList<string>)_processDefinitionKeys,
            ProcessVariables = _processVariables,
            TenantIds = (IReadOnlyList<string>)_tenantIds,
            /*
            BusinessKey = string.IsNullOrEmpty(_businessKey) ? null : _businessKey,
            ProcessDefinitionId = string.IsNullOrEmpty(_processDefinitionId) ? null : _processDefinitionId,
            ProcessDefinitionKey = string.IsNullOrEmpty(_processDefinitionKey) ? null : _processDefinitionKey,
            ProcessDefinitionVersionTag = string.IsNullOrEmpty(_processDefinitionVersionTag) ? null : _processDefinitionVersionTag,
            WithoutTenantId = _withoutTenantId
            */
        };
    }
}
