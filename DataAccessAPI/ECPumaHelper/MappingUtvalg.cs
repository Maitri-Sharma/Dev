using DataAccessAPI;
using DataAccessAPI.Controllers;
using Microsoft.Extensions.Logging;
using Puma.Infrastructure.Interface.KsupDB.Utvalg;
using Puma.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Puma.Shared.PumaEnum;

namespace DataAccessAPI.ECPumaHelper
{
    public class MappingUtvalg
    {
        #region Variables
        private readonly ILogger<MappingUtvalg> _logger;
        private readonly UtvalgController utvalgController;
        private readonly UtvalgListController utvalgListController;
        private readonly IUtvalgRepository _utvalgRepository;
        #endregion

        #region Constructors
        /// <summary>
        /// Paramaterized Constructor
        /// </summary>
        /// <param name="logger">Instance of Microsoft.Extensions.Logging</param>
        /// <param name="loggerUtvalg">Instance of Microsoft.Extensions.Logging</param>
        /// <param name="loggerreol">Instance of Microsoft.Extensions.Logging</param>
        /// <param name="loggerConfig">Instance of Microsoft.Extensions.Logging</param>
        /// <param name="loggerUtvalgList">Instance of Microsoft.Extensions.Logging</param>
        /// <param name="utvalgRepository">Instance of Utvalg repository</param>
        public MappingUtvalg(ILogger<MappingUtvalg> logger, ILogger<ConfigController> loggerConfig, ILogger<UtvalgController> loggerUtvalg, ILogger<ReolController> loggerreol, ILogger<UtvalgListController> loggerUtvalgList,IUtvalgRepository utvalgRepository)
        {
            _logger = logger;
            _utvalgRepository = utvalgRepository;

            utvalgController = new UtvalgController(loggerUtvalg, loggerConfig, loggerreol);
            utvalgListController = new UtvalgListController(loggerUtvalgList, loggerConfig, loggerUtvalg,  loggerreol, _utvalgRepository);
           
        }
        #endregion

        /// <summary>
        ///     ''' Mapper mellom ett KSPU Utvalg og ett wsUtvalg
        ///     ''' wsUtvalg er representasjonen av et utvalg/liste i webserviceentiteten.
        ///     ''' </summary>
        ///     ''' <param name="KspuUtvalg"></param>
        ///     ''' <returns></returns>
        ///     ''' <remarks></remarks>
        public UtvalgExtended MapKSPUUtvalgToWSUtvalg(Utvalg KspuUtvalg)
        {
            _logger.LogDebug("Inside into MapKSPUUtvalgToWSUtvalg");
            UtvalgExtended wsUtvalg = new UtvalgExtended();
            wsUtvalg.UtvalgId = KspuUtvalg.UtvalgId;
            wsUtvalg.Type = PumaEnum.UtvalgsTypeKode.Utvalg;
            wsUtvalg.Antall = Convert.ToInt32(KspuUtvalg.AntallWhenLastSaved);
            wsUtvalg.Navn = KspuUtvalg.Name;
            //if (KspuUtvalg.ListId != null)
            //    wsUtvalg.ParentId = KspuUtvalg.List.ListId;
            wsUtvalg.PreviewURL = ""; // Ikke impl ennå
            if (KspuUtvalg.Modifications != null)
            {
                string sistEndretAv = "";
                DateTime sistOppdatert = new DateTime(1, 1, 1);
                foreach (UtvalgModification m in KspuUtvalg.Modifications)
                {
                    if (m.ModificationTime.CompareTo(sistOppdatert) > 0)
                    {
                        sistOppdatert = m.ModificationTime.Date;
                        sistEndretAv = m.UserId;
                    }
                }
                wsUtvalg.SistOppdatert = sistOppdatert;
                wsUtvalg.SistEndretAv = sistEndretAv;
            }
            // for UtvalgExtended
            wsUtvalg.KundeNr = KspuUtvalg.KundeNummer;
            _logger.LogInformation("Data Returned: " + wsUtvalg);
            _logger.LogDebug("Exiting From MapKSPUUtvalgToWSUtvalg");

            return wsUtvalg;
        }

        /// <summary>
        ///     ''' Mapper mellom en KSPU Liste og ett wsUtvalg
        ///     ''' wsUtvalg er representasjonen av et utvalg/liste i webserviceentiteten.
        ///     ''' </summary>
        ///     ''' <param name="KspuListe"></param>
        ///     ''' <returns></returns>
        ///     ''' <remarks></remarks>
        public UtvalgExtended MapKSPUListeToWSUtvalg(UtvalgList KspuListe)
        {
            _logger.LogDebug("Inside into MapKSPUListeToWSUtvalg");
            UtvalgExtended wsUtvalg = new UtvalgExtended();
            wsUtvalg.UtvalgId = KspuListe.ListId;
            wsUtvalg.Type = PumaEnum.UtvalgsTypeKode.Liste;
            wsUtvalg.Antall = Convert.ToInt32(KspuListe.AntallWhenLastSaved);
            wsUtvalg.Navn = KspuListe.Name;
            if (KspuListe.ParentList != null)
                wsUtvalg.ParentId = KspuListe.ParentList.ListId;
            else
                wsUtvalg.ParentId = 0;
            wsUtvalg.PreviewURL = ""; // Ikke impl ennå
            wsUtvalg.SistOppdatert = KspuListe.SistOppdatert.Date;
            wsUtvalg.SistEndretAv = KspuListe.SistEndretAv;
            // for klassen UtvalgExtended
            wsUtvalg.KundeNr = KspuListe.KundeNummer;
            _logger.LogInformation("Data Returned: " + wsUtvalg);
            _logger.LogDebug("Exiting From MapKSPUListeToWSUtvalg");
            return wsUtvalg;
        }

        public List<UtvalgExtended> ConvertUtvalgListWithChildrenCollectionToWSUtvalg(UtvalgsListCollection ulc, ref System.Nullable<FeilKode> feilKode, bool isIdTypeSok = false)
        {
            _logger.LogDebug("Inside into ConvertUtvalgListWithChildrenCollectionToWSUtvalg");
            List<UtvalgExtended> resultat = new List<UtvalgExtended>();
            //UtvalgList ul;
            foreach (UtvalgList ul in ulc)
            {
                // For Id/Typesøk skal det sendes en feilmelding
                if (isIdTypeSok && ContainsUtvalglistOrderRef(ul))
                {
                    _logger.LogWarning("Webservice Finnutvalg: Angitt liste med id=" + ul.ListId + " er allerede knyttet til en ordre eller tilbud");
                    feilKode = new Nullable<FeilKode>(FeilKode.UtvalgKnyttetTilAnnetTilbudOrdre);
                    return resultat;
                }

                // hovedliste
                if (!HasUtvalglistOrderRef(ul))
                    resultat.Add(MapKSPUListeToWSUtvalg(ul));

                // tilhørende utvalg
                //Utvalg u;
                foreach (Utvalg u in ul.MemberUtvalgs)
                {
                    if (!HasUtvalgOrderRef(u))
                        resultat.Add(MapKSPUUtvalgToWSUtvalg(u));
                }

                // tilhørende lister
                //UtvalgList childUl;
                foreach (UtvalgList childUl in ul.MemberLists)
                {
                    if (!HasUtvalglistOrderRef(childUl))
                        resultat.Add(MapKSPUListeToWSUtvalg(childUl));
                    // tilhørende utvalg til tilhørende lister
                    //Utvalg childU;
                    foreach (Utvalg childU in childUl.MemberUtvalgs)
                    {
                        if (!HasUtvalgOrderRef(childU))
                            resultat.Add(MapKSPUUtvalgToWSUtvalg(childU));
                    }
                }
            }
            _logger.LogInformation("Data Returned: " + resultat);
            _logger.LogDebug("Exiting From MapKSPUListeToWSUtvalg");
            return resultat;
        }

        public List<UtvalgExtended> ConvertUtvalgsListCollectionToWSUtvalg(UtvalgsListCollection ulc, ref System.Nullable<FeilKode> feilKode, bool isIdTypeSok = false)
        {
            _logger.LogDebug("Inside into ConvertUtvalgsListCollectionToWSUtvalg");
            List<UtvalgExtended> resultat = new List<UtvalgExtended>();
            //UtvalgList ul;
            foreach (UtvalgList ul in ulc)
            {
                if (ContainsUtvalglistOrderRef(ul))
                {
                    if (isIdTypeSok)
                    {
                        _logger.LogWarning("Webservice Finnutvalg: Angitt liste med id=" + ul.ListId + " er allerede knyttet til en ordre eller tilbud");
                        feilKode = new Nullable<FeilKode>(FeilKode.UtvalgKnyttetTilAnnetTilbudOrdre);
                    }
                    else
                    {
                    }
                }
                else
                    resultat.Add(MapKSPUListeToWSUtvalg(ul));
            }

            _logger.LogInformation("Data Returned: " + resultat);
            _logger.LogDebug("Exiting From ConvertUtvalgsListCollectionToWSUtvalg");
            return resultat;
        }

        public List<UtvalgExtended> ConvertUtvalgCollectionToWSUtvalg(UtvalgCollection uc, ref System.Nullable<FeilKode> feilKode, bool isIdTypeSok = false)
        {
            _logger.LogDebug("Inside into ConvertUtvalgCollectionToWSUtvalg");
            List<UtvalgExtended> resultat = new List<UtvalgExtended>();
            //Utvalg u;
            foreach (Utvalg u in uc)
            {
                if (HasUtvalgOrderRef(u))
                {
                    if (isIdTypeSok)
                    {
                        _logger.LogWarning("Webservice Finnutvalg: Angitt utvalg med id=" + u.UtvalgId + " er allerede knyttet til en ordre eller tilbud");
                        feilKode = new Nullable<FeilKode>(FeilKode.UtvalgKnyttetTilAnnetTilbudOrdre);
                    }
                    else
                    {
                    }
                }
                else
                    resultat.Add(MapKSPUUtvalgToWSUtvalg(u));
            }
            _logger.LogInformation("Data Returned: " + resultat);
            _logger.LogDebug("Exiting From ConvertUtvalgCollectionToWSUtvalg");
            return resultat;
        }


        public List<ECPumaData> RemoveDuplicate(List<ECPumaData> resultat)
        {
            _logger.LogDebug("Inside into RemoveDuplicate");
            List<ECPumaData> updatedResultat = new List<ECPumaData>();
            bool isDuplicate = false;
            int count = resultat.Count - 1;
            for (int i = 0; i <= count; i++)
            {
                isDuplicate = false;
                int? id = resultat[i].UtvalgId;
                UtvalgsTypeKode type = resultat[i].Type;
                for (int j = i + 1; j <= count; j++)
                {
                    if (id == resultat[j].UtvalgId && type.Equals(resultat[j].Type))
                    {
                        isDuplicate = true;
                        break;
                    }
                }
                if (!isDuplicate)
                    updatedResultat.Add(resultat[i]);
            }
            _logger.LogInformation("Data Returned: " + updatedResultat);
            _logger.LogDebug("Exiting From RemoveDuplicate");
            return updatedResultat;
        }

        /// <summary>
        ///     ''' Alle utvalg som tilhører en liste eid av et annet kundenummer fjernes fra collection
        ///     ''' </summary>
        ///     ''' <param name="resultat"></param>
        ///     ''' <param name="kundenr"></param>
        ///     ''' <returns></returns>
        ///     ''' <remarks></remarks>
        public List<UtvalgExtended> RemoveUtvalgOwnedByOtherKundenr(List<UtvalgExtended> resultat, string kundenr)
        {
            _logger.LogDebug("Inside into RemoveUtvalgOwnedByOtherKundenr");
            List<UtvalgExtended> updatedResultat = new List<UtvalgExtended>();
            bool doAdd;
            foreach (UtvalgExtended res in resultat)
            {
                doAdd = true;
                if (res.ParentId != null/* TODO Change to default(_) if this is not a reference type */ )
                {
                    try
                    {
                        UtvalgList ul = (UtvalgList)utvalgListController.GetUtvalgListSimple((int)res.ParentId);
                        if (ul.KundeNummer != kundenr)
                            doAdd = false;
                    }
                    catch (Exception exception)
                    {
                        _logger.LogError(exception.Message);
                    }
                }
                if (doAdd)
                    updatedResultat.Add(res);
            }
            _logger.LogInformation("Data Returned: " + updatedResultat);
            _logger.LogDebug("Exiting From RemoveUtvalgOwnedByOtherKundenr");
            return updatedResultat;
        }

        /// <summary>
        ///     ''' Fjern alle utvalg fra lista som ha en tilhører en liste
        ///     ''' </summary>
        ///     ''' <param name="resultat"></param>
        ///     ''' <returns></returns>
        ///     ''' <remarks></remarks>
        public List<UtvalgExtended> RemoveUtvalgWithParent(List<UtvalgExtended> resultat)
        {
            _logger.LogDebug("Inside into RemoveUtvalgWithParent");
            List<UtvalgExtended> updatedResultat = new List<UtvalgExtended>();
            foreach (UtvalgExtended res in resultat)
            {
                if (res.ParentId == null/* TODO Change to default(_) if this is not a reference type */ )
                    updatedResultat.Add(res);
            }
            _logger.LogInformation("Data Returned: " + updatedResultat);
            _logger.LogDebug("Exiting From RemoveUtvalgWithParent");
            return updatedResultat;
        }

        /// <summary>
        ///     ''' Sjekker om en Utvalgliste har undeliggende utvalg/lister som allerede er koblet mot et tilbud eller en ordre.
        ///     ''' </summary>
        ///     ''' <param name="ul"></param>
        ///     ''' <returns></returns>
        ///     ''' <remarks></remarks>
        private bool ContainsUtvalglistOrderRef(UtvalgList ul)
        {
            _logger.LogDebug("Inside into ContainsUtvalglistOrderRef");
            if (ul.OrdreType.Equals(OrdreType.O))
                return true;
            if (ul.OrdreType.Equals(OrdreType.T))
                return true;

            // utvalg
            foreach (Utvalg u in ul.MemberUtvalgs)
            {
                if (u.OrdreType.Equals(OrdreType.O))
                    return true;
                if (u.OrdreType.Equals(OrdreType.T))
                    return true;
            }
            // tilhørende lister
            foreach (UtvalgList childUl in ul.MemberLists)
            {
                if (childUl.OrdreType.Equals(OrdreType.O))
                    return true;
                if (childUl.OrdreType.Equals(OrdreType.T))
                    return true;
                // tilhørende utvalg til tilhørende lister
                foreach (Utvalg childU in childUl.MemberUtvalgs)
                {
                    if (childU.OrdreType.Equals(OrdreType.O))
                        return true;
                    if (childU.OrdreType.Equals(OrdreType.T))
                        return true;
                }
            }
            //_logger.LogInformation("Data Returned: " + updatedResultat);
            _logger.LogDebug("Exiting From ContainsUtvalglistOrderRef");
            return false;
        }

        /// <summary>
        ///     ''' Sjekker om en Utvalgliste har allerede er koblet mot et tilbud eller en ordre.
        ///     ''' </summary>
        ///     ''' <param name="ul"></param>
        ///     ''' <returns></returns>
        ///     ''' <remarks></remarks>
        private bool HasUtvalglistOrderRef(UtvalgList ul)
        {
            _logger.LogDebug("Inside into HasUtvalglistOrderRef");
            if (ul.OrdreType.Equals(OrdreType.O))
                return true;
            if (ul.OrdreType.Equals(OrdreType.T))
                return true;
            //_logger.LogInformation("Data Returned: " + updatedResultat);
            _logger.LogDebug("Exiting From HasUtvalglistOrderRef");
            return false;
        }

        /// <summary>
        ///     ''' Sjekker om en Utvalget allerede er koblet mot et tilbud eller en ordre.
        ///     ''' </summary>
        ///     ''' <param name="u"></param>
        ///     ''' <returns></returns>
        ///     ''' <remarks></remarks>
        private bool HasUtvalgOrderRef(Utvalg u)
        {
            _logger.LogDebug("Inside into HasUtvalgOrderRef");
            if (u.OrdreType.Equals(OrdreType.O))
                return true;
            if (u.OrdreType.Equals(OrdreType.T))
                return true;
            //_logger.LogInformation("Data Returned: " + updatedResultat);
            _logger.LogDebug("Exiting From HasUtvalgOrderRef");
            return false;
        }
    }
}