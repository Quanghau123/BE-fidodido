using System.Text.Json.Serialization;
using FluentValidation;
using FluentValidation.AspNetCore;
using FidoDino.Application.Validation.User;

namespace FidoDino.API.Extensions;

public static class ControllerExtension
{
    public static IServiceCollection AddControllersWithValidation(
        this IServiceCollection services)
    {
        services.AddControllers()
            .AddJsonOptions(o =>
            {
                o.JsonSerializerOptions.Converters.Add(
                    new JsonStringEnumConverter());
            });

        services.AddFluentValidationAutoValidation();
        services.AddFluentValidationClientsideAdapters();
        services.AddValidatorsFromAssemblyContaining<CreateUserRequestValidator>();

        return services;
    }
}
