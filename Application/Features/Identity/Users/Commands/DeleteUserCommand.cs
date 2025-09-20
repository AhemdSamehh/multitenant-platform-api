using ABCShared.Library.Wrappers;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Identity.Users.Commands
{

	public record DeleteUserCommand : IRequest<IResponseWrapper>
	{
        public string UserId { get; set; }
    }

	public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand , IResponseWrapper>
	{
        private readonly IUserService _userService;

        public DeleteUserCommandHandler(IUserService userService)
        {
            _userService=userService;
        }
        public async Task<IResponseWrapper> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
		{
			var userId = await _userService.DeleteAsync(request.UserId);
            return await ResponseWrapper<string>.SuccessAsync(userId, message: "User deleted successfully.");
        }
	}
}
