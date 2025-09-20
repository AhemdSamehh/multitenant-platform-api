using ABCShared.Library.Constants;
using ABCShared.Library.Models.Requests.Identity;
using ABCShared.Library.Models.Responses.Identity;
using Application.Exceptions;
using Application.Features.Identity.Roles;
using Finbuckle.MultiTenant.Abstractions;
using Infrastructure.Constants;
using Infrastructure.Contexts;
using Infrastructure.Identity.Models;
using Infrastructure.Tenancy;
using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Identity
{
    public class RoleService : IRoleService
    {
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly IMultiTenantContextAccessor<ABCSchoolTenantInfo> _multiTenantContextAccessor;

        public RoleService(RoleManager<ApplicationRole> roleManager , UserManager<ApplicationUser> userManager ,
            ApplicationDbContext applicationDbContext , IMultiTenantContextAccessor<ABCSchoolTenantInfo> multiTenantContextAccessor)
        {
            _roleManager=roleManager;
            _userManager=userManager;
            _applicationDbContext=applicationDbContext;
            this._multiTenantContextAccessor=multiTenantContextAccessor;
        }

        public async Task<string> CreateAsync(CreateRoleRequest request)
        {
            var newRole = new ApplicationRole()
            {
                Name = request.Name,
                Description = request.Description,
            };
            var result = await _roleManager.CreateAsync(newRole);
            if (!result.Succeeded)
            {
                throw new IdentityException("Failed to Create a role", GetIdentityResultErrorDescription(result));
            }
            return newRole.Name;
        }

        public async Task<string> DeleteAsync(string id)
        {
            var roleInDb = await _roleManager.FindByIdAsync(id)
          ?? throw new NotFoundException(["Role dose not exists"]);
            if (RoleConstants.IsDefaultRole(roleInDb.Name))
            {
                throw new ConflictException([$"not allowed on {roleInDb.Name} role."]);
            }
            if ((await _userManager.GetUsersInRoleAsync(roleInDb.Name)).Count > 0)
            {
                throw new ConflictException([$"not allowed To Delete {roleInDb.Name} role. as is currently assigned to users"]);
            }
            var result = await _roleManager.DeleteAsync(roleInDb);
            if (!result.Succeeded)
            {
                throw new IdentityException("Failed to Delete Role", GetIdentityResultErrorDescription(result));
            }
            return roleInDb.Name;
        }

        public async Task<bool> DoseItExistsAsync(string name)
        {
            return await _roleManager.RoleExistsAsync(name);
        }

        public async Task<RoleResponse> GetRoleByIdAsync(string Id, CancellationToken cancellationToken)
        {
            var roleInDb = await _applicationDbContext.Roles.SingleOrDefaultAsync(r => r.Id == Id, cancellationToken)
                ?? throw new NotFoundException(["Role dose not exists"]);
            var mappedRole = roleInDb.Adapt<RoleResponse>();
            return mappedRole;
        }

        public Task<List<RoleResponse>> GetRolesAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task<RoleResponse> GetRoleWithPermissionsAsync(string id, CancellationToken cancellationToken)
        {
            var role = await GetRoleByIdAsync(id, cancellationToken);

            role.Permissions = await _applicationDbContext.RoleClaims.Where(rc=>rc.RoleId == id && rc.ClaimType == ClaimConstants.Permission)
                .Select(rc=>rc.ClaimValue).ToListAsync();

            return role;
        }

        public async Task<string> UpdateAsync(UpdateRoleRequest request)
        {
           var roleInDb = await _roleManager.FindByIdAsync(request.Id)
                ?? throw new NotFoundException([$"not allowed on role."]);
            if(RoleConstants.IsDefaultRole(roleInDb.Name))
            {
                throw new ConflictException([$"Change not allowed on system {roleInDb.Name}"]);
            }

            roleInDb.Name = request.Name;
            roleInDb.Description = request.Description;
            roleInDb.NormalizedName = request.Name.ToUpperInvariant();

            var result = await _roleManager.UpdateAsync(roleInDb);
            if (!result.Succeeded)
            {
                throw new IdentityException("Failed to Create a role", GetIdentityResultErrorDescription(result));
            }
            return roleInDb.Name;
        }

        public async Task<string> UpdatePermissionsAsync(UpdateRolePermissionsRequest request)
        {
            var roleInDb = await _roleManager.FindByIdAsync(request.RoleId)
                ?? throw new NotFoundException([$"not allowed on role."]);
            if(roleInDb.Name == RoleConstants.Admin)
            {
                throw new ConflictException([$"Not allowed to change permission for {roleInDb.Name} role"]);
            }
            if(_multiTenantContextAccessor.MultiTenantContext.TenantInfo.Id != TenancyConstants.Root.Id)
            {
                request.NewPermissions.RemoveAll(p => p.StartsWith("Permission.Tenants"));
            }
            var currentClaims = await _roleManager.GetClaimsAsync(roleInDb);
            foreach (var claim in currentClaims.Where(c=>!request.NewPermissions.Any(p=>p == c.Value)))
            {
                var result = await _roleManager.RemoveClaimAsync(roleInDb, claim);

                if (!result.Succeeded)
                {
                    throw new IdentityException("Failed to Delete Role", GetIdentityResultErrorDescription(result));
                }
            }
            foreach(var newPermissions in request.NewPermissions.Where(p=> !currentClaims.Any(c=>c.Value == p)))
            {
                await _applicationDbContext.RoleClaims.AddAsync(new ApplicationRoleClaim
                {
                    RoleId = roleInDb.Id,
                    ClaimType = ClaimConstants.Permission,
                    ClaimValue = newPermissions,
                    Description = "",
                    Group = ""
                });
            }

            await _applicationDbContext.SaveChangesAsync();

            return "Permissions Updated Successfully";

        }


        private List<string> GetIdentityResultErrorDescription(IdentityResult identityResult)
        {
            var errorDescription = new List<string>();
            foreach (var error in identityResult.Errors)
            {
                errorDescription.Add(error.Description);
            }
            return errorDescription;
        }
    }
}
