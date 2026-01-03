using BetashipEcommerce.CORE.Auditing;
using BetashipEcommerce.CORE.Auditing.Enums;
using BetashipEcommerce.CORE.Identity.ValueObjects;
using BetashipEcommerce.CORE.Repositories;
using BetashipEcommerce.DAL.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetashipEcommerce.DAL.Repositories
{
    internal sealed class AuditLogRepository : IAuditLogRepository
    {
        private readonly ApplicationDbContext _context;

        public AuditLogRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<AuditLog> AddAsync(
            AuditLog auditLog,
            CancellationToken cancellationToken = default)
        {
            _context.AuditLogs.Add(auditLog);
            await _context.SaveChangesAsync(cancellationToken);
            return auditLog;
        }

        public async Task<List<AuditLog>> GetByEntityAsync(
            string entityType,
            string entityId,
            CancellationToken cancellationToken = default)
        {
            return await _context.AuditLogs
                .Where(a => a.EntityType == entityType && a.EntityId == entityId)
                .OrderByDescending(a => a.Timestamp)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<List<AuditLog>> GetByUserAsync(
            UserId userId,
            DateTime? from = null,
            DateTime? to = null,
            CancellationToken cancellationToken = default)
        {
            var query = _context.AuditLogs.Where(a => a.UserId == userId);

            if (from.HasValue)
                query = query.Where(a => a.Timestamp >= from.Value);

            if (to.HasValue)
                query = query.Where(a => a.Timestamp <= to.Value);

            return await query
                .OrderByDescending(a => a.Timestamp)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<List<AuditLog>> GetRecentAsync(
            int count,
            CancellationToken cancellationToken = default)
        {
            return await _context.AuditLogs
                .OrderByDescending(a => a.Timestamp)
                .Take(count)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<List<AuditLog>> GetByActionAsync(
            AuditAction action,
            DateTime? from = null,
            DateTime? to = null,
            CancellationToken cancellationToken = default)
        {
            var query = _context.AuditLogs.Where(a => a.Action == action);

            if (from.HasValue)
                query = query.Where(a => a.Timestamp >= from.Value);

            if (to.HasValue)
                query = query.Where(a => a.Timestamp <= to.Value);

            return await query
                .OrderByDescending(a => a.Timestamp)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }
    }
}
