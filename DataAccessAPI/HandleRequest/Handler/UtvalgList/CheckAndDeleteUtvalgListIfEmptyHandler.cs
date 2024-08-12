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
    /// CheckAndDeleteUtvalgListIfEmptyHandler
    /// </summary>
    public class CheckAndDeleteUtvalgListIfEmptyHandler : IRequestHandler<RequestCheckAndDeleteUtvalgListIfEmpty, bool>
    {
        /// <summary>
        /// The utvalg repository
        /// </summary>
        private readonly IUtvalgListRepository _utvalgListRepository;
        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger<CheckAndDeleteUtvalgListIfEmptyHandler> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="CheckAndDeleteUtvalgListIfEmptyHandler"/> class.
        /// </summary>
        /// <param name="utvalgListRepository">The utvalg list repository.</param>
        /// <exception cref="System.ArgumentNullException">utvalgListRepository</exception>
        /// <param name="logger">Instance of logger</param>
        public CheckAndDeleteUtvalgListIfEmptyHandler(IUtvalgListRepository utvalgListRepository, ILogger<CheckAndDeleteUtvalgListIfEmptyHandler> logger)
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
        /// <exception cref="System.NotImplementedException"></exception>
        public async Task<bool> Handle(RequestCheckAndDeleteUtvalgListIfEmpty request, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Calling CheckAndDeleteUtvalgListIfEmpty from Repository");
            return await _utvalgListRepository.CheckAndDeleteUtvalgListIfEmpty(request.UtvalgListId, request.userName);
        }
    }
}
