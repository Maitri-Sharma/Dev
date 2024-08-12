using MediatR;
using RecreateServiceAPI.HandleRequest.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RecreateServiceAPI.HandleRequest.Handler
{
    public class RecreateServiceHandler : IRequestHandler<RequestRecreateService, bool>
    {
        public Task<bool> Handle(RequestRecreateService request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
