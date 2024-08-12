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
using Puma.Shared;
namespace DataAccessAPI.HandleRequest.Handler.UtvalgList
{
    public class GetUtvalgListSimplefromIdHandler : IRequestHandler<RequestGetUtvalgListSimple, ResponseGetUtvalgListSimple>
    {
        /// <summary>
        /// The utvalg repository
        /// </summary>
        private readonly IUtvalgListRepository _utvalgListRepository;
        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger<GetUtvalgListSimplefromIdHandler> _logger;

        /// <summary>
        /// The mapper
        /// </summary>
        private readonly IMapper _mapper;

        public GetUtvalgListSimplefromIdHandler(IUtvalgListRepository utvalgListRepository, ILogger<GetUtvalgListSimplefromIdHandler> logger, IMapper mapper)
        {
            _utvalgListRepository = utvalgListRepository ?? throw new ArgumentNullException(nameof(utvalgListRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }
        public async Task<ResponseGetUtvalgListSimple> Handle(RequestGetUtvalgListSimple request, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Calling GetUtvalgListSimple from Repository");
            var resultData = await _utvalgListRepository.GetUtvalgListSimple(request.listId);
            ResponseGetUtvalgListSimple result = null;
            if (resultData != null)
            {
                result = _mapper.Map<Puma.Shared.UtvalgList, ResponseGetUtvalgListSimple>(resultData);
            }

            return result;
        }
    }
}
