using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataAccessAPI.HandleRequest.Request.UtvalgList
{
    public class RequestIsUtvalgConnectedToParentList : IRequest<bool>
    {
        public int listId { get; set; }
    }
}
