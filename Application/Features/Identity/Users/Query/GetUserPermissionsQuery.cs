using ABCShared.Library.Wrappers;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Identity.Users.Query
{

	public record GetUserPermissionsQuery : IRequest<IResponseWrapper>
	{
        public string UserId { get; set; }
    }

	public class GetUserPermissionsQueryHandler : IRequestHandler<GetUserPermissionsQuery, IResponseWrapper>
	{
        private readonly IUserService _userService;

        public GetUserPermissionsQueryHandler(IUserService userService)
        {
            _userService=userService;
        }
        public async Task<IResponseWrapper> Handle(GetUserPermissionsQuery request, CancellationToken cancellationToken)
		{
			var permissions = await _userService.GetUserPermissionsAsync(request.UserId, cancellationToken);
            return await ResponseWrapper<List<string>>.SuccessAsync(permissions, message: "User permissions retrieved successfully.");
        }
	}
}
