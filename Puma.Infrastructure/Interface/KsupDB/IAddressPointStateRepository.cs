using Puma.DataLayer.DatabaseModel;
using Puma.Infrastructure.Interface.KsupDB;
using Puma.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Puma.Infrastructure.Interface
{

    /// <summary>
    /// Address point state repository
    /// </summary>
    public interface IAddressPointStateRepository : IKsupDBGenericRepository<AddressPointsState>
    {
        Task<int> SaveAdressPointsAPI(string userId, AddressPointList addressPointList);

        Task<List<AddressPoint>> GetAddressPointsState(string userId);
    }
}
