using ABCShared.Library.Models.Responses.Identity;
using ABCShared.Library.Wrappers;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Identity.Roles.Queries
{

	public record GetRoleWithPermissionsQuery : IRequest<IResponseWrapper>
	{
        public string RoleId { get; set; }
    }

	public class GetRoleWithPermissionsQueryHandler : IRequestHandler<GetRoleWithPermissionsQuery, IResponseWrapper>
	{
        private readonly IRoleService _roleService;

        public GetRoleWithPermissionsQueryHandler(IRoleService roleService)
        {
            _roleService=roleService;
        }
        public async Task<IResponseWrapper> Handle(GetRoleWithPermissionsQuery request, CancellationToken cancellationToken)
		{
            var role = await _roleService.GetRoleWithPermissionsAsync(request.RoleId, cancellationToken);
            return await ResponseWrapper<RoleResponse>.SuccessAsync(role);
		}
	}
}
