using ABCShared.Library.Models.Requests.Tenancy;
using ABCShared.Library.Wrappers;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Identity.Tenancy.Commands
{

	public record CreateTenantCommand : IRequest<IResponseWrapper>
	{
        public CreateTenantRequest  CreateTenant { get; set; }
    }

	public class CreateTenantCommandHandler : IRequestHandler<CreateTenantCommand , IResponseWrapper>
	{
        private readonly ITenantService _tenantService;

        public CreateTenantCommandHandler(ITenantService tenantService)
        {
            this._tenantService=tenantService;
        }
        public async Task<IResponseWrapper> Handle(CreateTenantCommand request, CancellationToken cancellationToken)
		{
            var tenantId = await _tenantService.CreateTenantAsync(request.CreateTenant , cancellationToken);

            return await ResponseWrapper<string>.SuccessAsync(tenantId);
		}
	}
}
