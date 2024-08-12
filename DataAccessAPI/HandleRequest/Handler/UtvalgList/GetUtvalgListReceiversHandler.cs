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
    public class GetUtvalgListReceiversHandler : IRequestHandler<RequestGetUtvalgListReceivers, ResponseGetUtvalgListReceivers>
    {
        /// <summary>
        /// The utvalgList repository
        /// </summary>
        private readonly IUtvalgListRepository _utvalgListRepository;
        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger<GetUtvalgListReceiversHandler> _logger;

        /// <summary>
        /// The mapper
        /// </summary>
        private readonly IMapper _mapper;

        public GetUtvalgListReceiversHandler(IUtvalgListRepository utvalgListRepository, ILogger<GetUtvalgListReceiversHandler> logger, IMapper mapper)
        {
            _utvalgListRepository = utvalgListRepository ?? throw new ArgumentNullException(nameof(utvalgListRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }
        public  async Task<ResponseGetUtvalgListReceivers> Handle(RequestGetUtvalgListReceivers request, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Calling GetUtvalgListReceivers from Repository");
            var utvalgListData = await _utvalgListRepository.GetUtvalgListReceivers(request.listId);
            ResponseGetUtvalgListReceivers response = null;
            if (utvalgListData?.Any() == true)
            {
                response = _mapper.Map<Puma.Shared.UtvalgReceiverList, ResponseGetUtvalgListReceivers>(utvalgListData);
            }
            return response;
            
        }
    }
}
