using AutoMapper;
using DataAccessAPI.HandleRequest.Request.Utvalg;
using DataAccessAPI.HandleRequest.Response.Utvalg;
using MediatR;
using Microsoft.Extensions.Logging;
using Puma.Infrastructure.Interface.KsupDB.Utvalg;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;


namespace DataAccessAPI.HandleRequest.Handler.Utvalg
{
    public class GetUtvalgReolIDsHandler : IRequestHandler<RequestGetUtvalgReolIDs, List<long>>
    {
        /// <summary>
        /// The utvalg repository
        /// </summary>
        private readonly IUtvalgRepository _utvalgRepository;
        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger<GetUtvalgDetailByIdHandler> _logger;

        public GetUtvalgReolIDsHandler(IUtvalgRepository utvalgRepository, ILogger<GetUtvalgDetailByIdHandler> logger)
        {
            _utvalgRepository = utvalgRepository ?? throw new ArgumentNullException(nameof(utvalgRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<List<long>> Handle(RequestGetUtvalgReolIDs request, CancellationToken cancellationToken)
        {
          return   await _utvalgRepository.GetUtvalgReolIDs(request.UtlvagId);
        }
    }
}
