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
    public class SearchUtvalgListByIsBasisHandler : IRequestHandler<RequestSearchUtvalgListByIsBasis, List<ResponseSearchUtvalgListSimple>>
    {
        /// <summary>
        /// The utvalgList repository
        /// </summary>
        private readonly IUtvalgListRepository _utvalgListRepository;
        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger<SearchUtvalgListByIsBasisHandler> _logger;

        /// <summary>
        /// The mapper
        /// </summary>
        private readonly IMapper _mapper;

        /// <summary>
        /// SearchUtvalgListByIsBasisHandler
        /// </summary>
        /// <param name="utvalgListRepository"></param>
        /// <param name="logger"></param>
        /// <param name="mapper"></param>
        public SearchUtvalgListByIsBasisHandler(IUtvalgListRepository utvalgListRepository, ILogger<SearchUtvalgListByIsBasisHandler> logger, IMapper mapper)
        {
            _utvalgListRepository = utvalgListRepository ?? throw new ArgumentNullException(nameof(utvalgListRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }
        /// <summary>
        /// Handle
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<List<ResponseSearchUtvalgListSimple>> Handle(RequestSearchUtvalgListByIsBasis request, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Calling SearchUtvalgListByIsBasisHandler from Repository");
            var utvalgListData = await _utvalgListRepository.SearchUtvalgListByIsBasis(request.utvalglistname, request.onlyBasisLists, request.searchMethod);
            List<ResponseSearchUtvalgListSimple> response = null;
            if (utvalgListData?.Any() == true)
            {
                if (!request.isBasedOn)
                {
                    Puma.Shared.UtvalgsListCollection listData = new Puma.Shared.UtvalgsListCollection();
                    listData.AddRange(utvalgListData.Where(x => x.BasedOn <= 0).ToList());
                    utvalgListData = listData;
                }
                    
                response = _mapper.Map<List<Puma.Shared.UtvalgList>, List<ResponseSearchUtvalgListSimple>>(utvalgListData).ToList();
            }
            return response;

            throw new NotImplementedException();
        }
    }
}
