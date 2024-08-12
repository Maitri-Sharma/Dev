using AutoMapper;
using DataAccessAPI.HandleRequest.Request.ByDel;
using DataAccessAPI.HandleRequest.Response.ByDel;
using MediatR;
using Microsoft.Extensions.Logging;
using Puma.Infrastructure.Interface.KsupDB;
using Puma.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DataAccessAPI.HandleRequest.Handler.ByDel
{
    public class GetAllBydelsHandler : IRequestHandler<RequestGetAllBydels, List<ResponseGetAllBydels>>
    {
        /// <summary>
        /// The utvalg repository
        /// </summary>
        private readonly IByDelRepository _byDelRepository;
        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger<GetAllBydelsHandler> _logger;

        /// <summary>
        /// The mapper
        /// </summary>
        private readonly IMapper _mapper;

        public GetAllBydelsHandler(IByDelRepository byDelRepository, ILogger<GetAllBydelsHandler> logger, IMapper mapper)
        {
            _byDelRepository = byDelRepository ?? throw new ArgumentNullException(nameof(byDelRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<List<ResponseGetAllBydels>> Handle(RequestGetAllBydels request, CancellationToken cancellationToken)
        {
            List<ResponseGetAllBydels> responseGetAllByDels = null;
            _logger.LogDebug("Calling GetAllBydels from Repository");
            var byDelsData = await _byDelRepository.GetAllBydels();
            if (byDelsData?.Any() == true)
            {
                responseGetAllByDels = _mapper.Map<List<Bydel>, List<ResponseGetAllBydels>>(byDelsData);
            }
            return responseGetAllByDels;
        }
    }
}
