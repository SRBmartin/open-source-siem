using FluentValidation;
using EBus.Abstractions;
using Siem.Platform.Shared.Application.Abstractions.Common.Http;

namespace Siem.Platform.Logging.Application.Utils;

public sealed class ValidationBehavior<TRequest, TResponse> : IPipelineBehaviour<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
        => _validators = validators;

    public async Task<TResponse> Handle(
        TRequest request,
        CancellationToken cancellationToken,
        RequestHandlerDelegate<TResponse> next)
    {
        if (!_validators.Any())
            return await next();

        var errors = new List<Error>();

        foreach (var v in _validators)
        {
            var res = await v.ValidateAsync(request, cancellationToken);
            if (!res.IsValid)
            {
                errors.AddRange(res.Errors.Select(e =>
                    new Error(e.ErrorCode ?? "validation.error", e.ErrorMessage)));
            }
        }

        if (errors.Count > 0)
            return ResultFactory.CreateFailure<TResponse>(errors);

        return await next();
    }

    private static class ResultFactory
    {
        public static TRes CreateFailure<TRes>(IEnumerable<Error> errs)
        {
            var t = typeof(TRes);

            if (t == typeof(Result))
                return (TRes)(object)Result.Failure(errs);

            if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Result<>))
            {
                var closed = typeof(Result<>).MakeGenericType(t.GetGenericArguments());
                var m = closed.GetMethod("Failure", new[] { typeof(IEnumerable<Error>) })!;
                return (TRes)m.Invoke(null, new object[] { errs })!;
            }

            throw new InvalidOperationException(
                $"ValidationBehavior expects TResponse to be Result or Result<T>, but was {t.FullName}.");
        }
    }
}