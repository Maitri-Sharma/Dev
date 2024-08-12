using DataAccessAPI.HandleRequest.Request.Logger;
using MediatR;
using Puma.Infrastructure.Interface.Logger;
using System;
using System.Threading;
using System.Threading.Tasks;
using Hangfire.Console;

namespace DataAccessAPI.HandleRequest.Handler.Logger
{
    public class PumaLogCleanServiceHandler : IRequestHandler<RequestPumaLogCleanService, bool>
    {
        /// <summary>
        /// Logger Repo
        /// </summary>
        private readonly ILoggerRepository _loggerRepository;

        /// <summary>
        /// Constuctor
        /// </summary>
        /// <param name="loggerRepository"></param>
        public PumaLogCleanServiceHandler(ILoggerRepository loggerRepository)
        {
            _loggerRepository = loggerRepository ?? throw new ArgumentNullException(nameof(loggerRepository));
        }

        /// <summary>
        /// Handle method
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<bool> Handle(RequestPumaLogCleanService request, CancellationToken cancellationToken)
        {
            request.context.WriteLine("Call method for clean up logs");
            await _loggerRepository.DeleteLogs();
            request.context.WriteLine("Clean up logs completed");
            return true;
        }
    }
}
