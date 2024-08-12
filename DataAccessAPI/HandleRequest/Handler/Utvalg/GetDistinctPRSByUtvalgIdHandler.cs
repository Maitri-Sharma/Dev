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
    public class GetDistinctPRSByUtvalgIdHandler : IRequestHandler<RequestGetDistinctPRSByUtvalgId, List<string>>
    {
        /// <summary>
        /// The utvalg repository
        /// </summary>
        private readonly IUtvalgRepository _utvalgRepository;
        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger<GetDistinctPRSByUtvalgIdHandler> _logger;

        public GetDistinctPRSByUtvalgIdHandler(IUtvalgRepository utvalgRepository, ILogger<GetDistinctPRSByUtvalgIdHandler> logger)
        {
            _utvalgRepository = utvalgRepository ?? throw new ArgumentNullException(nameof(utvalgRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<List<string>> Handle(RequestGetDistinctPRSByUtvalgId request, CancellationToken cancellationToken)
        {
            return await _utvalgRepository.GetDistinctPRS(request.Utvalgid);
        }
    }
}
