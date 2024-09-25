using System;
using Confluent.Kafka;
using Serilog.Events;

namespace Serilog.Sinks.VngCloudLogging
{
    public class VngCloudLoggingSinkOptions
    {
        /// <summary>
        /// Gets or sets the Kafka bootstrap servers, which are used to connect to the Kafka cluster.
        /// </summary>
        /// <example>hcm01-loghub01.vngcloud.vn:10092,hcm02-loghub01.vngcloud.vn:10092</example>
        public string BootstrapServers { get; set; }

        /// <summary>
        /// Gets or sets the default Kafka topic where log messages will be sent.
        /// </summary>
        /// <example>logs_topic</example>
        public string Topic { get; set; }

        /// <summary>
        /// Gets or sets the security protocol used for connecting to Kafka (e.g., SSL).
        /// Default is <see cref="SecurityProtocol.Ssl"/>.
        /// </summary>
        public SecurityProtocol SecurityProtocol { get; set; } = SecurityProtocol.Ssl;

        /// <summary>
        /// Gets or sets the file path to the Certificate Authority (CA) certificate for SSL.
        /// This certificate is used to verify the identity of the Kafka broker.
        /// </summary>
        /// <example>/path/to/ca-cert.pem</example>
        public string SslCaLocation { get; set; }

        /// <summary>
        /// Gets or sets the file path to the client's SSL certificate.
        /// This certificate is used for client authentication to the Kafka broker.
        /// </summary>
        /// <example>/path/to/client-cert.pem</example>
        public string SslCertificateLocation { get; set; }

        /// <summary>
        /// Gets or sets the file path to the client's SSL private key.
        /// This key is used in conjunction with the client's certificate to authenticate to the Kafka broker.
        /// </summary>
        /// <example>/path/to/client-key.pem</example>
        public string SslKeyLocation { get; set; }

        /// <summary>
        /// A function to determine the Kafka topic based on the log event.
        /// If provided, this function allows dynamic topic selection for each log event.
        /// If not provided, the default <see cref="Topic"/> will be used.
        /// </summary>
        /// <example>
        /// (logEvent) => logEvent.Level == LogEventLevel.Error ? "error_logs" : "info_logs"
        /// </example>
        public Func<LogEvent, string> TopicDecider { get; set; }
    }
}