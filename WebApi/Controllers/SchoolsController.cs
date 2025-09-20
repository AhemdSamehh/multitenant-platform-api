using Application.Features.Identity.Schools;
using Application.Features.Identity.Schools.Commands;
using Application.Features.Identity.Schools.Queries;
using Infrastructure.Constants;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ABCShared.Library.Constants;
using ABCShared.Library.Models.Requests.Schools;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SchoolsController : BaseApiController
    {
        [HttpPost("add")]
        [ShouldHavePermission(SchoolAction.Create , SchoolFeature.Schools)]
        public async Task<IActionResult> CreateSchoolAsync(CreateSchoolRequest createSchool)
        {
            var response = await Sender.Send(new CreateSchoolCommands { createSchool = createSchool });
            if (response.IsSuccessful)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpPut("update")]
        [ShouldHavePermission(SchoolAction.Update, SchoolFeature.Schools)]
        public async Task<IActionResult> UpdateSchoolAsync(UpdateSchoolRequest updateSchool)
        {
            var response = await Sender.Send(new UpdateSchoolCommand { updateSchool = updateSchool });
                if(response.IsSuccessful)
            {
              return Ok(response);
            }
                return BadRequest(response);
        }

        [HttpDelete("{schoolId}")]
        [ShouldHavePermission(SchoolAction.Delete, SchoolFeature.Schools)]
        public async Task<IActionResult> DeleteSchoolAsync(int schoolId)
        {
            var response = await Sender.Send(new DeleteSchoolCommand { SchoolId = schoolId });
            if(response.IsSuccessful)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpGet("by-id/{schoolId}")]
        [ShouldHavePermission(SchoolAction.Read, SchoolFeature.Schools)]
        public async Task<IActionResult> GetSchoolById(int schoolId)
        {
            var response = await Sender.Send(new GetSchoolByIdQuery { SchoolId = schoolId });
            if (response.IsSuccessful)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpGet("by-Name/{schoolId}")]
        [ShouldHavePermission(SchoolAction.Read, SchoolFeature.Schools)]
        public async Task<IActionResult> GetSchoolByName(string name)
        {
            var response = await Sender.Send(new GetSchoolByNameQuery { Name = name });
            if (response.IsSuccessful)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpGet("all")]
        [ShouldHavePermission(SchoolAction.Read, SchoolFeature.Schools)]
        public async Task<IActionResult> GetAllSchoolQuery()
        {
            var response = await Sender.Send(new GetSchoolsQuery());
            if (response.IsSuccessful)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }
    }
}
