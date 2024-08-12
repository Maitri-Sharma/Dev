using Puma.DataLayer.BusinessEntity;
using Puma.DataLayer.DatabaseModel;
using Puma.Infrastructure.Interface.KsupDB;
using Puma.Shared;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Puma.Infrastructure.Interface.KsupDB
{
    public interface IFylkeRepository : IKsupDBGenericRepository<AddressPointsState>
    {
        Task<FylkeCollection> GetAllFylkes();
        Task<Fylke> GetFylke(string FylkeId);
    }
}
