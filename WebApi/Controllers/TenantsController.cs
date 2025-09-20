using Application.Features.Identity.Tenancy;
using Application.Features.Identity.Tenancy.Commands;
using Application.Features.Identity.Tenancy.Query;
using Infrastructure.Constants;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ABCShared.Library.Constants;
using ABCShared.Library.Models.Requests.Tenancy;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TenantsController : BaseApiController
    {
        [HttpPost("add")]
        [ShouldHavePermission(SchoolAction.Create , SchoolFeature.Tenants)]
        public async Task<IActionResult> CreateTenantAsync(CreateTenantRequest createTenant)
        {
            var response = await Sender.Send(new CreateTenantCommand { CreateTenant = createTenant });

            if(response.IsSuccessful)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpPut("{tenantId}/activate")]
        [ShouldHavePermission(SchoolAction.Update, SchoolFeature.Tenants)]

        public async Task<IActionResult> ActivateTenantAsync(string tenantId)
        {
            var response = await Sender.Send(new ActivateTenantCommand { TenantId = tenantId });
            if(response.IsSuccessful)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }


        [HttpPut("{tenantId}/deactivate")]
        [ShouldHavePermission(SchoolAction.Update, SchoolFeature.Tenants)]

        public async Task<IActionResult> DeactivateTenantAsync(string tenantId)
        {
            var response = await Sender.Send(new DeactivateTenantCommand { TenantId = tenantId });
            if (response.IsSuccessful)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpPut("upgrade")]
        [ShouldHavePermission(SchoolAction.UpgradeSubscription, SchoolFeature.Tenants)]

        public async Task<IActionResult> UpgradeTenantSubscriptionAsync(UpdateTenantSubscriptionRequest updateTenantSubscription)
        {
            var response = await Sender.Send(new UpdateTenantSubscriptionCommand {UpdateTenantSubscription = updateTenantSubscription });
            if (response.IsSuccessful)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpGet("{tenantId}")]
        [ShouldHavePermission(SchoolAction.Read, SchoolFeature.Tenants)]

        public async Task<IActionResult> GetTenantByIdAsync(string tenantId)
        {
            var response = await Sender.Send(new GetTenantByIdQuery { TenantId = tenantId });
            if (response.IsSuccessful)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }


        [HttpGet("all")]
        [ShouldHavePermission(SchoolAction.Read, SchoolFeature.Tenants)]

        public async Task<IActionResult> GetTenantsAsync()
        {
            var response = await Sender.Send(new GetTenantQuery());
            if (response.IsSuccessful)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }
    }
}
