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
    public class CheckUtvalgNameExistsHandler : IRequestHandler<RequestUtvagNameExists, bool>
    {
        /// <summary>
        /// The utvalg repository
        /// </summary>
        private readonly IUtvalgRepository _utvalgRepository;
        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger<CheckUtvalgNameExistsHandler> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="CheckUtvalgNameExistsHandler"/> class.
        /// </summary>
        /// <param name="utvalgRepository">The utvalg repository.</param>
        /// <param name="logger">The logger.</param>
        /// <exception cref="System.ArgumentNullException">
        /// utvalgRepository
        /// or
        /// logger
        /// </exception>
        public CheckUtvalgNameExistsHandler(IUtvalgRepository utvalgRepository, ILogger<CheckUtvalgNameExistsHandler> logger)
        {
            _utvalgRepository = utvalgRepository ?? throw new ArgumentNullException(nameof(utvalgRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(RequestUtvagNameExists request, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Calling  UtvalgNameExists from Repository");
            bool isUtvalgNameExists = await _utvalgRepository.UtvalgNameExists(request.UtvalgName);

            return isUtvalgNameExists;
        }
    }
}
