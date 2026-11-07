using API.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace API.Middleware;

public class ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
{
	public async Task InvokeAsync(HttpContext context)
	{
		try
		{
			await next(context);
		}
		catch (NotFoundException e)
		{
			await Write(context, StatusCodes.Status404NotFound, "Not Found", e.Message);
		}
		catch (ConflictException e)
		{
			await Write(context, StatusCodes.Status409Conflict, "Conflict", e.Message);
		}
		catch (Exception e)
		{
			logger.LogError(e, "Unhandled error");
			await Write(context, StatusCodes.Status500InternalServerError, "Server Error",
				"Terjadi kesalahan di server.");
		}
	}

	private static Task Write(HttpContext context, int status, string title, string detail)
	{
		context.Response.StatusCode = status;
		var problem = new ProblemDetails { Status = status, Title = title, Detail = detail };
		return context.Response.WriteAsJsonAsync(problem);
	}
}
