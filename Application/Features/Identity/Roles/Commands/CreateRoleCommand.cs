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

	public record CreateRoleCommand : IRequest<IResponseWrapper>
	{
        public CreateRoleRequest  CreateRole { get; set; }
    }

	public class CreateRoleCommandHandler : IRequestHandler<CreateRoleCommand , IResponseWrapper>
	{
        private readonly IRoleService _roleService;

        public CreateRoleCommandHandler(IRoleService roleService)
        {
            _roleService=roleService;
        }
        public async Task<IResponseWrapper> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
		{
            var roleName = await _roleService.CreateAsync(request.CreateRole);

            return await ResponseWrapper<string>.SuccessAsync(roleName, $"Role Created {roleName} Successfully");
		}
	}
}
