using System;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenTelemetry;
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace Palprimes.Common.Logging
{
    public static class OpenTelemetryExtensions
    {

        public static IServiceCollection AddOpenTelemetry(this IServiceCollection services, string appName, ILoggingBuilder loggingBuilder, IConfiguration configuration)
        {
            var observabilitySettings = configuration.GetSection("Observability");

            var tracingExporter = observabilitySettings.GetValue<string>("TracingExporter");
            var metricsExporter = observabilitySettings.GetValue<string>("MetricsExporter");
            var logExporter = observabilitySettings.GetValue<string>("LogExporter");
            var otlpSettings = configuration.GetSection("Otlp");

            var assemblyVersion = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "unknown";
            var resourceBuilder = ResourceBuilder.CreateDefault().AddService(
                appName,
                serviceVersion: assemblyVersion,
                serviceInstanceId: Environment.MachineName);

            return services
                .AddTracing(tracingExporter, otlpSettings, resourceBuilder)
                .AddMetrics(metricsExporter, otlpSettings, resourceBuilder)
                .AddLogging(logExporter, otlpSettings, loggingBuilder, resourceBuilder)
                .Configure((Action<OpenTelemetryLoggerOptions>)(opt =>
                {
                    opt.IncludeScopes = true;
                    opt.ParseStateValues = true;
                    opt.IncludeFormattedMessage = true;
                }));

        }

        private static IServiceCollection AddMetrics(this IServiceCollection services, string metricsExporter, IConfigurationSection otlpSettings, ResourceBuilder resourceBuilder)
        {
            return services.AddOpenTelemetryMetrics(options =>
            {

                options.SetResourceBuilder(resourceBuilder)
                    .AddHttpClientInstrumentation()
                    .AddAspNetCoreInstrumentation();

                switch (metricsExporter)
                {
                    case "otlp":
                        options.AddOtlpExporter(otlpOptions =>
                        {
                            otlpOptions.Endpoint = new Uri(otlpSettings.GetValue<string>("Endpoint"));
                        });
                        break;
                    default:
                        options.AddConsoleExporter();
                        break;
                }
            });
        }

        private static IServiceCollection AddLogging(this IServiceCollection services, string logExporter, IConfigurationSection otlpSettings, ILoggingBuilder loggingBuilder, ResourceBuilder resourceBuilder)
        {
            loggingBuilder.ClearProviders();

            loggingBuilder.AddOpenTelemetry(options =>
            {
                options.SetResourceBuilder(resourceBuilder);
                switch (logExporter)
                {
                    case "otlp":
                        options.AddOtlpExporter(otlpOptions =>
                        {
                            otlpOptions.Endpoint = new Uri(otlpSettings.GetValue<string>("Endpoint"));
                        });
                        break;
                    default:
                        options.AddConsoleExporter();
                        break;
                }
            });

            return services;
        }

        private static IServiceCollection AddTracing(this IServiceCollection services, string tracingExporter, IConfigurationSection otlpSettings, ResourceBuilder resourceBuilder)
        {
            return services.AddOpenTelemetryTracing(options =>
            {
                options
                    .SetResourceBuilder(resourceBuilder)
                    .SetSampler(new AlwaysOnSampler())
                    .AddHttpClientInstrumentation()
                    .AddAspNetCoreInstrumentation();

                switch (tracingExporter)
                {

                    case "otlp":
                        options.AddZipkinExporter();
                        services.Configure<ZipkinExporterOptions>(otlpSettings);
                        break;

                    default:
                        options.AddConsoleExporter();

                        break;
                }
            });
        }
    }
}