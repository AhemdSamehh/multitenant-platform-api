using Infrastructure.Identity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;
using Finbuckle.MultiTenant;

namespace Infrastructure.Contexts
{
    public class DbConfigurations
    {
        public class ApplicationUserConfig : IEntityTypeConfiguration<ApplicationUser>
        {
            public void Configure(EntityTypeBuilder<ApplicationUser> builder)
            {
                builder
                    .ToTable("Users", SchemaNames.Identity)
                    .IsMultiTenant();
            }
        }

        public class ApplicationRoleConfig : IEntityTypeConfiguration<ApplicationRole>
        {
            public void Configure(EntityTypeBuilder<ApplicationRole> builder)
            {
                builder
                    .ToTable("Roles", SchemaNames.Identity)
                    .IsMultiTenant()
                    .AdjustUniqueIndexes();
            }
        }

        public class ApplicationRoleClaimConfig : IEntityTypeConfiguration<ApplicationRoleClaim>
        {
            public void Configure(EntityTypeBuilder<ApplicationRoleClaim> builder)
            {
                builder
                    .ToTable("RoleClaims", SchemaNames.Identity)
                    .IsMultiTenant();
            }
        }
        public class ApplicationUserRoleConfig : IEntityTypeConfiguration<IdentityUserRole<string>>
        {
            public void Configure(EntityTypeBuilder<IdentityUserRole<string>> builder)
            {
                builder
                    .ToTable("UserRoles", SchemaNames.Identity)
                    .IsMultiTenant()
                    ;
            }
        }

        public class ApplicationUserClaimConfig : IEntityTypeConfiguration<IdentityUserClaim<string>>
        {
            public void Configure(EntityTypeBuilder<IdentityUserClaim<string>> builder)
            {
                builder
                    .ToTable("UserClaims", SchemaNames.Identity)
                    .IsMultiTenant()
                    ;
            }
        }
        public class ApplicationUserLoginConfig : IEntityTypeConfiguration<IdentityUserLogin<string>>
        {
            public void Configure(EntityTypeBuilder<IdentityUserLogin<string>> builder)
            {
                builder
                    .ToTable("UserLogins", SchemaNames.Identity)
                    .IsMultiTenant()
                    ;
            }
        }

        public class ApplicationUserTokenConfig : IEntityTypeConfiguration<IdentityUserToken<string>>
        {
            public void Configure(EntityTypeBuilder<IdentityUserToken<string>> builder)
            {
                builder
                    .ToTable("UserTokens", SchemaNames.Identity)
                    .IsMultiTenant()
                    ;
            }
        }

        public class SchoolConfig : IEntityTypeConfiguration<School>
        {
            public void Configure(EntityTypeBuilder<School> builder)
            {
                builder
                    .ToTable("Scools", "Academics")
                    .IsMultiTenant();
                builder
                    .Property(s => s.Name)
                    .IsRequired()
                    .HasMaxLength(100);
            }
        }
    }
}
