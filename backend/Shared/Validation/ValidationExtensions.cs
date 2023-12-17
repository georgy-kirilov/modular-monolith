using FluentValidation.Results;
using Microsoft.AspNetCore.Http;

namespace Shared.Validation;

public static class ValidationExtensions
{
    public static IResult ToValidationProblem(this ValidationFailure validationFailure)
    {
        var errorsByFieldName = new Dictionary<string, Error[]>
        {
            [validationFailure.PropertyName] = [ new Error(validationFailure.ErrorCode, validationFailure.ErrorMessage) ]
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

    public static IResult ToValidationProblem(this ValidationResult validationResult)
    {
        var errorsByFieldName = validationResult
            .Errors
            .GroupBy(x => x.PropertyName)
            .ToDictionary(x => x.Key, x => x.Select(y => new Error(y.ErrorCode, y.ErrorMessage)).ToArray());

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

    public static IResult ToValidationProblem(this Error error)
    {
        var errorsByFieldName = new Dictionary<string, Error[]>
        {
            [error.Field] = [ new Error(error.Code, error.Message) ]
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
            .ToDictionary(x => x.Key, x => x.Select(y => new Error(y.Code, y.Message)).ToArray());

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

    public static IResult ToOkResult(this object value) => Results.Ok(value);
}

public sealed record Error(string Field, string Code, string Message)
{
    public Error(string code, string message) : this(string.Empty, code, message) { }
}
