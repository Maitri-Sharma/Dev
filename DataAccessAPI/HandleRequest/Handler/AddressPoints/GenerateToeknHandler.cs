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
using Puma.Infrastructure.Interface.KsupDB.OEBSService;

namespace DataAccessAPI.HandleRequest.Handler.AddressPoints
{
    public class GenerateToeknHandler : IRequestHandler<RequesGetToken, string>
    {
        /// <summary>
        /// The i oebs service repository
        /// </summary>
        private readonly IOEBSServiceRepository _iOEBSServiceRepository;

        public GenerateToeknHandler(IOEBSServiceRepository iOEBSServiceRepository)
        {
            _iOEBSServiceRepository = iOEBSServiceRepository ?? throw new ArgumentNullException(nameof(iOEBSServiceRepository));
        }

        public async Task<string> Handle(RequesGetToken request, CancellationToken cancellationToken)
        {
            return await _iOEBSServiceRepository.GetToken();
        }
    }
}
