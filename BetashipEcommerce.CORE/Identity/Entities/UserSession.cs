using BetashipEcommerce.CORE.SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetashipEcommerce.CORE.Identity.Entities
{
    public sealed class UserSession : Entity<Guid>
    {
        public string IpAddress { get; private set; }
        public string UserAgent { get; private set; }
        public DateTime LoginAt { get; private set; }
        public DateTime? LogoutAt { get; private set; }
        public bool IsActive => !LogoutAt.HasValue;

        private UserSession(Guid id, string ipAddress, string userAgent)
            : base(id)
        {
            IpAddress = ipAddress;
            UserAgent = userAgent;
            LoginAt = DateTime.UtcNow;
        }

        private UserSession() : base() { }

        public static UserSession Create(string ipAddress, string userAgent)
        {
            return new UserSession(Guid.NewGuid(), ipAddress, userAgent);
        }

        public void End()
        {
            LogoutAt = DateTime.UtcNow;
        }
    }

}
