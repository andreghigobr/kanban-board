using Kanban.Api.Common.Errors;
using Microsoft.AspNetCore.Mvc;

namespace Kanban.Api.Common.Extensions;

public static class ExceptionHandlingExtensions
{
    public static IServiceCollection AddGlobalExceptionHandling(this IServiceCollection services)
    {
        services.AddControllers(options =>
        {
            options.Filters.Add<GlobalExceptionFilter>();
        });

        return services;
    }
}
