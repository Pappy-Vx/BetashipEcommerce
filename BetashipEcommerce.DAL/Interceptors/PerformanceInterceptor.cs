using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetashipEcommerce.DAL.Interceptors
{
    /// <summary>
    /// Logs slow queries for performance monitoring
    /// </summary>
    public sealed class PerformanceInterceptor : DbCommandInterceptor
    {
        private readonly ILogger<PerformanceInterceptor> _logger;
        private const int SlowQueryThresholdMs = 500;

        public PerformanceInterceptor(ILogger<PerformanceInterceptor> logger)
        {
            _logger = logger;
        }

        public override DbDataReader ReaderExecuted(
            DbCommand command,
            CommandExecutedEventData eventData,
            DbDataReader result)
        {
            if (eventData.Duration.TotalMilliseconds > SlowQueryThresholdMs)
            {
                _logger.LogWarning(
                    "Slow query detected ({Duration}ms): {CommandText}",
                    eventData.Duration.TotalMilliseconds,
                    command.CommandText);
            }

            return base.ReaderExecuted(command, eventData, result);
        }

        public override async ValueTask<DbDataReader> ReaderExecutedAsync(
            DbCommand command,
            CommandExecutedEventData eventData,
            DbDataReader result,
            CancellationToken cancellationToken = default)
        {
            if (eventData.Duration.TotalMilliseconds > SlowQueryThresholdMs)
            {
                _logger.LogWarning(
                    "Slow query detected ({Duration}ms): {CommandText}",
                    eventData.Duration.TotalMilliseconds,
                    command.CommandText);
            }

            return await base.ReaderExecutedAsync(command, eventData, result, cancellationToken);
        }
    }

}
