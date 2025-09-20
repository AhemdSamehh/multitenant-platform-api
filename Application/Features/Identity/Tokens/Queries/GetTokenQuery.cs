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

	public record GetTokenQuery : IRequest<IResponseWrapper>
	{
        public TokenRequest  tokenRequest { get; set; }
    }

	public class GetTokenQueryHandler : IRequestHandler<GetTokenQuery, IResponseWrapper>
	{
        private readonly ITokenService _tokenService;

        public GetTokenQueryHandler(ITokenService tokenService)
        {
            _tokenService=tokenService;
        }
        public async Task<IResponseWrapper> Handle(GetTokenQuery request, CancellationToken cancellationToken)
		{
            var token = await _tokenService.LoginAsync(request.tokenRequest);
            return await ResponseWrapper<TokenResponse>.SuccessAsync(token);
		}
	}
}
