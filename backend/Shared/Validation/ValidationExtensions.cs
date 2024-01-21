using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;

namespace Shared.Validation;

public static class ValidationExtensions
{
    public static IResult ToOkResult(this object value) => Results.Ok(value);

    public static Error[] ToErrorsArray(this ValidationResult validationResult) =>
        validationResult
        .Errors
        .Select(err => new Error(err.PropertyName, err.ErrorCode, err.ErrorMessage))
        .ToArray();

    public static IResult ToValidationProblem(this Error error)
    {
        var errorsByFieldName = new Dictionary<string, Error[]>
        {
            [error.Field] = [ new Error(error.Field, error.Code, error.Message) ]
        };

        return Results.Problem
        (
            statusCode: StatusCodes.Status400BadRequest,
            title: "Bad Request",
            type: "https://tools.ietf.org/html/rfc7231#section-6.5.1",
            extensions: new Dictionary<string, object?>
            {
                ["errors"] = errorsByFieldName
            }
        );
    }

    public static IResult ToValidationProblem(this Error[] errors)
    {
        var errorsByFieldName = errors
            .GroupBy(x => x.Field)
            .ToDictionary(
                x => x.Key,
                x => x.Select(y => new Error(y.Field, y.Code, y.Message)).ToArray());

        return Results.Problem
        (
            statusCode: StatusCodes.Status400BadRequest,
            title: "Bad Request",
            type: "https://tools.ietf.org/html/rfc7231#section-6.5.1",
            extensions: new Dictionary<string, object?>
            {
                ["errors"] = errorsByFieldName
            }
        );
    }

    public static IRuleBuilderOptions<TRequest, TProperty> WithError<TRequest, TProperty>(
        this IRuleBuilderOptions<TRequest, TProperty> options,
        Error error,
        params object[] parameters)
    {
        var formattedMessage = error.Message;

        if (parameters.Length > 0)
        {
            formattedMessage = string.Format(formattedMessage, parameters);
        }

        if (!string.IsNullOrEmpty(formattedMessage))
        {
            options.WithMessage(formattedMessage);
        }

        if (!string.IsNullOrEmpty(error.Code))
        {
            options.WithErrorCode(error.Code);
        }

        if (!string.IsNullOrEmpty(error.Field))
        {
            options.WithName(error.Field);
        }

        return options;
    }
}
