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

	public record UpdateTenantSubscriptionCommand : IRequest<IResponseWrapper>
	{
        public UpdateTenantSubscriptionRequest UpdateTenantSubscription { get; set; }
    }

	public class UpdateTenantSubscriptionCommandHandler : IRequestHandler<UpdateTenantSubscriptionCommand , IResponseWrapper>
	{
        private readonly ITenantService _tenantService;

        public UpdateTenantSubscriptionCommandHandler(ITenantService tenantService)
        {
            _tenantService=tenantService;
        }
        public async Task<IResponseWrapper> Handle(UpdateTenantSubscriptionCommand request, CancellationToken cancellationToken)
		{
            var tenantId = await _tenantService.UpdateSubscriptionAsync(request.UpdateTenantSubscription);
           return await ResponseWrapper<string>.SuccessAsync(tenantId , "Tenant Subscription Updated Successfully");
		}
	}
}
