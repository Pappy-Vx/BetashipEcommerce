using BetashipEcommerce.CORE.Identity.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetashipEcommerce.CORE.Repositories
{
    public interface ICurrentUserService
    {
        UserId? UserId { get; }
        string? Username { get; }
        bool IsAuthenticated { get; }
    }
}
