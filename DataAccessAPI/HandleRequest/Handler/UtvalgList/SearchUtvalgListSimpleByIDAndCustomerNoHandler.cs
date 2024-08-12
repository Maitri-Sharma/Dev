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
    public class SearchUtvalgListSimpleByIDAndCustomerNoHandler : IRequestHandler<RequestSearchUtvalgListSimpleByIDAndCustomerNo, List<ResponseSearchUtvalgListSimpleByIDAndCustomerNo>>
    {
        /// <summary>
        /// The utvalg repository
        /// </summary>
        private readonly IUtvalgListRepository _utvalgListRepository;
        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger<SearchUtvalgListSimpleByIDAndCustomerNoHandler> _logger;

        /// <summary>
        /// The mapper
        /// </summary>
        private readonly IMapper _mapper;

        public SearchUtvalgListSimpleByIDAndCustomerNoHandler(IUtvalgListRepository utvalgListRepository, ILogger<SearchUtvalgListSimpleByIDAndCustomerNoHandler> logger, IMapper mapper)
        {
            _utvalgListRepository = utvalgListRepository ?? throw new ArgumentNullException(nameof(utvalgListRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }
        public async Task<List<ResponseSearchUtvalgListSimpleByIDAndCustomerNo>> Handle(RequestSearchUtvalgListSimpleByIDAndCustomerNo request, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Calling SearchUtvalgListSimpleByIDAndCustomerNo from Repository");
            var utvalgListData = await _utvalgListRepository.SearchUtvalgListSimpleByIDAndCustomerNo(request.utvalglistname, request.customerNos,request.agreementNos,request.forceCustomerAndAgreementCheck,request.extendedInfo,request.onlyBasisLists,request.includeChildrenUtvalg);
            List<ResponseSearchUtvalgListSimpleByIDAndCustomerNo> response = null;
            if (utvalgListData?.Any() == true)
            {
                response = _mapper.Map<List<Puma.Shared.UtvalgList>, List<ResponseSearchUtvalgListSimpleByIDAndCustomerNo>>(utvalgListData).ToList();
            }
            return response;
            
        }
    }
}
