using ABCShared.Library.Wrappers;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Identity.Schools.Commands
{

	public record DeleteSchoolCommand : IRequest<IResponseWrapper>
	{
        public int SchoolId { get; set; }
    }

	public class DeleteSchoolCommandHandler : IRequestHandler<DeleteSchoolCommand , IResponseWrapper>
	{
        private readonly ISchoolService _schoolService;

        public DeleteSchoolCommandHandler(ISchoolService schoolService)
        {
            _schoolService=schoolService;
        }
        public async Task<IResponseWrapper> Handle(DeleteSchoolCommand request, CancellationToken cancellationToken)
		{
			var schoolInDb = await _schoolService.GetByIdAsync(request.SchoolId);
            if(schoolInDb is not null)
            {
                var deleteSchoolId = await _schoolService.DeleteAsync(schoolInDb);
                return await ResponseWrapper<int>.SuccessAsync(deleteSchoolId, "School deleted successfully.");
            }
            return await ResponseWrapper<int>.FailAsync("School not found.");

        }
	}
}
