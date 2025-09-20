using ABCShared.Library.Models.Responses.Schools;
using ABCShared.Library.Wrappers;
using Mapster;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Identity.Schools.Queries
{

	public record GetSchoolByNameQuery : IRequest<IResponseWrapper>
	{
        public string Name { get; set; }
    }

	public class GetSchoolByNameQueryHandler : IRequestHandler<GetSchoolByNameQuery , IResponseWrapper>
	{
        private readonly ISchoolService _schoolService;

        public GetSchoolByNameQueryHandler(ISchoolService schoolService)
        {
            _schoolService=schoolService;
        }
        public async Task<IResponseWrapper> Handle(GetSchoolByNameQuery request, CancellationToken cancellationToken)
		{
			var schoolInDb = await _schoolService.GetByNameAsync(request.Name);

            if (schoolInDb is not null)
            {
                var mappedSchools = schoolInDb.Adapt<SchoolResponse>();
                return await ResponseWrapper<SchoolResponse>.SuccessAsync(mappedSchools);
            }
            return await ResponseWrapper<SchoolResponse>.FailAsync("School Is not Exist");
        }
	}
}
