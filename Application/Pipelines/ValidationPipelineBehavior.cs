using ABCShared.Library.Wrappers;
using FluentValidation;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Pipelines
{
    internal class ValidationPipelineBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;

        public ValidationPipelineBehavior(IEnumerable<IValidator<TRequest>> validators)
        {
            _validators=validators;
        }
        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            if(_validators.Any())
            {
                var context = new ValidationContext<TRequest>(request);
                var validationResults = await Task.WhenAll(_validators.Select(vr => vr.ValidateAsync(context, cancellationToken)));

                if(!validationResults.Any(vr=>vr.IsValid))
                {
                    List<string> errors = []; 

                    var failures = validationResults.SelectMany(vr=>vr.Errors).Where(f=>f != null).ToList();

                    foreach(var fail in failures)
                    {
                        errors.Add(fail.ErrorMessage);
                    }
                    return (TResponse)await ResponseWrapper.FailAsync(errors);
                }
            }
            return await next();
        }
    }
}
