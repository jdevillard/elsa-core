using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using System.Diagnostics;

namespace Elsa.Samples.AspNet.HelloWorld
{
    public static class Tracer
    {
        public static readonly ActivitySource ElsaActivitySource = new("Elsa Console Workflow");
        public static void  AddTelemetry(this WebApplicationBuilder builder)
        {
            var tracingOtlpEndpoint = builder.Configuration["OTLP_ENDPOINT_URL"];
            var otel = builder.Services.AddOpenTelemetry();

            

            // Configure OpenTelemetry Resources with the application name
            otel.ConfigureResource(resource => resource
                .AddService(serviceName: builder.Environment.ApplicationName));

            // Add Metrics for ASP.NET Core and our custom metrics and export to Prometheus
            otel.WithMetrics(metrics => metrics
                // Metrics provider from OpenTelemetry
                .AddAspNetCoreInstrumentation()
              //  .AddMeter(greeterMeter.Name)
                // Metrics provides by ASP.NET Core in .NET 8
                .AddMeter("Microsoft.AspNetCore.Hosting")
                .AddMeter("Microsoft.AspNetCore.Server.Kestrel")
                //.AddPrometheusExporter()
                
                );

            // Add Tracing for ASP.NET Core and our custom ActivitySource and export to Jaeger
            otel.WithTracing(tracing =>
            {
                tracing.AddAspNetCoreInstrumentation();
                tracing.AddHttpClientInstrumentation();
                tracing.AddSource("Elsa Console Workflow");
              //  tracing.AddSource(greeterActivitySource.Name);
                if (tracingOtlpEndpoint != null)
                {
                    tracing.AddOtlpExporter(otlpOptions =>
                    {
                        otlpOptions.Endpoint = new Uri(tracingOtlpEndpoint);
                    });
                }
                else
                {
                    tracing.AddConsoleExporter();
                }
            });
        }
    }
}
