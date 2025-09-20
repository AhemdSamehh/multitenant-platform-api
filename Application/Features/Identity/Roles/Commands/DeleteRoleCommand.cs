using ABCShared.Library.Wrappers;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Identity.Roles.Commands
{

	public record DeleteRoleCommand : IRequest<IResponseWrapper>
	{
        public string RoleId { get; set; }
    }

	public class DeleteRoleCommandHandler : IRequestHandler<DeleteRoleCommand , IResponseWrapper>
	{
        private readonly IRoleService _roleService;

        public DeleteRoleCommandHandler(IRoleService roleService)
        {
            _roleService=roleService;
        }
        public async Task<IResponseWrapper> Handle(DeleteRoleCommand request, CancellationToken cancellationToken)
		{
			var deleteRole = await _roleService.DeleteAsync(request.RoleId);
            return await ResponseWrapper.SuccessAsync($"Role {deleteRole} deleted Successfully");
		}
	}
}
