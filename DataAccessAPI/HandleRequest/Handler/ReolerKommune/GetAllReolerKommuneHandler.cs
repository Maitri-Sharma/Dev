

using AutoMapper;
using DataAccessAPI.HandleRequest.Request.ReolerKommune;
using DataAccessAPI.HandleRequest.Response.ReolerKommune;
using MediatR;
using Microsoft.Extensions.Logging;
using Puma.Infrastructure.Interface.KsupDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DataAccessAPI.HandleRequest.Handler.ReolerKommune
{
    public class GetAllReolerKommuneHandler : IRequestHandler<RequestGetAllReolerKommune, List<ResponseGetAllReolerKommune>>
    {
        /// <summary>
        /// The reoler kommune repository
        /// </summary>
        private readonly IReolerKommuneRepository _reolerKommuneRepository;

        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger<GetAllReolerKommuneHandler> _logger;

        /// <summary>
        /// The mapper
        /// </summary>
        private readonly IMapper _mapper;

        public GetAllReolerKommuneHandler(IReolerKommuneRepository reolerKommuneRepository, ILogger<GetAllReolerKommuneHandler> logger, IMapper mapper)
        {
            _reolerKommuneRepository = reolerKommuneRepository ?? throw new ArgumentNullException(nameof(reolerKommuneRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<List<ResponseGetAllReolerKommune>> Handle(RequestGetAllReolerKommune request, CancellationToken cancellationToken)
        {
            List<ResponseGetAllReolerKommune> responseGetAllReolerKommunes = null;
            _logger.LogDebug("Calling GetAllReolerKommune from Repository");
            var reolerKomuneData = await _reolerKommuneRepository.GetAllReolerKommune();
            if (reolerKomuneData?.Any() == true)
            {
                responseGetAllReolerKommunes = _mapper.Map<List<Puma.Shared.ReolerKommune>, List<ResponseGetAllReolerKommune>>(reolerKomuneData);
            }

            return responseGetAllReolerKommunes;
        }
    }
}
