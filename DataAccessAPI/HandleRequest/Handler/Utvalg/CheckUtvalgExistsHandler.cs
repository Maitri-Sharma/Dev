using DataAccessAPI.HandleRequest.Request.Utvalg;
using MediatR;
using Microsoft.Extensions.Logging;
using Puma.Infrastructure.Interface.KsupDB.Utvalg;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DataAccessAPI.HandleRequest.Handler.Utvalg
{
    /// <summary>
    /// Check utlvag exists handler
    /// </summary>
    public class CheckUtvalgExistsHandler : IRequestHandler<RequestIsUtvalgSkrivebeskyttetInDB, bool>
    {
        /// <summary>
        /// The utvalg repository
        /// </summary>
        private readonly IUtvalgRepository _utvalgRepository;
        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger<CheckUtvalgExistsHandler> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="CheckUtvalgExistsHandler"/> class.
        /// </summary>
        /// <param name="utvalgRepository">The utvalg repository.</param>
        /// <param name="logger">The logger.</param>
        /// <exception cref="System.ArgumentNullException">
        /// utvalgRepository
        /// or
        /// logger
        /// </exception>
        public CheckUtvalgExistsHandler(IUtvalgRepository utvalgRepository, ILogger<CheckUtvalgExistsHandler> logger)
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
        public async Task<bool> Handle(RequestIsUtvalgSkrivebeskyttetInDB request, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Calling IsUtvalgSkrivebeskyttetInDB from Repository ");

            //Check utvalg exists in DB
            bool isUtvalgExists = await _utvalgRepository.IsUtvalgSkrivebeskyttetInDB(request.utvalgId);
            _logger.LogInformation("Is Utvalg Skrivebeskyttet In DB: ", isUtvalgExists);

            return isUtvalgExists;
        }
    }
}
