using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using WM.Assessment.Application.Exceptions;
using WM.Assessment.Domain;

namespace WM.Assessment.Application.Behaviors
{
    /// <summary>
    ///     Wraps domain exceptions into app-specific exceptions
    /// </summary>
    /// <typeparam name="TRequest"></typeparam>
    /// <typeparam name="TResponse"></typeparam>
    public class ExceptionHandlingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken,
            RequestHandlerDelegate<TResponse> next)
        {
            try
            {
                //need to await result to allow exception to be caught within this behavior...
                var result = await next();
                return result;
            }
            //map domain exceptions to application layer exceptions
            catch (DomainException ex)
            {
                throw new BadRequestException(ex.Message);
            }
            catch (ApplicationException ex)
            {
                throw new BadRequestException(ex.Message);
            }
        }
    }
}