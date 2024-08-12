using DataAccessAPI;
using DataAccessAPI.Controllers;
using MediatR;
using Microsoft.Extensions.Logging;
using Puma.Infrastructure.Interface.KsupDB.Utvalg;
using Puma.Shared;
using System;
using System.Collections.Generic;
using static Puma.Shared.PumaEnum;

namespace DataAccessAPI.ECPumaHelper
{
    public class FinnUtvalgService
    {

        #region Variables
        private readonly ILogger<FinnUtvalgService> _logger;
        private readonly MappingUtvalg mappingUtvalg;
        private readonly IUtvalgRepository _utvalgRepository;
        private readonly IUtvalgListRepository _utvalgListRepository;

        private readonly ConfigController configController;
        private readonly IMediator _mediator;

        #endregion

        #region Constructors              
        /// <summary>
        /// Initializes a new instance of the <see cref="FinnUtvalgService"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="loggerMapping">The logger mapping.</param>
        /// <param name="loggerConfig">The logger configuration.</param>
        /// <param name="loggerUtvalg">The logger utvalg.</param>
        /// <param name="loggerreol">The loggerreol.</param>
        /// <param name="loggerUtvalgList">The logger utvalg list.</param>
        /// <param name="utvalgRepository">The utvalg repository.</param>
        /// <param name="utvalgListRepository">The utvalg list repository.</param>
        /// <param name="mediator">The mediator.</param>
        public FinnUtvalgService(ILogger<FinnUtvalgService> logger, ILogger<MappingUtvalg> loggerMapping, ILogger<ConfigController> loggerConfig, ILogger<UtvalgController> loggerUtvalg, ILogger<ReolController> loggerreol, ILogger<UtvalgListController> loggerUtvalgList,
             IUtvalgRepository utvalgRepository, IUtvalgListRepository utvalgListRepository, IMediator mediator = null)
        {
            _logger = logger;
            _utvalgRepository = utvalgRepository;
            _utvalgListRepository = utvalgListRepository;

            _mediator = mediator;
            mappingUtvalg = new MappingUtvalg(loggerMapping, loggerConfig, loggerUtvalg, loggerreol, loggerUtvalgList, _utvalgRepository);
            configController = new ConfigController(loggerConfig);
        }
        #endregion

        #region Public Methods
        ///     ''' <summary>
        ///     ''' </summary>
        ///     ''' <param name="kundenr"></param>
        ///     ''' <param name="id"></param>
        ///     ''' <param name="type"></param>
        ///     ''' <param name="inkluderDetaljer"></param>
        ///     ''' <param name="feilKode"></param>
        ///     ''' <returns></returns>
        public List<ECPumaData> FinnUtvalgIntegrasjon(string kundenr, int id, UtvalgsTypeKode type, bool inkluderDetaljer, ref System.Nullable<FeilKode> feilKode)
        {
            _logger.LogDebug("Inside into FinnUtvalgIntegrasjon");
            List<UtvalgExtended> resultat = new List<UtvalgExtended>();
            List<ECPumaData> resultatFinal = new List<ECPumaData>();
            _logger.LogInformation("Webservice Finnutvalg: id=" + id + " / kundenummer=" + kundenr);
            DateTime startTime = DateTime.Now;

            try
            {
                if (id != default(int))
                {
                    if (type == UtvalgsTypeKode.Utvalg)
                        resultat = mappingUtvalg.ConvertUtvalgCollectionToWSUtvalg(_utvalgRepository.SearchUtvalgByUtvalgId(id, false).Result, ref feilKode, true);
                    else if (type == UtvalgsTypeKode.Liste)
                    {
                        if (inkluderDetaljer == true)
                            resultat = mappingUtvalg.ConvertUtvalgListWithChildrenCollectionToWSUtvalg(_utvalgListRepository.SearchUtvalgListWithChildrenById(id, false).Result, ref feilKode, true);
                        else if (inkluderDetaljer == false)
                            resultat = mappingUtvalg.ConvertUtvalgsListCollectionToWSUtvalg(_utvalgListRepository.SearchUtvalgListWithChildrenById(id, false).Result, ref feilKode, true);
                    }

                    // har feilkode oppstått ved søk etter kandidat
                    if (feilKode.HasValue)
                        return resultatFinal;

                    // innholder søkeresultat noe treff
                    if (resultat.Count == 0)
                    {
                        feilKode = new Nullable<FeilKode>(FeilKode.UtvalgEksistererIkke);
                        return resultatFinal;
                    }

                    // er kundenummer oppgitt i tillegg til id og type
                    if (kundenr != null)
                    {
                        // Er kundenummer i resultat lik innsendt kundenummer? Sjekker kun bare topp utvalg eller liste.
                        if (resultat[0].KundeNr.ToString() != kundenr)
                        {
                            feilKode = new Nullable<FeilKode>(FeilKode.UtvalgIkkeKnyttetTilOpgittKunde);
                            return resultatFinal;
                        }
                    }

                    // det er treff -> konverter resultatet
                    resultatFinal = ConvertUtvalgExtendedToUtvalg(resultat);

                    // 'LOG
                    try
                    {
                        _logger.LogInformation("Webservice Finnutvalg: (1)Antall treff:" + resultatFinal.Count);
                        DateTime endTime = DateTime.Now;
                        _logger.LogInformation("Tid brukt.. (1)" + endTime.Subtract(startTime).TotalSeconds + " sekunder");
                        _logger.LogDebug("Exiting From FinnUtvalgIntegrasjon");
                    }
                    catch (Exception exception)
                    {
                        _logger.LogError(exception, exception.Message);
                    }

                    return resultatFinal;
                }
                else if (kundenr != null)
                {
                    List<UtvalgExtended> tmpResultat = new List<UtvalgExtended>();
                    List<UtvalgExtended> tmpResultat2 = new List<UtvalgExtended>();
                    if (inkluderDetaljer == true)
                    {
                        tmpResultat = mappingUtvalg.ConvertUtvalgListWithChildrenCollectionToWSUtvalg(_utvalgListRepository.SearchUtvalgListWithChildrenByKundeNummer(kundenr, SearchMethod.EqualsIgnoreCase, false).Result, ref feilKode, false);
                        tmpResultat.AddRange(mappingUtvalg.ConvertUtvalgCollectionToWSUtvalg(_utvalgRepository.SearchUtvalgByKundeNr(kundenr, SearchMethod.EqualsIgnoreCase, false).Result, ref feilKode, false));
                        tmpResultat2 = mappingUtvalg.RemoveUtvalgOwnedByOtherKundenr(tmpResultat, kundenr);
                        // convert to DataAccessAPI og fjern duplicater
                        resultatFinal = mappingUtvalg.RemoveDuplicate(ConvertUtvalgExtendedToUtvalg(tmpResultat2));
                    }
                    else
                    {
                        // returner alle topplister(aggregert info) + selvstendige utvalg som ikke tilhører en liste. Selvsetndige utvalg som tilhører an liste an annen kunde eier, skal ikke returneres
                        tmpResultat = mappingUtvalg.ConvertUtvalgsListCollectionToWSUtvalg(_utvalgListRepository.SearchUtvalgListWithChildrenByKundeNummer(kundenr, SearchMethod.EqualsIgnoreCase, false).Result, ref feilKode, false);
                        tmpResultat.AddRange(mappingUtvalg.ConvertUtvalgCollectionToWSUtvalg(_utvalgRepository.SearchUtvalgByKundeNr(kundenr, SearchMethod.EqualsIgnoreCase, false).Result, ref feilKode, false));
                        tmpResultat2 = mappingUtvalg.RemoveUtvalgWithParent(tmpResultat);
                        // convert to DataAccessAPI og fjern duplicater
                        resultatFinal = mappingUtvalg.RemoveDuplicate(ConvertUtvalgExtendedToUtvalg(tmpResultat2));
                    }

                    // innholder søkeresultat noe treff                
                    DateTime currTime = DateTime.Now;
                    if (resultatFinal.Count == 0 && !feilKode.HasValue)
                        feilKode = new Nullable<FeilKode>(FeilKode.UtvalgIkkeKnyttetTilOpgittKunde);

                    // 'LOG
                    try
                    {
                        _logger.LogInformation("Webservice Finnutvalg: (2)Antall UTVALG=" + resultatFinal.Count);
                        DateTime endTime = DateTime.Now;
                        _logger.LogInformation("Tid brukt.. (2)" + endTime.Subtract(startTime).TotalSeconds + " sekunder");
                        _logger.LogDebug("Exiting From FinnUtvalgIntegrasjon");
                    }
                    catch (Exception exception)
                    {
                        _logger.LogError(exception, exception.Message);
                    }

                    return resultatFinal;
                }
                else
                    // kommer hit dersom hverken id eller kundenummer er angitt
                    throw new Exception("Hverken Id eller Kundenummer er angitt i forespørsel");

            }
            catch (Exception outerexception)
            {
                _logger.LogError(outerexception, "Webservice 'FinnUtvalg' feilet for id:" + id + "/kundenr:" + kundenr + ". Mld:" + outerexception.Message);
                throw;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        ///     ''' Konverereter entiteten UtvalgExtend til Posten.KSPU.DataAccessAPI.BusinessEntities.Utvalg
        ///     ''' </summary>
        ///     ''' <param name="utvalgExtendedList"></param>
        ///     ''' <returns></returns>
        ///     ''' <remarks></remarks>
        private static List<ECPumaData> ConvertUtvalgExtendedToUtvalg(List<UtvalgExtended> utvalgExtendedList)
        {
            List<ECPumaData> resultat = new List<ECPumaData>();
            foreach (UtvalgExtended utvalgExtended in utvalgExtendedList)
            {
                ECPumaData utvalgData;
                utvalgData = utvalgExtended;
                resultat.Add(utvalgData);
            }
            return resultat;
        }

        #endregion

    }
}
