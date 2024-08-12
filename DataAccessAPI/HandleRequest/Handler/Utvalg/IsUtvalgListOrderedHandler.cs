using DataAccessAPI.HandleRequest.Request.Utvalg;
using MediatR;
using Microsoft.Extensions.Logging;
using Puma.Infrastructure.Interface.KsupDB.Utvalg;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DataAccessAPI.HandleRequest.Handler.Utvalg
{
    public class IsUtvalgListOrderedHandler : IRequestHandler<RequestIsUtvalgListOrdered, bool>
    {
        /// <summary>
        /// The utvalg repository
        /// </summary>
        private readonly IUtvalgRepository _utvalgRepository;
        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger<IsUtvalgListOrderedHandler> _logger;

        public IsUtvalgListOrderedHandler(IUtvalgRepository utvalgRepository, ILogger<IsUtvalgListOrderedHandler> logger)
        {
            _utvalgRepository = utvalgRepository ?? throw new ArgumentNullException(nameof(utvalgRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(RequestIsUtvalgListOrdered request, CancellationToken cancellationToken)
        {
            return await _utvalgRepository.IsUtvalgListOrdered(request.ID);
        }
    }
}
