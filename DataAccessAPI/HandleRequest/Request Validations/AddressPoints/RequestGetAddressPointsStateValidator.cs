using DataAccessAPI.HandleRequest.Request.AddressPoints;
using FluentValidation;

namespace DataAccessAPI.HandleRequest.Request_Validations.AddressPoints
{
    public class RequestGetAddressPointsStateValidator : AbstractValidator<RequestGetAddressPointsState>
    {
        public RequestGetAddressPointsStateValidator()
        {
            RuleFor(x => x.userId).NotEmpty();
        }
    }
}
