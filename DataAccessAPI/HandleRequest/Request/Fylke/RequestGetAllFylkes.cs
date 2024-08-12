

using DataAccessAPI.HandleRequest.Response.Fylke;
using MediatR;
using System.Collections.Generic;

namespace DataAccessAPI.HandleRequest.Request.Fylke
{
    public class RequestGetAllFylkes : IRequest<List<ResponseGetAllFylkes>>
    {
    }
}
