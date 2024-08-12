using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Puma.Infrastructure.Interface.KsupDB
{
    /// <summary>
    /// Interface for Map operation
    /// </summary>
    public interface IMapOperationRepository
    {
        Task<string> ExportMapImage(List<long> RouteIds, bool isCustomerWeb, string strDayDetails, string selectedAddress = "");
    }
}
