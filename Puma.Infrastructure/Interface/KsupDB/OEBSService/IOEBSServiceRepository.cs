using Puma.DataLayer.BusinessEntity.EC_Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Puma.Infrastructure.Interface.KsupDB.OEBSService
{
    /// <summary>
    ///IOEBSServiceRepository
    /// </summary>
    public interface IOEBSServiceRepository
    {
        /// <summary>
        /// Gets the order status.
        /// </summary>
        /// <returns></returns>
        Task<List<OrdreStatusDataEntity>> GetOrderStatus();

        /// <summary>
        /// Posts the selection distribution.
        /// </summary>
        /// <returns></returns>
        Task PostSelectionDistribution(OrderStatusResponseEntity orderStatusRequestEntity);

        Task<string> GetToken();

        Task<AvtaleOppslagResponse> AgreementLookup389(AgreementLookup agreementLookup);
    }
}
