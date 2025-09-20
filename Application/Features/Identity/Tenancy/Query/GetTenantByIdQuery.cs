using ABCShared.Library.Models.Responses.Tenancy;
using ABCShared.Library.Wrappers;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Identity.Tenancy.Query
{

	public record GetTenantByIdQuery : IRequest<IResponseWrapper>
	{
        public string TenantId { get; set; }
    }

	public class GetTenantByIdQueryHandler : IRequestHandler<GetTenantByIdQuery, IResponseWrapper>
	{
        private readonly ITenantService _tenantService;

        public GetTenantByIdQueryHandler(ITenantService tenantService)
        {
            _tenantService=tenantService;
        }
        public async Task<IResponseWrapper> Handle(GetTenantByIdQuery request, CancellationToken cancellationToken)
		{
			var tenantInDb = await _tenantService.GetTenantByIdAsync(request.TenantId);
            if(tenantInDb is not null)
            {
                return await ResponseWrapper<TenantResponse>.SuccessAsync(tenantInDb);
            }
            return await ResponseWrapper<TenantResponse>.FailAsync("Tenant dose not exist");

        }
    }
}
