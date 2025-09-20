using ABCShared.Library.Models.Requests.Tenancy;
using ABCShared.Library.Models.Responses.Tenancy;
using Application.Exceptions;
using Application.Features.Identity.Tenancy;
using Azure.Core;
using Finbuckle.MultiTenant;
using Finbuckle.MultiTenant.Abstractions;
using Infrastructure.Contexts;
using Mapster;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Tenancy
{
    public class TenantService : ITenantService
    {
        private readonly IMultiTenantStore<ABCSchoolTenantInfo> _tenantStore;
        private readonly ApplicationDbSeeder _applicationDbSeeder;
        private readonly IServiceProvider _serviceProvider;

        public TenantService(IMultiTenantStore<ABCSchoolTenantInfo> tenantStore  , ApplicationDbSeeder applicationDbSeeder , IServiceProvider serviceProvider)
        {
           _tenantStore=tenantStore;
            _applicationDbSeeder=applicationDbSeeder;
           _serviceProvider=serviceProvider;
        }
        public async Task<string> ActivateAsync(string id)
        {
            var tenantInDb = await _tenantStore.TryGetAsync(id);
            tenantInDb.IsActive = true;

            await _tenantStore.TryUpdateAsync(tenantInDb);
            return tenantInDb.Identifier;
        }

        public async Task<string> CreateTenantAsync(CreateTenantRequest request, CancellationToken cancellationToken)
        {
            // Check if tenant already exists
            var existingTenant = await _tenantStore.TryGetAsync(request.Identifier);
            if (existingTenant != null)
            {
                throw new ConflictException(new List<string> { $"Tenant with identifier '{request.Identifier}' already exists." });
            }

            var newTenant = new ABCSchoolTenantInfo
            {
                Id = request.Identifier,
                Identifier = request.Identifier,
                Name = request.Name,
                IsActive = request.IsActive,
                ConnectionString = request.ConnectionString,
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                ValidUpTo = request.ValidUpTo,
            };
            
            var addResult = await _tenantStore.TryAddAsync(newTenant);
            if (!addResult)
            {
                throw new ConflictException(new List<string> { $"Failed to add tenant '{request.Identifier}' to the store." });
            }

            // seeding tenant data 
            using var scope = _serviceProvider.CreateScope();

            scope.ServiceProvider.GetRequiredService<IMultiTenantContextSetter>()
                .MultiTenantContext = new MultiTenantContext<ABCSchoolTenantInfo>()
                {
                    TenantInfo = newTenant,
                };
            await scope.ServiceProvider.GetRequiredService<ApplicationDbSeeder>().InitializeDatabaseAsync(cancellationToken);

            return newTenant.Identifier;
        }

        public async Task<string> DeactivateAsync(string id)
        {
            var tenantInDb = await _tenantStore.TryGetAsync(id);
            tenantInDb.IsActive = false;
            await _tenantStore.TryUpdateAsync(tenantInDb);
            return tenantInDb.Identifier;
        }

        public async Task<List<TenantResponse>> GetTenantAsync()
        {
            var tenantInDb = await _tenantStore.GetAllAsync();
            return tenantInDb.Adapt<List<TenantResponse>>();
        }

        public async Task<TenantResponse> GetTenantByIdAsync(string id)
        {
            var tenantInDb = await _tenantStore.TryGetAsync(id);

            // Manual Mapping
            #region Manual Mapping
            //var tenantResponse = new TenantResponse
            //{
            //    Identifier = tenantInDb.Identifier,
            //    Name = tenantInDb.Name,
            //    IsActive = tenantInDb.IsActive,
            //    ConnectionString = tenantInDb.ConnectionString,
            //    Email = tenantInDb.Email,
            //    FirstName = tenantInDb.FirstName,
            //    LastName = tenantInDb.LastName,
            //    ValidUpTo = tenantInDb.ValidUpTo,
            //}; 
            #endregion

            //Mapster
            return tenantInDb.Adapt<TenantResponse>();
        }

        public async Task<string> UpdateSubscriptionAsync(UpdateTenantSubscriptionRequest updateTenantSubscription)
        {
            var tenantInDb = await _tenantStore.TryGetAsync(updateTenantSubscription.TenantId);
            tenantInDb.ValidUpTo = updateTenantSubscription.NewExpiryDate;
            await _tenantStore.TryUpdateAsync(tenantInDb);
            return tenantInDb.Identifier;
        }
    }
}
