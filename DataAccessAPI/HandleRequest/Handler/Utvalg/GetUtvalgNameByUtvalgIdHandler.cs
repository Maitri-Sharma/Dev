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
    public class GetUtvalgNameByUtvalgIdHandler : IRequestHandler<RequestGetUtlvagNameByUtvalgId, string>
    {
        /// <summary>
        /// The utvalg repository
        /// </summary>
        private readonly IUtvalgRepository _utvalgRepository;
        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger<GetUtvalgCampaignsByUtvalgIdHandler> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetUtvalgNameByUtvalgIdHandler"/> class.
        /// </summary>
        /// <param name="utvalgRepository">The utvalg repository.</param>
        /// <param name="logger">The logger.</param>
        /// <exception cref="System.ArgumentNullException">
        /// utvalgRepository
        /// or
        /// logger
        /// </exception>
        public GetUtvalgNameByUtvalgIdHandler(IUtvalgRepository utvalgRepository, ILogger<GetUtvalgCampaignsByUtvalgIdHandler> logger)
        {
            _utvalgRepository = utvalgRepository ?? throw new ArgumentNullException(nameof(utvalgRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Handles a request
        /// </summary>
        /// <param name="request">The request</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>
        /// Response from the request
        /// </returns>
        public async Task<string> Handle(RequestGetUtlvagNameByUtvalgId request, CancellationToken cancellationToken)
        {
            return await _utvalgRepository.GetUtvalgName(request.UtvagId);

        }
    }
}
