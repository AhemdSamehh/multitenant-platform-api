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

	public record ChangeUserPasswordCommand : IRequest<IResponseWrapper>
	{
        public ChangePasswordRequest ChangePassword { get; set; }
    }

	public class ChangeUserPasswordCommandHandler : IRequestHandler<ChangeUserPasswordCommand , IResponseWrapper>
	{
        private readonly IUserService _userService;

        public ChangeUserPasswordCommandHandler(IUserService userService)
        {
           _userService=userService;
        }
        public async Task<IResponseWrapper> Handle(ChangeUserPasswordCommand request, CancellationToken cancellationToken)
		{
			var userId = await _userService.ChangePasswordAsync(request.ChangePassword);
           return await ResponseWrapper<string>.SuccessAsync(userId, message: "Password changed successfully.");
        }
	}
}

