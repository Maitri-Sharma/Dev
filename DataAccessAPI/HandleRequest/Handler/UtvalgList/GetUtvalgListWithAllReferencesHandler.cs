using AutoMapper;
using DataAccessAPI.HandleRequest.Request.UtvalgList;
using DataAccessAPI.HandleRequest.Response.UtvalgList;
using MediatR;
using Microsoft.Extensions.Logging;
using Puma.Infrastructure.Interface.KsupDB.Utvalg;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DataAccessAPI.HandleRequest.Handler.UtvalgList
{
    /// <summary>
    /// GetUtvalgListWithAllReferencesHandler
    /// </summary>
    public class GetUtvalgListWithAllReferencesHandler : IRequestHandler<RequestGetUtvalgListWithAllReferences, ResponseGetUtvalgListWithAllReferences>
    {
        /// <summary>
        /// The utvalgList repository
        /// </summary>
        private readonly IUtvalgListRepository _utvalgListRepository;
        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger<GetUtvalgListWithAllReferencesHandler> _logger;

        /// <summary>
        /// The mapper
        /// </summary>
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetUtvalgListWithAllReferencesHandler"/> class.
        /// </summary>
        /// <param name="utvalgListRepository">The utvalg list repository.</param>
        /// <param name="logger">Instance of logger</param>
        /// <param name="mapper">The mapper.</param>
        /// <exception cref="System.ArgumentNullException">
        /// utvalgListRepository
        /// or
        /// mapper
        /// </exception>
        public GetUtvalgListWithAllReferencesHandler(IUtvalgListRepository utvalgListRepository, ILogger<GetUtvalgListWithAllReferencesHandler> logger, IMapper mapper)
        {
            _utvalgListRepository = utvalgListRepository ?? throw new ArgumentNullException(nameof(utvalgListRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
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
        public async Task<ResponseGetUtvalgListWithAllReferences> Handle(RequestGetUtvalgListWithAllReferences request, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Inside into GetUtvalgListWithAllReferences");
            var utvalgListData = await _utvalgListRepository.GetUtvalgListWithAllReferences(request.UtvalglistId);
            ResponseGetUtvalgListWithAllReferences response = null;
            if (utvalgListData != null)
            {
                long antallBeforeRecreation = utvalgListData.CalculateAntallBeforeRecreation();
                response = _mapper.Map<Puma.Shared.UtvalgList, ResponseGetUtvalgListWithAllReferences>(utvalgListData);
                //response.AntallBeforeRecreation = antallBeforeRecreation;
            }
            return response;
        }
    }
}
