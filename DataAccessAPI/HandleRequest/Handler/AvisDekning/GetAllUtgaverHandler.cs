using DataAccessAPI.HandleRequest.Request.AvisDekning;
using MediatR;
using Microsoft.Extensions.Logging;
using Puma.Infrastructure.Interface.KsupDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DataAccessAPI.HandleRequest.Handler.AvisDekning
{
    public class GetAllUtgaverHandler : IRequestHandler<RequestGetAllUtgaver, List<string>>
    {
        private readonly IAvisDekningRepository _avisDekningRepository;
        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger<GetAllUtgaverHandler> _logger;

        public GetAllUtgaverHandler(IAvisDekningRepository avisDekningRepository, ILogger<GetAllUtgaverHandler> logger)
        {
            _avisDekningRepository = avisDekningRepository ?? throw new ArgumentNullException(nameof(avisDekningRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<List<string>> Handle(RequestGetAllUtgaver request, CancellationToken cancellationToken)
        {
            return await _avisDekningRepository.GetAllUtgaver();
        }
    }
}
