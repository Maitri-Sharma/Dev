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
using Puma.DataLayer.BusinessEntity;

namespace DataAccessAPI.HandleRequest.Handler.AddressPoints
{
    public class SaveAddressPointsStatesHandler : IRequestHandler<RequestSaveAddressPointsState, int>
    {

        private readonly IAddressPointStateRepository _addressPointStateRepository;

        public SaveAddressPointsStatesHandler( IAddressPointStateRepository addressPointStateRepository)
        {
            _addressPointStateRepository = addressPointStateRepository ?? throw new ArgumentNullException(nameof(addressPointStateRepository));
        }

        public async Task<int> Handle(RequestSaveAddressPointsState request, CancellationToken cancellationToken)
        {
            return await _addressPointStateRepository.SaveAdressPointsAPI(request.userId, request.addressPointList);
        }
    }
}
