using DataAccessAPI.HandleRequest.Response.ArbeidsListState;
using MediatR;
using Puma.Shared;
using System.Collections.Generic;

namespace DataAccessAPI.HandleRequest.Request.ArbeidsListState
{
    public class RequestGetArbeidsListState : IRequest<List<ResponseArbeidsListEntryState>>
    {
        public string userId { get; set; }
    }
}
