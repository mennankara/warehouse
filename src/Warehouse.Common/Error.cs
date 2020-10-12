using System.Collections.Generic;

namespace Warehouse.Common
{
    public class Error
    {
        public Error(string message, Dictionary<string, string> data = null)
        {
            Message = message;
            AdditionalData = data;
        }

        public string Message { get; set; }

        public Dictionary<string, string> AdditionalData { get; set; }
    }
}
