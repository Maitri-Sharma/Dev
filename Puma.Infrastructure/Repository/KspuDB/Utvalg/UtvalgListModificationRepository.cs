using Microsoft.EntityFrameworkCore;
using Puma.DataLayer.DatabaseModel.KspuDB;
using Puma.Infrastructure.Interface.KsupDB.Utvalg;
using Puma.Shared;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using static Puma.Shared.PumaEnum;

namespace Puma.Infrastructure.Repository.KspuDB.Utvalg
{
    public class UtvalgListModificationRepository : KsupDBGenericRepository<UtvalgListModification>, IUtvalgListModificationRepository
    {
        public UtvalgListModificationRepository(KspuDBContext context) : base(context)
        {
        }

        public async Task<List<UtvalgList>> GetUtvalgListModifications(string utvalglistname, string[] customerNos, int[] agreementNos, bool forceCustomerAndAgreementCheck, bool extendedInfo, int onlyBasisLists, bool includeChildrenUtvalg)
        {
            var utvalgListData = await (from utvalglists in _context.utvalgList
                                        where utvalglists.utvalglistname.Contains(utvalglistname)
                                        && utvalglists.isbasis.Value == onlyBasisLists
                                        select new UtvalgList()
                                        {
                                            Name = utvalglists.utvalglistname,
                                            Modifications = (from utvalgListModification in _context.UtvalgListModifications
                                                             where utvalgListModification.UtvalgListId == utvalglists.Listid
                                                             select new Puma.Shared.UtvalgModification()
                                                             {
                                                                 ModificationId = utvalgListModification.UtvalgListModificationId,
                                                                 ModificationTime = utvalgListModification.ModificationDate,
                                                                 UserId = utvalgListModification.UserId,
                                                                 ListId = utvalgListModification.UtvalgListId
                                                             }).ToList(),
                                            AllowDouble = utvalglists.allowdouble.HasValue && utvalglists.allowdouble.Value > 0,
                                            AntallWhenLastSaved = utvalglists.antall.HasValue ? utvalglists.antall.Value : 0,
                                            KundeNummer = utvalglists.kundenummer,
                                            Logo = utvalglists.logo,
                                            OrdreReferanse = utvalglists.ordrereferanse,
                                            InnleveringsDato = utvalglists.innleveringsdato.HasValue ? utvalglists.innleveringsdato.Value : DateTime.MinValue,
                                            Avtalenummer = utvalglists.avtalenummer.HasValue ? utvalglists.avtalenummer.Value : 0,
                                            Weight = utvalglists.vekt.HasValue ? utvalglists.vekt.Value : 0,
                                            DistributionDate = utvalglists.distributiondate.HasValue ? utvalglists.distributiondate.Value : DateTime.MinValue,
                                            IsBasis = utvalglists.isbasis.HasValue && utvalglists.isbasis.Value > 0,
                                            BasedOn = utvalglists.basedon.HasValue ? utvalglists.basedon.Value : 0,
                                            WasBasedOn = utvalglists.wasbasedon.HasValue ? utvalglists.wasbasedon.Value : 0,
                                            Thickness = utvalglists.thickness.HasValue ? utvalglists.thickness.Value : 0,
                                            //OrdreStatus = (OrdreStatus)Enum.Parse(typeof(OrdreStatus), utvalglists.ordrestatus),
                                            //OrdreType = (OrdreType)Enum.Parse(typeof(OrdreType), utvalglists.ordretype),
                                            //DistributionType = (DistributionType)Enum.Parse(typeof(DistributionType), utvalglists.distributiontype)
                                        }).ToListAsync();
            return utvalgListData;


        }
    }
}
