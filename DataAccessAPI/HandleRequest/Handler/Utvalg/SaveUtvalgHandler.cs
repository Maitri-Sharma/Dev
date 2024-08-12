using AutoMapper;
using DataAccessAPI.HandleRequest.Request.Utvalg;
using DataAccessAPI.HandleRequest.Response.Utvalg;
using MediatR;
using Microsoft.Extensions.Logging;
using Puma.Infrastructure.Interface.KsupDB;
using Puma.Infrastructure.Interface.KsupDB.Utvalg;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DataAccessAPI.HandleRequest.Handler.Utvalg
{
    /// <summary>
    /// SaveUtvalgHandler
    /// </summary>
    public class SaveUtvalgHandler : IRequestHandler<RequestSaveUtvalg, ResponseSaveUtvalg>
    {

        /// <summary>
        /// The utvalg repository
        /// </summary>
        private readonly IUtvalgRepository _utvalgRepository;
        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger<SaveUtvalgHandler> _logger;

       
        /// <summary>
        /// The mapper
        /// </summary>
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="SaveUtvalgHandler"/> class.
        /// </summary>
        /// <param name="utvalgRepository">The utvalg repository.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="mapper">The mapper.</param>
        /// <exception cref="System.ArgumentNullException">
        /// utvalgRepository
        /// or
        /// logger
        /// or
        /// mapper
        /// </exception>
        public SaveUtvalgHandler(IUtvalgRepository utvalgRepository, ILogger<SaveUtvalgHandler> logger,
             IMapper mapper)
        {
            _utvalgRepository = utvalgRepository ?? throw new ArgumentNullException(nameof(utvalgRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        /// <summary>
        /// Handles a request
        /// </summary>
        /// <param name="request">The request</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>
        /// Response from the request
        /// </returns>
        public async Task<ResponseSaveUtvalg> Handle(RequestSaveUtvalg request, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Calling SaveUtvalgHandler from Repository");
            var utvalgId = await _utvalgRepository.SaveUtvalgData(request.utvalg, request.userName, request.saveOldReoler, request.skipHistory, request.forceUtvalgListId);
            
            ResponseSaveUtvalg response = null;
            _logger.LogDebug("Calling GetUtvalg from Repository");
            var utvalgData = await _utvalgRepository.GetUtvalg(Convert.ToInt32(utvalgId));
            if (utvalgData != null)
            {
                response = _mapper.Map<Puma.Shared.Utvalg, ResponseSaveUtvalg>(utvalgData);
            }

            return response;
        }
    }
}