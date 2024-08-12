using DataAccessAPI.HandleRequest.Request.AddressPoints;
using DataAccessAPI.HandleRequest.Response.AddressPoints;
using MediatR;
using Microsoft.Extensions.Logging;
using Puma.Infrastructure.Interface;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using AutoMapper;
using Puma.Shared;

namespace DataAccessAPI.HandleRequest.Handler.AddressPoints
{


    /// <summary>
    /// Handler to get address point states
    /// </summary>
    public class GetAddressPointsStatesHandler : IRequestHandler<RequestGetAddressPointsState, List<ResponseAddressPointState>>
    {

        private readonly ILogger<GetAddressPointsStatesHandler> _logger;

        private readonly IAddressPointStateRepository _addressPointStateRepository;

        private readonly IMapper _mapper;

        public GetAddressPointsStatesHandler(IAddressPointStateRepository addressPointStateRepository, ILogger<GetAddressPointsStatesHandler> logger,
            IMapper mapper)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _addressPointStateRepository = addressPointStateRepository ?? throw new ArgumentNullException(nameof(addressPointStateRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

        }



        /// <summary>
        /// Handle request
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<List<ResponseAddressPointState>> Handle(RequestGetAddressPointsState request, CancellationToken cancellationToken)
        {
            //_logger.LogDebug("Calling GetAddressPointsState from Repository");
            var addressPointsData = await _addressPointStateRepository.GetAddressPointsState(request.userId);

            var addresdata =  _addressPointStateRepository.Find(x => x.userid.ToLower() == request.userId.ToLower()).ToList();

            List<ResponseAddressPointState> lstResponse = new List<ResponseAddressPointState>();
            if (addressPointsData?.Any() == true)
            {
                lstResponse = _mapper.Map<List<AddressPoint>, List<ResponseAddressPointState>>(addressPointsData).ToList();
            }

            return lstResponse;
        }
    }
}
