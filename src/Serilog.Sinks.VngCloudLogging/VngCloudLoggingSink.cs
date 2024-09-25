using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Confluent.Kafka;
using Serilog.Events;
using Serilog.Formatting;
using Serilog.Sinks.PeriodicBatching;

namespace Serilog.Sinks.VngCloudLogging
{
    public class VngCloudLoggingSink : IBatchedLogEventSink
    {
        private const int FlushTimeoutSecs = 10;

        private readonly TopicPartition _globalTopicPartition;
        private readonly ITextFormatter _formatter;
        private readonly Func<LogEvent, string> _topicDecider;
        private readonly VngCloudLoggingSinkOptions _sinkOptions;
        private IProducer<Null, byte[]> _producer;

        public VngCloudLoggingSink(VngCloudLoggingSinkOptions sinkOptions, ITextFormatter formatter = null)
        {
            _sinkOptions = sinkOptions ?? throw new ArgumentNullException(nameof(sinkOptions));

            ConfigureKafkaConnection(
                _sinkOptions.BootstrapServers,
                _sinkOptions.SecurityProtocol,
                _sinkOptions.SslCaLocation,
                _sinkOptions.SslCertificateLocation,
                _sinkOptions.SslKeyLocation);

            _formatter = formatter ?? new Formatting.Json.JsonFormatter(renderMessage: true);

            if (_sinkOptions.Topic != null)
                _globalTopicPartition = new TopicPartition(_sinkOptions.Topic, Partition.Any);

            if (_sinkOptions.TopicDecider != null)
                _topicDecider = _sinkOptions.TopicDecider;
        }

        public Task OnEmptyBatchAsync() => Task.CompletedTask;

        public Task EmitBatchAsync(IEnumerable<LogEvent> batch)
        {
            foreach (var logEvent in batch)
            {
                Message<Null, byte[]> message;

                var topicPartition = _topicDecider == null
                    ? _globalTopicPartition
                    : new TopicPartition(_topicDecider(logEvent), Partition.Any);

                using (var render = new StringWriter(CultureInfo.InvariantCulture))
                {
                    _formatter.Format(logEvent, render);

                    message = new Message<Null, byte[]>
                    {
                        Value = Encoding.UTF8.GetBytes(render.ToString())
                    };
                }

                _producer.Produce(topicPartition, message);
            }

            _producer.Flush(TimeSpan.FromSeconds(FlushTimeoutSecs));

            return Task.CompletedTask;
        }

        private void ConfigureKafkaConnection(
            string bootstrapServers,
            SecurityProtocol securityProtocol,
            string sslCaLocation,
            string slCertificateLocation,
            string sslKeyLocation)
        {
            var config = new ProducerConfig
            {
                BootstrapServers = bootstrapServers,
                SecurityProtocol = securityProtocol,

                BatchNumMessages = 1000,
                QueueBufferingMaxMessages = 2000000,
                LingerMs = 10,

                // SSL Configuration
                SslCaLocation = sslCaLocation,
                SslCertificateLocation = slCertificateLocation,
                SslKeyLocation = sslKeyLocation,

                // Optional: Disable hostname verification (equivalent to ssl_endpoint_identification_algorithm => "")
                EnableSslCertificateVerification = false,

                // Producer retries configuration (optional)
                MessageSendMaxRetries = 3,
                RetryBackoffMs = 1000,
            };

            _producer = new ProducerBuilder<Null, byte[]>(config)
                .Build();
        }
    }
}