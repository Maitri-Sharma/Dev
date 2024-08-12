using DataAccessAPI.HandleRequest.Request.UtvalgList;
using MediatR;
using Microsoft.Extensions.Logging;
using Puma.Infrastructure.Interface.KsupDB.Utvalg;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DataAccessAPI.HandleRequest.Handler.UtvalgList
{
    public class CheckUtvalgListNameExistsHandler : IRequestHandler<RequestUtvalgListNameExists, bool>
    {
        /// <summary>
        /// The utvalg repository
        /// </summary>
        private readonly IUtvalgListRepository _utvalgListRepository;
        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger<CheckUtvalgListNameExistsHandler> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="CheckUtvalgListNameExistsHandler"/> class.
        /// </summary>
        /// <param name="utvalgListRepository">The utvalg repository.</param>
        /// <param name="logger">The logger.</param>
        /// <exception cref="System.ArgumentNullException">
        /// utvalgRepository
        /// or
        /// logger
        /// </exception>
        public CheckUtvalgListNameExistsHandler(IUtvalgListRepository utvalgListRepository, ILogger<CheckUtvalgListNameExistsHandler> logger)
        {
            _utvalgListRepository = utvalgListRepository ?? throw new ArgumentNullException(nameof(utvalgListRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(RequestUtvalgListNameExists request, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Calling UtvalgListNameExists from Repository");
            bool isUtvalgListNameExists = await _utvalgListRepository.UtvalgListNameExists(request.utvalglistname);

            return isUtvalgListNameExists;
            
        }
    }
}
