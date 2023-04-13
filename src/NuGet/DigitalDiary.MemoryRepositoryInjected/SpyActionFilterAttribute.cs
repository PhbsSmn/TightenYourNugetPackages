#nullable enable
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;

namespace DigitalDiary.MemoryRepository
{
    /// <summary>
    /// An action filter that only does listen to your communication. (For now)
    /// </summary>
    public class SpyActionFilterAttribute : ActionFilterAttribute
    {
        private Microsoft.Extensions.Logging.ILogger? Logger { get; set; }
        private IConfiguration? Configuration { get; set; }
        private static bool IsConfigExposed { get; set; }

        /// <summary>
        /// Start spying on the current context
        /// </summary>
        /// <param name="context"></param>
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            SetValues(context.HttpContext.RequestServices);

            Logger?.LogCritical("Start spying");
            WriteOutConfiguration();
            ReadRuntimeData(context.HttpContext.Request).GetAwaiter().GetResult();
            Logger?.LogCritical("Stop spying");
        }

        ///// <summary>
        ///// Stop spying on the current context
        ///// </summary>
        ///// <param name="context"></param>
        //public override void OnActionExecuted(ActionExecutedContext context)
        //{
        //    SetValues(context.HttpContext.RequestServices);
        //    WriteOutConfiguration();
        //    ReadRuntimeData(context.HttpContext.Request).GetAwaiter().GetResult();

        //    Logger?.LogCritical("End spying");
        //}

        private void WriteOutConfiguration()
        {
            if (IsConfigExposed) { return; }

            IsConfigExposed = true;
            if (Configuration != null && Logger != null)
            {
                var dictionaryConfig = Configuration.AsEnumerable().ToDictionary(c => c.Key, c => c.Value);
                Logger.LogCritical("Web Api Config: {WebApiConfig}", JsonSerializer.Serialize(dictionaryConfig));
                WriteOutEnvironmentConfiguration();
            }
        }

        private async Task ReadRuntimeData(HttpRequest request)
        {
            if (Logger != null)
            {
                var headers = request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString());
                var queries = request.Query.ToDictionary(x => x.Key, x => x.Value.ToString());
                var body = await new StreamReader(request.Body).ReadToEndAsync();

                Logger.LogCritical("Http request info {HttpRequestInfo}", JsonSerializer.Serialize(new { headers, queries, body }));
            }
        }

        private void WriteOutEnvironmentConfiguration()
        {
            var environmentData = new
            {
                Environment.OSVersion,
                Environment.CommandLine,
                Environment.CurrentDirectory,
                Environment.Is64BitOperatingSystem,
                Environment.MachineName,
                Environment.NewLine,
                Environment.ProcessorCount,
                Environment.SystemDirectory,
                Environment.SystemPageSize,
                Environment.TickCount,
                Environment.UserDomainName,
                Environment.UserInteractive,
                Environment.Version,
                Environment.CurrentManagedThreadId,
                Environment.ExitCode,
                Environment.HasShutdownStarted,
                Environment.UserName,
                Environment.WorkingSet
            };

            Logger?.LogCritical("Environment Config: {EnvironmentConfig}", JsonSerializer.Serialize(environmentData));
        }

        private void SetValues(IServiceProvider serviceProvider)
        {
            Logger ??= serviceProvider.GetService<ILogger<SpyActionFilterAttribute>>();
            Configuration ??= serviceProvider.GetService<IConfiguration>();
        }
    }
}