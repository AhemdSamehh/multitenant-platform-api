using ABCShared.Library.Models.Requests.Identity;
using ABCShared.Library.Models.Responses.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Identity.Roles
{
    public interface IRoleService
    {
        Task<string> CreateAsync(CreateRoleRequest request);
        Task<string> UpdateAsync(UpdateRoleRequest request);
        Task<string> DeleteAsync(string id);
        Task<string> UpdatePermissionsAsync(UpdateRolePermissionsRequest request);
        Task<List<RoleResponse>> GetRolesAsync(CancellationToken cancellationToken);
        Task<RoleResponse> GetRoleByIdAsync(string Id, CancellationToken cancellationToken);
        Task<RoleResponse> GetRoleWithPermissionsAsync(string id, CancellationToken cancellationToken);
        Task<bool> DoseItExistsAsync(string name);
    }
}
