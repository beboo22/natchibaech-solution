using System.Net;
using System.Text.Json;
using Travelsite.DTOs;

namespace Travelsite.Middleware
{
    internal class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;
        private readonly IWebHostEnvironment _env;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IWebHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            try
            {
                //take action with req  
                await _next.Invoke(httpContext); // go to 
                // teak action with res

            }

            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                httpContext.Response.ContentType = "application/json";

                var apiBehavior = _env.IsDevelopment() ?
                                new ApiResponse<string>(httpContext.Response.StatusCode, ex.StackTrace, ex.Message)
                                : new ApiResponse(httpContext.Response.StatusCode);



                var srializtion = JsonSerializer.Serialize(apiBehavior);


                await httpContext.Response.WriteAsync(srializtion);
            }
        }
    }
}
