using ABCShared.Library.Models.Requests.Identity;
using ABCShared.Library.Wrappers;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Identity.Roles.Commands
{

	public record UpdateRolePermissionCommand : IRequest<IResponseWrapper>
	{
        public UpdateRolePermissionsRequest  UpdateRolePermissions { get; set; }
    }

	public class UpdateRolePermissionCommandHandler : IRequestHandler<UpdateRolePermissionCommand , IResponseWrapper>
	{
        private readonly IRoleService _roleService;

        public UpdateRolePermissionCommandHandler(IRoleService roleService)
        {
            _roleService=roleService;
        }
        public async Task<IResponseWrapper> Handle(UpdateRolePermissionCommand request, CancellationToken cancellationToken)
		{
            var message = await _roleService.UpdatePermissionsAsync(request.UpdateRolePermissions);
            return await ResponseWrapper.SuccessAsync(message);
		}
	}
}
