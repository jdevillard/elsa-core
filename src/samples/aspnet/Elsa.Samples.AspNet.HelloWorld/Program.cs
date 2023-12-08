using Elsa.Extensions;
using Elsa.Samples.AspNet.HelloWorld;
using Microsoft.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

// Add Elsa services.
services.AddElsa(elsa => elsa
    .AddWorkflowsFrom<Program>()
        
    // Enable Elsa HTTP module (for HTTP related activities). 
    .UseHttp()
);
services.AddNotificationHandlersFrom(typeof(HandleLifecycle));
builder.Logging.AddSimpleConsole(option =>
{
    option.TimestampFormat = "HH:mm:ss.fff";
});
builder.AddTelemetry();
// Configure middleware pipeline.
var app = builder.Build();

if (app.Environment.IsDevelopment())
    app.UseDeveloperExceptionPage();

// Add Elsa HTTP middleware (to handle requests mapped to HTTP Endpoint activities)
app.UseWorkflows();

// Start accepting requests.
app.Run();