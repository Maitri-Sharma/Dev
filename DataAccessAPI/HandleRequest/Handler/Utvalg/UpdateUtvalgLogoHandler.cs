using DataAccessAPI.HandleRequest.Request.Utvalg;
using MediatR;
using Microsoft.Extensions.Logging;
using Puma.Infrastructure.Interface.KsupDB.Utvalg;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DataAccessAPI.HandleRequest.Handler.Utvalg
{
    /// <summary>
    /// UpdateUtvalgLogoHandler
    /// </summary>
    public class UpdateUtvalgLogoHandler : IRequestHandler<RequestUpdateUtvalgLogo, bool>
    {
        /// <summary>
        /// The utvalg repository
        /// </summary>
        private readonly IUtvalgRepository _utvalgRepository;
        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger<UpdateUtvalgLogoHandler> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateUtvalgLogoHandler"/> class.
        /// </summary>
        /// <param name="utvalgRepository">The utvalg repository.</param>
        /// <param name="logger">Instance of logger</param>
        /// <exception cref="System.ArgumentNullException">utvalgRepository</exception>
        public UpdateUtvalgLogoHandler(IUtvalgRepository utvalgRepository, ILogger<UpdateUtvalgLogoHandler> logger)
        {
            _utvalgRepository = utvalgRepository ?? throw new ArgumentNullException(nameof(utvalgRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Handles a request
        /// </summary>
        /// <param name="request">The request</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>
        /// Response from the request
        /// </returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public async Task<bool> Handle(RequestUpdateUtvalgLogo request, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Calling UpdateLogo from Repository");
            await _utvalgRepository.UpdateLogo(request.UtvalgId, request.Logo, request.Username);
            return true;
        }
    }
}
