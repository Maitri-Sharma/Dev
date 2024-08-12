
using AutoMapper;
using DataAccessAPI.HandleRequest.Request.Kommune;
using DataAccessAPI.HandleRequest.Response.Kommune;
using MediatR;
using Microsoft.Extensions.Logging;
using Puma.Infrastructure.Interface.KsupDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DataAccessAPI.HandleRequest.Handler.Kommune
{
    /// <summary>
    /// GetAllKommunesHandler
    /// </summary>
    public class GetAllKommunesHandler : IRequestHandler<RequestGetAllKommunes, List<ResponseGetAllKommunes>>
    {

        /// <summary>
        /// The kommune repository
        /// </summary>
        private readonly IKommuneRepository _kommuneRepository;

        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger<GetAllKommunesHandler> _logger;

        /// <summary>
        /// The mapper
        /// </summary>
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetAllKommunesHandler"/> class.
        /// </summary>
        /// <param name="kommuneRepository">The kommune repository.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="mapper">The mapper.</param>
        /// <exception cref="System.ArgumentNullException">
        /// kommuneRepository
        /// or
        /// mapper
        /// </exception>
        public GetAllKommunesHandler(IKommuneRepository kommuneRepository, ILogger<GetAllKommunesHandler> logger, IMapper mapper)
        {
            _kommuneRepository = kommuneRepository ?? throw new ArgumentNullException(nameof(kommuneRepository));
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
        /// <exception cref="System.NotImplementedException"></exception>
        public async Task<List<ResponseGetAllKommunes>> Handle(RequestGetAllKommunes request, CancellationToken cancellationToken)
        {
            List<ResponseGetAllKommunes> responseGetAllKommunes = null;
            _logger.LogDebug("Calling GetAllKommunes from Repository");
            var komunesData = await _kommuneRepository.GetAllKommunes();
            if (komunesData?.Any() == true)
            {
                responseGetAllKommunes = _mapper.Map<List<Puma.Shared.Kommune>, List<ResponseGetAllKommunes>>(komunesData);
            }

            return responseGetAllKommunes;
        }
    }
}
