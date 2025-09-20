using Domain.Entities;
using Finbuckle.MultiTenant.Abstractions;
using Infrastructure.Tenancy;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Contexts
{
    public class ApplicationDbContext : BaseDbContext
    {
        public ApplicationDbContext(IMultiTenantContextAccessor<ABCSchoolTenantInfo> multiTenantContextAccessor, DbContextOptions<ApplicationDbContext> options)
            : base(multiTenantContextAccessor, options)
        {

        }

        public DbSet<School> Schools => Set<School>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            
            // Apply entity configurations
            builder.ApplyConfiguration(new DbConfigurations.ApplicationUserConfig());
            builder.ApplyConfiguration(new DbConfigurations.ApplicationRoleConfig());
            builder.ApplyConfiguration(new DbConfigurations.ApplicationRoleClaimConfig());
            builder.ApplyConfiguration(new DbConfigurations.ApplicationUserRoleConfig());
            builder.ApplyConfiguration(new DbConfigurations.ApplicationUserClaimConfig());
            builder.ApplyConfiguration(new DbConfigurations.ApplicationUserLoginConfig());
            builder.ApplyConfiguration(new DbConfigurations.ApplicationUserTokenConfig());
            builder.ApplyConfiguration(new DbConfigurations.SchoolConfig());
        }
    }
}
