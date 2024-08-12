using DataAccessAPI.HandleRequest.Response.Utvalg;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DataAccessAPI.HandleRequest.Request.Utvalg
{
    public class RequestFindBasisUtvalgFordelingToSend : IRequest<List<ResponseUtvalgBasisFordeling>>
    {
    }
}
