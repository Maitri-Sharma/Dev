using AutoMapper;
using DataAccessAPI.HandleRequest.Request.Calendar;
using DataAccessAPI.HandleRequest.Response.Calendar;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
using Puma.Infrastructure.Interface.KsupDB;
using Puma.Infrastructure.Interface.KsupDB.Reol;
using Puma.Infrastructure.Interface.KsupDB.Utvalg;
using Puma.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DataAccessAPI.HandleRequest.Handler.Calendar
{
    public class GetRuteinfoHandler : IRequestHandler<RequestGetRuteinfo, ResponseGetRuteinfo>
    {

        /// <summary>
        /// The kapasitet repository
        /// </summary>
        private readonly IKapasitetRepository _kapasitetRepository;
        /// <summary>
        /// The kapasitet repository
        /// </summary>
        private readonly IGetPrsCalendarAdminDetailsRepository _getPrsCalendarAdminDetailsRepository;
        /// <summary>
        /// IReolRepository
        /// </summary>
        private readonly IReolRepository _reolRepository;
        /// <summary>
        /// The utvalg repository
        /// </summary>
        private readonly IUtvalgRepository _utvalgRepository;
        /// <summary>
        /// The utvalg repository
        /// </summary>
        private readonly IUtvalgListRepository _utvalgListRepository;
        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger<GetRuteinfoHandler> _logger;

        /// <summary>
        /// The mapper
        /// </summary>
        private readonly IMapper _mapper;

        public GetRuteinfoHandler(IKapasitetRepository kapasitetRepository, ILogger<GetRuteinfoHandler> logger, IMapper mapper, IGetPrsCalendarAdminDetailsRepository getPrsCalendarAdminDetailsRepository, IReolRepository reolRepository, IUtvalgRepository utvalgRepository, IUtvalgListRepository utvalgListRepository)
        {
            _kapasitetRepository = kapasitetRepository ?? throw new ArgumentNullException(nameof(kapasitetRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _getPrsCalendarAdminDetailsRepository = getPrsCalendarAdminDetailsRepository ?? throw new ArgumentNullException(nameof(getPrsCalendarAdminDetailsRepository));
            _reolRepository = reolRepository ?? throw new ArgumentNullException(nameof(reolRepository));
            _utvalgRepository = utvalgRepository ?? throw new ArgumentNullException(nameof(utvalgRepository));
            _utvalgListRepository = utvalgListRepository ?? throw new ArgumentNullException(nameof(utvalgListRepository));
        }

        /// <summary>
        /// Handle request
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<ResponseGetRuteinfo> Handle(RequestGetRuteinfo request, CancellationToken cancellationToken)
        {
            var response = new ResponseGetRuteinfo();
            try
            {
                var reoler = new ReolCollection();
                List<KapasitetRuter> ruterLackingCapacity = new List<KapasitetRuter>();
                if (request.RuteIDs.Count > 0)
                {
                    _logger.LogDebug("Calling GetReolsFromReolIDs from Repository");
                    reoler = await _reolRepository.GetReolsFromReolIDs(request.RuteIDs.ToArray());
                }
                else
                {

                    // Hent rutenr utfra utvalgsID - type isteden
                    // response.ErrorMessage = "RuteIds er ikke angitt."
                    _logger.LogWarning("GetRuteinfoResponse, " + "RuteIds er ikke angitt. Henter Ruter fra utvalg/liste informasjon");

                    // Validate input values
                    if (request.Id == 0)
                    {
                        response.ErrorMessage = "Id er ikke angitt.";
                        _logger.LogWarning("GetRuteinfoResponse, " + response.ErrorMessage.ToString());
                        return response;
                    }

                    if (!(request.Type.ToUpper().Equals("U") | request.Type.ToUpper().Equals("L")))
                    {
                        response.ErrorMessage = string.Format("Ugyldig type er angitt: {0}.", request.Type);
                        _logger.LogWarning("GetRuteinfoResponse, " + response.ErrorMessage.ToString());
                        return response;
                    }

                    if (request.Vekt < 0)
                    {
                        response.ErrorMessage = string.Format("Ugyldig vekt angitt: {0}.", request.Vekt);
                        _logger.LogWarning("GetRuteinfoResponse, " + response.ErrorMessage.ToString());
                        return response;
                    }

                    if (!(request.Distribusjonstype.ToUpper().Equals("B") | request.Distribusjonstype.ToUpper().Equals("S")))
                    {
                        response.ErrorMessage = string.Format("Ugyldig distribusjonstype er angitt: {0}.", request.Distribusjonstype);
                        _logger.LogWarning("GetRuteinfoResponse, " + response.ErrorMessage.ToString());
                        return response;
                    }

                    var dates = new List<string>();
                    dates.Add(request.ValgtDato.Date.ToString());

                    // Different action based on DistributionType
                    
                    if (request.Distribusjonstype.ToUpper().Equals("S") | request.Distribusjonstype.ToUpper().Equals("B"))
                    {
                        // get days ahead that sperrefrist corresponds to (today and next 2 virkedager)
                        var ruterLackingCapacityForPeriod = new List<RuterLackingCapacityForPeriod>();
                        var ruterLackingCapacityReolsForPeriod = new List<long>();
                        List<KapasitetDato> allDates = await _kapasitetRepository.GetKapasitetDatoer(request.ValgtDato, request.ValgtDato.AddDays(6d)); // add 6 more to get enough dates
                        var objGetPrsAdminCalendarData = new GetPrsAdminCalendarData();
                        objGetPrsAdminCalendarData = await _getPrsCalendarAdminDetailsRepository.GetPRSAdminCalendarDayDetail(request.ValgtDato);

                        // Dim datesInPeriod As List(Of KapasitetDato) = (From d As KapasitetDato In allDates Where Not d.Dato > request.ValgtDato Order By d.Dato Ascending).ToList()
                        List<KapasitetDato> datesInPeriod;
                        if (request.Distribusjonstype.ToUpper().Equals("S"))
                        {
                            datesInPeriod = (from d in allDates
                                             where !(d.Dato.Date > request.ValgtDato.Date) & (d.IsEarlyWeekFirstDay | d.IsEarlyWeekSecondDay)
                                             orderby d.Dato ascending
                                             select d).ToList();
                        }
                        else
                        {
                            datesInPeriod = (from d in allDates
                                             where !(d.Dato.Date > request.ValgtDato.Date) & (d.IsMidWeekFirstDay | d.IsMidWeekSecondDay)
                                             orderby d.Dato ascending
                                             select d).ToList();
                        }

                        // get all dates in period that is virkedag and distribusjonsdag
                        List<DateTime> deliveryDates = (from d in datesInPeriod
                                                        where d.Distribusjonsdag & d.Virkedag
                                                        orderby d.Dato ascending
                                                        select d.Dato).ToList();
                        List<KapasitetDato> sperrefristDates;
                        sperrefristDates = GetSperrefristDates(allDates, request.ValgtDato);
                        var actdeliveryDates = new List<string>();
                        if (objGetPrsAdminCalendarData.Dato != request.ValgtDato)
                        {
                            actdeliveryDates.Add(request.ValgtDato.Date.ToString());
                            actdeliveryDates.Add(objGetPrsAdminCalendarData.Dato.Date.ToString());
                            sperrefristDates.Clear();
                            var day1 = new KapasitetDato();
                            day1.Dato = request.ValgtDato;
                            day1.Distribusjonsdag = true;
                            day1.Virkedag = true;
                            var day2 = new KapasitetDato();
                            day2.Dato = objGetPrsAdminCalendarData.Dato;
                            day2.Distribusjonsdag = true;
                            day2.Virkedag = true;
                            sperrefristDates.Add(day1);
                            sperrefristDates.Add(day2);
                        }
                        else
                        {
                            foreach (KapasitetDato kapDato in sperrefristDates)
                                actdeliveryDates.Add(kapDato.Dato.Date.ToString());
                        }



                        // add next two virkedager as well, if they're delivery dates
                        // Dim virkedagCount As Integer = 1
                        // For Each d As KapasitetDato In allDates
                        // If (d.Dato > request.ValgtDato And d.Virkedag And virkedagCount < Config.CapacitySperrefristVirkedagLimit) Then
                        // virkedagCount += 1
                        // If (d.Distribusjonsdag) Then deliveryDates.Add(d.Dato)
                        // End If
                        // Next

                        object ruteIds = new List<long>();
                        ruteIds = await GetUniqueRuterAsync(request.Id, request.Type);

                        // Get a list of ruter lacking capacity for all delivery dates in the period
                        ruterLackingCapacity = await _kapasitetRepository.GetRuterLackingCapacity(actdeliveryDates, request.Id, request.Type, "HH", request.Vekt, request.Thickness);

                        // For Each ruteNoCap As KapasitetRuter In ruterLackingCapacity
                        // Dim totalDaysWithoutCapacity As List(Of KapasitetRuter) = ruterLackingCapacity.Where(Function(t) t.RuteNr = ruteNoCap.RuteNr).ToList()
                        // Dim matches As Boolean = ruterLackingCapacityReolsForPeriod.Where(Function(t) t = ruteNoCap.RuteNr).Any()

                        // If Not matches Then
                        // ruterLackingCapacityReolsForPeriod.Add(ruteNoCap.RuteNr)
                        // End If



                        // Next

                        foreach (KapasitetDato kDato in sperrefristDates)
                            ruterLackingCapacityForPeriod = (List<RuterLackingCapacityForPeriod>)GetRuterLackingCapacityForPeriod(sperrefristDates, kDato, ruterLackingCapacity, (List<long>)ruteIds, request.Vekt, request.Thickness);
                        foreach (RuterLackingCapacityForPeriod rutes in ruterLackingCapacityForPeriod)
                            ruterLackingCapacityReolsForPeriod.Add(rutes.ReolId);
                        _logger.LogDebug("Calling GetReolsFromReolIDs from Repository");
                        reoler = await _reolRepository.GetReolsFromReolIDs(ruterLackingCapacityReolsForPeriod.ToArray());
                    }
                    else // distribusjonType=B (spesifikk date)
                    {
                        // Get a list of ruter lacking capacity for this date
                        ruterLackingCapacity = await _kapasitetRepository.GetRuterLackingCapacity(dates, request.Id, request.Type, "HH", request.Vekt);
                        var rl = new List<long>();
                        foreach (KapasitetRuter rute in ruterLackingCapacity)
                            rl.Add(rute.RuteNr);
                        _logger.LogDebug("Calling GetReolsFromReolIDs from Repository");
                        reoler = await _reolRepository.GetReolsFromReolIDs(rl.ToArray());
                    }
                }

                var alleMottakere = default(long);
                if (reoler.Count > 0)
                {
                    List<RuteInfo> alleRuteInf = new List<RuteInfo>();
                    foreach (Puma.Shared.Reol r in reoler)
                    {
                        RuteInfo ruteInf = new RuteInfo();
                        ruteInf.RuteId = r.ReolId;
                        ruteInf.RuteNavn = r.Name;
                        ruteInf.Fylke = r.Fylke;
                        ruteInf.Kommune = r.Kommune;
                        ruteInf.Team = r.TeamName;
                        ruteInf.RuteAntallMotakere = r.Antall.Households;
                        ruteInf.RestAntall = ruterLackingCapacity.Where(x => x.RuteNr == r.ReolId)?.FirstOrDefault()?.RestAntall;
                        ruteInf.RestThickness = ruterLackingCapacity.Where(x => x.RuteNr == r.ReolId)?.FirstOrDefault()?.RestThickness;
                        ruteInf.RestVekt = ruterLackingCapacity.Where(x => x.RuteNr == r.ReolId)?.FirstOrDefault()?.RestVekt;
                        alleMottakere = Conversions.ToLong(Operators.AddObject(alleMottakere, ruteInf.RuteAntallMotakere));
                        alleRuteInf.Add(ruteInf);
                    }

                    response.RuteInfo = (List<RuteInfo>)alleRuteInf;
                    response.TotaltAntallBudruter = reoler.Count;
                    response.TotaltAntallMottakere = alleMottakere;
                    if (request.Type.ToUpper().Equals("L") & request.Id > 0)
                    {
                        _logger.LogDebug("Calling GetReolsInListInfoAsync from Repository");
                        response.RuterIListe = await GetReolsInListInfoAsync(request.Id, request.Type, reoler);
                    }
                }
            }
            catch (Exception ex)
            {
                response.ErrorMessage = ex.Message;
            }

            return response;
        }

        public List<KapasitetDato> GetSperrefristDates(List<KapasitetDato> allDates, DateTime dato)
        {
            var result = new List<KapasitetDato>();
            var dateListPrev = new List<DateTime>();
            var dateListNext = new List<DateTime>();
            int closestDatePast = 0;
            int closestDateFuture = 0;
            var dt1 = new DateTime();
            var sperreFrist1 = new KapasitetDato();
            foreach (KapasitetDato d in allDates)
            {
                if (d.Dato.Date < dato.Date & d.Virkedag)
                {
                    dateListPrev.Add(d.Dato);
                }
                else if (d.Dato.Date > dato.Date & d.Virkedag)
                {
                    dateListNext.Add(d.Dato);
                }
            }

            if (dateListPrev.Count > 0)
            {
                closestDatePast = dateListPrev.Min(t => dato - t).Days;
            }

            if (dateListNext.Count > 0)
            {
                closestDateFuture = dateListNext.Min(t => t - dato).Days;
            }

            if (closestDatePast == 0 & closestDateFuture == 0)
            {
                dt1 = dato;
            }
            else if (closestDatePast != 0 & closestDateFuture == 0)
            {
                dt1 = DateAndTime.DateAdd(DateInterval.Day, -closestDatePast, dato);
            }
            else if (closestDatePast == 0 & closestDateFuture != 0)
            {
                dt1 = DateAndTime.DateAdd(DateInterval.Day, closestDateFuture, dato);
            }
            else if (closestDatePast < closestDateFuture)
            {
                dt1 = DateAndTime.DateAdd(DateInterval.Day, -closestDatePast, dato);
            }
            else
            {
                dt1 = DateAndTime.DateAdd(DateInterval.Day, closestDateFuture, dato);
            }

            sperreFrist1 = allDates.Where(t => Operators.ConditionalCompareObjectEqual(t.Dato.Date, dt1, false)).Select(t => t).FirstOrDefault();
            object sperreFrist2 = allDates.Where(t => Operators.ConditionalCompareObjectEqual(t.Dato.Date, dato.Date, false)).Select(t => t).FirstOrDefault();
            if (dato != dt1)
            {
                result.Add(sperreFrist1);
                result.Add((KapasitetDato)sperreFrist2);
            }
            else
            {
                result.Add(sperreFrist1);
            }






            // For Each d As KapasitetDato In allDates


            // If d.Dato < dato And d.Virkedag Then
            // prev = d
            // End If

            // If d.Dato >= dato And d.Virkedag Then
            // Dim dateDif As Long = DateDiff(DateInterval.Day, dato, d.Dato)
            // Dim dateDifPrev As Long = 0

            // If Not prev Is Nothing Then
            // dateDifPrev = DateDiff(DateInterval.Day, prev.Dato, d.Dato)
            // End If

            // If dateDifPrev <> 0 And dateDifPrev < dateDif Then
            // result.Add(prev)
            // Else
            // result.Add(d)
            // End If


            // If result.Count >= Config.CapacitySperrefristVirkedagLimit Then Return result
            // End If
            // Next
            return result;
        }

        /// <summary>
        /// Get unique ruter for selected utvalg or liste
        /// </summary>
        /// <param name="id"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public async Task<List<long>> GetUniqueRuterAsync(int id, string type)
        {
            if (type.ToUpper().Equals("U"))
            {
                // check if basedOn
                _logger.LogDebug("Calling GetUtvalg from Repository");
                Puma.Shared.Utvalg utv = await _utvalgRepository.GetUtvalg(id);
                if (utv.BasedOn > 0)
                    id = utv.BasedOn;
                return await _utvalgRepository.GetUtvalgReolIDs(id);
            }
            else
            {
                _logger.LogDebug("Calling GetUtvalgListNoChild from Repository");
                Puma.Shared.UtvalgList l = await _utvalgListRepository.GetUtvalgListNoChild(id);
                if (l.BasedOn > 0)
                    id = l.BasedOn;
                return await _utvalgListRepository.GetUtvalgListReolIDs(id);
            }
        }

        public object GetRuterLackingCapacityForPeriod(List<KapasitetDato> allDates, KapasitetDato kDato, List<KapasitetRuter> ruterLackingCapacity, List<long> ruteIds, int weight = 0, double thickNess = 0.0d)
        {
            // get days ahead that sperrefrist corresponds to (today and next 2 virkedager)

            var sfDates = GetSperrefristDates(allDates, kDato.Dato).Where(x => x != null);
            List<DateTime> sfDeliveryDates = (from d in sfDates
                                              where d.Distribusjonsdag
                                              orderby d.Dato
                                              select d.Dato.Date).ToList();

            // Get all ruter lacking capacity for today and next 2 virkedager
            ILookup<long, KapasitetRuter> ruterLackingCapacityForSelectedDates = (from kapRute in ruterLackingCapacity
                                                                                  where sfDeliveryDates.Contains(kapRute.Dato.Date)
                                                                                  orderby kapRute.Dato ascending
                                                                                  select kapRute).ToLookup(k => k.RuteNr);
            var ruterLackingCapacityForPeriod = new List<RuterLackingCapacityForPeriod>();
            foreach (long ruteId in ruteIds)
            {

                // If ruteId = 6095010021 Thenj
                // Dim a As String = ""
                // End If
                //double totalAvailableThicknessForDuration;
                //int totalAvailableWeightForDuration;
                int weightCapTotal = 0;
                double thicknessCapTotal = 0.0d;
                int AntallCapTotal = 0;
                List<KapasitetRuter> totalDaysWithoutCapacity = ruterLackingCapacity.Where(t => Operators.ConditionalCompareObjectEqual(t.RuteNr, ruteId, false)).ToList();
                if (totalDaysWithoutCapacity is object & totalDaysWithoutCapacity.Count > 0)
                {
                    int count = 0;
                    foreach (DateTime dateInAvailableDate in sfDeliveryDates)
                    {
                        bool matches = totalDaysWithoutCapacity.Where(t => (bool)Operators.AndObject(Operators.ConditionalCompareObjectEqual(t.Dato.Date, dateInAvailableDate.Date, false), Operators.ConditionalCompareObjectEqual(t.RuteNr, ruteId, false))).Any();
                        if (matches)
                        {
                            count = count + 1;
                        }
                    }

                    //if (count == 2)
                    //{
                        foreach (DateTime sfDate in sfDeliveryDates)
                        {
                            int weightCap = 0;
                            double thicknessCap = 0.0d;
                            int AntallAvailable = 0;
                            weightCap = totalDaysWithoutCapacity.Where(t => (bool)Operators.AndObject(Operators.ConditionalCompareObjectEqual(t.Dato.Date, sfDate.Date, false), Operators.ConditionalCompareObjectEqual(t.RuteNr, ruteId, false))).Select(t => t.RestVekt).FirstOrDefault();
                            thicknessCap = totalDaysWithoutCapacity.Where(t => (bool)Operators.AndObject(Operators.ConditionalCompareObjectEqual(t.Dato.Date, sfDate.Date, false), Operators.ConditionalCompareObjectEqual(t.RuteNr, ruteId, false))).Select(t => t.RestThickness).FirstOrDefault();
                            AntallAvailable = totalDaysWithoutCapacity.Where(t => (bool)Operators.AndObject(Operators.ConditionalCompareObjectEqual(t.Dato.Date, sfDate.Date, false), Operators.ConditionalCompareObjectEqual(t.RuteNr, ruteId, false))).Select(t => t.RestAntall).FirstOrDefault();
                            weightCapTotal = weightCapTotal + weightCap;
                            thicknessCapTotal = thicknessCapTotal + thicknessCap;
                            AntallCapTotal = AntallCapTotal + AntallAvailable;
                        }

                        count = 0;
                        if (AntallCapTotal == 0 | weightCapTotal < weight | thicknessCapTotal < thickNess)
                        {
                            var ruterLacking = new RuterLackingCapacityForPeriod();
                            ruterLacking.ReolId = ruteId;
                            ruterLacking.Thickness = thicknessCapTotal;
                            ruterLacking.Vekt = weightCapTotal;
                            ruterLackingCapacityForPeriod.Add(ruterLacking);
                            // ruterLackingCapacitySperrefrist.Add(rute)
                        }
                    //}
                }


                // For Each sfDate As DateTime In sfDeliveryDates
                // Dim dato As DateTime = sfDate
                // Dim matches As Boolean = ruterLackingCapacity.Where(Function(t) t.Dato = dato And t.RuteNr = ruteId).Any()
                // Dim weightCap As Integer = weight
                // Dim thicknessCap As Double = thickNess

                // If matches Then
                // weightCap = ruterLackingCapacity.Where(Function(t) t.Dato = dato And t.RuteNr = ruteId).Select(Function(t) t.RestVekt).FirstOrDefault()
                // thicknessCap = ruterLackingCapacity.Where(Function(t) t.Dato = dato And t.RuteNr = ruteId).Select(Function(t) t.RestThickness).FirstOrDefault()
                // totalAvailableWeightForDuration = totalAvailableWeightForDuration + weightCap
                // totalAvailableThicknessForDuration = totalAvailableThicknessForDuration + thicknessCap
                // Else
                // totalAvailableWeightForDuration = weight
                // totalAvailableThicknessForDuration = thickNess
                // End If


                // Next

                // Dim datesLackingCapacityForRute As List(Of DateTime) = _
                // (From kapRute As KapasitetRuter In ruterLackingCapacityForSelectedDates(ruteId) Select kapRute.Dato).ToList()
                // 'This rute is ok unless there are no potential delivery dates left that are not lacking capacity for this rute:
                // Dim numberOfPotentialDeliveryDatesForRute As Integer = (From d As DateTime In sfDeliveryDates Where Not datesLackingCapacityForRute.Contains(d)).Count()



                // 'If numberOfPotentialDeliveryDatesForRute = 0 Then
                // '    ruterLackingCapacityForPeriod.Add(ruteId)
                // '    'ruterLackingCapacitySperrefrist.Add(rute)
                // 'End If
                // 'RDF Logic
                // If totalAvailableWeightForDuration < weight Or totalAvailableThicknessForDuration < thickNess Then
                // 'ruterLackingCapacityForPeriod.Add(ruteId)
                // 'ruterLackingCapacitySperrefrist.Add(rute)
                // End If


            }

            return ruterLackingCapacityForPeriod;
        }

        /// <summary>
        /// Get reols without capacity in selected liste
        /// </summary>
        /// <param name="id"></param>
        /// <param name="type"></param>
        /// <param name="reoler"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public async Task<List<ListInfo>> GetReolsInListInfoAsync(int id, string type, ReolCollection reoler)
        {
            List<ListInfo> alleRuteIListeInfo = new List<ListInfo>();
            _logger.LogDebug("Calling GetUtvalgListWithChildren from Repository");
            Puma.Shared.UtvalgList currUtvalgList = await _utvalgListRepository.GetUtvalgListWithChildren(id, true);
            bool includeListInfo = false;
            foreach (Puma.Shared.UtvalgList childList in currUtvalgList.MemberLists)
            {
                foreach (Puma.Shared.Utvalg utv in childList.MemberUtvalgs)
                {
                    ListInfo rInfo = new ListInfo();
                    bool includeLine = false;
                    includeListInfo = true;
                    rInfo.ListeId = childList.ListId;
                    rInfo.ListeNavn = childList.Name;
                    rInfo.UtvalgId = utv.UtvalgId;
                    rInfo.UtvalgNavn = utv.Name;
                    rInfo.RuteId = new List<long>();
                    foreach (Puma.Shared.Reol r in reoler)
                    {
                        if (reoler.Contains(r))
                        {
                            rInfo.RuteId.Add(r.ReolId);
                            includeLine = true;
                        }
                    }

                    if (includeLine)
                    {
                        alleRuteIListeInfo.Add(rInfo);
                    }
                }
            }

            foreach (Puma.Shared.Utvalg utv in currUtvalgList.MemberUtvalgs)
            {
                ListInfo rInfo = new ListInfo();
                bool includeLine = false;
                if (includeListInfo)
                {
                    rInfo.ListeId = currUtvalgList.ListId;
                    rInfo.ListeNavn = currUtvalgList.Name;
                }

                rInfo.UtvalgId = utv.UtvalgId;
                rInfo.UtvalgNavn = utv.Name;
                rInfo.RuteId = new List<long>();
                foreach (Puma.Shared.Reol r in utv.Reoler)
                {
                    if (reoler.Contains(r))
                    {
                        rInfo.RuteId.Add(r.ReolId);
                        includeLine = true;
                    }
                }

                if (includeLine)
                {
                    alleRuteIListeInfo.Add(rInfo);
                }
            }

            return (List<ListInfo>)alleRuteIListeInfo;
        }
    }
}
