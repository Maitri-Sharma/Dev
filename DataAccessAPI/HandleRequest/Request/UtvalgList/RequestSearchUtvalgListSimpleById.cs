using DataAccessAPI.HandleRequest.Response.UtvalgList;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataAccessAPI.HandleRequest.Request.UtvalgList
{
    public class RequestSearchUtvalgListSimpleById : IRequest<List<ResponseSearchUtvalgListSimpleById>>
    {
        public int utvalglistid { get; set; }

        public string customerNos { get; set; }
    }
}
