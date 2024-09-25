
# Serilog.Sinks.VngCloudLogging

Serilog sink that writes events to [VNG Cloud Logging](https://vmonitor.console.vngcloud.vn/quota-usages/log).

- Built for net8.0, net5.0, netstandard2.1
- [Release Notes](CHANGELOG.md)

## Usage

### Install package from NuGet

You can install the package using the following command:

```bash
dotnet add package Serilog.Sinks.VngCloudLogging
```

### Configure in code

Here is a basic configuration example:

```csharp
var options = new VngCloudLoggingSinkOptions
{
    BootstrapServers = "bootstrapServers",
    Topic = "topic",
    SecurityProtocol = SecurityProtocol.Ssl,
    SslCaLocation = Path.Combine(currentDirectory, "VNG.trust.pem"),
    SslCertificateLocation = Path.Combine(currentDirectory, "user.cer.pem"),
    SslKeyLocation = Path.Combine(currentDirectory, "user.key.pem")
};

Log.Logger = new LoggerConfiguration()
    .WriteTo.VngCloudLogging(options)
    .CreateLogger();
```

## Acknowledgments

This project is inspired by various open-source projects and contributions from the community. Special thanks to:

- [Serilog](https://github.com/serilog/serilog) for their robust logging framework.
- [Serilog.Sinks.Kafka](https://github.com/imburseag/serilog-sinks-kafka) for their implementation that served as a foundation for this sink.
- [Serilog.Sinks.GoogleCloudLogging](https://github.com/manigandham/serilog-sinks-googlecloudlogging) for additional insights and implementations.
- [VNG Cloud Logging](https://vmonitor.console.vngcloud.vn/quota-usages/log) for providing a reliable logging service.

Feel free to contribute to this project by submitting issues or pull requests!