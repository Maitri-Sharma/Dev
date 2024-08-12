using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataAccessAPI.HandleRequest.Request.AvisDekning
{
    public class RequestAvisExists : IRequest<bool>
    {
        public string utgave { get; set; }
    }
}
