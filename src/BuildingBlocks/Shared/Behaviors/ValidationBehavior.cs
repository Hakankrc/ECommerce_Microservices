using FluentValidation;
using MediatR;

namespace Shared.Behaviors;

/// <summary>
/// A pipeline behavior that intercepts requests before they reach the handler.
/// It validates the request utilizing FluentValidation rules.
/// </summary>
public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        // 1. Check if there are any validators attached to this request type
        if (!_validators.Any())
        {
            return await next();
        }

        // 2. Create the validation context
        var context = new ValidationContext<TRequest>(request);

        // 3. Execute all validators asynchronously
        var validationResults = await Task.WhenAll(_validators.Select(v => v.ValidateAsync(context, cancellationToken)));
        
        // 4. Collect all failures
        var failures = validationResults.SelectMany(r => r.Errors).Where(f => f != null).ToList();

        // 5. If there are failures, stop the pipeline and throw an exception
        // This exception will be caught by the GlobalExceptionHandler
        if (failures.Count != 0)
        {
            throw new ValidationException(failures);
        }

        // 6. If valid, proceed to the next step (the actual CommandHandler)
        return await next();
    }
}