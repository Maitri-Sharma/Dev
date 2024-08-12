using DataAccessAPI.HandleRequest.Request.Utvalg;
using MediatR;
using Microsoft.Extensions.Logging;
using Puma.Infrastructure.Interface.KsupDB.Utvalg;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DataAccessAPI.HandleRequest.Handler.Utvalg
{
    public class CheckUtvalgNamesExistsHandler : IRequestHandler<RequestUtvagsNameExists, bool>
    {
        /// <summary>
        /// The utvalg repository
        /// </summary>
        private readonly IUtvalgRepository _utvalgRepository;
        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger<CheckUtvalgNamesExistsHandler> _logger;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="utvalgRepository"></param>
        /// <param name="logger"></param>
        public CheckUtvalgNamesExistsHandler(IUtvalgRepository utvalgRepository, ILogger<CheckUtvalgNamesExistsHandler> logger)
        {
            _utvalgRepository = utvalgRepository ?? throw new ArgumentNullException(nameof(utvalgRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Handler
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<bool> Handle(RequestUtvagsNameExists request, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Calling  UtvalgNameExists from Repository");
            bool isUtvalgNameExists = await _utvalgRepository.UtvalgsNameExists(request.utvalgNames);

            return isUtvalgNameExists;
        }
    }
}
