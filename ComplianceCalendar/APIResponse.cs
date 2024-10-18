using System;
using System.Collections.Generic;
using System.Net;

namespace ComplianceCalendar.models
{
    public class APIResponse
    {
        public APIResponse()
        {
            var ErrorMessages = new List<string>();

        }

        public bool isSuccess { get; set; }
        public Object Result { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public List<string> ErrorMessages { get; set; }

    }
}
