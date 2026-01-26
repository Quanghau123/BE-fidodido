using FidoDino.Common;
using FidoDino.Common.Exceptions;

using System.Net;
using System.ComponentModel.DataAnnotations;

namespace FidoDino.API.Middleware
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        public ErrorHandlingMiddleware(RequestDelegate next) => _next = next;

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (OperationCanceledException)
            {
                context.Response.StatusCode = (int)HttpStatusCode.RequestTimeout;
                await context.Response.WriteAsJsonAsync(new ApiResponse<object>(false, "Request was cancelled"));
            }
            catch (UnauthorizedAccessException ex)
            {
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                await context.Response.WriteAsJsonAsync(new ApiResponse<object>(false, ex.Message));
            }
            catch (ForbiddenException ex)
            {
                context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                await context.Response.WriteAsJsonAsync(new ApiResponse<object>(false, ex.Message));
            }
            catch (NotFoundException ex)
            {
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                await context.Response.WriteAsJsonAsync(new ApiResponse<object>(false, ex.Message));
            }
            catch (ConflictException ex)
            {
                context.Response.StatusCode = (int)HttpStatusCode.Conflict;
                await context.Response.WriteAsJsonAsync(new ApiResponse<object>(false, ex.Message));
            }
            catch (ValidationException ex)
            {
                context.Response.StatusCode = 422;
                await context.Response.WriteAsJsonAsync(new ApiResponse<object>(false, ex.Message));
            }
            catch (ArgumentException ex)
            {
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                await context.Response.WriteAsJsonAsync(new ApiResponse<object>(false, ex.Message));
            }
            catch (Exception)
            {
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                await context.Response.WriteAsJsonAsync(new ApiResponse<object>(false, "Internal server error"));
            }
        }
    }
}
