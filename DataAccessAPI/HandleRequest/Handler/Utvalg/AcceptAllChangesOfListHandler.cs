using AutoMapper;
using DataAccessAPI.HandleRequest.Request.Utvalg;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Puma.Infrastructure.Interface.KsupDB.Utvalg;
using Puma.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static Puma.Shared.PumaEnum;

namespace DataAccessAPI.HandleRequest.Handler.Utvalg
{
    public class AcceptAllChangesOfListHandler : IRequestHandler<RequestAcceptAllChangesOfList, bool>
    {
        /// <summary>
        /// The utvalg list repository
        /// </summary>
        private readonly IUtvalgListRepository _utvalgListRepository;

        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger<AcceptAllChangesOfListHandler> _logger;


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="utvalgListRepository"></param>
        /// <param name="logger"></param>
        public AcceptAllChangesOfListHandler(IUtvalgListRepository utvalgListRepository, ILogger<AcceptAllChangesOfListHandler> logger)
        {
            _utvalgListRepository = utvalgListRepository ?? throw new ArgumentNullException(nameof(utvalgListRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        }

        /// <summary>
        /// Handler
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<bool> Handle(RequestAcceptAllChangesOfList request, CancellationToken cancellationToken)
        {

            _logger.LogInformation("Calling AcceptAllChangesOfList handler");

            if (request.ListId <= 0)
                new Exception("Please enter valid list id");

            ////Get All list data 
            //var listData = await _utvalgListRepository.GetUtvalgListWithAllReferences(request.ListId);

            //if (listData == null)
            //    new Exception("No Data found for specific list id");

            //call repo method to update data in DB
            _logger.LogInformation("call repo method to update data in DB. For List Id " + request.ListId);

            _ = await _utvalgListRepository.AcceptAllChangesForList(request.ListId, request.UserName);

            return true;


        }
    }
}
