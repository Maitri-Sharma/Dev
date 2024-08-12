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
    public class GetRuterLackingCapacityHandler : IRequestHandler<RequestGetRuterLackingCapacity, List<ResponseGetRuterLackingCapacity>>
    {
        /// <summary>
        /// The kapasitet repository
        /// </summary>
        private readonly IKapasitetRepository _kapasitetRepository;

        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger<GetRuterLackingCapacityHandler> _logger;

        /// <summary>
        /// The mapper
        /// </summary>
        private readonly IMapper _mapper;

        public GetRuterLackingCapacityHandler(IKapasitetRepository kapasitetRepository, ILogger<GetRuterLackingCapacityHandler> logger, IMapper mapper)
        {
            _kapasitetRepository = kapasitetRepository ?? throw new ArgumentNullException(nameof(kapasitetRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<List<ResponseGetRuterLackingCapacity>> Handle(RequestGetRuterLackingCapacity request, CancellationToken cancellationToken)
        {
            List<ResponseGetRuterLackingCapacity> responseGetRuterLackingCapacities = null;
            _logger.LogDebug("Calling GetRuterLackingCapacity from Repository");
            var ruterLackingCapacities = await _kapasitetRepository.GetRuterLackingCapacity(request.dates, request.id, request.type,
                request.receiverType, request.weight, request.thickness);

            if (ruterLackingCapacities?.Any() == true)
            {
                responseGetRuterLackingCapacities = _mapper.Map<List<KapasitetRuter>, List<ResponseGetRuterLackingCapacity>>(ruterLackingCapacities);
            }

            return responseGetRuterLackingCapacities;
        }
    }
}
