using System;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Puma.Infrastructure.Interface.KsupDB;
using Puma.Shared;
using static Puma.Shared.PumaEnum;

namespace DataAccessAPI.Helper
{
    /// <summary>
    /// 
    /// </summary>
    public class XML
    {
        #region Private variables

        private static string pathToXSDFile = string.Empty;
        private static string xpath_ProductionDate = string.Empty;
        private static string xpath_DistributionDate = string.Empty;
        private static int productionDateFreshInMinutes = 0;
        private static int numberOfDistributionDatesField = 0;
        private static int numberOfRoutesTotalField = 0;
        private static int numberOfRoutesPrDateField = 0;
        private static int numberOfCapacitiesPrDateField = 0;
        private static int numberOfErrorsField = 0;
        private static bool isFreshnessToBeValidated = true;
        private static CapacityDate capacityDate;
        private static CapacityRoute capacityRoute;
        private static PRSAdminCapacity prsAdminCapacity;
        private static string xpath_CalendarDetail = string.Empty;

        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger<XML> _logger;

        /// <summary>
        /// The get PRS calendar admin details repository
        /// </summary>
        private readonly IPumaRestCapacityRepository _pumaRestCapacityRepository;

        private readonly IConfiguration _configuration;
        #endregion

        #region Public Properties

        public static int NumberOfDistributionDates
        {
            get
            {
                return numberOfDistributionDatesField;
            }

            set
            {
                numberOfDistributionDatesField = value;
            }
        }

        public static int NumberOfRoutesTotal
        {
            get
            {
                return numberOfRoutesTotalField;
            }

            set
            {
                numberOfRoutesTotalField = value;
            }
        }

        public static int NumberOfRoutesPrDate
        {
            get
            {
                return numberOfRoutesPrDateField;
            }

            set
            {
                numberOfRoutesPrDateField = value;
            }
        }

        public static int NumberOfCapacitiesPrDate
        {
            get
            {
                return numberOfCapacitiesPrDateField;
            }

            set
            {
                numberOfCapacitiesPrDateField = value;
            }
        }

        public static int NumberOfErrors
        {
            get
            {
                return numberOfErrorsField;
            }

            set
            {
                numberOfErrorsField = value;
            }
        }

        #endregion

        #region Constructor

        public XML(ILogger<XML> logger, IPumaRestCapacityRepository pumaRestCapacityRepository, IConfiguration configuration)
        {
            string currentParameter = string.Empty;
            try
            {
                _configuration = configuration;
                //var reader = new AppSettingsReader();
                currentParameter = "PathToXSDFile";
                //pathToXSDFile = Convert.ToString(configuration.GetValue("PathToXSDFile", typeof(string)));
                pathToXSDFile = _configuration.GetValue<string>("PathToXSDFile");
                currentParameter = "XPATH_ProductionDate";
                //xpath_ProductionDate = Convert.ToString(configuration.GetValue("XPATH_ProductionDate", typeof(string)));
                xpath_ProductionDate = _configuration.GetValue<string>("XPATH_ProductionDate");
                currentParameter = "XPATH_DistributionDate";
                //xpath_DistributionDate = Convert.ToString(configuration.GetValue("XPATH_DistributionDate", typeof(string)));
                xpath_DistributionDate = _configuration.GetValue<string>("XPATH_DistributionDate");
                currentParameter = "ProductionDateFreshInMinutes";
                //productionDateFreshInMinutes = Convert.ToInt32(configuration.GetValue("ProductionDateFreshInMinutes", typeof(int)));
                productionDateFreshInMinutes = _configuration.GetValue<int>("ProductionDateFreshInMinutes");
                currentParameter = "IsFreshnessToBeValidated";

                currentParameter = "XPATH_CalendarDetail";
                xpath_CalendarDetail = _configuration.GetValue<string>("XPATH_CalendarDetail");
                //isFreshnessToBeValidated = Convert.ToBoolean(configuration.GetValue("IsFreshnessToBeValidated", typeof(bool)));
                isFreshnessToBeValidated = _configuration.GetValue<bool>("IsFreshnessToBeValidated");
                _logger = logger ?? throw new ArgumentNullException(nameof(logger));
                _pumaRestCapacityRepository = pumaRestCapacityRepository ?? throw new ArgumentNullException(nameof(pumaRestCapacityRepository));
            }
            catch (Exception ex)
            {
                string error = string.Format("Restkapasitet Posten.Puma.SpareCapacity.Library.XML.Constructor" + Microsoft.VisualBasic.Constants.vbCrLf + "Current parameter: {0}" + Microsoft.VisualBasic.Constants.vbCrLf + "Exception:" + Microsoft.VisualBasic.Constants.vbCrLf + "{1}", currentParameter, ex.Message);
                throw new ApplicationException(error);
            }
        }

        #endregion

        #region CheckCreationDate

        /// <summary>
        /// 
        /// </summary>
        /// <param name="capacityInfo"></param>
        public void CheckCreationDate(ref CapacityInfo capacityInfo)
        {
            try
            {
                var maxCreationDate = DateTime.MinValue;
                string maxCreationDateFile = string.Empty;
                foreach (var file in capacityInfo.UnzipFiles)
                {
                    // string executableLocation = Path.GetDirectoryName(
                    //Assembly.GetExecutingAssembly().Location);
                    // string xslLocation = Path.Combine(executableLocation, "RestCapacity.xsd");
                    var doc = CreateAndValidate(file, pathToXSDFile);
                    var creationDate = XML.DateTimeXMLNodeGetValue(doc, xpath_ProductionDate);
                    _logger.LogInformation(string.Format("Melding {0} har produksjonsdato {1}", file, creationDate.ToString()), RestCapacityMessageType.InfoMessage);
                    if (creationDate > maxCreationDate)
                    {
                        maxCreationDate = creationDate;
                        maxCreationDateFile = file;
                    }
                }

                capacityInfo.IsFresh = CheckFreshness(maxCreationDate);
                capacityInfo.FreshFile = maxCreationDateFile;
                capacityInfo.FreshFileDate = maxCreationDate;
            }

            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        #endregion

        #region ExtractRestCapacityInfo

        /// <summary>
        /// 
        /// </summary>
        /// <param name="XMLFile"></param>
        /// public void ExtractRestCapacityInfo(XmlDocument capacityDoc)
        public async Task ExtractRestCapacityInfo(string XMLFile)
        {
            var capacityDoc = XML.Create(XMLFile);
            Console.WriteLine("Påbegynner uttrekk av informasjon");
            _logger.LogInformation(string.Format("Påbegynner uttrekk av informasjon fra fil: '{0}'.", XMLFile), RestCapacityMessageType.InfoMessage);
            var nodesInDistributionDates = capacityDoc.SelectNodes(xpath_DistributionDate);
            var nodesInCalendarDetails = capacityDoc.SelectNodes(xpath_CalendarDetail);
            foreach (XmlNode node in nodesInDistributionDates)
            {
                XmlElement element = node as XmlElement;
                if (element is object)
                {
                    await GetDateInfo(element);

                }
            }

            if (nodesInCalendarDetails.Count > 0)
            {
                foreach (XmlNode node in nodesInCalendarDetails)
                {
                    XmlElement element = node as XmlElement;
                    if (element is object)
                    {
                        await GetPRSCalendarInfo(element);
                    }
                }
            }

            Console.WriteLine("Avslutter uttrekk av informasjon");
            _logger.LogInformation("Avslutter uttrekk av informasjon", RestCapacityMessageType.InfoMessage);
        }

        #endregion

        #region GetDateInfo

        /// <summary>
        /// 
        /// </summary>
        /// <param name="element"></param>
        private async Task GetDateInfo(XmlElement element)
        {
            if (element.HasChildNodes)
            {
                capacityDate = new CapacityDate();
                capacityRoute = new CapacityRoute();
                foreach (XmlNode nodesInDistributionDate in element.ChildNodes)
                {
                    try
                    {
                        switch (nodesInDistributionDate.Name ?? "")
                        {
                            case "Date":
                                {
                                    var tempDate = DateTime.MinValue;
                                    if (DateTime.TryParse(nodesInDistributionDate.InnerText, out tempDate))
                                    {
                                        capacityDate.Date = tempDate;
                                        capacityRoute.Date = capacityDate.Date;
                                    }
                                    else
                                    {
                                        throw new ApplicationException(string.Format("Konverteringsfeil i GetDateInfo. {0} kan ikke konverteres til Date", nodesInDistributionDate.InnerText));
                                    }

                                    break;
                                }

                            case "WeekNo":
                                {
                                    int tempInt = int.MinValue;
                                    if (int.TryParse(nodesInDistributionDate.InnerText, out tempInt))
                                    {
                                        capacityDate.WeekNumber = tempInt;
                                    }
                                    else
                                    {
                                        throw new ApplicationException(string.Format("Konverteringsfeil i GetDateInfo. {0} kan ikke konverteres til WeekNo", nodesInDistributionDate.InnerText));
                                    }

                                    break;
                                }

                            case "DistributionDay":
                                {
                                    capacityDate.DistributionDay = nodesInDistributionDate.InnerText.Trim();
                                    break;
                                }

                            case "BusinessDay":
                                {
                                    capacityDate.BusinessDay = nodesInDistributionDate.InnerText.Trim();
                                    break;
                                }

                            case "Routes":
                                {
                                    await GetRoutesInfo(nodesInDistributionDate);
                                    break;
                                }

                            default:
                                {
                                    break;
                                }
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex.Message);
                        numberOfErrorsField += 1;
                        continue;
                    }
                }

                // System.Diagnostics.Trace.WriteLine("-----------------------CAPACITYDATE-------------------");
                // System.Diagnostics.Trace.WriteLine(string.Format("Date: {0}", capacityDate.Date));
                // System.Diagnostics.Trace.WriteLine(string.Format("WeekNumber: {0}", capacityDate.WeekNumber));
                // System.Diagnostics.Trace.WriteLine(string.Format("DistributionDay: {0}", capacityDate.DistributionDay));
                // System.Diagnostics.Trace.WriteLine(string.Format("BusinessDay: {0}", capacityDate.BusinessDay));
                // System.Diagnostics.Trace.WriteLine("------------------------------------------------------");

                try
                {
                    await _pumaRestCapacityRepository.Kapasitetdato_AddAsync(capacityDate.Date, capacityDate.WeekNumber, capacityDate.DistributionDay, capacityDate.BusinessDay);
                    Console.WriteLine("Lagret dato: {0}. Antall ruter: {1}. Antall kapasiteter: {2}", capacityDate.Date.ToShortDateString(), NumberOfRoutesPrDate.ToString(), NumberOfCapacitiesPrDate.ToString());
                    _logger.LogInformation(string.Format("Lagret dato: {0}. Antall ruter: {1}. Antall kapasiteter: {2}", capacityDate.Date.ToShortDateString(), NumberOfRoutesPrDate.ToString(), NumberOfCapacitiesPrDate.ToString()), RestCapacityMessageType.InfoMessage);
                    NumberOfDistributionDates += 1;
                    NumberOfRoutesPrDate = 0;
                    NumberOfCapacitiesPrDate = 0;
                }
                catch (Exception ex)
                {
                    numberOfErrorsField += 1;
                    _logger.LogError(ex.Message);
                }
            }
        }

        #endregion

        #region GetRoute(s)Info

        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        private async Task GetRoutesInfo(XmlNode node)
        {
            if (node.HasChildNodes)
            {
                foreach (XmlNode nodesInRoutes in node.ChildNodes)
                    await GetRouteInfo(nodesInRoutes);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        private async Task GetRouteInfo(XmlNode node)
        {
            char padCharacter = '0';
            if (node.HasChildNodes)
            {
                foreach (XmlNode nodesInRoute in node.ChildNodes)
                {
                    try
                    {
                        switch (nodesInRoute.Name ?? "")
                        {
                            case "TeamNo":
                                {
                                    capacityRoute.ReolNumberString = nodesInRoute.InnerText.Trim();
                                    capacityRoute.ReolNumberString = capacityRoute.ReolNumberString.PadLeft(6, padCharacter);
                                    break;
                                }

                            case "RouteNo":
                                {
                                    string temp = nodesInRoute.InnerText.Trim();
                                    temp = temp.PadLeft(4, padCharacter);
                                    capacityRoute.ReolNumberString += temp;
                                    long tempInt = long.MinValue;
                                    if (long.TryParse(capacityRoute.ReolNumberString, out tempInt))
                                    {
                                        capacityRoute.ReolNumberInt = tempInt;
                                    }
                                    else
                                    {
                                        throw new ApplicationException(string.Format("Konverteringsfeil i GetRouteInfo. {0} kan ikke konverteres til RouteNo", capacityRoute.ReolNumberString));
                                    }

                                    break;
                                }

                            case "Capacities":
                                {
                                    await GetCapacitiesInfo(nodesInRoute);
                                    break;
                                }

                            default:
                                {
                                    break;
                                }
                        }
                    }
                    catch (Exception ex)
                    {
                        numberOfErrorsField += 1;
                        _logger.LogError(ex.Message);
                        continue;
                    }
                }
            }
        }

        #endregion

        #region GetCapacitiesInfo

        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        private async Task GetCapacitiesInfo(XmlNode node)
        {
            if (node.HasChildNodes)
            {
                foreach (XmlNode nodesInCapacities in node.ChildNodes)
                {
                    await GetCapacityInfo(nodesInCapacities);

                    // System.Diagnostics.Trace.WriteLine("-----------------------CAPACITYROUTE------------------");
                    // System.Diagnostics.Trace.WriteLine(string.Format("Date: {0}", capacityRoute.Date.ToString()));
                    // System.Diagnostics.Trace.WriteLine(string.Format("ReolNumber: {0}", capacityRoute.ReolNumberInt.ToString()));
                    // System.Diagnostics.Trace.WriteLine(string.Format("RecipientType: {0}", capacityRoute.RecipientType.ToString()));
                    // System.Diagnostics.Trace.WriteLine(string.Format("RestCount: {0}", capacityRoute.RestCount.ToString()));
                    // System.Diagnostics.Trace.WriteLine(string.Format("RestWeight: {0}", capacityRoute.RestWeight.ToString()));
                    // System.Diagnostics.Trace.WriteLine("------------------------------------------------------");

                    try
                    {
                        await _pumaRestCapacityRepository.Kapasitetruter_AddAsync(capacityRoute.Date, capacityRoute.ReolNumberInt, capacityRoute.RestWeight, capacityRoute.RestCount, capacityRoute.RecipientType, capacityRoute.RestThickness);
                        NumberOfRoutesTotal += 1;
                        NumberOfRoutesPrDate += 1;
                        NumberOfCapacitiesPrDate += 1;
                    }
                    catch (Exception ex)
                    {
                        numberOfErrorsField += 1;
                        _logger.LogError(ex.Message);
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        private async Task GetCapacityInfo(XmlNode node)
        {
            await Task.Run(() => { });
            if (node.HasChildNodes)
            {
                foreach (XmlNode nodesInRoute in node.ChildNodes)
                {
                    try
                    {
                        switch (nodesInRoute.Name ?? "")
                        {
                            case "RecipientType":
                                {
                                    capacityRoute.RecipientType = nodesInRoute.InnerText.Trim();
                                    break;
                                }

                            case "RestCount":
                                {
                                    int tempInt = int.MinValue;
                                    if (int.TryParse(nodesInRoute.InnerText, out tempInt))
                                    {
                                        capacityRoute.RestCount = tempInt;
                                    }
                                    else
                                    {
                                        throw new ApplicationException(string.Format("Konverteringsfeil i GetCapacityInfo. {0} kan ikke konverteres til RestCount", nodesInRoute.InnerText));
                                    }

                                    break;
                                }

                            case "RestWeight":
                                {
                                    int tempInt = int.MinValue;
                                    if (int.TryParse(nodesInRoute.InnerText, out tempInt))
                                    {
                                        capacityRoute.RestWeight = tempInt;
                                    }
                                    else
                                    {
                                        throw new ApplicationException(string.Format("Konverteringsfeil i GetCapacityInfo. {0} kan ikke konverteres til RestWeight", nodesInRoute.InnerText));
                                    }

                                    break;
                                }
                            // RDF
                            case "RestThickness":
                                {
                                    double tempdec = double.MinValue;
                                    if (double.TryParse(nodesInRoute.InnerText, out tempdec))
                                    {
                                        capacityRoute.RestThickness = tempdec;
                                    }
                                    else
                                    {
                                        throw new ApplicationException(string.Format("Konverteringsfeil i GetCapacityInfo. {0} kan ikke konverteres til RestThickness", nodesInRoute.InnerText));
                                    }

                                    break;
                                }

                            default:
                                {
                                    break;
                                }
                        }
                    }
                    catch (Exception __unusedException1__)
                    {
                        _logger.LogError(__unusedException1__.Message);
                        continue;
                    }
                }
            }
        }

        #endregion

        #region Local Utilities

        /// <summary>
        /// 
        /// </summary>
        /// <param name="maxCreationDate"></param>
        /// <returns></returns>
        private bool CheckFreshness(DateTime maxCreationDate)
        {
            bool returnValue = false;
            if (!isFreshnessToBeValidated)
            {
                if (true)
                {
                    _logger.LogInformation("Krav til ferskhet i meldinger blir ikke utført. Jfr konfigurasjon.", RestCapacityMessageType.InfoMessage);
                    returnValue = true;
                }
            }
            else if (maxCreationDate.AddMinutes(productionDateFreshInMinutes) > DateTime.Now)
            {
                _logger.LogInformation("Nyeste melding tilfredstiller krav til ferskhet", RestCapacityMessageType.InfoMessage);
                returnValue = true;
            }
            else
            {
                _logger.LogInformation("Nyeste melding tilfredstiller IKKE krav til ferskhet", RestCapacityMessageType.InfoMessage);
            }

            return returnValue;
        }

        #endregion

        #region GetPRSCalendarInfo

        /// <summary>
        /// 
        /// </summary>
        /// <param name="element"></param>
        private async Task GetPRSCalendarInfo(XmlElement element)
        {
            if (element.HasChildNodes)
            {
                prsAdminCapacity = new PRSAdminCapacity();
                foreach (XmlNode nodesInCalendarDetail in element.ChildNodes)
                {
                    try
                    {
                        switch (nodesInCalendarDetail.Name ?? "")
                        {
                            case "Date":
                                {
                                    var tempDate = DateTime.MinValue;
                                    if (DateTime.TryParse(nodesInCalendarDetail.InnerText, out tempDate))
                                    {
                                        prsAdminCapacity.Date = tempDate;
                                    }
                                    else
                                    {
                                        throw new ApplicationException(string.Format("Konverteringsfeil i GetPRSCalendarInfo. {0} kan ikke konverteres til Date", nodesInCalendarDetail.InnerText));
                                    }

                                    break;
                                }

                            case "IsHoliday":
                                {
                                    prsAdminCapacity.IsHoliday = nodesInCalendarDetail.InnerText.Trim();
                                    break;
                                }

                            case "IsEarlyWeekFirstDay":
                                {
                                    prsAdminCapacity.IsEarlyWeekFirstDay = nodesInCalendarDetail.InnerText.Trim();
                                    break;
                                }

                            case "IsEarlyWeekSecondDay":
                                {
                                    prsAdminCapacity.IsEarlyWeekSecondDay = nodesInCalendarDetail.InnerText.Trim();
                                    break;
                                }

                            case "IsMidWeekFirstDay":
                                {
                                    prsAdminCapacity.IsMidWeekFirstDay = nodesInCalendarDetail.InnerText.Trim();
                                    break;
                                }

                            case "IsMidWeekSecondDay":
                                {
                                    prsAdminCapacity.IsMidWeekSecondDay = nodesInCalendarDetail.InnerText.Trim();
                                    break;
                                }

                            case "FrequencyType":
                                {
                                    prsAdminCapacity.FrequencyType = nodesInCalendarDetail.InnerText.Trim();
                                    break;
                                }

                            case "LastModifiedDate":
                                {
                                    var tempDate1 = DateTime.MinValue;
                                    if (DateTime.TryParse(nodesInCalendarDetail.InnerText, out tempDate1))
                                    {
                                        prsAdminCapacity.LastModifiedDate = tempDate1;
                                    }
                                    else
                                    {
                                        throw new ApplicationException(string.Format("Konverteringsfeil i GetPRSCalendarInfo. {0} kan ikke konverteres til Date", nodesInCalendarDetail.InnerText));
                                    }

                                    break;
                                }

                            case "WeekNo":
                                {
                                    int tempInt = int.MinValue;
                                    if (int.TryParse(nodesInCalendarDetail.InnerText, out tempInt))
                                    {
                                        prsAdminCapacity.WeekNo = tempInt;
                                    }
                                    else
                                    {
                                        throw new ApplicationException(string.Format("Konverteringsfeil i GetPRSCalendarInfo. {0} kan ikke konverteres til WeekNo", nodesInCalendarDetail.InnerText));
                                    }

                                    break;
                                }

                            default:
                                {
                                    break;
                                }
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex.Message);
                        numberOfErrorsField += 1;
                        continue;
                    }
                }

                await _pumaRestCapacityRepository.Add_PRS_Calendar_To_PumaAsync(prsAdminCapacity.Date, prsAdminCapacity.IsHoliday, prsAdminCapacity.IsEarlyWeekFirstDay, prsAdminCapacity.IsEarlyWeekSecondDay, prsAdminCapacity.IsMidWeekFirstDay, prsAdminCapacity.IsMidWeekSecondDay, prsAdminCapacity.FrequencyType, prsAdminCapacity.LastModifiedDate, prsAdminCapacity.WeekNo);
            }
        }

        #endregion

        #region XMLDocument Create

        /// <summary>
        /// 
        /// </summary>
        /// <param name="XMLFile"></param>
        /// <returns></returns>
        public static XmlDocument Create(string XMLFile)
        {
            var settings = new XmlReaderSettings();
            var reader = XmlReader.Create(XMLFile, settings);
            var document = new XmlDocument();
            document.Load(reader);
            reader.Close();
            return document;
        }

        #endregion

        #region CreateAndValidate

        /// <summary>
        /// 
        /// </summary>
        /// <param name="XMLFile"></param>
        /// <param name="XSDFile"></param>
        /// <returns></returns>
        public XmlDocument CreateAndValidate(string XMLFile, string XSDFile)
        {
            try
            {
                var eventHandler = new ValidationEventHandler(ValidationEventHandler);
                var settings = new XmlReaderSettings();
                settings.Schemas.Add(string.Empty, XSDFile);
                settings.ValidationType = ValidationType.Schema;
                var reader = XmlReader.Create(XMLFile, settings);
                var document = new XmlDocument();
                document.Load(reader);
                document.Validate(eventHandler);
                return document;
            }
            catch (XmlSchemaValidationException ex)
            {
                string error = string.Format("Dokument '{0}' kan ikke valideres. Årsak: {1}", XMLFile, ex.Message);
                throw new ApplicationException(error);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ValidationEventHandler(object sender, ValidationEventArgs e)
        {
            switch (e.Severity)
            {
                case XmlSeverityType.Error:
                    {
                        Console.WriteLine("Error: {0}", e.Message);
                        break;
                    }

                case XmlSeverityType.Warning:
                    {
                        Console.WriteLine("Warning {0}", e.Message);
                        break;
                    }
            }
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="XpathToNode"></param>
        /// <param name="newProductionDate"></param>
        public void UpdateDateTimeNodeToUTC(ref XmlDocument doc, string XpathToNode, DateTime newProductionDate)
        {
            var node = doc.SelectSingleNode(XpathToNode);
            var dateOffset = new DateTimeOffset(newProductionDate, TimeZoneInfo.Local.GetUtcOffset(newProductionDate));
            node.InnerText = dateOffset.ToString("o");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="XPathExpression"></param>
        /// <returns></returns>
        public static DateTime DateTimeXMLNodeGetValue(XmlDocument doc, string XPathExpression)
        {
            var returnValue = DateTime.MinValue;
            var node = doc.SelectSingleNode(XPathExpression);
            string productionTime = node.InnerText;
            DateTime.TryParse(node.InnerText, out returnValue);
            return returnValue;
        }
    }
}