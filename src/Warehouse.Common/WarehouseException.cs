using System;
using System.Runtime.Serialization;

namespace Warehouse.Common
{
    public class WarehouseException : Exception
    {
        public WarehouseException()
        {
        }

        public WarehouseException(string message) : base(message)
        {
        }

        public WarehouseException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected WarehouseException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
