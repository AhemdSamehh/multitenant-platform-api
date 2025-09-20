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

	public record GetAllUsersQuery : IRequest<IResponseWrapper>
	{
    }

	public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, IResponseWrapper>
	{
        private readonly IUserService _userService;

        public GetAllUsersQueryHandler(IUserService userService)
        {
            _userService=userService;
        }
        public async Task<IResponseWrapper> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
		{
			var users = await _userService.GetAllAsync(cancellationToken);
            return await ResponseWrapper<List<UserResponse>>.SuccessAsync(users, message: "Users retrieved successfully.");
        }
	}
}
