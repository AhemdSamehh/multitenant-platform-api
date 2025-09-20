using Infrastructure.Tenancy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.OpenApi
{
    public class TenantHeaderAttribute() : SwaggerHeaderAttribute(
        TenancyConstants.TenantIdName,
            "Input Your tenant name to access this API", string.Empty, true)
    {
        //public TenantHeaderAttribute() :
        //    base(TenancyConstants.TenantIdName, description: "Enter Your Tenant Name to access this API", defaultValue : string.Empty, isRequired : true)
        //{
        //}
    }
}
