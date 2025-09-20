using ABCShared.Library.Models.Requests.Token;
using ABCShared.Library.Models.Responses.Token;
using ABCShared.Library.Wrappers;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Identity.Tokens.Queries
{

	public record GetRefreshTokenQuery : IRequest<IResponseWrapper>
	{
        public RefreshTokenRequest  refreshTokenRequest { get; set; }
    }

	public class GetRefreshTokenQueryHandler : IRequestHandler<GetRefreshTokenQuery, IResponseWrapper>
	{
        private readonly ITokenService _tokenService;

        public GetRefreshTokenQueryHandler(ITokenService tokenService)
        {
            _tokenService=tokenService;
        }
        public async Task<IResponseWrapper> Handle(GetRefreshTokenQuery request, CancellationToken cancellationToken)
		{
            var refreshToken = await _tokenService.RefreshTokenAsync(request.refreshTokenRequest);

            return await ResponseWrapper<TokenResponse>.SuccessAsync(refreshToken);
		}
	}
}
