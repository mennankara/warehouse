using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Warehouse.Common
{
    public class ResultBuilder : ResultBuilder<object>
    {
        public ResultBuilder FromResult(Result result)
        {
            if (result.IsSuccessful)
            {
                WithSuccess();
            }
            else
            {
                WithFailure(result.FailureStatusCode);
                WithErrors(result.Errors);
            }

            return this;
        }

        public new Result Build()
        {
            return base.Build();
        }
    }

    public class ResultBuilder<T>
    {
        private readonly Result<T> _result;

        public ResultBuilder()
        {
            _result = new Result<T>();
        }

        public ResultBuilder<T> FromResult<T2>(Result<T2> result)
        {
            if (result.IsSuccessful)
            {
                WithSuccess();
            }
            else
            {
                WithFailure(result.FailureStatusCode);
                WithErrors(result.Errors);
            }

            return this;
        }

        public ResultBuilder<T> FromException(WarehouseException exception)
        {
            WithFailure(HttpStatusCode.InternalServerError);
            WithError(exception.Message);
            return this;
        }

        public virtual ResultBuilder<T> WithData(T data)
        {
            _result.Response = data;
            return this;
        }

        public ResultBuilder<T> WithSuccess()
        {
            _result.IsSuccessful = true;
            return this;
        }

        public ResultBuilder<T> WithFailure(HttpStatusCode statusCode)
        {
            _result.IsSuccessful = false;
            _result.FailureStatusCode = statusCode;
            return this;
        }

        public ResultBuilder<T> WithError(string message, Dictionary<string, string> data = null)
        {
            _result.Errors.Add(new Error(message, data));
            return this;
        }

        public ResultBuilder<T> WithErrors(IEnumerable<Error> errors)
        {
            _result.Errors.AddRange(errors);
            return this;
        }

        public bool HasErrors()
        {
            return _result.Errors.Any();
        }

        public virtual Result<T> Build()
        {
            return _result;
        }
    }
}
