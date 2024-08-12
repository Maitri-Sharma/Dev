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
    public class SearchUtvalgByUtvalgIdAndCustNoHandler : IRequestHandler<RequestSearchUtvalgByUtvalgIdAndCustoNo, List<ResponseGetUtlvagDetailByUtvalgIdAndCustNo>>
    {
        /// <summary>
        /// The utvalg repository
        /// </summary>
        private readonly IUtvalgRepository _utvalgRepository;
        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger<SearchUtvalgByUtvalgIdAndCustNoHandler> _logger;

        /// <summary>
        /// The mapper
        /// </summary>
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="SearchUtvalgByUtvalgIdAndCustNoHandler"/> class.
        /// </summary>
        /// <param name="utvalgRepository">The utvalg repository.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="mapper">The mapper.</param>
        /// <exception cref="System.ArgumentNullException">
        /// utvalgRepository
        /// or
        /// logger
        /// or
        /// mapper
        /// </exception>
        public SearchUtvalgByUtvalgIdAndCustNoHandler(IUtvalgRepository utvalgRepository, ILogger<SearchUtvalgByUtvalgIdAndCustNoHandler> logger, IMapper mapper)
        {
            _utvalgRepository = utvalgRepository ?? throw new ArgumentNullException(nameof(utvalgRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<List<ResponseGetUtlvagDetailByUtvalgIdAndCustNo>> Handle(RequestSearchUtvalgByUtvalgIdAndCustoNo request, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Calling SearchUtvalgByUtvalgIdAndCustmerNo from Repository");
            var utvalgData = await _utvalgRepository.SearchUtvalgByUtvalgIdAndCustmerNo(request.UtvagId,request.customerNos,request.agreementNos,request.forceCustomerAndAgreementCheck,request.IncludeReol,request.ExtendInfo,request.onlyBasisUtvalg);
            List<ResponseGetUtlvagDetailByUtvalgIdAndCustNo> response = null;
            if (utvalgData?.Any() == true)
            {
                response = _mapper.Map<List<Puma.Shared.Utvalg>, List<ResponseGetUtlvagDetailByUtvalgIdAndCustNo>>(utvalgData).ToList();
            }
            return response;
        }
    }
}
