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

	public record UpdateUserCommand : IRequest<IResponseWrapper>
	{
        public UpdateUserRequest  UpdateUser { get; set; }
    }

	public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand , IResponseWrapper>
	{
        private readonly IUserService _userService;

        public UpdateUserCommandHandler(IUserService userService)
        {
            _userService=userService;
        }
        public async Task<IResponseWrapper> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
		{
            var userId = await _userService.UpdateAsync(request.UpdateUser);
            return await ResponseWrapper<string>.SuccessAsync(userId, message: "User updated successfully.");
        }
	}
}
