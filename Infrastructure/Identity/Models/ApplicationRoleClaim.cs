using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Identity.Models
{
    public class ApplicationRoleClaim : IdentityRoleClaim<string>
    {
        public string Description { get; set; }
        public string Group { get; set; }
        
        // TenantId will be automatically added by Finbuckle.MultiTenant
        // when the entity is configured with .IsMultiTenant()
    }
}
