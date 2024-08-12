using Puma.DataLayer.DatabaseModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Puma.Infrastructure.Interface.KsupDB
{
    public interface IPumaRestCapacityRepository : IKsupDBGenericRepository<AddressPointsState>
    {
        Task Kapasitet_Ruter_Dato_AllAsync();
        Task Kapasitet_Delete_And_ImportAsync();
        Task Kapasitetdato_AddAsync(DateTime dato, int uke_nr, string distribusjonDag, string virkeDag);
        Task Kapasitetruter_AddAsync(DateTime dato, long reolNr, int restVekt, int restAntall, string mottakerType, double restThickness);
        Task Add_PRS_Calendar_To_PumaAsync(DateTime dato, string isHoliday, string isEarlyWeekFirstDay, string isEarlyWeekSecondDay, string isMidWeekFirstDay, string isMidWeekSecondDay, string frequencyType, DateTime lastModificationDate, long weekNo);

    }
}