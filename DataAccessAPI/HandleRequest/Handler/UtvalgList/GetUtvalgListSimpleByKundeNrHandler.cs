using AutoMapper;
using DataAccessAPI.HandleRequest.Request.UtvalgList;
using DataAccessAPI.HandleRequest.Response.UtvalgList;
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
    public class GetUtvalgListSimpleByKundeNrHandler : IRequestHandler<RequestGetUtvalgListSimpleByKundeNr, ResponseGetUtvalgListSimpleByKundeNr>
    {
        /// <summary>
        /// The utvalgList repository
        /// </summary>
        private readonly IUtvalgListRepository _utvalgListRepository;
        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger<GetUtvalgListSimpleByKundeNrHandler> _logger;

        /// <summary>
        /// The mapper
        /// </summary>
        private readonly IMapper _mapper;

        public GetUtvalgListSimpleByKundeNrHandler(IUtvalgListRepository utvalgListRepository, ILogger<GetUtvalgListSimpleByKundeNrHandler> logger, IMapper mapper)
        {
            _utvalgListRepository = utvalgListRepository ?? throw new ArgumentNullException(nameof(utvalgListRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }
        public  async Task<ResponseGetUtvalgListSimpleByKundeNr> Handle(RequestGetUtvalgListSimpleByKundeNr request, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Calling GetUtvalgListSimpleByKundeNr from Repository");
            var utvalgListData = await _utvalgListRepository.GetUtvalgListSimpleByKundeNr(request.kundenummer);
            ResponseGetUtvalgListSimpleByKundeNr response = null;
            if (utvalgListData?.Any() == true)
            {
                response = _mapper.Map<List<Puma.Shared.UtvalgList>,  ResponseGetUtvalgListSimpleByKundeNr>(utvalgListData);
            }
            return response;
            
        }
    }
}
