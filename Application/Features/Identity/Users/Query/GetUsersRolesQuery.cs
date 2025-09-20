using ABCShared.Library.Models.Responses.Identity;
using ABCShared.Library.Wrappers;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Identity.Users.Query
{

	public record GetUsersRolesQuery : IRequest<IResponseWrapper>
	{
        public string UserId { get; set; }
    }

	public class GetUsersRolesQueryHandler : IRequestHandler<GetUsersRolesQuery, IResponseWrapper>
	{
        private readonly IUserService _userService;

        public GetUsersRolesQueryHandler(IUserService userService)
        {
            _userService=userService;
        }
        public async Task<IResponseWrapper> Handle(GetUsersRolesQuery request, CancellationToken cancellationToken)
		{
			var userRoles = await _userService.GetUserRolesAsync(request.UserId , cancellationToken);
            return await ResponseWrapper<List<UserRolesResponse>>.SuccessAsync(message: "User roles retrieved successfully.");
        }
	}
}
