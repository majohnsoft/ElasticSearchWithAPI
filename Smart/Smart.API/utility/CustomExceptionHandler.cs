using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Smart.API.utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Smart.Business.utility
{
    public class CustomExceptionHandler : IExceptionFilter
    {
        private ILogger<CustomExceptionHandler> _logger;

        public CustomExceptionHandler(ILogger<CustomExceptionHandler> logger)
        {
            _logger = logger;
        }

        public void OnException(ExceptionContext context)
        {
            HttpStatusCode statusCode = (context.Exception as WebException != null &&
                           ((HttpWebResponse)(context.Exception as WebException).Response) != null) ?
                            ((HttpWebResponse)(context.Exception as WebException).Response).StatusCode
                            : getErrorCode(context.Exception.GetType());
            string errorMessage = context.Exception.Message;
            string customErrorMessage = "An Error has occurred.";
            string stackTrace = context.Exception.StackTrace;

            HttpResponse response = context.HttpContext.Response;
            response.StatusCode = (int)statusCode;
            response.ContentType = "application/json";
            var result = JsonConvert.SerializeObject(
                new
                {
                    message = customErrorMessage,
                    isError = true,
                    errorMessage = errorMessage,
                    errorCode = statusCode,
                    model = string.Empty
                });

            _logger.LogInformation(result); 
            response.ContentLength = result.Length;
            response.WriteAsync(result);
        }
        private HttpStatusCode getErrorCode(Type exceptionType)
        {
            Exceptions tryParseResult;
            if (Enum.TryParse<Exceptions>(exceptionType.Name, out tryParseResult))
            {
                switch (tryParseResult)
                {
                    case Exceptions.NullReferenceException:
                        return HttpStatusCode.LengthRequired;

                    case Exceptions.FileNotFoundException:
                        return HttpStatusCode.NotFound;

                    case Exceptions.OverflowException:
                        return HttpStatusCode.RequestedRangeNotSatisfiable;

                    case Exceptions.OutOfMemoryException:
                        return HttpStatusCode.ExpectationFailed;

                    case Exceptions.InvalidCastException:
                        return HttpStatusCode.PreconditionFailed;

                    case Exceptions.ObjectDisposedException:
                        return HttpStatusCode.Gone;

                    case Exceptions.UnauthorizedAccessException:
                        return HttpStatusCode.Unauthorized;

                    case Exceptions.NotImplementedException:
                        return HttpStatusCode.NotImplemented;

                    case Exceptions.NotSupportedException:
                        return HttpStatusCode.NotAcceptable;

                    case Exceptions.InvalidOperationException:
                        return HttpStatusCode.MethodNotAllowed;

                    case Exceptions.TimeoutException:
                        return HttpStatusCode.RequestTimeout;

                    case Exceptions.ArgumentException:
                        return HttpStatusCode.BadRequest;

                    case Exceptions.StackOverflowException:
                        return HttpStatusCode.RequestedRangeNotSatisfiable;

                    case Exceptions.FormatException:
                        return HttpStatusCode.UnsupportedMediaType;

                    case Exceptions.IOException:
                        return HttpStatusCode.NotFound;

                    case Exceptions.IndexOutOfRangeException:
                        return HttpStatusCode.ExpectationFailed;

                    default:
                        return HttpStatusCode.InternalServerError;
                }
            }
            else
            {
                return HttpStatusCode.InternalServerError;
            }
        }

    }
}
