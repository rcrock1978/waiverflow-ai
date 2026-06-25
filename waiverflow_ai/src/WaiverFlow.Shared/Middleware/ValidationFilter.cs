using FluentValidation;
using System.Net;
using System.Text.Json;

namespace WaiverFlow.Shared.Middleware;

public class ValidationFilter<T> : IEndpointFilter
{
    private readonly IValidator<T> _validator;

    public ValidationFilter(IValidator<T> validator) => _validator = validator;

    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var model = context.Arguments.OfType<T>().FirstOrDefault();
        if (model is null) return await next(context);

        var result = await _validator.ValidateAsync(model);
        if (result.IsValid) return await next(context);

        return Results.Problem(
            statusCode: (int)HttpStatusCode.BadRequest,
            type: "https://tools.ietf.org/html/rfc7807",
            title: "Validation Failed",
            detail: JsonSerializer.Serialize(result.Errors.Select(e => new
            {
                field = e.PropertyName,
                error = e.ErrorMessage
            }))
        );
    }
}
