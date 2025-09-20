using ABCShared.Library.Constants;
using ABCShared.Library.Models.Requests.Identity;
using ABCShared.Library.Models.Responses.Identity;
using Application.Exceptions;
using Application.Features.Identity.Users;
using Azure.Core;
using Finbuckle.MultiTenant;
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
    internal class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly IMultiTenantContextAccessor<ABCSchoolTenantInfo> _tenantContextAccessor;

        public UserService(UserManager<ApplicationUser> userManager , RoleManager<ApplicationRole> roleManager , ApplicationDbContext applicationDbContext,
            IMultiTenantContextAccessor<ABCSchoolTenantInfo> tenantContextAccessor)
        {
            _userManager=userManager;
            _roleManager=roleManager;
            _applicationDbContext=applicationDbContext;
            _tenantContextAccessor=tenantContextAccessor;
        }
        public async Task<string> ActivateOrDeactivateAsync(string userId, bool activation)
        {
            var userInDb = await _userManager.FindByIdAsync(userId)
                ?? throw new NotFoundException(["User does not exist"] );
            userInDb.IsActive=activation;
            var result = await _userManager.UpdateAsync(userInDb);
            if (!result.Succeeded)
            {
                throw new IdentityException("Failed to update user activation", IdentityHelper.GetIdentityResultErrorDescription(result));
            }
            return userId;

        }

        public async Task<string> AssignRolesAsync(string userId, UserRolesRequest roles)
        {
            var userInDb = await GetUserAsync(userId);
            if (await _userManager.IsInRoleAsync(userInDb, RoleConstants.Admin) && roles.UserRoles.Any(ur => !ur.IsAssigned && ur.Name == RoleConstants.Admin))
            {
                var adminUsersCount = (await _userManager.GetUsersInRoleAsync(RoleConstants.Admin)).Count();
                if (userInDb.Email == TenancyConstants.Root.Email)
                {
                    if (_tenantContextAccessor.MultiTenantContext.TenantInfo.Id == TenancyConstants.Root.Id)
                    {
                        throw new ConflictException(["Not allowed to remove Admin Role For a Root Tenant user"]);
                    }
                }
                else if (adminUsersCount <= 2)
                {
                    throw new ConflictException(["Tenant should have at least three admin users"]);
                }
            }
            foreach (var userRole in roles.UserRoles)
            {
                    if (userRole.IsAssigned)
                    {
                        if (!await _userManager.IsInRoleAsync(userInDb, userRole.Name))
                        {
                            await _userManager.AddToRoleAsync(userInDb, userRole.Name);
                        }
                    }
                    else
                    {
                        await _userManager.RemoveFromRoleAsync(userInDb, userRole.Name);
                    }
            }
            return userInDb.Id;
        }

        public async Task<string> ChangePasswordAsync(ChangePasswordRequest request)
        {
            var userInDb = await GetUserAsync(request.UserId);

            if (request.NewPassword != request.ConfirmNewPassword)
            {
                throw new ConflictException(["Password do not match"]);
            }

            var result = await _userManager.ChangePasswordAsync(userInDb, request.CurrentPassword, request.NewPassword);
            if (!result.Succeeded)
            {
                throw new IdentityException("Failed to change password", IdentityHelper.GetIdentityResultErrorDescription(result));
            }
            return userInDb.Id;
        }

        public async Task<string> CreateAsync(CreateUserRequest request)
        {
            if (request.Password != request.ConfirmPassword)
            {
                throw new ConflictException(["Password do not match"]);
            }
            if(await IsEmailTakenAsync(request.Email))
            {
                throw new ConflictException(["Email is already taken"]);
            }
            var newUser = new ApplicationUser
            {
                UserName = request.Email,
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                PhoneNumber = request.PhoneNumber,
                IsActive = request.IsActive,
                EmailConfirmed = true,
            };
            var result = await _userManager.CreateAsync(newUser, request.Password);
            if (!result.Succeeded)
            {
                throw new IdentityException("Failed to create user", IdentityHelper.GetIdentityResultErrorDescription(result));
            }
            return newUser.Id;
        }

        public async Task<string> DeleteAsync(string userId)
        {
            var userInDb = await GetUserAsync(userId);
            if(userInDb.Email == TenancyConstants.Root.Email)
            {
                throw new ConflictException(["Not allowed to delete Root Tenant user"]);
            }
            _applicationDbContext.Users.Remove(userInDb);
            await _applicationDbContext.SaveChangesAsync();
            return userId;
        }

        public async Task<List<UserResponse>> GetAllAsync(CancellationToken cancellationToken)
        {
            var usersInDb = await _applicationDbContext.Users.ToListAsync(cancellationToken);
            return usersInDb.Adapt<List<UserResponse>>();
        }

        public async Task<UserResponse> GetByIdAsync(string userId, CancellationToken cancellationToken)
        {
            var userInDb = await GetUserAsync(userId);
            return userInDb.Adapt<UserResponse>();
        }

        public async Task<List<UserRoleResponse>> GetUserRolesAsync(string userId, CancellationToken cancellationToken)
        {
            var userInDb = await GetUserAsync(userId);
            var usersRoles = new List<UserRoleResponse>();
            var roleInDb = await _roleManager.Roles.ToListAsync(cancellationToken);
            foreach (var role in roleInDb)
            {
                usersRoles.Add(new UserRoleResponse
                {
                    RoleId = role.Id,
                    Name = role.Name,
                    Description = role.Description,
                    IsAssigned = await _userManager.IsInRoleAsync(userInDb, role.Name)
                });
            }
            return usersRoles;
        }

        public async Task<List<string>> GetUserPermissionsAsync(string userId, CancellationToken cancellationToken)
        {
            var userInDb = await GetUserAsync(userId);
            var userRolesName = await _userManager.GetRolesAsync(userInDb);
            var permissions = new List<string>();
            foreach (var role in await _roleManager.Roles.Where(r=> userRolesName.Contains(r.Name)).ToListAsync(cancellationToken))
            {
                permissions.AddRange(await _applicationDbContext.RoleClaims.Where(rc => rc.RoleId == role.Id && rc.ClaimType == ClaimConstants.Permission)
                    .Select(rc => rc.ClaimValue).ToListAsync(cancellationToken));
            }
            return permissions.Distinct().ToList();
        }

        public async Task<bool> IsEmailTakenAsync(string email)
        => await _userManager.FindByEmailAsync(email) is not null;

        public async Task<bool> IsPermissionAssignedAsync(string userId, string permission, CancellationToken cancellationToken)
        {
            return (await GetUserPermissionsAsync(userId , cancellationToken)).Contains(permission);
        }

        public async Task<string> UpdateAsync(UpdateUserRequest request)
        {
            var userInDb = await GetUserAsync(request.Id);

            userInDb.FirstName = request.FirstName;
            userInDb.LastName = request.LastName;
            userInDb.PhoneNumber = request.PhoneNumber;

            var result = await _userManager.UpdateAsync(userInDb);
            if (!result.Succeeded)
            {
                throw new IdentityException("Failed to update user", IdentityHelper.GetIdentityResultErrorDescription(result));
            }
            return userInDb.Id;
        }

        private async Task<ApplicationUser> GetUserAsync(string userId)
        {
            var userInDb = await _userManager.FindByIdAsync(userId)
                ?? throw new NotFoundException(["User Dose'nt Found"]);
            return userInDb;
        }
    }
}
