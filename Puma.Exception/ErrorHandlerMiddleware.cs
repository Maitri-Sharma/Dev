using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Puma.CustomException
{
    /// <summary>
    /// Middleware to handle all types of exception
    /// </summary>
    public class ErrorHandlerMiddleware
    {
        private readonly RequestDelegate _next;

        public ErrorHandlerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (PumaException error)
            {
                await HandleExceptionAsync(context, error);

            }
            catch (System.Exception error)
            {
               await HandleExceptionAsync(context,error);

            }
          
        }

        private Task HandleExceptionAsync(HttpContext context, System.Exception exception)
        {
            PumaExceptionModel pumaExceptionModel = new PumaExceptionModel();

            string result;
            if (exception.GetType() == typeof(PumaException))
            {
                var pumaException = (PumaException)exception;

                pumaExceptionModel.Code = (int)pumaException.statusCode;
                if (pumaException.pumaExceptionModels?.Any() == true)
                {
                    result = JsonConvert.SerializeObject(pumaException.pumaExceptionModels);
                }
                else
                {
                    pumaExceptionModel.Message = pumaException.Message;
                    result = JsonConvert.SerializeObject(pumaExceptionModel);

                }
            }
            else {
                pumaExceptionModel.Code = (int)HttpStatusCode.InternalServerError;
                pumaExceptionModel.Message = exception.Message;
                result = JsonConvert.SerializeObject(pumaExceptionModel);
            }

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            return context.Response.WriteAsync(result);
        }
    }
}
