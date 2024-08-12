

using AutoMapper;
using DataAccessAPI.HandleRequest.Request.Kapasitet;
using DataAccessAPI.HandleRequest.Response.Kapasitet;
using MediatR;
using Microsoft.Extensions.Logging;
using Puma.Infrastructure.Interface.KsupDB;
using Puma.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DataAccessAPI.HandleRequest.Handler.Kapasitet
{
    public class GetDatesLackingCapacityHandler : IRequestHandler<RequestGetDatesLackingCapacity, List<ResponseGetDatesLackingCapacity>>
    {
        /// <summary>
        /// The kapasitet repository
        /// </summary>
        private readonly IKapasitetRepository _kapasitetRepository;

        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger<GetDatesLackingCapacityHandler> _logger;

        /// <summary>
        /// The mapper
        /// </summary>
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetDatesLackingCapacityHandler"/> class.
        /// </summary>
        /// <param name="kapasitetRepository">The kapasitet repository.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="mapper">The mapper.</param>
        /// <exception cref="System.ArgumentNullException">
        /// kapasitetRepository
        /// or
        /// mapper
        /// </exception>
        public GetDatesLackingCapacityHandler(IKapasitetRepository kapasitetRepository, ILogger<GetDatesLackingCapacityHandler> logger, IMapper mapper)
        {
            _kapasitetRepository = kapasitetRepository ?? throw new ArgumentNullException(nameof(kapasitetRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<List<ResponseGetDatesLackingCapacity>> Handle(RequestGetDatesLackingCapacity request, CancellationToken cancellationToken)
        {
            List<ResponseGetDatesLackingCapacity> responseGetDatesLackingCapacities = null;
            _logger.LogDebug("Calling GetDatesLackingCapacity from Repository");
            var datesLackingCapacity = await _kapasitetRepository.GetDatesLackingCapacity(request.fromDate, request.toDate, request.id, request.type, request.receiverType, request.weight, request.thickness);
            if (datesLackingCapacity?.Any() == true)
            {
                responseGetDatesLackingCapacities = _mapper.Map<List<LackingCapacity>, List<ResponseGetDatesLackingCapacity>>(datesLackingCapacity);
            }

            return responseGetDatesLackingCapacities;
        }
    }
}
