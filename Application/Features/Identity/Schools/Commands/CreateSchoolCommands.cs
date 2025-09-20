using ABCShared.Library.Models.Requests.Schools;
using ABCShared.Library.Wrappers;
using Domain.Entities;
using Mapster;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Identity.Schools.Commands
{

	public record CreateSchoolCommands : IRequest<IResponseWrapper>
	{
        public CreateSchoolRequest  createSchool { get; set; }
    }

	public class CreateSchoolCommandsHandler : IRequestHandler<CreateSchoolCommands , IResponseWrapper>
	{
        private readonly ISchoolService _schoolService;

        public CreateSchoolCommandsHandler(ISchoolService schoolService)
        {
            _schoolService=schoolService;
        }
        public async Task<IResponseWrapper> Handle(CreateSchoolCommands request, CancellationToken cancellationToken)
		{
            var newSchool = request.createSchool.Adapt<School>();
            var schoolId = await _schoolService.CreateAsync(newSchool);
            return await ResponseWrapper<int>.SuccessAsync(schoolId, "School created successfully.");
        }
	}
}
