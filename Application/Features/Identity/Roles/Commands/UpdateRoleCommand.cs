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

	public record UpdateRoleCommand : IRequest<IResponseWrapper>
	{
        public UpdateRoleRequest UpdateRole { get; set; }
    }

	public class UpdateRoleCommandHandler : IRequestHandler<UpdateRoleCommand , IResponseWrapper>
	{
        private readonly IRoleService _roleService;

        public UpdateRoleCommandHandler(IRoleService roleService)
        {
            _roleService=roleService;
        }
        public async Task<IResponseWrapper> Handle(UpdateRoleCommand request, CancellationToken cancellationToken)
		{
            var updatedRole = await _roleService.UpdateAsync(request.UpdateRole);
            return await ResponseWrapper.SuccessAsync($"Role {updatedRole} updated Successfully");
		}
	}
}
