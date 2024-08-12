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
    public class SearchUtvalgListSimpleByIdHandler : IRequestHandler<RequestSearchUtvalgListSimpleById, List<ResponseSearchUtvalgListSimpleById>>
    {
        /// <summary>
        /// The utvalgList repository
        /// </summary>
        private readonly IUtvalgListRepository _utvalgListRepository;
        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger<SearchUtvalgListSimpleByIdHandler> _logger;

        /// <summary>
        /// The mapper
        /// </summary>
        private readonly IMapper _mapper;

        public SearchUtvalgListSimpleByIdHandler(IUtvalgListRepository utvalgListRepository, ILogger<SearchUtvalgListSimpleByIdHandler> logger, IMapper mapper)
        {
            _utvalgListRepository = utvalgListRepository ?? throw new ArgumentNullException(nameof(utvalgListRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }
        public async Task<List<ResponseSearchUtvalgListSimpleById>> Handle(RequestSearchUtvalgListSimpleById request, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Calling SearchUtvalgListSimpleById from Repository");
            var resultData = await _utvalgListRepository.SearchUtvalgListSimpleById(request.utvalglistid);
            List<ResponseSearchUtvalgListSimpleById> result = null;
            if (resultData != null)
            {
                result = _mapper.Map<List<Puma.Shared.UtvalgList>, List<ResponseSearchUtvalgListSimpleById>>(resultData.ToList());
                if (!string.IsNullOrWhiteSpace(request.customerNos))
                    return result.Where(x => x.KundeNummer.ToLower() == request.customerNos.ToLower()).ToList();
            }
            return result;

        }
    }
}
