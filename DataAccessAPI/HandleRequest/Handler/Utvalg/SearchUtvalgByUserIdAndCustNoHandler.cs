using AutoMapper;
using DataAccessAPI.HandleRequest.Request.Utvalg;
using DataAccessAPI.HandleRequest.Response.Utvalg;
using MediatR;
using Microsoft.Extensions.Logging;
using Puma.Infrastructure.Interface.KsupDB.Utvalg;
using Puma.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
namespace DataAccessAPI.HandleRequest.Handler.Utvalg
{
    public class SearchUtvalgByUserIdAndCustNoHandler : IRequestHandler<RequestSearchUtvalgByUserIdandCustNo, List<ResponseGetUtlvagDetailByUserIdAndCustNo>>
    {
        /// <summary>
        /// The utvalg repository
        /// </summary>
        private readonly IUtvalgRepository _utvalgRepository;
        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger<SearchUtvalgByUserIdAndCustNoHandler> _logger;

        /// <summary>
        /// The mapper
        /// </summary>
        private readonly IMapper _mapper;

        public SearchUtvalgByUserIdAndCustNoHandler(IUtvalgRepository utvalgRepository, ILogger<SearchUtvalgByUserIdAndCustNoHandler> logger, IMapper mapper)
        {
            _utvalgRepository = utvalgRepository ?? throw new ArgumentNullException(nameof(utvalgRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<List<ResponseGetUtlvagDetailByUserIdAndCustNo>> Handle(RequestSearchUtvalgByUserIdandCustNo request, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Calling SearchUtvalgByUserIDAndCustNo from Repository");
            var utvalgData = await _utvalgRepository.SearchUtvalgByUserIDAndCustNo(request.userID,request.customerNos,request.agreementNos,request.forceCustomerAndAgreementCheck,request.onlyBasisUtvalg);
            List<ResponseGetUtlvagDetailByUserIdAndCustNo> response = null;
            if (utvalgData?.Any() == true)
            {
                response = _mapper.Map<List<Puma.Shared.Utvalg>, List<ResponseGetUtlvagDetailByUserIdAndCustNo>>(utvalgData).ToList();
            }
            return response;
        }
    }
}
