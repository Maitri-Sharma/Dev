using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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
using static Puma.Shared.PumaEnum;

namespace DataAccessAPI.HandleRequest.Handler.Calendar
{
    public class GetRestcapacityHandler: IRequestHandler<RequestGetRestcapacity, ResponseGetRestcapacity>
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
        private readonly ILogger<GetRestcapacityHandler> _logger;
        private readonly IConfigurationRepository _configurationRepository;
        /// <summary>
        /// The mapper
        /// </summary>
        private readonly IMapper _mapper;

        public GetRestcapacityHandler(IKapasitetRepository kapasitetRepository, ILogger<GetRestcapacityHandler> logger, IMapper mapper, IGetPrsCalendarAdminDetailsRepository getPrsCalendarAdminDetailsRepository, IReolRepository reolRepository, IUtvalgRepository utvalgRepository, IUtvalgListRepository utvalgListRepository, IConfigurationRepository configurationRepository)
        {
            _kapasitetRepository = kapasitetRepository ?? throw new ArgumentNullException(nameof(kapasitetRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _getPrsCalendarAdminDetailsRepository = getPrsCalendarAdminDetailsRepository ?? throw new ArgumentNullException(nameof(getPrsCalendarAdminDetailsRepository));
            _reolRepository = reolRepository ?? throw new ArgumentNullException(nameof(reolRepository));
            _utvalgRepository = utvalgRepository ?? throw new ArgumentNullException(nameof(utvalgRepository));
            _utvalgListRepository = utvalgListRepository ?? throw new ArgumentNullException(nameof(utvalgListRepository));
            _configurationRepository = configurationRepository ?? throw new ArgumentNullException(nameof(configurationRepository));
        }

        /// <summary>
        /// Handle request
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<ResponseGetRestcapacity> Handle(RequestGetRestcapacity request, CancellationToken cancellationToken)
        {
            var response = new ResponseGetRestcapacity();
            string GetCapacityserviceCutoffLimit;
            // Validate input values
            if (request.Id == 0)
            {
                response.ErrorMessage = "Id er ikke angitt.";
                _logger.LogWarning("GetRestcapacityResponse, " + response.ErrorMessage.ToString());
                return response;
            }

            if (!(request.Type.ToUpper().Equals("U") | request.Type.ToUpper().Equals("L")))
            {
                response.ErrorMessage = string.Format("Ugyldig type er angitt: {0}.", request.Type);
                _logger.LogWarning("GetRestcapacityResponse, " + response.ErrorMessage.ToString());
                return response;
            }

            if (request.Vekt < 0)
            {
                response.ErrorMessage = string.Format("Ugyldig vekt angitt: {0}.", request.Vekt);
                _logger.LogWarning("GetRestcapacityResponse, " + response.ErrorMessage.ToString());
                return response;
            }

            if (!(request.Distribusjonstype.ToUpper().Equals("B") | request.Distribusjonstype.ToUpper().Equals("S")))
            {
                response.ErrorMessage = string.Format("Ugyldig distribusjonstype er angitt: {0}.", request.Distribusjonstype);
                _logger.LogWarning("GetRestcapacityResponse, " + response.ErrorMessage.ToString());
                return response;
            }

            // get Kapasitetsgrense
            GetCapacityserviceCutoffLimit = await _configurationRepository.GetConfigValue(Puma.DataLayer.BusinessEntity.Constants.GetCapacityserviceCutoffLimitPercent);
            if (GetCapacityserviceCutoffLimit == null | GetCapacityserviceCutoffLimit == "") 
            GetCapacityserviceCutoffLimit = "85";
            int kapasitetsgrense = Convert.ToInt32(GetCapacityserviceCutoffLimit);
            if (kapasitetsgrense < 0 | kapasitetsgrense > 100)
            {
                response.ErrorMessage = string.Format("Ugyldig kapasitetsgrense angitt i konfigurasjon: {0}.", kapasitetsgrense);
                _logger.LogWarning("GetRestcapacityResponse, " + response.ErrorMessage.ToString());
                return response;
            }

            // Different action based on DistributionType
            // If request.Distribusjonstype.ToUpper.Equals("S") Then
            // Dim cSmanager As New CapacitySperrefristManager()
            // response = cSmanager.GetCapacity(request, kapasitetsgrense)
            // Else 'distribusjonType=B
            // Dim manager As New CapacitySpecificDateManager()
            // response = manager.GetCapacity(request, kapasitetsgrense)
            // End If

            //var cSmanager = new CapacitySperrefristManager();
            response = await GetCapacity(request, kapasitetsgrense);
            return response;
        }

        public async Task<ResponseGetRestcapacity> GetCapacity(RequestGetRestcapacity request, int kapasitetsgrense)
        {
            var sw = new Stopwatch();
            sw.Start();
            List<KapasitetDato> allDates = await _kapasitetRepository.GetKapasitetDatoer(request.StartDato, request.SluttDato.AddDays(6d)); // add 6 more to get enough dates
            List<KapasitetDato> datesInPeriod;
            if (request.Distribusjonstype.ToUpper().Equals("S"))
            {
                datesInPeriod = (from d in allDates
                                 where !(d.Dato.Date > request.SluttDato.Date) & (d.IsEarlyWeekFirstDay | d.IsEarlyWeekSecondDay)
                                 orderby d.Dato ascending
                                 select d).ToList();
            }
            else
            {
                datesInPeriod = (from d in allDates
                                 where !(d.Dato.Date > request.SluttDato.Date) & (d.IsMidWeekFirstDay | d.IsMidWeekSecondDay)
                                 orderby d.Dato ascending
                                 select d).ToList();
            }




            // before RDF
            // Dim datesInPeriod As List(Of KapasitetDato) = (From d As KapasitetDato In allDates Where Not d.Dato > request.SluttDato Order By d.Dato Ascending).ToList()
            // get all dates in period that is virkedag and distribusjonsdag
            List<string > deliveryDates = (from d in datesInPeriod
                                            where d.Distribusjonsdag & d.Virkedag
                                            orderby d.Dato ascending
                                            select d.Dato.Date.ToString()).ToList();
            // add next two virkedager as well, if they're delivery dates
            int virkedagCount = 1;
            string CapacitySperrefristVirkedag = await _configurationRepository.GetConfigValue(Puma.DataLayer.BusinessEntity.Constants.CapacitySperrefristVirkedag);
            if (CapacitySperrefristVirkedag == null | CapacitySperrefristVirkedag == "")
                CapacitySperrefristVirkedag = "4";
            int CapacitySperrefristVirkedagLimit = Convert.ToInt32(CapacitySperrefristVirkedag);
            foreach (KapasitetDato d in allDates)
            {
                if (d.Dato > request.SluttDato & d.Virkedag & virkedagCount < CapacitySperrefristVirkedagLimit)
                {
                    virkedagCount += 1;
                    if (d.Distribusjonsdag)
                        deliveryDates.Add(d.Dato.Date.ToString());
                }
            }

            // In this version(mail Leif): If only Business, do no check RestCapasity.
            var receivers = await GetReceiversAsync(request.Id, request.Type);
            if (receivers.HasOnlyReceiverType(ReceiverType.Businesses))
            {
                return GetBusinessCapacityResponse(datesInPeriod);
            }

            long totalReceivers = await _kapasitetRepository.GetTotalAntall(request.Id, request.Type);
            List<long> ruteIds = await GetRuteIdsAsync(request);

            // Get a list of ruter lacking capacity for all delivery dates in the period
            List<KapasitetRuter> ruterLackingCapacity = await _kapasitetRepository.GetRuterLackingCapacity(deliveryDates, request.Id, request.Type, "HH", request.Vekt, request.Thickness);

            // check each Rute on 3-days period.
            var response = new ResponseGetRestcapacity();
            response.Kapasitet = new List<RestCapacity>();
            foreach (KapasitetDato kDato in datesInPeriod)
            {
                var rc = new RestCapacity();
                response.Kapasitet.Add(rc);
                rc.Dato = kDato.Dato;
                rc.U = kDato.UkeNr;
                rc.DD = kDato.Distribusjonsdag;
                rc.VD = kDato.Virkedag;
                rc.IsSelectable = true;
                rc.MK = new List<List<long>>();
                if (!kDato.Virkedag)
                {
                    rc.K = false;
                }
                else
                {
                    // get days ahead that sperrefrist corresponds to (today and next 2 virkedager)
                    List<RuterLackingCapacityForPeriod> ruterLackingCapacityForPeriod = new List<RuterLackingCapacityForPeriod>();
                    var ruterLackingCapacityRuteId = new List<long>();
                    // ruterLackingCapacityForPeriod = GetRuterLackingCapacityForPeriod(allDates, kDato, ruterLackingCapacity, ruteIds)
                    ruterLackingCapacityForPeriod = (List<RuterLackingCapacityForPeriod>)GetRuterLackingCapacityForPeriod(datesInPeriod, kDato, ruterLackingCapacity, ruteIds, request.Vekt, request.Thickness);
                    foreach (RuterLackingCapacityForPeriod reolIdWithoutCap in ruterLackingCapacityForPeriod)
                        ruterLackingCapacityRuteId.Add(reolIdWithoutCap.ReolId);


                    // add to list, except if below kapasitetsgrense
                    if (ruterLackingCapacityForPeriod.Count == 0)
                    {
                        rc.K = true;
                    }
                    else
                    {
                        _logger.LogDebug("Calling GetReolsFromReolIDs from Repository");
                        ReolCollection reoler = await _reolRepository.GetReolsFromReolIDs(ruterLackingCapacityRuteId.ToArray());
                        long householdsWithCapacity = totalReceivers - reoler.FindAntall().Households;
                        if (householdsWithCapacity >= totalReceivers * kapasitetsgrense / 100d)
                        {
                            // There is partial capacity for this date
                            rc.K = true;
                            foreach (RuterLackingCapacityForPeriod reolId in ruterLackingCapacityForPeriod)

                                // Dim reolData As KapasitetRuter = ruterLackingCapacity.Where(Function(t) t.RuteNr = reolId.ReolId).Select(Function(t) t).FirstOrDefault()
                                rc.MK.Add(new List<long>(new long[] { reolId.ReolId, reolId.Vekt, 0L, (long)Math.Round(reolId.Thickness) }));
                        }
                        else
                        {
                            // There is not enough capacity for this date
                            rc.K = false;
                        }
                    }
                }
            }

            sw.Stop();
            Debug.WriteLine("Getting sperrefrist capacity information: " + sw.ElapsedMilliseconds);
            return response;
        }
        /// <summary>
        /// Get Receivers for selected utvalg or liste
        /// </summary>
        /// <param name="id"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public async Task<UtvalgReceiverList> GetReceiversAsync(int id, string type)
        {
            if (type.ToUpper().Equals("U"))
            {
                var utvalg = new Puma.Shared.Utvalg();
                utvalg.UtvalgId = id;
                await _utvalgRepository.GetUtvalgReceiver(utvalg);
                return utvalg.Receivers;
            }
            else
            {
                return await _utvalgListRepository.GetUtvalgListReceivers(id);
            }
        }

        private ResponseGetRestcapacity GetBusinessCapacityResponse(List<KapasitetDato> dates)
        {
            var response = new ResponseGetRestcapacity();
            response.Kapasitet = new List<RestCapacity>();
            foreach (KapasitetDato kDato in dates)
            {
                var rc = new RestCapacity();
                rc.Dato = kDato.Dato;
                rc.U = kDato.UkeNr;
                rc.DD = kDato.Distribusjonsdag;
                rc.VD = kDato.Virkedag;
                rc.MK = new List<List<long>>();
                // rc.K = (rc.DD And rc.VD)
                rc.K = rc.VD; // For sperrefrist, it makes sense to mark all virkedager as available. Check with Posten?
                response.Kapasitet.Add(rc);
            }

            return response;
        }

        private async Task<List<long>> GetRuteIdsAsync(RequestGetRestcapacity request)
        {
            List<long> ruteIds = new List<long>();
            ruteIds = await GetUniqueRuterAsync(request.Id, request.Type);

            // If (request.Type.ToUpper().Equals("U")) Then
            // ruteIds = DAUtvalg.GetUtvalgReolIDs(request.Id)
            // Else
            // ruteIds = DAUtvalgList.GetUtvalgListReolIDs(request.Id)
            // End If
            return ruteIds;
        }

        public List<RuterLackingCapacityForPeriod> GetRuterLackingCapacityForPeriod(List<KapasitetDato> allDates, KapasitetDato kDato, List<KapasitetRuter> ruterLackingCapacity, List<long> ruteIds, int weight = 0, double thickNess = 0.0d)
        {
            // get days ahead that sperrefrist corresponds to (today and next 2 virkedager)

            var sfDates = GetSperrefristDates(allDates, kDato.Dato);
            List<DateTime> sfDeliveryDates = (from d in sfDates
                                              where d.Distribusjonsdag
                                              orderby d.Dato
                                              select d.Dato).ToList();

            // Get all ruter lacking capacity for today and next 2 virkedager
            ILookup<long, KapasitetRuter> ruterLackingCapacityForSelectedDates = (from kapRute in ruterLackingCapacity
                                                                                  where sfDeliveryDates.Contains(kapRute.Dato)
                                                                                  orderby kapRute.Dato ascending
                                                                                  select kapRute).ToLookup(k => k.RuteNr);
            List<RuterLackingCapacityForPeriod> ruterLackingCapacityForPeriod = new List<RuterLackingCapacityForPeriod>();
            foreach (long ruteId in ruteIds)
            {

                // If ruteId = 6095010021 Then
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
                        bool matches = totalDaysWithoutCapacity.Where(t => (bool)Operators.AndObject(Operators.ConditionalCompareObjectEqual(t.Dato, dateInAvailableDate, false), Operators.ConditionalCompareObjectEqual(t.RuteNr, ruteId, false))).Any();
                        if (matches)
                        {
                            count = count + 1;
                        }
                    }

                    if (count == 2)
                    {
                        foreach (DateTime sfDate in sfDeliveryDates)
                        {
                            int weightCap = 0;
                            double thicknessCap = 0.0d;
                            int AntallAvailable = 0;
                            weightCap = totalDaysWithoutCapacity.Where(t => (bool)Operators.AndObject(Operators.ConditionalCompareObjectEqual(t.Dato, sfDate, false), Operators.ConditionalCompareObjectEqual(t.RuteNr, ruteId, false))).Select(t => t.RestVekt).FirstOrDefault();
                            thicknessCap = totalDaysWithoutCapacity.Where(t => (bool)Operators.AndObject(Operators.ConditionalCompareObjectEqual(t.Dato, sfDate, false), Operators.ConditionalCompareObjectEqual(t.RuteNr, ruteId, false))).Select(t => t.RestThickness).FirstOrDefault();
                            AntallAvailable = totalDaysWithoutCapacity.Where(t => (bool)Operators.AndObject(Operators.ConditionalCompareObjectEqual(t.Dato, sfDate, false), Operators.ConditionalCompareObjectEqual(t.RuteNr, ruteId, false))).Select(t => t.RestAntall).FirstOrDefault();
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
                    }
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
                if (d.Dato < dato & d.Virkedag)
                {
                    dateListPrev.Add(d.Dato);
                }
                else if (d.Dato > dato & d.Virkedag)
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

            sperreFrist1 = allDates.Where(t => Operators.ConditionalCompareObjectEqual(t.Dato, dt1, false)).Select(t => t).FirstOrDefault();
            object sperreFrist2 = allDates.Where(t => Operators.ConditionalCompareObjectEqual(t.Dato, dato, false)).Select(t => t).FirstOrDefault();
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
                // Puma.Shared.Utvalg utvalg = new Utvalg();
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

    }
}
