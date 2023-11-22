# Logging and Monitoring in ASP.NET Core

> [!NOTE]
> This repo has an [updated branch that targets .NET 8](https://github.com/dahlsailrunner/logging-monitoring-aspnet6/tree/aspnet8).

This is the repository with the code associated with this Pluralsight course: [https://app.pluralsight.com/library/courses/logging-monitoring-aspdotnet-core-6](https://app.pluralsight.com/library/courses/logging-monitoring-aspdotnet-core-6)

The course (and this repo) demonstrates logging and monitoring within ASP.NET Core 6, including the following
features:

* Injection and use of the `ILogger<T>` interface
* Default providers and configuration within ASP.NET Core 6
* Log levels and their usage
* Categories for log entries
* Filtering log entries with both configuration and code
* Exception Handling and logging
* Scopes
* Event Ids and how to use them
* Techniques for hiding sensitive information
* Semantic logging
* Third party logging libraries (like Serilog and NLog)
* Using third party libraries to write to various destinations (Splunk, Seq, Application Insights)
* Using the `LoggerMessage` source generator
* Health checks and things that monitor the health checks
* Traceability with OpenTelemetry

Repo URL: [https://github.com/dahlsailrunner/logging-monitoring-aspnet6](https://github.com/dahlsailrunner/logging-monitoring-aspnet6)

## VS Code Setup

The `C#` extension is required to use this repo.  I have some other settings that you may be curious about
and they are described in my [VS Code gist](https://gist.github.com/dahlsailrunner/1765b807940e29951ea6bdfb36cd85dd).
