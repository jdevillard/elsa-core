using Azure.Messaging.ServiceBus;
using Elsa.Contracts;
using Elsa.Helpers;
using Elsa.Modules.AzureServiceBus.Activities;
using Elsa.Modules.AzureServiceBus.Models;
using Elsa.Runtime.Contracts;
using Elsa.Runtime.Models;
using Microsoft.Extensions.Logging;

namespace Elsa.Modules.AzureServiceBus.Services;

/// <summary>
/// Processes messages received via a queue specified through the <see cref="MessageReceivedTriggerPayload"/>.
/// When a message is received, the appropriate workflows are executed.
/// </summary>
public class Worker : IAsyncDisposable
{
    private static readonly string BookmarkName = TypeNameHelper.GenerateTypeName<MessageReceived>();
    private readonly ServiceBusProcessor _processor;
    private readonly IHasher _hasher;
    private readonly IWorkflowServer _workflowServer;
    private readonly ILogger _logger;
    private int _refCount = 1;

    public Worker(string queueOrTopic, string? subscription, ServiceBusClient client, IHasher hasher, IWorkflowServer workflowServer, ILogger<Worker> logger)
    {
        QueueOrTopic = queueOrTopic;
        Subscription = subscription;
        _hasher = hasher;
        _workflowServer = workflowServer;
        _logger = logger;

        var options = new ServiceBusProcessorOptions();
        var processor = subscription == null ? client.CreateProcessor(queueOrTopic, options) : client.CreateProcessor(queueOrTopic, subscription, options);

        processor.ProcessMessageAsync += OnMessageReceivedAsync;
        processor.ProcessErrorAsync += OnErrorAsync;
        _processor = processor;
    }

    public string QueueOrTopic { get; }
    public string? Subscription { get; }

    /// <summary>
    /// Maintains the number of workflows are relying on this worker. When it goes to zero, the worker will be removed.
    /// </summary>
    public int RefCount
    {
        get => _refCount;
        private set
        {
            if (value < 0)
                throw new ArgumentException("RefCount cannot be less than zero");

            _refCount = value;
        }
    }

    public async Task StartAsync(CancellationToken cancellationToken = default) => await _processor.StartProcessingAsync(cancellationToken);
    public void IncrementRefCount() => RefCount++;
    public void DecrementRefCount() => RefCount--;
    public async ValueTask DisposeAsync() => await _processor.DisposeAsync();
    private async Task OnMessageReceivedAsync(ProcessMessageEventArgs args) => await InvokeWorkflowsAsync(args.Message, args.CancellationToken);

    private Task OnErrorAsync(ProcessErrorEventArgs args)
    {
        _logger.LogError(args.Exception, "An error occurred while processing {EnrityPath}", args.EntityPath);
        return Task.CompletedTask;
    }

    private async Task InvokeWorkflowsAsync(ServiceBusReceivedMessage message, CancellationToken cancellationToken)
    {
        var payload = new MessageReceivedTriggerPayload(QueueOrTopic, Subscription);
        var hash = _hasher.Hash(payload);
        var stimulus = Stimulus.Standard(BookmarkName, hash);
        var executionResults = (await _workflowServer.ExecuteStimulusAsync(stimulus, cancellationToken)).ToList();

        _logger.LogInformation("Triggered {WorkflowCount} workflows", executionResults.Count);
    }
}