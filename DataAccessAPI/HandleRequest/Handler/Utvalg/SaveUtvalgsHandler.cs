using AutoMapper;
using DataAccessAPI.HandleRequest.Request.Utvalg;
using DataAccessAPI.HandleRequest.Response.Utvalg;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DataAccessAPI.HandleRequest.Handler.Utvalg
{
    /// <summary>
    /// SaveUtvalgsHandler
    /// </summary>
    public class SaveUtvalgsHandler : IRequestHandler<RequestSaveUtvalgs, List<ResponseSaveUtvalgs>>
    {

        /// <summary>
        /// The mediator
        /// </summary>
        private readonly IMediator _mediator;

        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger<SaveUtvalgsHandler> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="SaveUtvalgsHandler"/> class.
        /// </summary>
        /// <param name="mediator">The mediator.</param>
        /// <param name="logger">The logger.</param>
        /// <exception cref="System.ArgumentNullException">
        /// mediator
        /// or
        /// logger
        /// </exception>
        public SaveUtvalgsHandler(IMediator mediator, ILogger<SaveUtvalgsHandler> logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
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
        public async Task<List<ResponseSaveUtvalgs>> Handle(RequestSaveUtvalgs request, CancellationToken cancellationToken)
        {
            List<ResponseSaveUtvalgs> response = new();

            _logger.LogDebug("Request to save list of selection" + JsonConvert.SerializeObject(request));

            foreach (var itemUtvalg in request.utvalgs)
            {
                RequestSaveUtvalg requestSaveUtvalg = new RequestSaveUtvalg();
                requestSaveUtvalg.utvalg = itemUtvalg;
                requestSaveUtvalg.forceUtvalgListId = request.forceUtvalgListId;
                requestSaveUtvalg.saveOldReoler = request.saveOldReoler;
                requestSaveUtvalg.skipHistory = request.skipHistory;
                requestSaveUtvalg.userName = request.userName;

                ResponseSaveUtvalgs responseSave = new();
                _logger.LogDebug("calling  handler to save utvalg" + JsonConvert.SerializeObject(itemUtvalg));

                try
                {
                    responseSave.utvalg = await _mediator.Send(requestSaveUtvalg);
                }
                catch (Exception ex)
                {
                    responseSave.ErrorMessage = "Error during save utvalg :" + itemUtvalg.Name + "with error :" + ex.Message;
                    _logger.LogError("Error during save utvalg" + itemUtvalg.Name, ex);

                }
                response.Add(responseSave);
            }

            return response;

        }
    }
}
