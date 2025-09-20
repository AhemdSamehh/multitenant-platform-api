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

	public record GetTenantQuery : IRequest<IResponseWrapper>
	{

	}

	public class GetTenantQueryHandler : IRequestHandler<GetTenantQuery, IResponseWrapper>
	{
        private readonly ITenantService _tenantService;

        public GetTenantQueryHandler(ITenantService tenantService)
        {
            this._tenantService=tenantService;
        }
        public async Task<IResponseWrapper> Handle(GetTenantQuery request, CancellationToken cancellationToken)
		{
			var tenants = await _tenantService.GetTenantAsync();
			return await ResponseWrapper<List<TenantResponse>>.SuccessAsync(tenants);
		}
	}
}
