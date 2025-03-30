
using APICatalogo.Models;

namespace APICatalogo.Extensions;

public static class ApiExceptionMiddlewareExtensions
{
	public static void ConfigureExceptionHandler(this IApplicationBuilder app)
	{
		app.UseExceptionHandler(appError =>
		{
			appError.Run(async context =>
			{
				context.Response.StatusCode = (int)System.Net.HttpStatusCode.InternalServerError;
				context.Response.ContentType = "application/json";
				var contextFeature = context.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerFeature>();
				if (contextFeature != null)
				{
					await context.Response.WriteAsync(new ErrorDetails()
					{
						StatusCode = context.Response.StatusCode,
						Message = contextFeature.Error.Message,
						Trace = contextFeature.Error.StackTrace
					}.ToString());
				}
			});
		});
	}
}