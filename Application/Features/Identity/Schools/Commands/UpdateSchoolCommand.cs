using ABCShared.Library.Models.Requests.Schools;
using ABCShared.Library.Wrappers;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Identity.Schools.Commands
{

	public record UpdateSchoolCommand : IRequest<IResponseWrapper>
	{
        public UpdateSchoolRequest  updateSchool { get; set; }
    }

	public class UpdateSchoolCommandHandler : IRequestHandler<UpdateSchoolCommand , IResponseWrapper>
	{
        private readonly ISchoolService _schoolService;

        public UpdateSchoolCommandHandler(ISchoolService schoolService)
        {
            _schoolService=schoolService;
        }
        public async Task<IResponseWrapper> Handle(UpdateSchoolCommand request, CancellationToken cancellationToken)
		{
			var schoolInDb = await _schoolService.GetByIdAsync(request.updateSchool.Id);
            if (schoolInDb is not null)
            {
                schoolInDb.Name = request.updateSchool.Name;
                schoolInDb.EstablishedDate = request.updateSchool.EstablishedDate;
                var updatedSchool = await _schoolService.UpdateAsync(schoolInDb);
                return await ResponseWrapper<int>.SuccessAsync(updatedSchool);
            }
            return await ResponseWrapper<int>.FailAsync("School not found");
        }
	}
}
