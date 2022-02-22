using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Elsa.Persistence.Entities;

namespace Elsa.Modules.Scheduling.Contracts;

/// <summary>
/// Schedules jobs for the specified list of workflow triggers.
/// </summary>
public interface IWorkflowTriggerScheduler
{
    Task ScheduleTriggersAsync(IEnumerable<WorkflowTrigger> triggers, CancellationToken cancellationToken = default);
    Task UnscheduleTriggersAsync(IEnumerable<WorkflowTrigger> triggers, CancellationToken cancellationToken = default);
}