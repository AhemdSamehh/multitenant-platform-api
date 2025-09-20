using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Identity
{
    public static class IdentityHelper
    {
        public static List<string> GetIdentityResultErrorDescription(IdentityResult identityResult)
        {
            var errorDescription = new List<string>();
            foreach (var error in identityResult.Errors)
            {
                errorDescription.Add(error.Description);
            }
            return errorDescription;
        }
    }
}
