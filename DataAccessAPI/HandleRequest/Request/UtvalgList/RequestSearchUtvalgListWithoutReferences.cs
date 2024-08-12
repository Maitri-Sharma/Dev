using DataAccessAPI.HandleRequest.Response.UtvalgList;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Puma.Shared.PumaEnum;

namespace DataAccessAPI.HandleRequest.Request.UtvalgList
{
    public class RequestSearchUtvalgListWithoutReferences : IRequest<ResponseSearchUtvalgListWithoutReferences>
    {
        public string Utvalglistname { get; set; }

        public SearchMethod searchMethod { get; set; }

        public string customerNumber { get; set; }
        public bool canHaveEmptyCustomerNumber { get; set; }
    }
}
