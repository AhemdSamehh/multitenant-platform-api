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

	public record UpdateUserStatusCommand : IRequest<IResponseWrapper>
	{
        public ChangeUserStatusRequest ChangeUserStatus { get; set; }
    }

	public class UpdateUserStatusCommandHandler : IRequestHandler<UpdateUserStatusCommand , IResponseWrapper>
	{
        private readonly IUserService _userService;

        public UpdateUserStatusCommandHandler(IUserService userService)
        {
            _userService=userService;
        }
        public async Task<IResponseWrapper> Handle(UpdateUserStatusCommand request, CancellationToken cancellationToken)
		{
			var userId = await _userService.ActivateOrDeactivateAsync(request.ChangeUserStatus.UserId , request.ChangeUserStatus.Activation);
            return await ResponseWrapper<string>.SuccessAsync(userId, message: "User status updated successfully.");
        }
	}
}
