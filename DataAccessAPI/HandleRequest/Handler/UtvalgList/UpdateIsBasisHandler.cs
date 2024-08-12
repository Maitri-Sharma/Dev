using DataAccessAPI.HandleRequest.Request.UtvalgList;
using MediatR;
using Microsoft.Extensions.Logging;
using Puma.Infrastructure.Interface.KsupDB.Utvalg;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DataAccessAPI.HandleRequest.Handler.UtvalgList
{
    /// <summary>
    /// UpdateIsBasisHandler
    /// </summary>
    public class UpdateIsBasisHandler : IRequestHandler<RequestUpdateIsBasis, bool>
    {
        /// <summary>
        /// The utvalg list repository
        /// </summary>
        private readonly IUtvalgListRepository _utvalgListRepository;
        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger<UpdateIsBasisHandler> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateIsBasisHandler"/> class.
        /// </summary>
        /// <param name="utvalgListRepository">The utvalg list repository.</param>
        /// <param name="logger">Instance of logger</param>
        /// <exception cref="System.ArgumentNullException">utvalgListRepository</exception>
        public UpdateIsBasisHandler(IUtvalgListRepository utvalgListRepository, ILogger<UpdateIsBasisHandler> logger)
        {
            _utvalgListRepository = utvalgListRepository ?? throw new ArgumentNullException(nameof(utvalgListRepository));
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
        public async Task<bool> Handle(RequestUpdateIsBasis request, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Calling UpdateIsBasis from Repository");
            await _utvalgListRepository.UpdateIsBasis(request.ListId, request.IsBasis,request.BasedOn);
            return true;
        }
    }
}
