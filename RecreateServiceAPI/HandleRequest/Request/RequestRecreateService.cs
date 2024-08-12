using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecreateServiceAPI.HandleRequest.Request
{
    public class RequestRecreateService : IRequest<bool>
    {
        public string UtvalgId { get; set; }

        public string EmailId { get; set; }
    }
}
