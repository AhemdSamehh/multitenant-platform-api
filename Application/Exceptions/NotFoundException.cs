using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Application.Exceptions
{
    public class NotFoundException : Exception
    {
        public List<string> ErrorMessages { get; set; }
        public HttpStatusCode StatusCode { get; set; }

        public NotFoundException( List<string> errorMessages = default, HttpStatusCode statusCode = HttpStatusCode.NotFound)
            
        {
            ErrorMessages = errorMessages;
            StatusCode = statusCode;
        }
    }
}
