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
    public class GetFylkeHandler : IRequestHandler<RequestGetFylke, ResponseGetFylkes>
    {
        /// <summary>
        /// The fylke repository
        /// </summary>
        private readonly IFylkeRepository _fylkeRepository;

        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger<GetFylkeHandler> _logger;

        /// <summary>
        /// The mapper
        /// </summary>
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetFylkeHandler"/> class.
        /// </summary>
        /// <param name="fylkeRepository">The fylke repository.</param>
        ///  <param name="logger">The logger.</param>
        /// <param name="mapper">The mapper.</param>
        /// <exception cref="System.ArgumentNullException">
        /// fylkeRepository
        /// or
        /// mapper
        /// </exception>
        public GetFylkeHandler(IFylkeRepository fylkeRepository, ILogger<GetFylkeHandler> logger, IMapper mapper)
        {
            _fylkeRepository = fylkeRepository ?? throw new ArgumentNullException(nameof(fylkeRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

        }

        public async Task<ResponseGetFylkes> Handle(RequestGetFylke request, CancellationToken cancellationToken)
        {
            ResponseGetFylkes responseGetFylke = null;
            //TODO: We have to create function in DB which retruns data based on Fyker Id
            _logger.LogDebug("Calling GetAllFylkes from Repository");
            var fylkersData = await _fylkeRepository.GetAllFylkes();
            if (fylkersData?.Any() == true)
            {
                var fylke = fylkersData.Where(x => x.FylkeID.ToLower() == request.FylkeId.ToLower())?.FirstOrDefault();
                responseGetFylke = _mapper.Map<Puma.Shared.Fylke, ResponseGetFylkes>(fylke);
            }

            if (responseGetFylke == null)
            {
                throw new Exception("Fant ikke fylket med fylkesid " + responseGetFylke.FylkeID + " i databasen.");
            }
            return responseGetFylke;
        }
    }
}
