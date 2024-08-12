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
    //public class SearchUtvalgListWithoutReferencesHandler : IRequestHandler<RequestSearchUtvalgListWithoutReferences, ResponseSearchUtvalgListWithoutReferences>
    //{
        ///// <summary>
        ///// The utvalgList repository
        ///// </summary>
        //private readonly IUtvalgListRepository _utvalgListRepository;
        ///// <summary>
        ///// The logger
        ///// </summary>
        //private readonly ILogger<SearchUtvalgListWithoutReferencesHandler> _logger;

        ///// <summary>
        ///// The mapper
        ///// </summary>
        //private readonly IMapper _mapper;

        //public SearchUtvalgListWithoutReferencesHandler(IUtvalgListRepository utvalgListRepository, ILogger<SearchUtvalgListWithoutReferencesHandler> logger, IMapper mapper)
        //{
        //    _utvalgListRepository = utvalgListRepository ?? throw new ArgumentNullException(nameof(utvalgListRepository));
        //    _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        //    _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        //}
        //public async Task<ResponseSearchUtvalgListWithoutReferences> Handle(RequestSearchUtvalgListWithoutReferences request, CancellationToken cancellationToken)
        //{
        //    var utvalgListData = await _utvalgListRepository.SearchUtvalgListWithoutReferences(request.Utvalglistname, request.customerNumber,request.searchMethod,request.canHaveEmptyCustomerNumber);
        //    ResponseSearchUtvalgListWithoutReferences response = null;
        //    if (utvalgListData?.Any() == true)
        //    {
        //        response = _mapper.Map<List<Puma.Shared.UtvalgsListCollection>,  ResponseSearchUtvalgListWithoutReferences> (utvalgListData);
        //    }
        //    return response;
            
        //}
   // }
}
