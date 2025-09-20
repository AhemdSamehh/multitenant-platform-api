using ABCShared.Library.Models.Requests.Identity;
using ABCShared.Library.Wrappers;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Identity.Users.Commands
{

	public record UpdateUserRolesCommand : IRequest<IResponseWrapper>
	{
        public string RoleId { get; set; }
        public UserRolesRequest UpdateUserRoles { get; set; }
    }

	public class UpdateUserRolesCommandHandler : IRequestHandler<UpdateUserRolesCommand , IResponseWrapper>
	{
        private readonly IUserService _userService;

        public UpdateUserRolesCommandHandler(IUserService userService)
        {
            _userService=userService;
        }
        public async Task<IResponseWrapper> Handle(UpdateUserRolesCommand request, CancellationToken cancellationToken)
		{
			var userId = await _userService.AssignRolesAsync(request.RoleId, request.UpdateUserRoles);
            return await ResponseWrapper<string>.SuccessAsync(userId, message: "User roles updated successfully.");
        }
	}
}
