using AutoMapper;
using DataAccessAPI.HandleRequest.Request.Utvalg;
using DataAccessAPI.HandleRequest.Response.Utvalg;
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
    public class OrderedUtvalgAndListsForUpdateHandler : IRequestHandler<RequestOrderedUtvalgAndListsForUpdate, List<ResponseOrderedUtvalgAndListsForUpdate>>
    {
        /// <summary>
        /// The utvalg repository
        /// </summary>
        private readonly IUtvalgRepository _utvalgRepository;
        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger<OrderedUtvalgAndListsForUpdateHandler> _logger;

        /// <summary>
        /// The mapper
        /// </summary>
        private readonly IMapper _mapper;

        public OrderedUtvalgAndListsForUpdateHandler(IUtvalgRepository utvalgRepository, ILogger<OrderedUtvalgAndListsForUpdateHandler> logger, IMapper mapper)
        {
            _utvalgRepository = utvalgRepository ?? throw new ArgumentNullException(nameof(utvalgRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<List<ResponseOrderedUtvalgAndListsForUpdate>> Handle(RequestOrderedUtvalgAndListsForUpdate request, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Calling GetOrderedUtvalgAndListsForUpdateRequest from Repository");
            var utvalgData = await _utvalgRepository.GetOrderedUtvalgAndListsForUpdateRequest(request.DeliveryDate);
            List<ResponseOrderedUtvalgAndListsForUpdate> response = null;
            if (utvalgData?.Any() == true)
            {
                response = _mapper.Map<List<Puma.Shared.AutoUpdateMessage>, List<ResponseOrderedUtvalgAndListsForUpdate>>(utvalgData).ToList();
            }
            return response;
        }
    }
}
