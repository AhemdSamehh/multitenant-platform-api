using ABCShared.Library.Constants;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Constants
{
    public class ShouldHavePermissionAttribute : AuthorizeAttribute
    {
        public ShouldHavePermissionAttribute(string action , string feature)
        {
            Policy = SchoolPermission.NameFor(action, feature);
        }
    }
}
