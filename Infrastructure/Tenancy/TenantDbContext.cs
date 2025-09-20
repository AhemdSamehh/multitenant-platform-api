using Finbuckle.MultiTenant.EntityFrameworkCore.Stores.EFCoreStore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Tenancy
{
    public class TenantDbContext(DbContextOptions<TenantDbContext> options): EFCoreStoreDbContext<ABCSchoolTenantInfo>(options)
    {
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ABCSchoolTenantInfo>()
                .ToTable("Tenants", "Multitenancy");
                
        }
    }
}
