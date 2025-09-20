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

	public record GetRolesQuery : IRequest<IResponseWrapper>
	{
	}

	public class GetRolesQueryHandler : IRequestHandler<GetRolesQuery, IResponseWrapper>
	{
        private readonly IRoleService _roleService;

        public GetRolesQueryHandler(IRoleService roleService)
        {
            _roleService=roleService;
        }
        public async Task<IResponseWrapper> Handle(GetRolesQuery request, CancellationToken cancellationToken)
		{
			var roles = await _roleService.GetRolesAsync(cancellationToken);
			return await ResponseWrapper<List<RoleResponse>>.SuccessAsync(roles);

		}
	}
}
