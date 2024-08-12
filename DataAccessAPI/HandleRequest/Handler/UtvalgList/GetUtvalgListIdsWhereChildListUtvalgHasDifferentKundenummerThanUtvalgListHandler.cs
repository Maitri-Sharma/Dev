using DataAccessAPI.HandleRequest.Request.UtvalgList;
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
    //public class GetUtvalgListIdsWhereChildListUtvalgHasDifferentKundenummerThanUtvalgListHandler : IRequestHandler<RequestGetUtvalgListIdsWhereChildListUtvalgHasDifferentKundenummerThanUtvalgList, bool>
    //{
    //    /// <summary>
    //    /// The utvalgList repository
    //    /// </summary>
    //    private readonly IUtvalgListRepository _utvalgListRepository;
    //    /// <summary>
    //    /// The logger
    //    /// </summary>
    //    private readonly ILogger<GetUtvalgListIdsWhereChildListUtvalgHasDifferentKundenummerThanUtvalgListHandler> _logger;

    //    public GetUtvalgListIdsWhereChildListUtvalgHasDifferentKundenummerThanUtvalgListHandler(IUtvalgListRepository utvalgListRepository, ILogger<GetUtvalgListIdsWhereChildListUtvalgHasDifferentKundenummerThanUtvalgListHandler> logger)
    //    {
    //        _utvalgListRepository = utvalgListRepository ?? throw new ArgumentNullException(nameof(utvalgListRepository));
    //        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    //    }
    //    public async Task<bool> Handle(RequestGetUtvalgListIdsWhereChildListUtvalgHasDifferentKundenummerThanUtvalgList request, CancellationToken cancellationToken)
    //    {

    //        throw new NotImplementedException();
    //    }
    //}
}
