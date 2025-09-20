using ABCShared.Library.Models.Requests.Identity;
using ABCShared.Library.Models.Responses.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Identity.Users
{
    public interface IUserService
    {
        Task<string> CreateAsync(CreateUserRequest request);
        Task<string> UpdateAsync(UpdateUserRequest request);
        Task<string> DeleteAsync(string userId);
        Task<string> ActivateOrDeactivateAsync(string userId, bool activation);
        Task<string> ChangePasswordAsync(ChangePasswordRequest request);
        Task<string> AssignRolesAsync(string userId, UserRolesRequest roles);
        Task<List<UserResponse>> GetAllAsync(CancellationToken cancellationToken);
        Task<UserResponse> GetByIdAsync(string userId, CancellationToken cancellationToken);
        Task<List<UserRoleResponse>> GetUserRolesAsync(string userId, CancellationToken cancellationToken);
        Task<bool> IsEmailTakenAsync(string email);
        Task<List<string>> GetUserPermissionsAsync(string userId, CancellationToken cancellationToken);
        Task<bool> IsPermissionAssignedAsync(string userId, string permission, CancellationToken cancellationToken);
    }
}
