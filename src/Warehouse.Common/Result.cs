using System.Collections.Generic;
using System.Net;

namespace Warehouse.Common
{
    public class Result
    {
        public bool IsSuccessful { get; set; }

        public List<Error> Errors { get; set; } = new List<Error>();
        public HttpStatusCode FailureStatusCode { get; set; } = HttpStatusCode.NotImplemented;
    }

    public class Result<T> : Result
    {
        public T Response { get; set; }
    }
}
