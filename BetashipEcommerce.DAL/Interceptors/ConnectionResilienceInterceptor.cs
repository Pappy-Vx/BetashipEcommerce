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
    /// Logs database connection events for monitoring
    /// </summary>
    public sealed class ConnectionResilienceInterceptor : DbConnectionInterceptor
    {
        private readonly ILogger<ConnectionResilienceInterceptor> _logger;

        public ConnectionResilienceInterceptor(ILogger<ConnectionResilienceInterceptor> logger)
        {
            _logger = logger;
        }

        public override void ConnectionFailed(DbConnection connection, ConnectionErrorEventData eventData)
        {
            _logger.LogError(
                eventData.Exception,
                "Database connection failed: {ConnectionString}",
                connection.ConnectionString);

            base.ConnectionFailed(connection, eventData);
        }

        public override async Task ConnectionFailedAsync(
            DbConnection connection,
            ConnectionErrorEventData eventData,
            CancellationToken cancellationToken = default)
        {
            _logger.LogError(
                eventData.Exception,
                "Database connection failed: {ConnectionString}",
                connection.ConnectionString);

            await base.ConnectionFailedAsync(connection, eventData, cancellationToken);
        }

        public override void ConnectionOpened(DbConnection connection, ConnectionEndEventData eventData)
        {
            _logger.LogDebug("Database connection opened: {ConnectionId}", eventData.ConnectionId);
            base.ConnectionOpened(connection, eventData);
        }
    }
}
