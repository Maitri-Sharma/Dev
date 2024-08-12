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
    public class GetNumberOfBudruterInTeamByTeamNRHandler : IRequestHandler<RequestGetNumberOfBudruterInTeamByTeamNR, string>
    {
        /// <summary>
        /// The utvalg repository
        /// </summary>
        private readonly IUtvalgRepository _utvalgRepository;
        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger<GetDistinctPRSByUtvalgIdHandler> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetNumberOfBudruterInTeamByTeamNRHandler"/> class.
        /// </summary>
        /// <param name="utvalgRepository">The utvalg repository.</param>
        /// <param name="logger">The logger.</param>
        /// <exception cref="System.ArgumentNullException">
        /// utvalgRepository
        /// or
        /// logger
        /// </exception>
        public GetNumberOfBudruterInTeamByTeamNRHandler(IUtvalgRepository utvalgRepository, ILogger<GetDistinctPRSByUtvalgIdHandler> logger)
        {
            _utvalgRepository = utvalgRepository ?? throw new ArgumentNullException(nameof(utvalgRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<string> Handle(RequestGetNumberOfBudruterInTeamByTeamNR request, CancellationToken cancellationToken)
        {
            return await _utvalgRepository.GetNumberOfBudruterInTeamByTeamNR(request.Teamnr);
        }
    }
}
