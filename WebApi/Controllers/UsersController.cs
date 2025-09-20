using Application.Features.Identity.Users.Commands;
using Application.Features.Identity.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Infrastructure.Constants;
using Application.Features.Identity.Users.Query;
using ABCShared.Library.Models.Requests.Identity;
using ABCShared.Library.Constants;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : BaseApiController
    {
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> RegisterUSerAsync([FromBody] CreateUserRequest createUserRequest)
        {
            var response = await Sender.Send(new CreateUserCommand { CreateUser = createUserRequest });
            if (response.IsSuccessful)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }
        [HttpPut("update")]
        [ShouldHavePermission(SchoolAction.Update, SchoolFeature.Users)]
        public async Task<IActionResult> UpdateUSerAsync([FromBody] UpdateUserRequest updateUser)
        {
            var response = await Sender.Send(new UpdateUserCommand { UpdateUser = updateUser });
            if (response.IsSuccessful)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }


        [HttpPut("update-status")]
        [ShouldHavePermission(SchoolAction.Update, SchoolFeature.Users)]
        public async Task<IActionResult> ChangeUserStatusAsync([FromBody] ChangeUserStatusRequest changeUserStatus)
        {
            var response = await Sender.Send(new UpdateUserStatusCommand { ChangeUserStatus = changeUserStatus });
            if (response.IsSuccessful)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpPut("update-roles/{roleId}")]
        [ShouldHavePermission(SchoolAction.Update, SchoolFeature.UserRoles)]
        public async Task<IActionResult> UpdateUserAsync([FromBody] UserRolesRequest userRolesRequest, string roleId)
        {
            var response = await Sender.Send(new UpdateUserRolesCommand { UpdateUserRoles = userRolesRequest, RoleId = roleId });
            if (response.IsSuccessful)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpDelete("delete/{userId}")]
        [ShouldHavePermission(SchoolAction.Delete, SchoolFeature.Users)]
        public async Task<IActionResult> DeleteUserAsync(string userId)
        {
            var response = await Sender.Send(new DeleteUserCommand { UserId = userId });
            if (response.IsSuccessful)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpGet("all")]
        [ShouldHavePermission(SchoolAction.Read, SchoolFeature.Users)]

        public async Task<IActionResult> GetUsersAsync()
        {
            var response = await Sender.Send(new GetAllUsersQuery() { });
            if (response.IsSuccessful)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpGet("{userId}")]
        [ShouldHavePermission(SchoolAction.Read, SchoolFeature.Users)]
        public async Task<IActionResult> GetUserByIdAsync(string userId)
        {
            var response = await Sender.Send(new GetUserByIdQuery { UserId = userId });
            if (response.IsSuccessful)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpGet("permission/{userId}")]
        [ShouldHavePermission(SchoolAction.Read, SchoolFeature.RoleClaims)]
        public async Task<IActionResult> GetUserPermissionsAsync(string userId)
        {
            var response = await Sender.Send(new GetUserPermissionsQuery { UserId = userId });
            if (response.IsSuccessful)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpGet("users-roles/{userId}")]
        [ShouldHavePermission(SchoolAction.Read, SchoolFeature.UserRoles)]
        public async Task<IActionResult> GetUserRolesAsync(string userId)
        {
            var response = await Sender.Send(new GetUsersRolesQuery { UserId = userId });
            if (response.IsSuccessful)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }
    }
}
