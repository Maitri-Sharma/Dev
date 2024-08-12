using AutoMapper;
using DataAccessAPI.HandleRequest.Request.UtvalgList;
using DataAccessAPI.HandleRequest.Response.UtvalgList;
using MediatR;
using Microsoft.Extensions.Logging;
using Puma.Infrastructure.Interface.KsupDB.Utvalg;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DataAccessAPI.HandleRequest.Handler.UtvalgList
{
    public class GetUtvalgListReolIDsHandler : IRequestHandler<RequestGetUtvalgListReolIDs, List<long>>
    {
        /// <summary>
        /// The utvalg repository
        /// </summary>
        private readonly IUtvalgListRepository _utvalgListRepository;
        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger<GetUtvalgListReolIDsHandler> _logger;

        /// <summary>
        /// The mapper
        /// </summary>
        //private readonly IMapper _mapper;

        public GetUtvalgListReolIDsHandler(IUtvalgListRepository utvalgListRepository, ILogger<GetUtvalgListReolIDsHandler> logger)
        {
            _utvalgListRepository = utvalgListRepository ?? throw new ArgumentNullException(nameof(utvalgListRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
           // _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }
        public async Task<List<long>> Handle(RequestGetUtvalgListReolIDs request, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Calling GetUtvalgListReolIDs from Repository");
            return  await _utvalgListRepository.GetUtvalgListReolIDs(request.listId);
            //ResponseGetUtvalgListReolIDs result = null;
            //if (resultData != null)
            //{
            //    result = _mapper.Map<Puma.Shared.Reol, List<ResponseGetUtvalgListReolIDs>>(resultData).ToList();
            //}

            //return result;
            
        }
    }
}
