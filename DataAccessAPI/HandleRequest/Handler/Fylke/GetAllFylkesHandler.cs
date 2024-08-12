using AutoMapper;
using DataAccessAPI.HandleRequest.Request.Fylke;
using DataAccessAPI.HandleRequest.Response.Fylke;
using MediatR;
using Microsoft.Extensions.Logging;
using Puma.Infrastructure.Interface.KsupDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
namespace DataAccessAPI.HandleRequest.Handler.Fylke
{
    /// <summary>
    ///GetAllFylkesHandler
    /// </summary>
    public class GetAllFylkesHandler : IRequestHandler<RequestGetAllFylkes, List<ResponseGetAllFylkes>>
    {
        /// <summary>
        /// The utvalg repository
        /// </summary>
        private readonly IFylkeRepository _fylkeRepository;

        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger<GetAllFylkesHandler> _logger;

        /// <summary>
        /// The mapper
        /// </summary>
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetAllFylkesHandler"/> class.
        /// </summary>
        /// <param name="fylkeRepository">The fylke repository.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="mapper">The mapper.</param>
        /// <exception cref="System.ArgumentNullException">
        /// fylkeRepository
        /// or
        /// mapper
        /// </exception>
        public GetAllFylkesHandler(IFylkeRepository fylkeRepository, ILogger<GetAllFylkesHandler> logger, IMapper mapper)
        {
            _fylkeRepository = fylkeRepository ?? throw new ArgumentNullException(nameof(fylkeRepository));
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
        public async Task<List<ResponseGetAllFylkes>> Handle(RequestGetAllFylkes request, CancellationToken cancellationToken)
        {
            List<ResponseGetAllFylkes> responseGetAllFylkes = null;
            _logger.LogDebug("Calling GetAllFylkes from Repository");
            var fylkesData = await _fylkeRepository.GetAllFylkes();
            if (fylkesData?.Any() == true)
            {
                responseGetAllFylkes = _mapper.Map<List<Puma.Shared.Fylke>, List<ResponseGetAllFylkes>>(fylkesData);
            }

            return responseGetAllFylkes;
        }
    }
}
