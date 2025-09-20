using ABCShared.Library.Constants;
using ABCShared.Library.Models.Requests.Token;
using Application.Features.Identity.Tokens;
using Application.Features.Identity.Tokens.Queries;
using Infrastructure.Constants;
using Infrastructure.OpenApi;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TokenController : BaseApiController
    {
        [HttpPost("login")]
        [AllowAnonymous]
        [TenantHeader]
        [OpenApiOperation("Used to obtain jwt for")]
        public async Task<IActionResult> GetTokenAsync([FromBody] TokenRequest tokenRequest)
        {
            var response = await Sender.Send(new GetTokenQuery { tokenRequest =  tokenRequest });
            if(response.IsSuccessful)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpPost("Refresh-Token")]
        [ShouldHavePermission(SchoolAction.RefreshToken , SchoolFeature.Tokens)]
        public async Task<IActionResult> GetRefreshTokenAsync([FromBody] RefreshTokenRequest refreshTokenRequest)
        {
            var response = await Sender.Send(new GetRefreshTokenQuery { refreshTokenRequest = refreshTokenRequest });
            if (response.IsSuccessful)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }
    }
}
