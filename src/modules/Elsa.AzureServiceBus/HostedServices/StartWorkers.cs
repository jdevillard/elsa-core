using Elsa.AzureServiceBus.Activities;
using Elsa.AzureServiceBus.Contracts;
using Elsa.AzureServiceBus.Models;
using Elsa.Extensions;
using Elsa.Workflows.Helpers;
using Elsa.Workflows.Runtime.Contracts;
using Elsa.Workflows.Runtime.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Elsa.AzureServiceBus.HostedServices;

/// <summary>
/// Creates workers for each trigger &amp; bookmark in response to updated workflow trigger indexes and bookmarks.
/// </summary>
public class StartWorkers : IHostedService
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly IWorkerManager _workerManager;

    /// <summary>
    /// Constructor.
    /// </summary>
    public StartWorkers(IServiceScopeFactory serviceScopeFactory, IWorkerManager workerManager)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _workerManager = workerManager;
    }

    /// <inheritdoc />
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var triggerStore = scope.ServiceProvider.GetRequiredService<ITriggerStore>();
        var bookmarkStore = scope.ServiceProvider.GetRequiredService<IBookmarkStore>();

        var activityType = ActivityTypeNameHelper.GenerateTypeName<MessageReceived>();
        var triggerFilter = new TriggerFilter { Name = activityType};
        var triggers = (await triggerStore.FindManyAsync(triggerFilter, cancellationToken)).Select(x => x.GetPayload<MessageReceivedTriggerPayload>()).ToList();
        var bookmarkFilter = new BookmarkFilter { ActivityTypeName = activityType };
        var bookmarks = (await bookmarkStore.FindManyAsync(bookmarkFilter, cancellationToken)).Select(x => x.GetPayload<MessageReceivedTriggerPayload>()).ToList();
        var payloads = triggers.Concat(bookmarks).ToList();

        await EnsureWorkersAsync(payloads, cancellationToken);
    }

    /// <inheritdoc />
    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    private async Task EnsureWorkersAsync(IEnumerable<MessageReceivedTriggerPayload> payloads, CancellationToken cancellationToken)
    {
        foreach (var payload in payloads) await _workerManager.EnsureWorkerAsync(payload.QueueOrTopic, payload.Subscription, cancellationToken);
    }
}