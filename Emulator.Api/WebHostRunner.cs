namespace Emulator.Api
{
    using System;
    using System.Diagnostics;
    //using Autofac.Extensions.DependencyInjection;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    //using Microsoft.Extensions.DependencyInjection;
    //using Phoenix.Api.Common.Logging;
    //using Phoenix.Api.Common.Mvc.Middlewares;
    //using Serilog;
    //using Serilog.Core;
    //using Serilog.Debugging;

    public class WebHostRunner<TStartup> : IDisposable
        where TStartup : class
    {
        private readonly string[] _args;
        private readonly string _environmentName;
        private readonly string _instanceId;
        private readonly Stopwatch _stopwatch = new Stopwatch();
        private IConfiguration _configuration;
        //private ILogger _logger;

        public WebHostRunner(string serviceName, string[] args)
        {
            ServiceName = serviceName;
            _args = args;
            _environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            _instanceId = Environment.MachineName;
            if (string.IsNullOrEmpty(_instanceId))
            {
                _instanceId = Environment.GetEnvironmentVariable("COMPUTERNAME");
            }

            if (string.IsNullOrWhiteSpace(_instanceId))
            {
                _instanceId = Environment.GetEnvironmentVariable("HOSTNAME");
            }

            var processId = Process.GetCurrentProcess().Id;
            Console.Title = ServiceName;
            Console.WriteLine($"Initializing {ServiceName} {processId} {_instanceId} for environment: {_environmentName}");
            Initialize();
            BuildWebHost();
        }

        public string ServiceName { get; }

        public IWebHost WebHost { get; private set; }

        public void Run()
        {
            StartWebHost();

            try
            {
                _stopwatch.Restart();
                WebHost.WaitForShutdown(); // block until it's released
                _stopwatch.Stop();
                //_logger.ForContext("ElapsedMs", _stopwatch.ElapsedMilliseconds)
                //    .Warning("{ApplicationName} {InstanceId} terminated after {ElapsedMs}ms", ServiceName, _instanceId, _stopwatch.ElapsedMilliseconds);
            }
            catch (Exception ex)
            {
                if (_stopwatch.IsRunning)
                {
                    _stopwatch.Stop();
                }

                //_logger.ForContext("ElapsedMs", _stopwatch.ElapsedMilliseconds)
                //    .Fatal(ex, "{ApplicationName} {InstanceId} terminated unexpectedly after {ElapsedMs}ms", ServiceName, _instanceId, _stopwatch.ElapsedMilliseconds);
                throw;
            }
            finally
            {
                //Log.CloseAndFlush();
            }
        }

        public void Dispose()
        {
            WebHost.Dispose();
        }

        private void BuildWebHost()
        {
            try
            {
                //var healthEndpointUrl = _configuration.GetValue($"HealthCheck:Endpoint", "/health");

                //_logger.Debug("{ServiceName} {InstanceId} creating host", ServiceName, _instanceId);
                _stopwatch.Restart();

                WebHost = new WebHostBuilder()
                    .UseShutdownTimeout(TimeSpan.FromSeconds(10))
                    .UseSetting(WebHostDefaults.DetailedErrorsKey, "true")
                    .UseKestrel(x => x.AddServerHeader = false)
                    //.UseSerilog(Log.Logger)
                    .UseConfiguration(_configuration)
                    //.ConfigureAppHealthHostingConfiguration(options => { options.HealthEndpoint = healthEndpointUrl; })
                    .UseDefaultServiceProvider(options => options.ValidateScopes = false)
                    .UseStartup<TStartup>()
                    //.ConfigureServices((context, services) =>
                    //{
                    //    services.AddAutofac();
                    //})
                    .Build();

                _stopwatch.Stop();

                //_logger.Debug("{ServiceName} {InstanceId} host created after {ElapsedMs}ms", ServiceName, _instanceId, _stopwatch.ElapsedMilliseconds);
            }
            catch (Exception ex)
            {
                if (_stopwatch.IsRunning)
                {
                    _stopwatch.Stop();
                }

                //_logger.Fatal(ex, "{ApplicationName} {InstanceId} host creation failed after {ElapsedMs}ms", ServiceName, _instanceId, _stopwatch.ElapsedMilliseconds);
                //Log.CloseAndFlush();
                throw;
            }
        }

        private void StartWebHost()
        {
            try
            {
                //Log.Debug("{ApplicationName} {InstanceId} starting host...", ServiceName, _instanceId);

                _stopwatch.Restart();
                WebHost.Start(); // this doesn't block the thread
                _stopwatch.Stop();

                //_logger.Information(
                //    "{ApplicationName} {InstanceId} host started with {Environment} configuration after {ElapsedMs}ms",
                //    ServiceName,
                //    _instanceId,
                //    _environmentName,
                //    _stopwatch.ElapsedMilliseconds);
            }
            catch (Exception ex)
            {
                if (_stopwatch.IsRunning)
                {
                    _stopwatch.Stop();
                }

                //_logger.Fatal(ex, "{ApplicationName} {InstanceId} host start failed after {ElapsedMs}ms", ServiceName, _instanceId, _stopwatch.ElapsedMilliseconds);
                //Log.CloseAndFlush();
                throw;
            }
        }

        private void Initialize()
        {
            _stopwatch.Start();
            _configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json")
                .AddJsonFile($"appsettings.{_environmentName}.json", optional: true)
                .AddEnvironmentVariables()
                .AddCommandLine(_args)
                .Build();

            //Log.Logger = new LoggerConfiguration()
            //    .ReadFromSettings(_configuration)
            //    .Enrich.FromLogContext()
            //    .Enrich.WithProperty("Environment", _environmentName)
            //    .Enrich.WithProperty("ApplicationName", ServiceName)
            //    .Enrich.WithProperty("InstanceId", _instanceId)
            //    .CreateLogger();

            //var selfLogger = new LoggerConfiguration()
            //    .ReadFromSettings(_configuration.GetSection("SelfLog"))
            //    .Enrich.FromLogContext()
            //    .Enrich.WithProperty("Environment", _environmentName)
            //    .Enrich.WithProperty("ApplicationName", ServiceName)
            //    .Enrich.WithProperty("InstanceId", _instanceId)
            //    .CreateLogger();

            //SelfLog.Enable(e => selfLogger.Error("{ErrorMessage}", e));

            //_logger = Log.ForContext<WebHostRunner<TStartup>>();

            _stopwatch.Stop();

            //_logger.Debug("{ServiceName} {InstanceId} initialized after {ElapsedMs}ms", ServiceName, _instanceId, _stopwatch.Elapsed.TotalMilliseconds);
        }
    }
}