using ABCShared.Library.Models.Requests.Tenancy;
using ABCShared.Library.Models.Responses.Tenancy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Identity.Tenancy
{
    public interface ITenantService
    {
        Task<string> CreateTenantAsync(CreateTenantRequest createTenant, CancellationToken cancellationToken);
        Task<string> ActivateAsync(string id);
        Task<string> DeactivateAsync(string id);
        Task<string> UpdateSubscriptionAsync(UpdateTenantSubscriptionRequest updateTenantSubscription);
        Task<List<TenantResponse>> GetTenantAsync();

        Task<TenantResponse> GetTenantByIdAsync(string id);
    }
}
