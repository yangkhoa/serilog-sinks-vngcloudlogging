using System;
using Confluent.Kafka;
using Serilog.Configuration;
using Serilog.Events;
using Serilog.Formatting;
using Serilog.Sinks.PeriodicBatching;

namespace Serilog.Sinks.VngCloudLogging
{
    public static class LoggerConfigurationExtensions
    {
        /// <summary>
        /// Writes log events to VNG Cloud Logging.
        /// </summary>
        /// <param name="loggerConfiguration">Logger sink configuration.</param>
        /// <param name="sinkOptions">VNG Cloud Logging sink options.</param>
        /// <param name="batchSizeLimit">The maximum number of events to include in a single batch. The default is 100.</param>
        /// <param name="period">The time to wait between checking for event batches. The default is five seconds.</param>
        /// <returns>Configuration object allowing method chaining.</returns>
        public static LoggerConfiguration VngCloudLogging(
            this LoggerSinkConfiguration loggerConfiguration,
            VngCloudLoggingSinkOptions sinkOptions,
            int batchSizeLimit = 50,
            int period = 5,
            ITextFormatter formatter = null)
        {
            return loggerConfiguration.VngCloudLogging(
                sinkOptions.BootstrapServers,
                batchSizeLimit,
                period,
                sinkOptions.SecurityProtocol,
                sinkOptions.SslCaLocation,
                sinkOptions.SslCertificateLocation,
                sinkOptions.SslKeyLocation,
                sinkOptions.Topic,
                topicDecider: null,
                formatter);
        }

        private static LoggerConfiguration VngCloudLogging(
            this LoggerSinkConfiguration loggerConfiguration,
            string bootstrapServers = null,
            int batchSizeLimit = 50,
            int period = 5,
            SecurityProtocol securityProtocol = SecurityProtocol.Plaintext,
            string sslCaLocation = null,
            string slCertificateLocation = null,
            string sslKeyLocation = null,
            string topic = null,
            Func<LogEvent, string> topicDecider = null,
            ITextFormatter formatter = null)
        {
            var vnCloudLoggingSinkOptions = new VngCloudLoggingSinkOptions
            {
                BootstrapServers = bootstrapServers,
                SecurityProtocol = securityProtocol,
                SslCaLocation = sslCaLocation,
                SslCertificateLocation = slCertificateLocation,
                SslKeyLocation = sslKeyLocation,
                Topic = topic,
                TopicDecider = topicDecider
            };

            var batchedSink = new VngCloudLoggingSink(
                vnCloudLoggingSinkOptions,
                formatter);

            var batchingOptions = new PeriodicBatchingSinkOptions
            {
                BatchSizeLimit = batchSizeLimit,
                Period = TimeSpan.FromSeconds(period)
            };

            var batchingSink = new PeriodicBatchingSink(
                batchedSink,
                batchingOptions);

            return loggerConfiguration
                .Sink(batchingSink);
        }
    }
}