using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Tenancy
{
    public class TenancyConstants
    {
        public const string TenantIdName = "tenant";
        public const string DefaultPassword = "P@ssw0rd@123";
        public const string FirstName = "Ahmed";
        public const string LastName = "Sameh";

        public static class Root
        {
            public const string Name = "root";
            public const string Email = "admin.root@abcSchool.com";
            public const string Id = "root";
        }
    }
}
