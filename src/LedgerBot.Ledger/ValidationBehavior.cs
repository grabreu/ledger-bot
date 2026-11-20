using Desfecho;
using FluentValidation;
using Mediator;

namespace LedgerBot.Ledger;

public sealed class ValidationBehavior<TMessage, TResponse>(IEnumerable<IValidator<TMessage>> validators) : IPipelineBehavior<TMessage, TResponse> where TMessage : IMessage
{
    public async ValueTask<TResponse> Handle(TMessage message, MessageHandlerDelegate<TMessage, TResponse> next, CancellationToken cancellationToken)
    {
        if (!validators.Any())
        {
            return await next(message, cancellationToken);
        }

        var validationResults = await Task.WhenAll(
            validators.Select(validator => validator.ValidateAsync(new ValidationContext<TMessage>(message), cancellationToken)));

        var failures = validationResults
            .SelectMany(result => result.Errors)
            .Where(failure => failure is not null)
            .ToList();

        if (failures.Count > 0)
        {
            var errors = new ErrorList(failures.Select(failure => Result.Invalid(failure.PropertyName, failure.ErrorMessage)));
            return (dynamic)errors;
        }

        return await next(message, cancellationToken);
    }
}
