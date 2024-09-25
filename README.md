# Serilog.Sinks.VngCloudLogging

Serilog sink that writes events to [VNG Cloud Logging](https://vmonitor.console.vngcloud.vn/quota-usages/log).

-   Built for `net8.0`, `net5.0`, `netstandard2.1`
-   [Release Notes](CHANGELOG.md)

## Usage

#### Install [package from Nuget](https://www.nuget.org/packages/Serilog.Sinks.VngCloudLogging/):

```
dotnet add package Serilog.Sinks.VngCloudLogging
```

#### Configure in code:

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
Log.Logger = new LoggerConfiguration().WriteTo.VngCloudLogging(config).CreateLogger();
```
