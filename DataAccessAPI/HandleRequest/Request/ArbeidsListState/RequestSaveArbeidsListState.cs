using MediatR;
using Puma.Shared;
using System.Collections.Generic;

namespace DataAccessAPI.HandleRequest.Request.ArbeidsListState
{
    public class RequestSaveArbeidsListState : IRequest<int>
    {
      public List<ArbeidsListEntryState> arbeidsListState { get; set; }
    }
}
