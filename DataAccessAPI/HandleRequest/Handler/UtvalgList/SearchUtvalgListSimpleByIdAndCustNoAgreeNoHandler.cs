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
    public class SearchUtvalgListSimpleByIdAndCustNoAgreeNoHandler : IRequestHandler<RequestSearchUtvalgListSimpleByIdAndCustNoAgreeNo, List<ResponseSearchUtvalgListSimpleByIdAndCustNoAgreeNo>>
    {
        /// <summary>
        /// The utvalgList repository
        /// </summary>
        private readonly IUtvalgListRepository _utvalgListRepository;
        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger<SearchUtvalgListSimpleByIdAndCustNoAgreeNoHandler> _logger;

        /// <summary>
        /// The mapper
        /// </summary>
        private readonly IMapper _mapper;

        public SearchUtvalgListSimpleByIdAndCustNoAgreeNoHandler(IUtvalgListRepository utvalgListRepository, ILogger<SearchUtvalgListSimpleByIdAndCustNoAgreeNoHandler> logger, IMapper mapper)
        {
            _utvalgListRepository = utvalgListRepository ?? throw new ArgumentNullException(nameof(utvalgListRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }
        public  async Task<List<ResponseSearchUtvalgListSimpleByIdAndCustNoAgreeNo>> Handle(RequestSearchUtvalgListSimpleByIdAndCustNoAgreeNo request, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Calling SearchUtvalgListSimpleByIdAndCustNoAgreeNo from Repository");
            var utvalgListData = await _utvalgListRepository.SearchUtvalgListSimpleByIdAndCustNoAgreeNo(request.utvalglistid, request.customerNos,request.agreementNos,request.forceCustomerAndAgreementCheck,request.extendedInfo,request.onlyBasisLists);
            List<ResponseSearchUtvalgListSimpleByIdAndCustNoAgreeNo> response = null;
            if (utvalgListData?.Any() == true)
            {
                response = _mapper.Map<List<Puma.Shared.UtvalgList>, List<ResponseSearchUtvalgListSimpleByIdAndCustNoAgreeNo>>(utvalgListData).ToList();
            }
            return response;

        }
    }
}
