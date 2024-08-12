using AutoMapper;
using DataAccessAPI.HandleRequest.Request.Utvalg;
using DataAccessAPI.HandleRequest.Response.Utvalg;
using MediatR;
using Microsoft.Extensions.Logging;
using Puma.Infrastructure.Interface.KsupDB.Utvalg;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DataAccessAPI.HandleRequest.Handler.Utvalg
{
    public class GetUtvalgLastSavedByHandler : IRequestHandler<RequestGetUtlvagLastSavedBy, string>
    {
        /// <summary>
        /// The utvalg repository
        /// </summary>
        private readonly IUtvalgRepository _utvalgRepository;
        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger<GetUtvalgLastSavedByHandler> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetUtvalgLastSavedByHandler"/> class.
        /// </summary>
        /// <param name="utvalgRepository">The utvalg repository.</param>
        /// <param name="logger">The logger.</param>
        /// <exception cref="System.ArgumentNullException">
        /// utvalgRepository
        /// or
        /// logger
        /// </exception>
        public GetUtvalgLastSavedByHandler(IUtvalgRepository utvalgRepository, ILogger<GetUtvalgLastSavedByHandler> logger)
        {
            _utvalgRepository = utvalgRepository ?? throw new ArgumentNullException(nameof(utvalgRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<string> Handle(RequestGetUtlvagLastSavedBy request, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Calling LastSavedBy from Repository");
            string lastSavedBy = await _utvalgRepository.LastSavedBy(request.UtvalgId);
            
            return lastSavedBy;
        }
    }
}
