using FluentValidation;
using MediatR;
using Puma.CustomException;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace DataAccessAPI.Extensions
{
    /// <summary>
    /// Class to handle fluent validations
    /// </summary>
    /// <typeparam name="TRequest"></typeparam>
    /// <typeparam name="TResponse"></typeparam>
    public class ValidationBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;
        public ValidationBehaviour(IEnumerable<IValidator<TRequest>> validators)
        {
            _validators = validators;
        }
        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {

            var failures = _validators
                .Select(r => r.Validate(request)).
                SelectMany(result => result.Errors.Distinct())
                .Where(f => f != null).ToList();

            var response = new List<PumaExceptionModel>();
            if (failures?.Any() == true)
            {
                response = failures.Select(x => new PumaExceptionModel()
                { Code = (int)HttpStatusCode.PreconditionFailed, Message = x.ErrorMessage })?.ToList();

                throw new PumaException(response)
                {
                    statusCode = HttpStatusCode.PreconditionFailed
                };
            }

            return await next();
        }
    }
}
