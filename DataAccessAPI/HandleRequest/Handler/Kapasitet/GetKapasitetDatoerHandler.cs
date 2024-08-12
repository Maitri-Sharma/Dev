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
    public class GetKapasitetDatoerHandler : IRequestHandler<RequestGetKapasitetDatoer, List<ResponseGetKapasitetDatoer>>
    {
        /// <summary>
        /// The team repository
        /// </summary>
        private readonly IKapasitetRepository _kapasitetRepository;

        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger<GetKapasitetDatoerHandler> _logger;

        /// <summary>
        /// The mapper
        /// </summary>
        private readonly IMapper _mapper;

        public GetKapasitetDatoerHandler(IKapasitetRepository kapasitetRepository, ILogger<GetKapasitetDatoerHandler> logger, IMapper mapper)
        {
            _kapasitetRepository = kapasitetRepository ?? throw new ArgumentNullException(nameof(kapasitetRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<List<ResponseGetKapasitetDatoer>> Handle(RequestGetKapasitetDatoer request, CancellationToken cancellationToken)
        {
            List<ResponseGetKapasitetDatoer> responseGetKapasitetDatoers = null;
            _logger.LogDebug("Calling GetKapasitetDatoer from Repository");
            var kapasitetDatoersData = await _kapasitetRepository.GetKapasitetDatoer(request.FromDate, request.ToDate);
            if (kapasitetDatoersData?.Any() == true)
            {
                responseGetKapasitetDatoers = _mapper.Map<List<KapasitetDato>, List<ResponseGetKapasitetDatoer>>(kapasitetDatoersData);
            }
            return responseGetKapasitetDatoers;
        }
    }
}
