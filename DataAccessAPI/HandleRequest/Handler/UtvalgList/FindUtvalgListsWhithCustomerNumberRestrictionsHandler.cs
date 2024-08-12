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
    //public class FindUtvalgListsWhithCustomerNumberRestrictionsHandler : IRequestHandler<RequestFindUtvalgListsWhithCustomerNumberRestrictions, bool>
    //{
    //    /// <summary>
    //    /// The utvalgList repository
    //    /// </summary>
    //    private readonly IUtvalgListRepository _utvalgListRepository;
    //    /// <summary>
    //    /// The logger
    //    /// </summary>
    //    private readonly ILogger<FindUtvalgListsWhithCustomerNumberRestrictionsHandler> _logger;

    //    public FindUtvalgListsWhithCustomerNumberRestrictionsHandler(IUtvalgListRepository utvalgListRepository, ILogger<FindUtvalgListsWhithCustomerNumberRestrictionsHandler> logger)
    //    {
    //        _utvalgListRepository = utvalgListRepository ?? throw new ArgumentNullException(nameof(utvalgListRepository));
    //        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    //    }

    //    public async Task<bool> Handle(RequestFindUtvalgListsWhithCustomerNumberRestrictions request, CancellationToken cancellationToken)
    //    {
    //        var utvalgListData = await _utvalgListRepository.FindUtvalgListsWhithCustomerNumberRestrictions(request.listName,request.customerNumber);
    //        List<ResponseFindUtvalgListsWhithCustomerNumberRestrictions> response = null;
    //        if (utvalgListData?.Any() == true)
    //        {
    //            response = _mapper.Map<List<Puma.Shared.UtvalgList>, List<ResponseFindUtvalgListsWhithCustomerNumberRestrictions>>(utvalgListData).ToList();
    //        }
    //        return response;
            
    //    }
    //}
}
