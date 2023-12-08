using Elsa.Mediator.Contracts;
using Elsa.Workflows.Core.Contracts;
using Elsa.Workflows.Core.Notifications;
using System.Diagnostics;

namespace Elsa.Samples.AspNet.HelloWorld
{
    public class HandleLifecycle
     : INotificationHandler<WorkflowExecuting>,
     INotificationHandler<WorkflowExecuted>,

     INotificationHandler<ActivityExecuting>,
     INotificationHandler<ActivityExecuted>
    {
        private readonly ILogger<HandleLifecycle> _logger;

        public HandleLifecycle(ILogger<HandleLifecycle> logger)
        {
            _logger = logger;
        }

        private void PrintVariable(string eventName, IExecutionContext context)
        {
            //Should start from the current and loop in the parent context until no more.
            foreach (var variable in context.Variables)
            {
                _logger.LogInformation($"{eventName}::{variable.Name} -> {variable.Value}");
            }
        }
        public Task HandleAsync(WorkflowExecuting notification, CancellationToken cancellationToken)
        {
            Activity.Current?.AddEvent(new ActivityEvent("WorkflowExecuting"));
            PrintVariable("WorkflowExecuting", notification.WorkflowExecutionContext);
            _logger.LogInformation($"WorkflowExecuting::{Activity.Current?.Id}");
            //notification.WorkflowExecutionContext.Properties
            return Task.CompletedTask;
        }

        public Task HandleAsync(WorkflowExecuted notification, CancellationToken cancellationToken)
        {
            Activity.Current?.AddEvent(new ActivityEvent("WorkflowExecuted"));
            PrintVariable("WorkflowExecuted", notification.WorkflowExecutionContext);
            _logger.LogInformation($"WorkflowExecuted:: {Activity.Current?.Id}");
            return Task.CompletedTask;
        }

        public Task HandleAsync(ActivityExecuting notification, CancellationToken cancellationToken)
        {
            Activity.Current?.AddEvent(new ActivityEvent($"ActivityExecuting {notification.ActivityExecutionContext.Activity.Name}"));
            _logger.LogInformation($"ActivityExecuting::{notification.ActivityExecutionContext.Activity.Name} - Otel: {Activity.Current?.Id}");
            PrintVariable($"ActivityExecuting::{notification.ActivityExecutionContext.Activity.Name}", notification.ActivityExecutionContext);
            //notification.ActivityExecutionContext.GetVariableValues()

            //  .GetVariable("TEstVariable");

            return Task.CompletedTask;
        }

        public Task HandleAsync(ActivityExecuted notification, CancellationToken cancellationToken)
        {
            Activity.Current?.AddEvent(new ActivityEvent($"ActivityExecuted {notification.ActivityExecutionContext.Activity.Name}"));
            _logger.LogInformation($"ActivityExecuted::{notification.ActivityExecutionContext.Activity.Name}");
            PrintVariable($"ActivityExecuted::{notification.ActivityExecutionContext.Activity.Name}", notification.ActivityExecutionContext);
            return Task.CompletedTask;
        }
    }
}
