using ABCShared.Library.Constants;
using Finbuckle.MultiTenant.Abstractions;
using Infrastructure.Constants;
using Infrastructure.Identity.Models;
using Infrastructure.Tenancy;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Contexts
{
    public class ApplicationDbSeeder
    {
        private readonly IMultiTenantContextAccessor<ABCSchoolTenantInfo> _multiTenantContextAccessor;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _applicationDbContext;

        public ApplicationDbSeeder(IMultiTenantContextAccessor<ABCSchoolTenantInfo> multiTenantContextAccessor ,
            RoleManager<ApplicationRole> roleManager , UserManager<ApplicationUser> userManager , ApplicationDbContext applicationDbContext)
        {
            _multiTenantContextAccessor=multiTenantContextAccessor;
            _roleManager=roleManager;
            _userManager=userManager;
            _applicationDbContext=applicationDbContext;
        }

        public async Task InitializeDatabaseAsync(CancellationToken cancellationToken)
        {
            if (_applicationDbContext.Database.GetMigrations().Any())
            {
                if ((await _applicationDbContext.Database.GetAppliedMigrationsAsync(cancellationToken)).Any())
                {
                    await _applicationDbContext.Database.MigrateAsync(cancellationToken);
                }
            }

            if (await _applicationDbContext.Database.CanConnectAsync(cancellationToken))
            {
                await InitializeDefaultRolesAsync(cancellationToken);
                await InitializeAdminUserAsync();
            }
        }

        private async Task InitializeAdminUserAsync()
        {
            if (string.IsNullOrEmpty(_multiTenantContextAccessor.MultiTenantContext.TenantInfo.Email)) return;
            if (await _userManager.Users.SingleOrDefaultAsync(user => user.Email == _multiTenantContextAccessor.MultiTenantContext.TenantInfo.Email)
                 is not ApplicationUser incomingUser)
            {
                incomingUser = new ApplicationUser
                {
                    FirstName = TenancyConstants.FirstName,
                    LastName = TenancyConstants.LastName,
                    Email = _multiTenantContextAccessor.MultiTenantContext.TenantInfo.Email,
                    UserName = _multiTenantContextAccessor.MultiTenantContext.TenantInfo.Email,
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true,
                    NormalizedEmail = _multiTenantContextAccessor.MultiTenantContext.TenantInfo.Email.ToUpperInvariant(),
                    NormalizedUserName = _multiTenantContextAccessor.MultiTenantContext.TenantInfo.Email.ToUpperInvariant(),
                    IsActive = true,
                };
                var passwordHash = new PasswordHasher<ApplicationUser>();
                incomingUser.PasswordHash = passwordHash.HashPassword(incomingUser, TenancyConstants.DefaultPassword);
                await _userManager.CreateAsync(incomingUser);
            }
            if (!await _userManager.IsInRoleAsync(incomingUser, RoleConstants.Admin))
            {
                await _userManager.AddToRoleAsync(incomingUser, RoleConstants.Admin);
            }

        }

        private async Task InitializeDefaultRolesAsync(CancellationToken cancellationToken)
        {
            foreach (var roleName in RoleConstants.DefaultRoles)
            {
                if (await _roleManager.Roles.SingleOrDefaultAsync(role => role.Name == roleName, cancellationToken)
                    is not ApplicationRole existingRole)
                {
                    existingRole = new ApplicationRole()
                    {
                        Name = roleName,
                        Description = $"{roleName} Role",
                    };
                    await _roleManager.CreateAsync(existingRole);
                }

                // Assign permissions to the newly added role
                if (roleName == RoleConstants.Basic)
                {
                    await AssignPermissionsToRoleAsync(SchoolPermissions.Basic, existingRole, cancellationToken);
                }
                else if (roleName == RoleConstants.Admin)
                {
                    await AssignPermissionsToRoleAsync(SchoolPermissions.Admin, existingRole, cancellationToken);
                    if (_multiTenantContextAccessor.MultiTenantContext.TenantInfo.Id == TenancyConstants.Root.Id)
                    {
                        await AssignPermissionsToRoleAsync(SchoolPermissions.Root, existingRole, cancellationToken);
                    }
                }
            }
        }

        private async Task AssignPermissionsToRoleAsync(IReadOnlyList<SchoolPermission> incomingRolePermission, ApplicationRole currentRole,
      CancellationToken cancellationToken)
        {
            var currentClaims = await _roleManager.GetClaimsAsync(currentRole);

            foreach (var rolePermission in incomingRolePermission)
            {
                if (!currentClaims.Any(c => c.Type == ClaimConstants.Permission && c.Value == rolePermission.Name))
                {
                    await _applicationDbContext.RoleClaims.AddAsync(new ApplicationRoleClaim
                    {
                        RoleId = currentRole.Id,
                        ClaimType = ClaimConstants.Permission,
                        ClaimValue = rolePermission.Name,
                        Description = rolePermission.Description,
                        Group = rolePermission.Group,
                    }, cancellationToken);
                    await _applicationDbContext.SaveChangesAsync(cancellationToken);
                }
            }

        }

    }
}
 