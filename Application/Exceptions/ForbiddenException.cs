using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Application.Exceptions
{
    public class ForbiddenException : Exception
    {
        public List<string> ErrorMessages { get; set; }
        public HttpStatusCode StatusCode { get; set; }

        public ForbiddenException( List<string> errorMessages = default, HttpStatusCode statusCode = HttpStatusCode.Forbidden)
            
        {
            ErrorMessages = errorMessages;
            StatusCode = statusCode;
        }
    }
}
