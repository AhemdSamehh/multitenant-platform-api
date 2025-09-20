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

	public record GetSchoolsQuery : IRequest<IResponseWrapper>
	{

	}

	public class GetSchoolsQueryHandler : IRequestHandler<GetSchoolsQuery , IResponseWrapper>
	{
        private readonly ISchoolService _schoolService;

        public GetSchoolsQueryHandler(ISchoolService schoolService)
        {
            _schoolService=schoolService;
        }
        public async Task<IResponseWrapper> Handle(GetSchoolsQuery request, CancellationToken cancellationToken)
        {
            var schoolInDb = await _schoolService.GetAllAsync();
            var mappedSchools = schoolInDb.Adapt<List<SchoolResponse>>();
            if (schoolInDb?.Count> 0)
            {
                return await ResponseWrapper<List<SchoolResponse>>.SuccessAsync(mappedSchools);
            }
               return await ResponseWrapper<List<SchoolResponse>>.FailAsync("No schools found.");
        }

	}
}
