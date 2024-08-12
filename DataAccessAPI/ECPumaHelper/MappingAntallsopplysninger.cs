using DataAccessAPI;
using DataAccessAPI.Controllers;
using Microsoft.Extensions.Logging;
using Puma.Infrastructure.Interface.KsupDB.Utvalg;
using Puma.Shared;
using System;
using System.Collections.Generic;
using static Puma.Shared.PumaEnum;

namespace DataAccessAPI.ECPumaHelper
{
    public class MappingAntallsopplysninger
    {
        #region Variables
        private readonly ILogger<MappingAntallsopplysninger> _logger;
        private readonly UtvalgController utvalgController;
        private readonly UtvalgListController utvalgListController;
        private readonly IUtvalgListRepository _utvalgListRepository;

        private readonly IUtvalgRepository _utvalgRepository;
        private readonly ConfigController configController;

        // private readonly MappingUtvalg mappingUtvalg;
        #endregion

        #region Constructors                
        /// <summary>
        /// Initializes a new instance of the <see cref="MappingAntallsopplysninger"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="loggerConfig">The logger configuration.</param>
        /// <param name="loggerUtvalg">The logger utvalg.</param>
        /// <param name="loggerreol">The loggerreol.</param>
        /// <param name="loggerUtvalgList">The logger utvalg list.</param>
        /// <param name="utvalgRepository">The utvalg repository.</param>
        /// <param name="utvalgListRepository">The utvalg list repository.</param>
        public MappingAntallsopplysninger(ILogger<MappingAntallsopplysninger> logger, ILogger<ConfigController> loggerConfig, ILogger<UtvalgController> loggerUtvalg, ILogger<ReolController> loggerreol, ILogger<UtvalgListController> loggerUtvalgList,
            IUtvalgRepository utvalgRepository, IUtvalgListRepository utvalgListRepository)
        {
            _logger = logger;
            utvalgController = new UtvalgController(loggerUtvalg, loggerConfig, loggerreol);
            utvalgListController = new UtvalgListController(loggerUtvalgList, loggerConfig, loggerUtvalg,  loggerreol,_utvalgRepository);
            configController = new ConfigController(loggerConfig);
            _utvalgRepository = utvalgRepository;
            _utvalgListRepository = utvalgListRepository;


            //mappingUtvalg = new MappingUtvalg(loggerMapping, loggerConfig, loggerUtvalg, loggerreoltable, loggerreol, loggerUtvalgList);
        }
        #endregion

        #region Public Methods

        // Metoden tar en utvalgssamling, og oversetter dette til et Antallsopplysningerobjekt som er det objektet Posten Ordre forventer å få tilbake.
        // Det skal aldri være (vil aldri være) mer enn et utvalg i samlingen ved kall til denne metoden.
        public List<Antallsopplysninger> ConvertUtvalgToWSAntallsopplysninger(UtvalgCollection kspuUC)
        {
            _logger.LogDebug("Inside into ConvertUtvalgToWSAntallsopplysninger");
            Utvalg kspuU;
            List<Antallsopplysninger> antallsopplysningerliste = new List<Antallsopplysninger>();
            try
            {
                kspuU = kspuUC[0];
            }
            catch (IndexOutOfRangeException exception)
            {
                _logger.LogError(exception.Message);
                return antallsopplysningerliste;
            }
            catch (ArgumentOutOfRangeException exception)
            {
                _logger.LogError(exception.Message);
                return antallsopplysningerliste;
            }
            antallsopplysningerliste.Add(ConvertUtvalgToAntallsopplysninger(kspuU));

            _logger.LogInformation("Data Returned: " + antallsopplysningerliste);
            _logger.LogDebug("Exiting From ConvertUtvalgToWSAntallsopplysninger");
            return antallsopplysningerliste;
        }

        // Metoden tar en utvalgslistesamling, og oversetter dette til et Antallsopplysningerobjekt som er det objektet Posten Ordre forventer å få tilbake.
        // Det skal aldri være (vil aldri være) mer enn en utvalgsliste i samlingen ved kall til denne metoden.
        // NEW 05.08.2007: 
        // Det skal kun returners ETT objekt i lista(trenger da inegn liste, men boholder den for å unngå ny wsdl.
        // Antallet for alle underliggende utvalg og lister akkumulleres opp.
        // Det forutsettes at man i KSPU ikke lage lister som inneholder ulike fordelingstyper(Kommune/Bydel- /Normaldistrubusjon) 
        public List<Antallsopplysninger> ConvertUtvalgToWSAntallsopplysninger(UtvalgsListCollection kspuULC)
        {
            _logger.LogDebug("Inside into ConvertUtvalgToWSAntallsopplysninger");
            UtvalgList kspuUL;
            List<Antallsopplysninger> antallsopplysningerlisteTemp = new List<Antallsopplysninger>();

            try
            {
                kspuUL = kspuULC[0];
            }
            catch (IndexOutOfRangeException exception)
            {
                _logger.LogError(exception.Message);
                return antallsopplysningerlisteTemp;
            }
            catch (ArgumentOutOfRangeException exception)
            {
                _logger.LogError(exception.Message);
                return antallsopplysningerlisteTemp;
            }

            // Samler alle utvalg i lista og barnelistene i antallsopplysningerliste        
            if (kspuUL.MemberUtvalgs.Count > 0)
            {
                foreach (Utvalg u in kspuUL.MemberUtvalgs)
                    antallsopplysningerlisteTemp.Add(ConvertUtvalgToAntallsopplysninger(u));
            }
            if (kspuUL.MemberLists.Count > 0)
            {
                foreach (UtvalgList utvl in kspuUL.MemberLists)
                {
                    if (utvl.MemberUtvalgs.Count > 0)
                    {
                        foreach (Utvalg u in utvl.MemberUtvalgs)
                            antallsopplysningerlisteTemp.Add(ConvertUtvalgToAntallsopplysninger(u));
                    }
                }
            }

            //if (antallsopplysningerlisteTemp.Count == 0)
            //    return antallsopplysningerlisteTemp;

            // Oppretter ny liste med ETT objekt og akkumulerer opp antallsinfo.
            List<Antallsopplysninger> antallsopplysningerliste = new List<Antallsopplysninger>();
            Antallsopplysninger antallsopplysninger = new Antallsopplysninger();
            antallsopplysninger.UtvalgsRef = kspuUL.Name;
            antallsopplysninger.SistOppdatert = kspuUL.SistOppdatert;
            antallsopplysninger.Vekt = kspuUL.Weight;
            antallsopplysninger.Tykkelse = kspuUL.Thickness;
            antallsopplysninger.Distribusjonsdato = kspuUL.DistributionDate.ToString("ddMMyyyy");
            antallsopplysninger.Distribusjonstype = Utils.EntityMapping(kspuUL.DistributionType);
            antallsopplysninger.InvolvertePRS = (List<string>)_utvalgListRepository.GetDistinctPRSByListID(kspuUL.ListId).Result;
            // Akkumuler antall per sone og type
            List<MottakerAntallSone> antallPrSone = new List<MottakerAntallSone>();

            if (antallsopplysningerlisteTemp.Count != 0)
            {
                antallsopplysninger.FordelingsType = GetFordelingstype(antallsopplysningerlisteTemp);
                foreach (Antallsopplysninger ao in antallsopplysningerlisteTemp)
                {
                    AkkumulerAntallsopplysninger(antallPrSone, ao.AntallPrSone);
                    antallsopplysninger.AntallDemografi += ao.AntallDemografi;
                    antallsopplysninger.AntallSegmenter += ao.AntallSegmenter;
                }
            }
            antallsopplysninger.AntallPrSone = antallPrSone;

            antallsopplysningerliste.Add(antallsopplysninger);

            _logger.LogInformation("Data Returned: " + antallsopplysningerliste);
            _logger.LogDebug("Exiting From ConvertUtvalgToWSAntallsopplysninger");
            return antallsopplysningerliste;
        }


        #endregion

        #region Private Methods
        // Metoden tar en utvalgs-samling, og legger sammen totalt antall mottakere pr. prissone, for alle utvalgene i samlingen
        private static void FillInWSAntallPrSoneFromUtvalg(Utvalg u, ref Antallsopplysninger antallsopplysninger)
        {
            int antZone0Households = 0;
            int antZone1Households = 0;
            int antZone2Households = 0;
            int antZone0Houses = 0;
            int antZone1Houses = 0;
            int antZone2Houses = 0;
            int antZone0Businesses = 0;
            int antZone1Businesses = 0;
            int antZone2Businesses = 0;
            int antZone0Farmers = 0;
            int antZone1Farmers = 0;
            int antZone2Farmers = 0;
            AntallInformation antZone0 = u.Reoler.FindAntallForPriceZone(0);
            AntallInformation antZone1 = u.Reoler.FindAntallForPriceZone(1);
            AntallInformation antZone2 = u.Reoler.FindAntallForPriceZone(2);

            foreach (UtvalgReceiver receiver in u.Receivers)
            {
                if (receiver.ReceiverId == ReceiverType.Businesses)
                {
                    antZone0Businesses += antZone0.Businesses;
                    antZone1Businesses += antZone1.Businesses;
                    antZone2Businesses += antZone2.Businesses;
                }
                if (receiver.ReceiverId == ReceiverType.Farmers)
                {
                    if (!u.Receivers.ContainsReceiver(ReceiverType.Households))
                    {
                        if (u.Receivers.ContainsReceiver(ReceiverType.FarmersReserved))
                        {
                            antZone0Farmers += (antZone0.Farmers + antZone0.FarmersReserved);
                            antZone1Farmers += (antZone1.Farmers + antZone1.FarmersReserved);
                            antZone2Farmers += (antZone2.Farmers + antZone2.FarmersReserved);
                        }
                        else
                        {
                            antZone0Farmers += antZone0.Farmers;
                            antZone1Farmers += antZone1.Farmers;
                            antZone2Farmers += antZone2.Farmers;
                        }
                    }
                }
                if (receiver.ReceiverId == ReceiverType.Households)
                {
                    if (u.Receivers.ContainsReceiver(ReceiverType.HouseholdsReserved))
                    {
                        antZone0Households += (antZone0.Households + antZone0.HouseholdsReserved);
                        antZone1Households += (antZone1.Households + antZone1.HouseholdsReserved);
                        antZone2Households += (antZone2.Households + antZone2.HouseholdsReserved);
                    }
                    else
                    {
                        antZone0Households += antZone0.Households;
                        antZone1Households += antZone1.Households;
                        antZone2Households += antZone2.Households;
                    }
                }
                if (receiver.ReceiverId == ReceiverType.Houses)
                {
                    if (!u.Receivers.ContainsReceiver(ReceiverType.Households))
                    {
                        if (u.Receivers.ContainsReceiver(ReceiverType.HousesReserved))
                        {
                            antZone0Houses += (antZone0.Houses + antZone0.HousesReserved);
                            antZone1Houses += (antZone1.Houses + antZone1.HousesReserved);
                            antZone2Houses += (antZone2.Houses + antZone2.HousesReserved);
                        }
                        else
                        {
                            antZone0Houses += antZone0.Houses;
                            antZone1Houses += antZone1.Houses;
                            antZone2Houses += antZone2.Houses;
                        }
                    }
                }
            }
            if (antZone0Households > 0)
                antallsopplysninger.AntallPrSone.Add(new MottakerAntallSone(MottakerGruppeKode.Husholdninger, "0", antZone0Households));
            if (antZone1Households > 0)
                antallsopplysninger.AntallPrSone.Add(new MottakerAntallSone(MottakerGruppeKode.Husholdninger, "1", antZone1Households));
            if (antZone2Households > 0)
                antallsopplysninger.AntallPrSone.Add(new MottakerAntallSone(MottakerGruppeKode.Husholdninger, "2", antZone2Households));
            if (antZone0Houses > 0)
                antallsopplysninger.AntallPrSone.Add(new MottakerAntallSone(MottakerGruppeKode.EneboligerOgRekkehus, "0", antZone0Houses));
            if (antZone1Houses > 0)
                antallsopplysninger.AntallPrSone.Add(new MottakerAntallSone(MottakerGruppeKode.EneboligerOgRekkehus, "1", antZone1Houses));
            if (antZone2Houses > 0)
                antallsopplysninger.AntallPrSone.Add(new MottakerAntallSone(MottakerGruppeKode.EneboligerOgRekkehus, "2", antZone2Houses));
            if (antZone0Businesses > 0)
                antallsopplysninger.AntallPrSone.Add(new MottakerAntallSone(MottakerGruppeKode.Virksomheter, "0", antZone0Businesses));
            if (antZone1Businesses > 0)
                antallsopplysninger.AntallPrSone.Add(new MottakerAntallSone(MottakerGruppeKode.Virksomheter, "1", antZone1Businesses));
            if (antZone2Businesses > 0)
                antallsopplysninger.AntallPrSone.Add(new MottakerAntallSone(MottakerGruppeKode.Virksomheter, "2", antZone2Businesses));
            if (antZone0Farmers > 0)
                antallsopplysninger.AntallPrSone.Add(new MottakerAntallSone(MottakerGruppeKode.Gardbrukere, "0", antZone0Farmers));
            if (antZone1Farmers > 0)
                antallsopplysninger.AntallPrSone.Add(new MottakerAntallSone(MottakerGruppeKode.Gardbrukere, "1", antZone1Farmers));
            if (antZone2Farmers > 0)
                antallsopplysninger.AntallPrSone.Add(new MottakerAntallSone(MottakerGruppeKode.Gardbrukere, "2", antZone2Farmers));
        }

        // Metoden oppretter er Antallsopplysningerobjekt fra et KSPU-utvalg
        private Antallsopplysninger ConvertUtvalgToAntallsopplysninger(Utvalg kspuU)
        {
            Antallsopplysninger antallsopplysninger;
            antallsopplysninger = new Antallsopplysninger(null, "", null, null, null, new List<MottakerAntallSone>(), null, null, null, Distribusjonstype.Null, new List<string>());
            antallsopplysninger.FordelingsType = MappingUtvalgsdetaljer.GetFordelingstype(kspuU);
            antallsopplysninger.UtvalgsRef = kspuU.Name;
            if (kspuU.Modifications != null)
            {
                //string sistEndretAv = "";
                DateTime sistOppdatert = new DateTime(1, 1, 1);
                foreach (UtvalgModification m in kspuU.Modifications)
                {
                    if (m.ModificationTime.CompareTo(sistOppdatert) > 0)
                        sistOppdatert = m.ModificationTime;
                }
                antallsopplysninger.SistOppdatert = sistOppdatert;
            }
            antallsopplysninger.Vekt = kspuU.Weight;
            antallsopplysninger.Tykkelse = kspuU.Thickness;
            antallsopplysninger.Distribusjonsdato = kspuU.DistributionDate.ToString("ddMMyyyy");
            antallsopplysninger.Distribusjonstype = Utils.EntityMapping(kspuU.DistributionType);
            if (kspuU.BasedOn > 0)
            {
                antallsopplysninger.InvolvertePRS = _utvalgRepository.GetDistinctPRS(kspuU.BasedOn).Result;
            }
            else
            {
                antallsopplysninger.InvolvertePRS = _utvalgRepository.GetDistinctPRS(kspuU.UtvalgId).Result;
            }
            FillInWSAntallPrSoneFromUtvalg(kspuU, ref antallsopplysninger);
            AccumulateCountForSegmentAndDemography(kspuU, antallsopplysninger);

            return antallsopplysninger;
        }

        // Adderer antallet pr mottakertype og sone
        private static void AkkumulerAntallsopplysninger(List<MottakerAntallSone> mainMAS, List<MottakerAntallSone> newMAS)
        {
            bool isNew = true;
            // for hver mottakerAntallSone i new liste
            foreach (MottakerAntallSone newMA in newMAS)
            {
                isNew = true;
                // sjekk om sone/type finnes i mainlista fra før
                foreach (MottakerAntallSone mainMA in mainMAS)
                {
                    if (newMA.Sone == mainMA.Sone & newMA.MottakerGruppe == mainMA.MottakerGruppe)
                    {
                        // adder antall
                        mainMA.Antall = mainMA.Antall + newMA.Antall;
                        isNew = false;
                    }
                }
                if (isNew)
                    mainMAS.Add(newMA);
            }
        }


        // Alle eller ingen utvalg i ei liste har kommune-/bydelsdistr(full).
        // Dersom et utvalg i ei liste har reserverte(Informasjon) er fordelingstype Informasjon
        // Eller er fordelingstype Normal
        private static FordelingsTypeKode GetFordelingstype(List<Antallsopplysninger> aol)
        {
            foreach (Antallsopplysninger ao in aol)
            {
                if (ao.FordelingsType == FordelingsTypeKode.KommuneBydel)
                    return FordelingsTypeKode.KommuneBydel;
                if (ao.FordelingsType == FordelingsTypeKode.Informasjon)
                    return FordelingsTypeKode.Informasjon;
            }
            return FordelingsTypeKode.Normal;
        }

        private static void AccumulateCountForSegmentAndDemography(Utvalg u, Antallsopplysninger antallsopplysninger)
        {
            // setter antall hvor utvalg er basert på Segmentanalyse
            if (u.Criterias.ContainsCriteriaType(CriteriaType.Segment))
                antallsopplysninger.AntallSegmenter += u.TotalAntall;

            // setter antall hvor utvalg er basert på Demografianalyse
            if (u.Criterias.ContainsCriteriaType(CriteriaType.Demography))
                antallsopplysninger.AntallDemografi += u.TotalAntall;
        }
    



    #endregion

    }
}

