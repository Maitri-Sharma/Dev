using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DataAccessAPI.HandleRequest.Request.Report;
using Hangfire.Console;
using Hangfire.Server;
using MediatR;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Puma.Infrastructure.Interface.KsupDB.Utvalg;
using Puma.DataLayer.BusinessEntity.Report;
using static System.Net.Mime.MediaTypeNames;
using static Puma.Shared.PumaEnum;
using System.Net.Http.Headers;
using Puma.Infrastructure.Interface.KsupDB;

namespace DataAccessAPI.HandleRequest.Handler.Report
{
    /// <summary>
    /// ReportJobRequestHandler
    /// </summary>
    public class ReportJobRequestHandler : IRequestHandler<ReportJobRequest, MediatR.Unit>
    {
        /// <summary>
        /// The utvalg list repository
        /// </summary>
        private readonly IUtvalgListRepository _utvalgListRepository;
        /// <summary>
        /// The configuration
        /// </summary>
        private readonly IConfiguration config;
        /// <summary>
        /// The HTTP client factory
        /// </summary>
        private readonly IHttpClientFactory httpClientFactory;
        /// <summary>
        /// The context
        /// </summary>
        private PerformContext _context;
        /// <summary>
        /// The request job
        /// </summary>
        private ReportJobRequest requestJob;
        /// <summary>
        /// The mediator
        /// </summary>
        private IMediator _mediator;
        /// <summary>
        /// Map operation Repository
        /// </summary>
        private readonly IMapOperationRepository _mapOperationRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReportJobRequestHandler"/> class.
        /// </summary>
        /// <param name="utvalgListRepository">The utvalg list repository.</param>
        /// <param name="config">The configuration.</param>
        /// <param name="httpClientFactory">The HTTP client factory.</param>
        /// <param name="mediator">The mediator.</param>
        /// <param name="mapOperationRepository">The mapOperationRepository.</param>
        /// <exception cref="System.ArgumentNullException">mediator</exception>
        public ReportJobRequestHandler(IUtvalgListRepository utvalgListRepository, IConfiguration config, IHttpClientFactory httpClientFactory,
            IMediator mediator, IMapOperationRepository mapOperationRepository)
        {
            _utvalgListRepository = utvalgListRepository;
            this.config = config;
            this.httpClientFactory = httpClientFactory;
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _mapOperationRepository = mapOperationRepository ?? throw new ArgumentNullException(nameof(mapOperationRepository));

        }
        /// <summary>
        /// Handles a request
        /// </summary>
        /// <param name="request">The request</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>
        /// Response from the request
        /// </returns>
        /// <exception cref="System.Net.Http.HttpRequestException">JsReport Calling Failed with error Code " + response.StatusCode + " " + response.Content.ReadAsStringAsync().Result</exception>
        public async Task<Unit> Handle(ReportJobRequest request, CancellationToken cancellationToken)
        {
            requestJob = request;
            int zone0 = 0;
            int zone1 = 0;
            int zone2 = 0;
            int dag1 = 0;
            int dag2 = 0;
            int total = 0;
            bool showBusiness = false;
            bool showHouseholds = false;
            bool showHouseholdReserved = false;

            var utvalgListData = await _utvalgListRepository.GetUtvalgListWithAllReferences(request.listId);

            _context = request.context;

            List<ItemUtvalg> utvalgsList = new List<ItemUtvalg>();
            List<ItemList> listCollection = new List<ItemList>();

            _context.WriteLine("Accessing Member Utvalgs in List - started");
            int index = 0;
            //As per Bug #73124 name should be same as in puma web. so removed order by from member utvalg and utvalglist
            foreach (var list in utvalgListData.MemberLists.ToList())
            {
                ItemList itemlist = new ItemList();
                itemlist.name = list.Name;
                itemlist.logo = list.Logo;
                itemlist.items = new List<ItemUtvalg>();
                
                foreach (var utvalg in list.MemberUtvalgs)
                {
                    if (utvalg.Receivers != null && utvalg.Receivers.Any())
                    {
                        showBusiness = utvalg.Receivers.ContainsReceiver(ReceiverType.Businesses);
                        showHouseholds = utvalg.Receivers.ContainsReceiver(ReceiverType.Households);
                        showHouseholdReserved = utvalg.Receivers.ContainsReceiver(ReceiverType.HouseholdsReserved);
                    }

                    itemlist.items.Add(GetItemUtvalg(utvalg, showBusiness, showHouseholds, showHouseholdReserved, request.isCustomerWeb, request.strDayDetails, request.level, request.uptoLevel, index, request.reportType));
                    index++;
                }

                itemlist.items.ForEach(x =>
                {
                    x.totalH0 = x.Reoler.Any() ? x.Reoler.Sum(x => x.H0) : 0;
                    x.totalH1 = x.Reoler.Any() ? x.Reoler.Sum(x => x.H1) : 0;
                    x.totalH2 = x.Reoler.Any() ? x.Reoler.Sum(x => x.H2) : 0;
                    x.totalV0 = x.Reoler.Any() ? x.Reoler.Sum(x => x.V0) : 0;
                    x.totalV1 = x.Reoler.Any() ? x.Reoler.Sum(x => x.V1) : 0;
                    x.totalV2 = x.Reoler.Any() ? x.Reoler.Sum(x => x.V2) : 0;
                    x.totalHD1 = x.Reoler.Any() ? x.Reoler.Sum(x => x.HHD1) : 0;
                    x.totalHD2 = x.Reoler.Any() ? x.Reoler.Sum(x => x.HHD2) : 0;
                    x.totalVD1 = x.Reoler.Any() ? x.Reoler.Sum(x => x.VHD1) : 0;
                    x.totalVD2 = x.Reoler.Any() ? x.Reoler.Sum(x => x.VHD2) : 0;
                });

                itemlist.zone0 = itemlist.items.Sum(x => x.zone0);
                zone0 = zone0 + itemlist.zone0;
                itemlist.zone1 = itemlist.items.Sum(x => x.zone1);
                zone1 = zone1 + itemlist.zone1;
                itemlist.zone2 = itemlist.items.Sum(x => x.zone2);
                zone2 = zone2 + itemlist.zone2;
                itemlist.dag1 = itemlist.items.Sum(x => x.dag1);
                dag1 = dag1 + itemlist.dag1;
                itemlist.dag2 = itemlist.items.Sum(x => x.dag2);
                dag2 = dag2 + itemlist.dag2;
                itemlist.total = itemlist.items.Sum(x => x.total);
                total = total + itemlist.total;
                listCollection.Add(itemlist);
            }

            if (utvalgListData.MemberUtvalgs.Count > 0)
            {
                ItemList itemlist = new ItemList();
                itemlist.name = utvalgListData.Name;
                itemlist.logo = utvalgListData.Logo;
                itemlist.items = new List<ItemUtvalg>();
                //As per Bug #73124 name should be same as in puma web. so removed order by from member utvalg and utvalglist
                foreach (var utvalg in utvalgListData.MemberUtvalgs.ToList())
                {
                    if (utvalg.Receivers != null && utvalg.Receivers.Any())
                    {
                        showBusiness = utvalg.Receivers.ContainsReceiver(ReceiverType.Businesses);
                        showHouseholds = utvalg.Receivers.ContainsReceiver(ReceiverType.Households);
                        showHouseholdReserved = utvalg.Receivers.ContainsReceiver(ReceiverType.HouseholdsReserved);
                    }

                    itemlist.items.Add(GetItemUtvalg(utvalg, showBusiness, showHouseholds, showHouseholdReserved, request.isCustomerWeb, request.strDayDetails, request.level, request.uptoLevel, index, request.reportType));


                    index++;
                }

                itemlist.items.ForEach(x =>
                {
                    x.totalH0 = x.Reoler.Any() ? x.Reoler.Sum(x => x.H0) : 0;
                    x.totalH1 = x.Reoler.Any() ? x.Reoler.Sum(x => x.H1) : 0;
                    x.totalH2 = x.Reoler.Any() ? x.Reoler.Sum(x => x.H2) : 0;
                    x.totalV0 = x.Reoler.Any() ? x.Reoler.Sum(x => x.V0) : 0;
                    x.totalV1 = x.Reoler.Any() ? x.Reoler.Sum(x => x.V1) : 0;
                    x.totalV2 = x.Reoler.Any() ? x.Reoler.Sum(x => x.V2) : 0;
                    x.totalHD1 = x.Reoler.Any() ? x.Reoler.Sum(x => x.HHD1) : 0;
                    x.totalHD2 = x.Reoler.Any() ? x.Reoler.Sum(x => x.HHD2) : 0;
                    x.totalVD1 = x.Reoler.Any() ? x.Reoler.Sum(x => x.VHD1) : 0;
                    x.totalVD2 = x.Reoler.Any() ? x.Reoler.Sum(x => x.VHD2) : 0;
                });

                itemlist.zone0 = itemlist.items.Sum(x => x.zone0);
                zone0 = zone0 + itemlist.zone0;
                itemlist.zone1 = itemlist.items.Sum(x => x.zone1);
                zone1 = zone1 + itemlist.zone1;
                itemlist.zone2 = itemlist.items.Sum(x => x.zone2);
                zone2 = zone2 + itemlist.zone2;
                itemlist.dag1 = itemlist.items.Sum(x => x.dag1);
                dag1 = dag1 + itemlist.dag1;
                itemlist.dag2 = itemlist.items.Sum(x => x.dag2);
                dag2 = dag2 + itemlist.dag2;
                itemlist.total = itemlist.items.Sum(x => x.total);
                total = total + itemlist.total;
                listCollection.Add(itemlist);
            }

            _context.WriteLine("Accessing Member Utvalgs in List - Completed");

            //Prepare the Object for Report Service 
            ReportPayload reportPayload = new ReportPayload()
            {
                reportName = utvalgListData.Name,
                emailTo = request.emailTo,
                islist = true,
                isEmail = true,
                zone0 = zone0,
                zone1 = zone1,
                zone2 = zone2,
                dag1 = dag1,
                dag2 = dag2,
                total = total,
                smtp = config.GetValue<string>("SMTPServer"),
                distrDate = (request.DistrDate == null ? "" : request.DistrDate),
                isMapwithData = (request.reportType == "excelDataOnly" ? false : true), //to be implemented
                listArray = listCollection,
                level = request.level,
                uptoLevel = request.uptoLevel,
                logo = utvalgListData.Logo,
                listId = utvalgListData.ListId.ToString()

            };

            JsReport js = new JsReport();
            js.template = new Template() { name = (request.reportType == "pdf" ? "UtvalgPdf" : "UtvalgToExcel") };
            js.data = reportPayload;

            string Data = JsonConvert.SerializeObject(reportPayload);

            var stringContent = new StringContent(JsonConvert.SerializeObject(js), Encoding.UTF8, Application.Json);
            _context.WriteLine("Calling JsReport APi");

            var client = httpClientFactory.CreateClient();//"https://dev.pumainternweb.bring.no/reports/api/report"
            client.Timeout = TimeSpan.FromMinutes(30);
            var response = await client.PostAsync(config.GetValue<string>("Report_API_URL"), stringContent);

            _context.WriteLine("Finished Calling JsReport APi");

            //Call Js Report using http client
            if (response.IsSuccessStatusCode)
            {
                var returnObj = response.Content.ReadAsStringAsync().Result;
            }
            else
            {
                throw new HttpRequestException("JsReport Calling Failed with error Code " + response.StatusCode + " " + response.Content.ReadAsStringAsync().Result);
            }
            return MediatR.Unit.Value;
        }

        /// <summary>
        /// Gets the item utvalg.
        /// </summary>
        /// <param name="utvalg">The utvalg.</param>
        /// <param name="showBusiness">if set to <c>true</c> [show business].</param>
        /// <param name="showHouseholds">if set to <c>true</c> [show households].</param>
        /// <param name="showHouseholdReserved">if set to <c>true</c> [show household reserved].</param>
        /// <param name="isCustomerWeb">if set to <c>true</c> [is customer web].</param>
        /// <param name="strDayDetails">The string day details.</param>
        /// <param name="level">The level.</param>
        /// <param name="uptoLevel">The upto level.</param>
        /// <param name="index">Index.</param>
        /// <param name="reportType">Type of report.</param>
        /// <returns></returns>
        public ItemUtvalg GetItemUtvalg(Puma.Shared.Utvalg utvalg, bool showBusiness, bool showHouseholds, bool showHouseholdReserved, bool isCustomerWeb, string strDayDetails, int level, int uptoLevel, int index, string reportType)
        {
            int totalCount = 0;
            int zone0Count = 0;
            int zone1Count = 0;
            int zone2Count = 0;
            int dag2Count = 0;
            int dag1Count = 0;
            int houseHolds = 0;
            int business = 0;

            List<long> ReolIds = new List<long>();

            utvalg.Reoler.ForEach(x => ReolIds.Add(x.ReolId));

            List<BasicDetail> utvalgReoler = formatReportData(utvalg, showBusiness, showHouseholds, showHouseholdReserved, strDayDetails, level, uptoLevel, reportType);

            int cnt = 0;
            foreach (var item in utvalgReoler)
            {
                cnt++;
                if (item.children?.Count > 0)
                {
                    foreach (var childitem in item.children)
                    {
                        cnt++;
                        if (childitem.children?.Count > 0)
                        {
                            foreach (var subchildItem in childitem.children)
                            {
                                cnt++;
                                if (subchildItem.children?.Count > 0)
                                {
                                    foreach (var lastItem in subchildItem.children)
                                    {
                                        cnt++;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            //Make and fetch image

            _context.WriteLine("Accessing ExportImage");

            string bfferImage = string.Empty;
            if (!string.IsNullOrWhiteSpace(requestJob.DistrDate))
            {
                bfferImage = _mapOperationRepository.ExportMapImage(ReolIds, isCustomerWeb, strDayDetails, requestJob.selectedAddress).Result;
            }
            else
            {
                bfferImage = ExportMapImage(ReolIds, isCustomerWeb, strDayDetails, requestJob.selectedAddress).Result;
            }

            _context.WriteLine("Accessing ExportImage Finished");

            utvalgReoler.ForEach((item) =>
            {
                totalCount += item.total;
                zone0Count += item.zone0;
                zone1Count += item.zone1;
                zone2Count += item.zone2;
                dag1Count += item.HHD1 + item.VHD1;
                dag2Count += item.HHD2 + item.VHD2;
                houseHolds += item.house + item.householdsReserved;
                business += item.businesses;
            });

            return new ItemUtvalg()
            {
                Index = index,
                id = 10,
                itemsCount = cnt,
                showHousehold = showHouseholds,
                showBusiness = showBusiness,
                showReserverd = showHouseholdReserved,
                name = utvalg.Name,
                HouseHolds = houseHolds,
                Business = business,
                Reoler = utvalgReoler,
                imgUrl = "",
                forhandlerpartyk = utvalg.Logo,
                dag1 = dag1Count,
                dag2 = dag2Count,
                zone0 = zone0Count,
                zone1 = zone1Count,
                zone2 = zone2Count,
                total = totalCount,
                imagedata = bfferImage
            };
        }

        /// <summary>
        /// Distributions the details.
        /// </summary>
        /// <param name="dayDetails">The day details.</param>
        /// <param name="showHouseholds">if set to <c>true</c> [show households].</param>
        /// <param name="showBusiness">if set to <c>true</c> [show business].</param>
        /// <param name="data">The data.</param>
        /// <param name="showHouseholdsReserved">The showHouseholdsReserved.</param>
        /// <returns></returns>
        public List<int> distributionDetails(string dayDetails, bool showHouseholds, bool showBusiness, Puma.Shared.Reol data, bool showHouseholdsReserved)
        {
            int VHD1 = 0;
            int VHD2 = 0;
            int HHD1 = 0;
            int HHD2 = 0;

            switch (dayDetails)
            {
                case "A-uke, tidliguke":
                    if (data.Frequency == "A")
                    {
                        if (showBusiness)
                        {
                            VHD1 = 0;
                            VHD2 = 0;
                        }
                        if (showHouseholds)
                        {
                            HHD1 = data.Antall.PriorityHouseholdsReserved + data.Antall.NonPriorityHouseholdsReserved;
                            HHD2 = 0;
                        }
                    }
                    else if (data.Frequency == "B")
                    {
                        if (showBusiness)
                        {
                            VHD1 = 0;
                            VHD2 = 0;
                        }
                        if (showHouseholds)
                        {
                            HHD1 = data.Antall.PriorityHouseholdsReserved;
                            HHD2 = data.Antall.NonPriorityHouseholdsReserved;
                        }
                    }
                    else if (data.Frequency == "0")
                    {
                        if (showBusiness)
                        {
                            VHD1 = 0;
                            VHD2 = 0;
                        }
                        if (showHouseholds)
                        {
                            HHD1 = data.Antall.PriorityHouseholdsReserved + data.Antall.NonPriorityHouseholdsReserved;
                            HHD2 = 0;
                        }
                    };
                    break;
                case "A-uke, midtuke":
                    if (data.Frequency == "A")
                    {
                        if (showBusiness)
                        {
                            VHD1 = data.Antall.PriorityBusinessReserved + data.Antall.NonPriorityBusinessReserved;
                            VHD2 = 0;
                        }
                        if (showHouseholds)
                        {
                            HHD1 = data.Antall.PriorityHouseholdsReserved + data.Antall.NonPriorityHouseholdsReserved;
                            HHD2 = 0;
                        }
                    }
                    else if (data.Frequency == "B")
                    {
                        if (showBusiness)
                        {
                            VHD1 = 0;
                            VHD2 = data.Antall.PriorityBusinessReserved + data.Antall.NonPriorityBusinessReserved;
                        }
                        if (showHouseholds)
                        {
                            HHD1 = 0;
                            HHD2 = data.Antall.PriorityHouseholdsReserved + data.Antall.NonPriorityHouseholdsReserved;
                        }
                    }
                    else if (data.Frequency == "0")
                    {
                        if (showBusiness)
                        {
                            VHD1 = data.Antall.PriorityBusinessReserved + data.Antall.NonPriorityBusinessReserved;
                            VHD2 = 0;
                        }
                        if (showHouseholds)
                        {
                            HHD1 = data.Antall.PriorityHouseholdsReserved + data.Antall.NonPriorityHouseholdsReserved;
                            HHD2 = 0;
                        }
                    };
                    break;
                case "B-uke, tidliguke":
                    if (data.Frequency == "A")
                    {
                        if (showBusiness)
                        {
                            VHD1 = 0;
                            VHD2 = 0;
                        }
                        if (showHouseholds)
                        {
                            HHD1 = data.Antall.PriorityHouseholdsReserved;
                            HHD2 = data.Antall.NonPriorityHouseholdsReserved;
                        }
                    }
                    else if (data.Frequency == "B")
                    {
                        if (showBusiness)
                        {
                            VHD1 = 0;
                            VHD2 = 0;
                        }
                        if (showHouseholds)
                        {
                            HHD1 = data.Antall.PriorityHouseholdsReserved + data.Antall.NonPriorityHouseholdsReserved;
                            HHD2 = 0;
                        }
                    }
                    else if (data.Frequency == "0")
                    {
                        if (showBusiness)
                        {
                            VHD1 = 0;
                            VHD2 = 0;
                        }
                        if (showHouseholds)
                        {
                            HHD1 = data.Antall.PriorityHouseholdsReserved + data.Antall.NonPriorityHouseholdsReserved;
                            HHD2 = 0;
                        }
                    };
                    break;
                case "B-uke, midtuke":
                    if (data.Frequency == "A")
                    {
                        if (showBusiness)
                        {
                            VHD1 = 0;
                            VHD2 = data.Antall.PriorityBusinessReserved + data.Antall.NonPriorityBusinessReserved;
                        }
                        if (showHouseholds)
                        {
                            HHD1 = 0;
                            HHD2 = data.Antall.PriorityHouseholdsReserved + data.Antall.NonPriorityHouseholdsReserved;
                        }
                    }
                    else if (data.Frequency == "B")
                    {
                        if (showBusiness)
                        {
                            VHD1 = data.Antall.PriorityBusinessReserved + data.Antall.NonPriorityBusinessReserved;
                            VHD2 = 0;
                        }
                        if (showHouseholds)
                        {
                            HHD1 = data.Antall.PriorityHouseholdsReserved + data.Antall.NonPriorityHouseholdsReserved;
                            HHD2 = 0;
                        }
                    }
                    else if (data.Frequency == "0")
                    {
                        if (showBusiness)
                        {
                            VHD1 = data.Antall.PriorityBusinessReserved + data.Antall.NonPriorityBusinessReserved;
                            VHD2 = 0;
                        }
                        if (showHouseholds)
                        {
                            HHD1 = data.Antall.PriorityHouseholdsReserved + data.Antall.NonPriorityHouseholdsReserved;
                            HHD2 = 0;
                        }
                    };
                    break;
            }

            return new List<int>() { VHD1, VHD2, HHD1, HHD2 };

        }

        /// <summary>
        /// Gets the house holds count.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="reoler">The reoler.</param>
        /// <param name="item">The item.</param>
        /// <param name="strDayDetails">The string day details.</param>
        /// <param name="showHouseholds">if set to <c>true</c> [show households].</param>
        /// <param name="showBusiness">if set to <c>true</c> [show business].</param>
        /// <param name="showHouseholdsReserved">if set to <c>true</c> [show households reserved].</param>
        /// <returns></returns>
        private T GetHouseHoldsCount<T>(Puma.Shared.Reol reoler, T item, string strDayDetails, bool showHouseholds, bool showBusiness, bool showHouseholdsReserved) where T : BasicDetail, new()
        {
            List<int> DistrDetails = distributionDetails(strDayDetails, showHouseholds, showBusiness, reoler,showHouseholdsReserved);

            item.house = showHouseholds ? reoler.Antall.Households : 0;
            item.householdsReserved = showHouseholdsReserved ? reoler.Antall.HouseholdsReserved : 0;
            item.businesses = showBusiness ? reoler.Antall.Businesses : 0;
            item.HHD1 = DistrDetails[2];
            item.HHD2 = DistrDetails[3];
            item.VHD1 = DistrDetails[0];
            item.VHD2 = DistrDetails[1];
            item.zone0 = (reoler.PrisSone == 0 ? (showHouseholds ? reoler.Antall.Households : 0) + (showBusiness ? reoler.Antall.Businesses : 0) + (showHouseholdsReserved ? reoler.Antall.HouseholdsReserved : 0) : 0);
            item.zone1 = (reoler.PrisSone == 1 ? (showHouseholds ? reoler.Antall.Households : 0) + (showBusiness ? reoler.Antall.Businesses : 0) + (showHouseholdsReserved ? reoler.Antall.HouseholdsReserved : 0) : 0);
            item.zone2 = (reoler.PrisSone == 2 ? (showHouseholds ? reoler.Antall.Households : 0) + (showBusiness ? reoler.Antall.Businesses : 0) + (showHouseholdsReserved ? reoler.Antall.HouseholdsReserved : 0) : 0);
            item.H0 = (reoler.PrisSone == 0 ? (showHouseholds ? reoler.Antall.Households : 0) + (showHouseholdsReserved ? reoler.Antall.HouseholdsReserved : 0) : 0);
            item.H1 = (reoler.PrisSone == 1 ? (showHouseholds ? reoler.Antall.Households : 0) + (showHouseholdsReserved ? reoler.Antall.HouseholdsReserved : 0) : 0);
            item.H2 = (reoler.PrisSone == 2 ? (showHouseholds ? reoler.Antall.Households : 0) + (showHouseholdsReserved ? reoler.Antall.HouseholdsReserved : 0) : 0);
            item.V0 = (reoler.PrisSone == 0 ? (showBusiness ? reoler.Antall.Businesses : 0) : 0);
            item.V1 = (reoler.PrisSone == 1 ? (showBusiness ? reoler.Antall.Businesses : 0) : 0);
            item.V2 = (reoler.PrisSone == 2 ? (showBusiness ? reoler.Antall.Businesses : 0) : 0);
            item.total = ((showHouseholds ? reoler.Antall.Households : 0) + (showBusiness ? reoler.Antall.Businesses : 0) + (showHouseholdsReserved ? reoler.Antall.HouseholdsReserved : 0));

            return item;
        }

        /// <summary>
        /// Fills the recursive.
        /// </summary>
        /// <param name="flatObjects">The flat objects.</param>
        /// <param name="parentId">The parent identifier.</param>
        /// <returns></returns>
        private static List<BasicDetail> FillRecursive(List<BasicDetail> flatObjects, int? parentId = null)
        {
            List<BasicDetail> basicDetails = new List<BasicDetail>();
            for (int i = 0; i < flatObjects.Count; i++)
            {
                if (flatObjects[i] != null)
                {
                    if (flatObjects[i].pkey == null)
                    {
                        int index = basicDetails.FindIndex(x => x.key == flatObjects[i].key);

                        if (index < 0)
                        {
                            basicDetails.Add(flatObjects[i]);
                        }
                        else
                        {
                            basicDetails[index].house += flatObjects[i].house;
                            basicDetails[index].businesses += flatObjects[i].businesses;
                            basicDetails[index].householdsReserved += flatObjects[i].householdsReserved;
                            basicDetails[index].HHD1 += flatObjects[i].HHD1;
                            basicDetails[index].HHD2 += flatObjects[i].HHD2;
                            basicDetails[index].VHD1 += flatObjects[i].VHD1;
                            basicDetails[index].VHD2 += flatObjects[i].VHD2;
                            basicDetails[index].H0 += flatObjects[i].H0;
                            basicDetails[index].H1 += flatObjects[i].H1;
                            basicDetails[index].H2 += flatObjects[i].H2;
                            basicDetails[index].V0 += flatObjects[i].V0;
                            basicDetails[index].V1 += flatObjects[i].V1;
                            basicDetails[index].V2 += flatObjects[i].V2;
                            basicDetails[index].zone0 += flatObjects[i].zone0;
                            basicDetails[index].zone1 += flatObjects[i].zone1;
                            basicDetails[index].zone2 += flatObjects[i].zone2;
                            basicDetails[index].total += flatObjects[i].total;
                        }
                    }
                    else
                    {
                        int parentIndex = flatObjects.FindIndex(x => x.key == flatObjects[i].pkey);
                        int childIndex = -1;

                        if (flatObjects[parentIndex].children is not null)
                        {
                            childIndex = flatObjects[parentIndex].children.FindIndex(y => y.key == flatObjects[i].key);
                        }
                        else
                        {
                            flatObjects[parentIndex].children = new List<BasicDetail>();
                        }

                        if (childIndex < 0)
                        {
                            flatObjects[parentIndex].children.Add(flatObjects[i]);
                        }
                        else
                        {
                            flatObjects[parentIndex].children[childIndex].house += flatObjects[i].house;
                            flatObjects[parentIndex].children[childIndex].businesses += flatObjects[i].businesses;
                            flatObjects[parentIndex].children[childIndex].householdsReserved += flatObjects[i].householdsReserved;
                            flatObjects[parentIndex].children[childIndex].HHD1 += flatObjects[i].HHD1;
                            flatObjects[parentIndex].children[childIndex].HHD2 += flatObjects[i].HHD2;
                            flatObjects[parentIndex].children[childIndex].VHD1 += flatObjects[i].VHD1;
                            flatObjects[parentIndex].children[childIndex].VHD2 += flatObjects[i].VHD2;
                            flatObjects[parentIndex].children[childIndex].H0 += flatObjects[i].H0;
                            flatObjects[parentIndex].children[childIndex].H1 += flatObjects[i].H1;
                            flatObjects[parentIndex].children[childIndex].H2 += flatObjects[i].H2;
                            flatObjects[parentIndex].children[childIndex].V0 += flatObjects[i].V0;
                            flatObjects[parentIndex].children[childIndex].V1 += flatObjects[i].V1;
                            flatObjects[parentIndex].children[childIndex].V2 += flatObjects[i].V2;
                            flatObjects[parentIndex].children[childIndex].zone0 += flatObjects[i].zone0;
                            flatObjects[parentIndex].children[childIndex].zone1 += flatObjects[i].zone1;
                            flatObjects[parentIndex].children[childIndex].zone2 += flatObjects[i].zone2;
                            flatObjects[parentIndex].children[childIndex].total += flatObjects[i].total;

                        }
                    }
                }
            }

            return basicDetails;
        }

        /// <summary>
        /// Formats the report data.
        /// </summary>
        /// <param name="utvalg">The utvalg.</param>
        /// <param name="showBusiness">if set to <c>true</c> [show business].</param>
        /// <param name="showHouseholds">if set to <c>true</c> [show households].</param>
        /// <param name="showHouseholdsReserved">if set to <c>true</c> [show households reserved].</param>
        /// <param name="strDayDetails">The string day details.</param>
        /// <param name="level">The level.</param>
        /// <param name="uptolevel">The uptolevel.</param>
        /// <param name="reportType">Type of report.</param>
        /// <returns></returns>
        private List<BasicDetail> formatReportData(Puma.Shared.Utvalg utvalg, bool showBusiness, bool showHouseholds, bool showHouseholdsReserved, string strDayDetails, int level, int uptolevel, string reportType)
        {


            List<BasicDetail> routesData = new List<BasicDetail>();

            foreach (var reoler in utvalg.Reoler)
            {
                if (level <= 0)
                {
                    BasicDetail flk = new BasicDetail()
                    {
                        name = reoler.Fylke,
                        cat = "flyke",
                        prisZone = reoler.PrisSone,
                        key = reoler.FylkeId,
                        pkey = null,
                        CssClass = reportType.ToLower() == "pdf" ? "flykerow" : "5"
                    };
                    var flykeobj = GetHouseHoldsCount<BasicDetail>(reoler, flk, strDayDetails, showHouseholds, showBusiness, showHouseholdsReserved);

                    routesData.Add(flk);
                }

                if (uptolevel <= 0) continue;

                if (level <= 1)
                {
                    BasicDetail kommune = new BasicDetail()
                    {
                        name = reoler.Kommune.ToString(),
                        cat = "kommune",
                        prisZone = reoler.PrisSone,
                        key = reoler.KommuneId,
                        pkey = level == 1 ? null : reoler.FylkeId,
                        CssClass = reportType.ToLower() == "pdf" ? "kommunerow" : "6"
                    };
                    var kommuneobj = GetHouseHoldsCount<BasicDetail>(reoler, kommune, strDayDetails, showHouseholds, showBusiness, showHouseholdsReserved);
                    routesData.Add(kommune);
                }
                if (uptolevel <= 1) continue;

                if (level <= 2)
                {
                    BasicDetail team = new BasicDetail()
                    {
                        name = reoler.TeamName.ToString(),
                        cat = "team",
                        prisZone = reoler.PrisSone,
                        key = reoler.KommuneId + reoler.TeamName,
                        pkey = level == 2 ? null : reoler.KommuneId,
                        CssClass = reportType.ToLower() == "pdf" ? "teamrow" : "7"
                    };
                    var teamobj = GetHouseHoldsCount<BasicDetail>(reoler, team, strDayDetails, showHouseholds, showBusiness, showHouseholdsReserved);
                    routesData.Add(team);
                }
                if (uptolevel <= 2) continue;

                if (level == 3 && uptolevel == 4)
                {
                    BasicDetail postalData = new BasicDetail()
                    {
                        name = reoler.PostalZone,
                        cat = "route",
                        prisZone = reoler.PrisSone,
                        key = reoler.PostalZone.ToString(),
                        pkey = null,
                        CssClass = reportType.ToLower() == "pdf" ? "postalrow" : "8"
                    };

                    _ = GetHouseHoldsCount<BasicDetail>(reoler, postalData, strDayDetails, showHouseholds, showBusiness, showHouseholdsReserved);
                    routesData.Add(postalData);

                    BasicDetail route = new BasicDetail()
                    {
                        name = reoler.Name,
                        cat = "route",
                        prisZone = reoler.PrisSone,
                        key = reoler.ReolId.ToString(),
                        pkey = reoler.PostalZone.ToString(),
                        CssClass = reportType.ToLower() == "pdf" ? "ruterow" : "8"
                    };

                    _ = GetHouseHoldsCount<BasicDetail>(reoler, route, strDayDetails, showHouseholds, showBusiness, showHouseholdsReserved);
                    routesData.Add(route);
                }
                else
                {
                    BasicDetail route = new BasicDetail()
                    {
                        name = level == 3 ? reoler.PostalZone : reoler.Name,
                        cat = "route",
                        prisZone = reoler.PrisSone,
                        key = level == 3 ? reoler.PostalZone.ToString() : reoler.ReolId.ToString(),
                        pkey = level >= 3 ? null : reoler.KommuneId + reoler.TeamName,
                        CssClass = reportType.ToLower() == "pdf" ? "ruterow" : "8"
                    };

                    var routeobj = GetHouseHoldsCount<BasicDetail>(reoler, route, strDayDetails, showHouseholds, showBusiness, showHouseholdsReserved);
                    routesData.Add(route);
                }
            }

            List<BasicDetail> formattedList = FillRecursive(routesData, null);

            return formattedList;
        }

        /// <summary>
        /// Exports the map image.
        /// </summary>
        /// <param name="RouteIds">The route ids.</param>
        /// <param name="isCustomerWeb">if set to <c>true</c> [is customer web].</param>
        /// <param name="strDayDetails">The string day details.</param>
        /// <param name="selectedAddress">The selected address.</param>
        /// <returns></returns>
        /// <exception cref="System.Net.Http.HttpRequestException">
        /// Error Occured while calling Query URI " + httpResponseMessageQ.StatusCode + " " + httpResponseMessageQ.Content.ReadAsStringAsync().Result
        /// or
        /// Error occured while pulling Map Image from Server " + httpResponseMessage.StatusCode + " " + httpResponseMessage.Content.ReadAsStringAsync().Result
        /// </exception>
        private async Task<string> ExportMapImage(List<long> RouteIds, bool isCustomerWeb, string strDayDetails, string selectedAddress = "")
        {
            //https://dev.pumamapservices.bring.no/arcgis/rest/services
            Uri _budruteLayer = new Uri(config.GetValue<string>("MapQuery_API_URL") + "5/query?f=json&returnExtentOnly=true&returnCountOnly=true&spatialRel=esriSpatialRelIntersects&where=reol_id in");
            string ImageUrl = "";
            string BufferImage = "";
            string routeIdsList = "";

            foreach (var id in RouteIds)
            {
                routeIdsList += "'" + id.ToString() + "'" + (RouteIds[RouteIds.Count - 1] == id ? "" : ",");
            }

            _budruteLayer = new Uri(_budruteLayer.AbsoluteUri + "(" + routeIdsList + ")");

            int adrlayerId = -1;
            switch (strDayDetails)
            {
                case "A-uke, tidliguke": adrlayerId = 0; break;
                case "A-uke, midtuke": adrlayerId = 1; break;
                case "B-uke, tidliguke": adrlayerId = 2; break;
                case "B-uke, midtuke": adrlayerId = 3; break;
                default: adrlayerId = -1; break;
            }

            List<SelectedAddress> lstselectedAddress = new List<SelectedAddress>();
            if (!string.IsNullOrWhiteSpace(selectedAddress))
            {
                lstselectedAddress = JsonConvert.DeserializeObject<List<SelectedAddress>>(selectedAddress);
            }
            List<FeaturePoint> featuresPoint = new List<FeaturePoint>();

            List<FeatureText> featuresText = new List<FeatureText>();

            if (lstselectedAddress?.Any() == true && lstselectedAddress.Count > 0)
            {
                foreach (var addrPoint in lstselectedAddress)
                {
                    featuresPoint.Add(new FeaturePoint()
                    {
                        geometry = new Geometry()
                        {
                            x = addrPoint.geometry != null ? addrPoint.geometry.x : addrPoint.location.x,
                            y = addrPoint.geometry != null ? addrPoint.geometry.y : addrPoint.location.y,
                            spatialReference = new SpatialReference()
                            {
                                wkid = 32633
                            }
                        }
                    });

                    featuresText.Add(new FeatureText()
                    {
                        geometry = new Geometry()
                        {
                            x = addrPoint.geometry != null ? addrPoint.geometry.x : addrPoint.location.x,
                            y = addrPoint.geometry != null ? addrPoint.geometry.y : addrPoint.location.y,
                            spatialReference = new SpatialReference()
                            {
                                wkid = 32633
                            }
                        },
                        symbol = new Symbol()
                        {
                            color = new List<int>() { 0,0,0,255
                       },
                            xoffset = 3,
                            yoffset = 3,
                            type = "esriTS",
                            horizontalAlignment = "center",
                            verticalAlignment = "bottom",
                            text = !string.IsNullOrWhiteSpace(addrPoint.attributes.display) ? addrPoint.attributes.display : addrPoint.attributes.Match_addr,
                            font = new Font()
                            {
                                family = "Arial",
                                decoration = "none",
                                size = 10,
                                style = "normal",
                                weight = "bold",
                            }
                        }
                    });

                }
            }

            string cmd = $" reol_id in ({routeIdsList}" + ")" + (isCustomerWeb ? " AND UPPER(REOLTYPE) <> UPPER('Boks')" : "");

            string dynamicLayer = "{ \"id\":5, \"layerDefinition\": { \"source\":{ \"type\":\"mapLayer\",\"mapLayerId\":5},\"definitionExpression\":\"" + cmd + "\",\"drawingInfo\":{ \"showLabels\":true, \"renderer\": {\"type\": \"simple\",\"symbol\": {\"type\": \"esriSFS\",\"style\": \"esriSFSSolid\",\"color\": [237, 54, 21, 70],\"outline\": {\"type\": \"esriSLS\",\"color\": [237, 54, 21, 255],\"width\": 0.75,\"style\": \"esriSLSSolid\"}}}} } }" +
               ",{ \"id\":6,\"layerDefinition\": { \"source\":{ \"type\":\"mapLayer\",\"mapLayerId\":6},\"drawingInfo\":{ \"showLabels\":true} } }" +
               ",{ \"id\":7,\"layerDefinition\": { \"source\":{ \"type\":\"mapLayer\",\"mapLayerId\":7},\"drawingInfo\":{ \"showLabels\":true} } }" +
                       ",{ \"id\":10,\"layerDefinition\": { \"source\":{ \"type\":\"mapLayer\",\"mapLayerId\":10},\"drawingInfo\":{ \"showLabels\":true} } }" +
                       ",{ \"id\":11,\"layerDefinition\": { \"source\":{ \"type\":\"mapLayer\",\"mapLayerId\":11},\"drawingInfo\":{ \"showLabels\":true} } }" +
                       ",{ \"id\":12,\"layerDefinition\": { \"source\":{ \"type\":\"mapLayer\",\"mapLayerId\":12},\"drawingInfo\":{ \"showLabels\":true} } }" +
                       //",{ \"id\":15,\"layerDefinition\": { \"source\":{ \"type\":\"mapLayer\",\"mapLayerId\":15},\"drawingInfo\":{ \"showLabels\":true} } }" +
                       // ",{ \"id\":16,\"layerDefinition\": { \"source\":{ \"type\":\"mapLayer\",\"mapLayerId\":16},\"drawingInfo\":{ \"showLabels\":true} } }" +
                       ",{ \"id\":17,\"layerDefinition\": { \"source\":{ \"type\":\"mapLayer\",\"mapLayerId\":17},\"drawingInfo\":{ \"showLabels\":true} } }" +
                       ",{ \"id\":18,\"layerDefinition\": { \"source\":{ \"type\":\"mapLayer\",\"mapLayerId\":18},\"drawingInfo\":{ \"showLabels\":true} } }" +
                       ",{ \"id\":20,\"layerDefinition\": { \"source\":{ \"type\":\"mapLayer\",\"mapLayerId\":20},\"drawingInfo\":{ \"showLabels\":true} } }" +
                       ",{ \"id\":21,\"layerDefinition\": { \"source\":{ \"type\":\"mapLayer\",\"mapLayerId\":21},\"drawingInfo\":{ \"showLabels\":true} } }" +
                       ",{ \"id\":22,\"layerDefinition\": { \"source\":{ \"type\":\"mapLayer\",\"mapLayerId\":22},\"drawingInfo\":{ \"showLabels\":true} } }" +
                       ",{ \"id\":23,\"layerDefinition\": { \"source\":{ \"type\":\"mapLayer\",\"mapLayerId\":23},\"drawingInfo\":{ \"showLabels\":true} } }" +
                       ",{ \"id\":24,\"layerDefinition\": { \"source\":{ \"type\":\"mapLayer\",\"mapLayerId\":24},\"drawingInfo\":{ \"showLabels\":true} } }" +
                       ",{ \"id\":27,\"layerDefinition\": { \"source\":{ \"type\":\"mapLayer\",\"mapLayerId\":27},\"drawingInfo\":{ \"showLabels\":true} } }" +
                       ",{ \"id\":28,\"layerDefinition\": { \"source\":{ \"type\":\"mapLayer\",\"mapLayerId\":28},\"drawingInfo\":{ \"showLabels\":true} } }" +
                       ",{ \"id\":29,\"layerDefinition\": { \"source\":{ \"type\":\"mapLayer\",\"mapLayerId\":29},\"drawingInfo\":{ \"showLabels\":true} } }" +
                       ",{ \"id\":31,\"layerDefinition\": { \"source\":{ \"type\":\"mapLayer\",\"mapLayerId\":31},\"drawingInfo\":{ \"showLabels\":true} } }" +
                       ",{ \"id\":32,\"layerDefinition\": { \"source\":{ \"type\":\"mapLayer\",\"mapLayerId\":32},\"drawingInfo\":{ \"showLabels\":true} } }" +
                       ",{ \"id\":35,\"layerDefinition\": { \"source\":{ \"type\":\"mapLayer\",\"mapLayerId\":35},\"drawingInfo\":{ \"showLabels\":true} } }" +
                       ",{ \"id\":38,\"layerDefinition\": { \"source\":{ \"type\":\"mapLayer\",\"mapLayerId\":38},\"drawingInfo\":{ \"showLabels\":true} } }" +
                       ",{ \"id\":39,\"layerDefinition\": { \"source\":{ \"type\":\"mapLayer\",\"mapLayerId\":39},\"drawingInfo\":{ \"showLabels\":true} } }" +
                       ",{ \"id\":41,\"layerDefinition\": { \"source\":{ \"type\":\"mapLayer\",\"mapLayerId\":41},\"drawingInfo\":{ \"showLabels\":true} } }" +
                       ",{ \"id\":42,\"layerDefinition\": { \"source\":{ \"type\":\"mapLayer\",\"mapLayerId\":42},\"drawingInfo\":{ \"showLabels\":true} } }" +
                       ",{ \"id\":43,\"layerDefinition\": { \"source\":{ \"type\":\"mapLayer\",\"mapLayerId\":43},\"drawingInfo\":{ \"showLabels\":true} } }" +
                       ",{ \"id\":45,\"layerDefinition\": { \"source\":{ \"type\":\"mapLayer\",\"mapLayerId\":45},\"drawingInfo\":{ \"showLabels\":true} } }" +
                       ",{ \"id\":46,\"layerDefinition\": { \"source\":{ \"type\":\"mapLayer\",\"mapLayerId\":46},\"drawingInfo\":{ \"showLabels\":true} } }" +
                       ",{ \"id\":47,\"layerDefinition\": { \"source\":{ \"type\":\"mapLayer\",\"mapLayerId\":47},\"drawingInfo\":{ \"showLabels\":true} } }" +
                       ",{ \"id\":50,\"layerDefinition\": { \"source\":{ \"type\":\"mapLayer\",\"mapLayerId\":50},\"drawingInfo\":{ \"showLabels\":true} } }" +
                       ",{ \"id\":51,\"layerDefinition\": { \"source\":{ \"type\":\"mapLayer\",\"mapLayerId\":51},\"drawingInfo\":{ \"showLabels\":true} } }" +
                       ",{ \"id\":53,\"layerDefinition\": { \"source\":{ \"type\":\"mapLayer\",\"mapLayerId\":53},\"drawingInfo\":{ \"showLabels\":true} } }" +
                       ",{ \"id\":55,\"layerDefinition\": { \"source\":{ \"type\":\"mapLayer\",\"mapLayerId\":55},\"drawingInfo\":{ \"showLabels\":true} } }" +
                       ",{ \"id\":58,\"layerDefinition\": { \"source\":{ \"type\":\"mapLayer\",\"mapLayerId\":58},\"drawingInfo\":{ \"showLabels\":true} } }" +
                       ",{ \"id\":60,\"layerDefinition\": { \"source\":{ \"type\":\"mapLayer\",\"mapLayerId\":60},\"drawingInfo\":{ \"showLabels\":true} } }" +
                       ",{ \"id\":62,\"layerDefinition\": { \"source\":{ \"type\":\"mapLayer\",\"mapLayerId\":62},\"drawingInfo\":{ \"showLabels\":true} } }" +
                       ",{ \"id\":65,\"layerDefinition\": { \"source\":{ \"type\":\"mapLayer\",\"mapLayerId\":65},\"drawingInfo\":{ \"showLabels\":true} } }" +
                       ",{ \"id\":67,\"layerDefinition\": { \"source\":{ \"type\":\"mapLayer\",\"mapLayerId\":67},\"drawingInfo\":{ \"showLabels\":true} } }" +
                       ",{ \"id\":69,\"layerDefinition\": { \"source\":{ \"type\":\"mapLayer\",\"mapLayerId\":69},\"drawingInfo\":{ \"showLabels\":true} } }";


            //Generate Token
            string token = await GenerateToken();

            // Create the feature table from the service URL.
            //ServiceFeatureTable _featureTable = new ServiceFeatureTable(_budruteLayer);

            // Create the feature layer from the table.
            //FeatureLayer myFeatureLayer = new FeatureLayer(_featureTable);
            // QueryParameters queryStates = new QueryParameters { WhereClause = cmd };
            try
            {
                var client = httpClientFactory.CreateClient();
                client.Timeout = TimeSpan.FromMinutes(30);

                Dictionary<string, string> mapQuery = new Dictionary<string, string>();
                mapQuery.Add("Where", cmd);
                mapQuery.Add("geometryType", "esriGeometryEnvelope");
                mapQuery.Add("spatialRel", "esriSpatialRelIntersects");
                mapQuery.Add("returnExtentOnly", "true");
                mapQuery.Add("returnGeometry", "false");
                mapQuery.Add("f", "pjson");

                client.DefaultRequestHeaders.Referrer = new Uri(config.GetSection("MapTokenGenerateURL").Value);

                var serviceBodyJsonQuery = new FormUrlEncodedContent(mapQuery);
                serviceBodyJsonQuery.Headers.Clear();
                serviceBodyJsonQuery.Headers.Add("Content-Type", "application/x-www-form-urlencoded");

                //client.DefaultRequestHeaders.Add("referer", "https://pumagisserver.qa.posten.no/arcgis");
                //client.DefaultRequestHeaders.Add("User-Agent", "PostmanRuntime/7.29.0");

                //client.DefaultRequestHeaders.Authorization
                //  = new AuthenticationHeaderValue("Bearer", token);
                //serviceBodyJsonQuery.Headers.Add("Authorization", "Bearer " + token);
                //https://dev.pumamapservices.bring.no/arcgis/rest/services/		config.GetValue<string>("MapQuery_API_URL") + "5/query?token=" + token	"https://pumagisserver.qa.posten.no/arcgis/rest/services/KSPU/MapServer/5/query?token=gQ8eFKJKiSoNLJ7Z93vKe9SV7y_ARPtqnWjCNBPCAH11zSrOV6D4u4oYRBTGEkJv"	string
                //token = "AndcUVffb3-A1ZOjKzi4sshN3WHRbGEyYVZTSFoxaOZkRfj8LD2YntZI5UX-LG3mkNcNaRmkbOobIMjwZSaSweLMr6MfvQnHa_OrcyL-bA8.";

                using var httpResponseMessageQ = await client.PostAsync(config.GetValue<string>("MapQuery_API_URL") + "5/query?token=" + token, serviceBodyJsonQuery);

                //using var httpResponseMessageQ = await client.PostAsync(config.GetValue<string>("MapQuery_API_URL") + "5/query", serviceBodyJsonQuery);

                //_context.WriteLine("_budruteLayer " + _budruteLayer);
                //var response = await client.GetAsync(_budruteLayer);

                string resultExtent = "";

                if (httpResponseMessageQ.IsSuccessStatusCode)
                {
                    resultExtent = httpResponseMessageQ.Content.ReadAsStringAsync().Result;
                }
                else
                {
                    throw new HttpRequestException("Error Occured while calling Query URI " + httpResponseMessageQ.StatusCode + " " + httpResponseMessageQ.Content.ReadAsStringAsync().Result);
                }
                resultExtent = resultExtent.Replace("\n", "");

                //Envelope resultExtent = await _featureTable.QueryExtentAsync(queryStates);

                if (adrlayerId != -1)
                {
                    dynamicLayer = "{ \"id\":" + adrlayerId + ",\"layerDefinition\":{ \"source\":{ \"type\":\"mapLayer\",\"mapLayerId\": " + adrlayerId + " },\"definitionExpression\":\"" + cmd + "\",\"drawingInfo\":{ \"showLabels\":true} } }," + dynamicLayer;

                }


                string objData = "{\"mapOptions\": " + resultExtent + ",\"operationalLayers\": [{ \"url\": " + "\"" + config.GetValue<string>("MapExport_API_URL") + "" + "\",\"token\": " + "\"" + token + "" + "\",\"layers\":[" + dynamicLayer + "]}" +
                   // ",{ \"id\":\"map_graphics\",\"featureCollection\": { \"layers\":[{ \"layerDefinition\": { \"name\": \"pointLayer\",\"geometryType\": \"esriGeometryPoint\",\"drawingInfo\": { \"renderer\": { \"type\": \"simple\",\"symbol\": { \"type\": \"esriSMS\",\"style\": \"esriSMSCircle\",\"color\": [0,0,255,255],\"size\": 8,\"outline\": { \"color\": [0,0,255,255],\"width\": 1} } } } },\"featureSet\":{ \"features\":" + JsonConvert.SerializeObject(featuresPoint) + "} },{ \"layerDefinition\": { \"name\": \"textLayer\",\"geometryType\": \"esriGeometryPoint\"},\"featureSet\": { \"features\": " + JsonConvert.SerializeObject(featuresText) + "} }]}}" +
                   "],\"exportOptions\":{ \"outputSize\":[600,450],\"dpi\":100}}";
                if (lstselectedAddress?.Any() == true && lstselectedAddress.Count > 0)
                {
                    objData = "{\"mapOptions\": " + resultExtent + ",\"operationalLayers\": [{ \"url\": " + "\"" + config.GetValue<string>("MapExport_API_URL") + "" + "\",\"token\": " + "\"" + token + "" + "\",\"layers\":[" + dynamicLayer + "]}" +
                   ",{ \"id\":\"map_graphics\",\"featureCollection\": { \"layers\":[{ \"layerDefinition\": { \"name\": \"pointLayer\",\"geometryType\": \"esriGeometryPoint\",\"drawingInfo\": { \"renderer\": { \"type\": \"simple\",\"symbol\": { \"type\": \"esriSMS\",\"style\": \"esriSMSCircle\",\"color\": [0,0,255,255],\"size\": 8,\"outline\": { \"color\": [0,0,255,255],\"width\": 1} } } } },\"featureSet\":{ \"features\":" + JsonConvert.SerializeObject(featuresPoint) + "} },{ \"layerDefinition\": { \"name\": \"textLayer\",\"geometryType\": \"esriGeometryPoint\"},\"featureSet\": { \"features\": " + JsonConvert.SerializeObject(featuresText) + "} }]}}" +
                  "],\"exportOptions\":{ \"outputSize\":[600,450],\"dpi\":100}}";
                }


                //token = await GenerateToken();

                Dictionary<string, string> map = new Dictionary<string, string>();
                map.Add("Web_Map_as_JSON", objData);
                map.Add("Format", "png8");
                map.Add("Layout_Template", "map_only");
                map.Add("f", "pjson");

                var serviceBodyJson = new FormUrlEncodedContent(map);
                serviceBodyJson.Headers.Clear();
                serviceBodyJson.Headers.Add("Content-Type", "application/x-www-form-urlencoded");

                //client.DefaultRequestHeaders.Authorization
                //  = new AuthenticationHeaderValue("Bearer", token);

                //https://dev.pumamapservices.bring.no/arcgis/rest/services/
                _context.WriteLine("Calling Export Map");
                _context.WriteLine("ObjData : " + objData);

                using var httpResponseMessage = await client.PostAsync(config.GetValue<string>("MapPrint_API_URL") + "Export%20Web%20Map%20Task/execute?token=" + token, serviceBodyJson);

                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    var Obj = JsonConvert.DeserializeObject<WebMapResponse>(httpResponseMessage.Content.ReadAsStringAsync().Result);
                    _context.WriteLine("Calling ImageBuffer Obj.results[0]" + JsonConvert.SerializeObject(Obj));

                    if (Obj?.results?.Any() == true)
                    {
                        ImageUrl = Obj?.results[0]?.value?.url;
                        if (ImageUrl.IndexOf(".png") > 0)
                        {
                            ImageUrl = ImageUrl.Replace("http://", "https://");
                            var bytes = await client.GetByteArrayAsync(ImageUrl);
                            BufferImage = Convert.ToBase64String(bytes);
                        }
                    }
                    else
                    {
                        _context.WriteLine("Response from Map service" + httpResponseMessage.Content.ReadAsStringAsync().Result);

                    }
                }
                else
                {
                    throw new HttpRequestException("Error occured while pulling Map Image from Server " + httpResponseMessage.StatusCode + " " + httpResponseMessage.Content.ReadAsStringAsync().Result);
                }
            }
            catch (Exception e)
            {
                _context.WriteLine("Error Occured" + e.Message);
                throw;
            }

            return BufferImage;
        }


        /// <summary>
        /// Generates the token.
        /// </summary>
        /// <returns></returns>
        private async Task<string> GenerateToken()
        {
            return await _mediator.Send(new RequestGenerateReportToken());
        }

    }




    /// <summary>
    /// 
    /// </summary>
    public class BasicDetail
    {
        /// <summary>
        /// The name
        /// </summary>
        public string name;
        /// <summary>
        /// The cat
        /// </summary>
        public string cat;
        /// <summary>
        /// The pris zone
        /// </summary>
        public int prisZone;
        /// <summary>
        /// The key
        /// </summary>
        public string key;
        /// <summary>
        /// The house
        /// </summary>
        public int house;
        /// <summary>
        /// The households reserved
        /// </summary>
        public int householdsReserved;
        /// <summary>
        /// The businesses
        /// </summary>
        public int businesses;
        /// <summary>
        /// The hh d1
        /// </summary>
        public int HHD1;
        /// <summary>
        /// The hh d2
        /// </summary>
        public int HHD2;
        /// <summary>
        /// The vh d1
        /// </summary>
        public int VHD1;
        /// <summary>
        /// The vh d2
        /// </summary>
        public int VHD2;
        /// <summary>
        /// The zone0
        /// </summary>
        public int zone0;
        /// <summary>
        /// The zone1
        /// </summary>
        public int zone1;
        /// <summary>
        /// The zone2
        /// </summary>
        public int zone2;
        /// <summary>
        /// The pkey
        /// </summary>
        public string pkey;
        /// <summary>
        /// The h0
        /// </summary>
        public int H0;
        /// <summary>
        /// The h1
        /// </summary>
        public int H1;
        /// <summary>
        /// The h2
        /// </summary>
        public int H2;
        /// <summary>
        /// The v0
        /// </summary>
        public int V0;
        /// <summary>
        /// The v1
        /// </summary>
        public int V1;
        /// <summary>
        /// The v2
        /// </summary>
        public int V2;
        /// <summary>
        /// The total
        /// </summary>
        public int total;

        /// <summary>
        /// Class to display in report
        /// </summary>
        public string CssClass { get; set; }
        /// <summary>
        /// The children
        /// </summary>
        public List<BasicDetail> children;
    }

    /// <summary>
    /// 
    /// </summary>
    public class ItemList
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string name { get; set; }
        /// <summary>
        /// Gets or sets the logo.
        /// </summary>
        /// <value>
        /// The logo.
        /// </value>
        public string logo { get; set; }
        /// <summary>
        /// The items
        /// </summary>
        public List<ItemUtvalg> items;
        /// <summary>
        /// Gets or sets the zone0.
        /// </summary>
        /// <value>
        /// The zone0.
        /// </value>
        public int zone0 { get; set; }
        /// <summary>
        /// Gets or sets the zone1.
        /// </summary>
        /// <value>
        /// The zone1.
        /// </value>
        public int zone1 { get; set; }
        /// <summary>
        /// Gets or sets the zone2.
        /// </summary>
        /// <value>
        /// The zone2.
        /// </value>
        public int zone2 { get; set; }
        /// <summary>
        /// Gets or sets the total.
        /// </summary>
        /// <value>
        /// The total.
        /// </value>
        public int total { get; set; }
        /// <summary>
        /// Gets or sets the dag1.
        /// </summary>
        /// <value>
        /// The dag1.
        /// </value>
        public int dag1 { get; set; }
        /// <summary>
        /// Gets or sets the dag2.
        /// </summary>
        /// <value>
        /// The dag2.
        /// </value>
        public int dag2 { get; set; }


    }
    /// <summary>
    /// 
    /// </summary>
    public class ItemUtvalg
    {
        /// <summary>
        /// Index
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public int id { get; set; }
        /// <summary>
        /// Gets or sets the items count.
        /// </summary>
        /// <value>
        /// The items count.
        /// </value>
        public int itemsCount { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether [show reserverd].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [show reserverd]; otherwise, <c>false</c>.
        /// </value>
        public bool showReserverd { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether [show household].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [show household]; otherwise, <c>false</c>.
        /// </value>
        public bool showHousehold { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [show business].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [show business]; otherwise, <c>false</c>.
        /// </value>
        public bool showBusiness { get; set; }
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string name { get; set; }
        /// <summary>
        /// Gets or sets the house holds.
        /// </summary>
        /// <value>
        /// The house holds.
        /// </value>
        public int HouseHolds { get; set; }
        /// <summary>
        /// Gets or sets the business.
        /// </summary>
        /// <value>
        /// The business.
        /// </value>
        public int Business { get; set; }
        /// <summary>
        /// Gets or sets the reoler.
        /// </summary>
        /// <value>
        /// The reoler.
        /// </value>
        public List<BasicDetail> Reoler { get; set; }
        /// <summary>
        /// Gets or sets the img URL.
        /// </summary>
        /// <value>
        /// The img URL.
        /// </value>
        public string imgUrl { get; set; }

        /// <summary>
        /// Gets or sets the forhandlerpartyk.
        /// </summary>
        /// <value>
        /// The forhandlerpartyk.
        /// </value>
        public string forhandlerpartyk { get; set; }
        /// <summary>
        /// Gets or sets the zone0.
        /// </summary>
        /// <value>
        /// The zone0.
        /// </value>
        public int zone0 { get; set; }
        /// <summary>
        /// Gets or sets the zone1.
        /// </summary>
        /// <value>
        /// The zone1.
        /// </value>
        public int zone1 { get; set; }
        /// <summary>
        /// Gets or sets the zone2.
        /// </summary>
        /// <value>
        /// The zone2.
        /// </value>
        public int zone2 { get; set; }
        /// <summary>
        /// Gets or sets the total.
        /// </summary>
        /// <value>
        /// The total.
        /// </value>
        public int total { get; set; }
        /// <summary>
        /// Gets or sets the dag1.
        /// </summary>
        /// <value>
        /// The dag1.
        /// </value>
        public int dag1 { get; set; }
        /// <summary>
        /// Gets or sets the dag2.
        /// </summary>
        /// <value>
        /// The dag2.
        /// </value>
        public int dag2 { get; set; }
        /// <summary>
        /// Gets or sets the imagedata.
        /// </summary>
        /// <value>
        /// The imagedata.
        /// </value>
        public string imagedata { get; set; }

        /// <summary>
        /// Total HD1
        /// </summary>
        public int totalHD1 { get; set; }

        /// <summary>
        /// Total HD2
        /// </summary>

        public int totalHD2 { get; set; }

        /// <summary>
        /// Total VD1
        /// </summary>
        public int totalVD1 { get; set; }

        /// <summary>
        /// Total VD2
        /// </summary>

        public int totalVD2 { get; set; }

        /// <summary>
        /// Total H0
        /// </summary>
        public int totalH0 { get; set; }

        /// <summary>
        /// Total H1
        /// </summary>
        public int totalH1 { get; set; }

        /// <summary>
        /// Total H2
        /// </summary>
        public int totalH2 { get; set; }

        /// <summary>
        /// Total v0
        /// </summary>
        public int totalV0 { get; set; }

        /// <summary>
        /// Total V1
        /// </summary>
        public int totalV1 { get; set; }

        /// <summary>
        /// Total V2
        /// </summary>
        public int totalV2 { get; set; }

    }

    /// <summary>
    /// 
    /// </summary>
    public class JsReport
    {
        /// <summary>
        /// The template
        /// </summary>
        public Template template;
        /// <summary>
        /// The data
        /// </summary>
        public ReportPayload data;
    }
    /// <summary>
    /// 
    /// </summary>
    public class ReportPayload
    {
        /// <summary>
        /// Gets or sets the name of the report.
        /// </summary>
        /// <value>
        /// The name of the report.
        /// </value>
        public string reportName { get; set; }
        /// <summary>
        /// Gets or sets the SMTP.
        /// </summary>
        /// <value>
        /// The SMTP.
        /// </value>
        public string smtp { get; set; }
        /// <summary>
        /// Gets or sets the level.
        /// </summary>
        /// <value>
        /// The level.
        /// </value>
        public int level { get; set; }
        /// <summary>
        /// Gets or sets the upto level.
        /// </summary>
        /// <value>
        /// The upto level.
        /// </value>
        public int uptoLevel { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="ReportPayload"/> is islist.
        /// </summary>
        /// <value>
        ///   <c>true</c> if islist; otherwise, <c>false</c>.
        /// </value>
        public bool islist { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this instance is mapwith data.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is mapwith data; otherwise, <c>false</c>.
        /// </value>
        public bool isMapwithData { get; set; }
        /// <summary>
        /// Gets or sets the distr date.
        /// </summary>
        /// <value>
        /// The distr date.
        /// </value>
        public string distrDate { get; set; }
        /// <summary>
        /// Gets or sets the zone0.
        /// </summary>
        /// <value>
        /// The zone0.
        /// </value>
        public int zone0 { get; set; }
        /// <summary>
        /// Gets or sets the zone1.
        /// </summary>
        /// <value>
        /// The zone1.
        /// </value>
        public int zone1 { get; set; }
        /// <summary>
        /// Gets or sets the zone2.
        /// </summary>
        /// <value>
        /// The zone2.
        /// </value>
        public int zone2 { get; set; }
        /// <summary>
        /// Gets or sets the total.
        /// </summary>
        /// <value>
        /// The total.
        /// </value>
        public int total { get; set; }
        /// <summary>
        /// Gets or sets the dag1.
        /// </summary>
        /// <value>
        /// The dag1.
        /// </value>
        public int dag1 { get; set; }
        /// <summary>
        /// Gets or sets the dag2.
        /// </summary>
        /// <value>
        /// The dag2.
        /// </value>
        public int dag2 { get; set; }
        /// <summary>
        /// Gets or sets the list array.
        /// </summary>
        /// <value>
        /// The list array.
        /// </value>
        public List<ItemList> listArray { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this instance is email.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is email; otherwise, <c>false</c>.
        /// </value>
        public bool isEmail { get; set; }
        /// <summary>
        /// Gets or sets the email to.
        /// </summary>
        /// <value>
        /// The email to.
        /// </value>
        public string emailTo { get; set; }

        /// <summary>
        /// Logo
        /// </summary>
        public string logo { get; set; }

        public string listId { get; set; }
    }
    /// <summary>
    /// 
    /// </summary>
    public class Template
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string name { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class WebMapResponse
    {
        /// <summary>
        /// The results
        /// </summary>
        public List<WebMapValue> results;
        /// <summary>
        /// The messages
        /// </summary>
        public List<string> messages;
    }

    /// <summary>
    /// 
    /// </summary>
    public class WebMapValue
    {
        /// <summary>
        /// Gets or sets the name of the parameter.
        /// </summary>
        /// <value>
        /// The name of the parameter.
        /// </value>
        public string paramName { get; set; }
        /// <summary>
        /// Gets or sets the type of the data.
        /// </summary>
        /// <value>
        /// The type of the data.
        /// </value>
        public string dataType { get; set; }
        /// <summary>
        /// The value
        /// </summary>
        public ImageURL value;
    }

    /// <summary>
    /// 
    /// </summary>
    public class ImageURL
    {
        /// <summary>
        /// Gets or sets the URL.
        /// </summary>
        /// <value>
        /// The URL.
        /// </value>
        public string url { get; set; }
    }
}
