using Camunda.Worker.Endpoints;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
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
    private List<string> _variables;
    private List<string> _processDefinitionIds;
    private List<string> _processDefinitionKeys;
    private Dictionary<string, string>? _processVariables;
    private List<string> _tenantIds;
    private string? _businessKey;
    private string? _processDefinitionId;
    private string? _processDefinitionKey;
    private string? _processDefinitionVersionTag;
    private bool _withoutTenantId;
    private readonly string _topics = "topics";

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
        _topicNames.Add(topicName);
        _lockDuration = _config.GetSection($"{_topics}:{topicName}:LockDuration").Get<int>();
        _localVariables = _config.GetSection($"{_topics}:{topicName}:LocalVariables").Get<bool>();
        _deserializeValues = _config.GetSection($"{_topics}:{topicName}:DeserializeValues").Get<bool>();
        _includeExtensionProperties = _config.GetSection($"{_topics}:{topicName}:IncludeExtensionProperties").Get<bool>();
        _variables = _config.GetSection($"{_topics}:{topicName}:Variables").Get<List<string>>();
        _processDefinitionKeys = _config.GetSection($"{_topics}:{topicName}:ProcessDefinitionKeys").Get<List<string>>();
        _processDefinitionIds = _config.GetSection($"{_topics}:{topicName}:ProcessDefinitionIds").Get<List<string>>();
        _processVariables = _config.GetSection($"{_topics}:{topicName}:ProcessVariables").Get<Dictionary<string, string>>();
        _tenantIds = _config.GetSection($"{_topics}:{topicName}:TenantIds").Get<List<string>>();
        _businessKey = _config.GetSection($"{_topics}:{topicName}:BusinessKey").Get<string>();
        _processDefinitionId = _config.GetSection($"{_topics}:{topicName}:ProcessDefinitionId").Get<string>();
        _processDefinitionKey = _config.GetSection($"{_topics}:{topicName}:ProcessDefinitionKey").Get<string>();
        _processDefinitionVersionTag = _config.GetSection($"{_topics}:{topicName}:ProcessDefinitionVersionTag").Get<string>();
        _withoutTenantId = _config.GetSection($"{_topics}:{topicName}:WithoutTenantId").Get<bool>();
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
            Variables = _variables,
            ProcessDefinitionIds = _processDefinitionIds,
            ProcessDefinitionKeys = _processDefinitionKeys,
            ProcessVariables = _processVariables,
            TenantIds = _tenantIds,
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
