using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Puma.DataLayer.BusinessEntity.Report;
using Microsoft.Extensions.Logging;
using System.IO;
using Puma.Infrastructure.Interface.KsupDB;
using System.Dynamic;
using Puma.DataLayer.BusinessEntity;
using System.Net;

namespace Puma.Infrastructure.Repository.KspuDB
{
    /// <summary>
    /// Repository for Map operation
    /// </summary>
    public class MapOperationRepository : IMapOperationRepository
    {

        /// <summary>
        /// The configuration
        /// </summary>
        private readonly IConfiguration config;


        /// <summary>
        /// Http Client Factory
        /// </summary>
        private readonly IHttpClientFactory httpClientFactory;
        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger<MapOperationRepository> _logger;

        /// <summary>
        /// App setting repository
        /// </summary>
        private readonly IAppSettingRepository _appSettingRepository;

        public MapOperationRepository(IConfiguration config, IHttpClientFactory httpClientFactory, ILogger<MapOperationRepository> logger, IAppSettingRepository appSettingRepository)
        {
            this.config = config ?? throw new ArgumentNullException(nameof(config));
            this.httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _appSettingRepository = appSettingRepository ?? throw new ArgumentNullException(nameof(appSettingRepository));
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
        public async Task<string> ExportMapImage(List<long> RouteIds, bool isCustomerWeb, string strDayDetails, string selectedAddress = "")
        {
            //https://dev.pumamapservices.bring.no/arcgis/rest/services
            Uri _budruteLayer = new Uri(config.GetSection("MapQuery_API_URL").Value + "5/query?f=json&returnExtentOnly=true&returnCountOnly=true&spatialRel=esriSpatialRelIntersects&where=reol_id in");
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

            #region Commented code for address point
            //List<SelectedAddress> lstselectedAddress = new List<SelectedAddress>();
            //if (!string.IsNullOrWhiteSpace(selectedAddress))
            //{
            //    lstselectedAddress = JsonConvert.DeserializeObject<List<SelectedAddress>>(selectedAddress);

            //}
            //List<FeaturePoint> featuresPoint = new List<FeaturePoint>();

            //List<FeatureText> featuresText = new List<FeatureText>();

            //if (lstselectedAddress?.Any() == true && lstselectedAddress.Count > 0)
            //{
            //    foreach (var addrPoint in lstselectedAddress)
            //    {
            //        featuresPoint.Add(new FeaturePoint()
            //        {
            //            geometry = new Geometry()
            //            {
            //                x = addrPoint.geometry != null ? addrPoint.geometry.x : addrPoint.location.x,
            //                y = addrPoint.geometry != null ? addrPoint.geometry.y : addrPoint.location.y,
            //                spatialReference = new SpatialReference()
            //                {
            //                    wkid = 32633
            //                }
            //            }
            //        });

            //        featuresText.Add(new FeatureText()
            //        {
            //            geometry = new Geometry()
            //            {
            //                x = addrPoint.geometry != null ? addrPoint.geometry.x : addrPoint.location.x,
            //                y = addrPoint.geometry != null ? addrPoint.geometry.y : addrPoint.location.y,
            //                spatialReference = new SpatialReference()
            //                {
            //                    wkid = 32633
            //                }
            //            },
            //            symbol = new Symbol()
            //            {
            //                color = new List<int>() { 0,0,0,255
            //           },
            //                xoffset = 3,
            //                yoffset = 3,
            //                type = "esriTS",
            //                horizontalAlignment = "center",
            //                verticalAlignment = "bottom",
            //                text = !string.IsNullOrWhiteSpace(addrPoint.attributes.display) ? addrPoint.attributes.display : addrPoint.attributes.Match_addr,
            //                font = new Font()
            //                {
            //                    family = "Arial",
            //                    decoration = "none",
            //                    size = 10,
            //                    style = "normal",
            //                    weight = "bold",
            //                }
            //            }
            //        });

            //    }
            //} 
            #endregion

            string cmd = $" reol_id in ({routeIdsList}" + ")" + (isCustomerWeb ? " AND UPPER(REOLTYPE) <> UPPER('Boks')" : "");

            #region Code for dynamic layer
            //string dynamicLayer = "{ \"id\":5, \"layerDefinition\": { \"source\":{ \"type\":\"mapLayer\",\"mapLayerId\":5},\"definitionExpression\":\"" + cmd + "\",\"drawingInfo\":{ \"showLabels\":true, \"renderer\": {\"type\": \"simple\",\"symbol\": {\"type\": \"esriSFS\",\"style\": \"esriSFSSolid\",\"color\": [237, 54, 21, 70],\"outline\": {\"type\": \"esriSLS\",\"color\": [237, 54, 21, 255],\"width\": 0.75,\"style\": \"esriSLSSolid\"}}}} } }" +
            //    ",{ \"id\":6,\"layerDefinition\": { \"source\":{ \"type\":\"mapLayer\",\"mapLayerId\":7},\"drawingInfo\":{ \"showLabels\":true} } }" +
            //    ",{ \"id\":7,\"layerDefinition\": { \"source\":{ \"type\":\"mapLayer\",\"mapLayerId\":7},\"drawingInfo\":{ \"showLabels\":true} } }" +
            //            ",{ \"id\":10,\"layerDefinition\": { \"source\":{ \"type\":\"mapLayer\",\"mapLayerId\":10},\"drawingInfo\":{ \"showLabels\":true} } }" +
            //            ",{ \"id\":11,\"layerDefinition\": { \"source\":{ \"type\":\"mapLayer\",\"mapLayerId\":11},\"drawingInfo\":{ \"showLabels\":true} } }" +
            //            ",{ \"id\":12,\"layerDefinition\": { \"source\":{ \"type\":\"mapLayer\",\"mapLayerId\":12},\"drawingInfo\":{ \"showLabels\":true} } }" +
            //            ",{ \"id\":17,\"layerDefinition\": { \"source\":{ \"type\":\"mapLayer\",\"mapLayerId\":17},\"drawingInfo\":{ \"showLabels\":true} } }" +
            //            ",{ \"id\":18,\"layerDefinition\": { \"source\":{ \"type\":\"mapLayer\",\"mapLayerId\":18},\"drawingInfo\":{ \"showLabels\":true} } }" +
            //            ",{ \"id\":20,\"layerDefinition\": { \"source\":{ \"type\":\"mapLayer\",\"mapLayerId\":20},\"drawingInfo\":{ \"showLabels\":true} } }" +
            //            ",{ \"id\":21,\"layerDefinition\": { \"source\":{ \"type\":\"mapLayer\",\"mapLayerId\":21},\"drawingInfo\":{ \"showLabels\":true} } }" +
            //            ",{ \"id\":27,\"layerDefinition\": { \"source\":{ \"type\":\"mapLayer\",\"mapLayerId\":27},\"drawingInfo\":{ \"showLabels\":true} } }" +
            //            ",{ \"id\":28,\"layerDefinition\": { \"source\":{ \"type\":\"mapLayer\",\"mapLayerId\":28},\"drawingInfo\":{ \"showLabels\":true} } }" +
            //            ",{ \"id\":29,\"layerDefinition\": { \"source\":{ \"type\":\"mapLayer\",\"mapLayerId\":29},\"drawingInfo\":{ \"showLabels\":true} } }" +
            //            ",{ \"id\":31,\"layerDefinition\": { \"source\":{ \"type\":\"mapLayer\",\"mapLayerId\":31},\"drawingInfo\":{ \"showLabels\":true} } }" +
            //            ",{ \"id\":32,\"layerDefinition\": { \"source\":{ \"type\":\"mapLayer\",\"mapLayerId\":32},\"drawingInfo\":{ \"showLabels\":true} } }" +
            //            ",{ \"id\":35,\"layerDefinition\": { \"source\":{ \"type\":\"mapLayer\",\"mapLayerId\":35},\"drawingInfo\":{ \"showLabels\":true} } }" +
            //            ",{ \"id\":38,\"layerDefinition\": { \"source\":{ \"type\":\"mapLayer\",\"mapLayerId\":38},\"drawingInfo\":{ \"showLabels\":true} } }" +
            //            ",{ \"id\":39,\"layerDefinition\": { \"source\":{ \"type\":\"mapLayer\",\"mapLayerId\":39},\"drawingInfo\":{ \"showLabels\":true} } }" +
            //            ",{ \"id\":42,\"layerDefinition\": { \"source\":{ \"type\":\"mapLayer\",\"mapLayerId\":42},\"drawingInfo\":{ \"showLabels\":true} } }" +
            //            ",{ \"id\":46,\"layerDefinition\": { \"source\":{ \"type\":\"mapLayer\",\"mapLayerId\":46},\"drawingInfo\":{ \"showLabels\":true} } }" +
            //            ",{ \"id\":53,\"layerDefinition\": { \"source\":{ \"type\":\"mapLayer\",\"mapLayerId\":53},\"drawingInfo\":{ \"showLabels\":true} } }" +
            //            ",{ \"id\":55,\"layerDefinition\": { \"source\":{ \"type\":\"mapLayer\",\"mapLayerId\":55},\"drawingInfo\":{ \"showLabels\":true} } }" +
            //            ",{ \"id\":58,\"layerDefinition\": { \"source\":{ \"type\":\"mapLayer\",\"mapLayerId\":58},\"drawingInfo\":{ \"showLabels\":true} } }" +
            //            ",{ \"id\":60,\"layerDefinition\": { \"source\":{ \"type\":\"mapLayer\",\"mapLayerId\":60},\"drawingInfo\":{ \"showLabels\":true} } }" +
            //            ",{ \"id\":62,\"layerDefinition\": { \"source\":{ \"type\":\"mapLayer\",\"mapLayerId\":62},\"drawingInfo\":{ \"showLabels\":true} } }" +
            //            ",{ \"id\":65,\"layerDefinition\": { \"source\":{ \"type\":\"mapLayer\",\"mapLayerId\":65},\"drawingInfo\":{ \"showLabels\":true} } }" +
            //            ",{ \"id\":67,\"layerDefinition\": { \"source\":{ \"type\":\"mapLayer\",\"mapLayerId\":67},\"drawingInfo\":{ \"showLabels\":true} } }" +
            //            ",{ \"id\":69,\"layerDefinition\": { \"source\":{ \"type\":\"mapLayer\",\"mapLayerId\":69},\"drawingInfo\":{ \"showLabels\":true} } }"; 
            #endregion


            //Generate Token
            string token = await GenerateToken();

            try
            {

                string resultExtent = await GetExtent(token, cmd);


                //if (adrlayerId != -1)
                //{
                //    dynamicLayer = "{ \"id\":" + adrlayerId + ",\"layerDefinition\":{ \"source\":{ \"type\":\"mapLayer\",\"mapLayerId\": " + adrlayerId + " },\"definitionExpression\":\"" + cmd + "\",\"drawingInfo\":{ \"showLabels\":true} } }," + dynamicLayer;

                //}


                dynamic layerDefinition = new ExpandoObject();

                var dictionary = (IDictionary<string, object>)layerDefinition;
                dictionary.Add("5", cmd);
                if (adrlayerId != -1)
                {
                    dictionary.Add(adrlayerId.ToString(), cmd);
                }

                string strLayerDefinition = JsonConvert.SerializeObject(layerDefinition);


                string includeLayer = "5";
                if (adrlayerId != -1)
                {
                    includeLayer += "," + adrlayerId;
                }
                Dictionary<string, string> map = new Dictionary<string, string>();
                map.Add("bbox", resultExtent);
                map.Add("layers", "include:" + includeLayer + "");
                map.Add("layerDefs", strLayerDefinition);
                map.Add("size", "550,400");
                map.Add("format", "png8");
                map.Add("dpi", "100");
                map.Add("f", "pjson");
                map.Add("bboxSR", "");
                map.Add("imageSR", "");
                map.Add("historicMoment", "");
                map.Add("transparent", "false");
                map.Add("time", "");
                map.Add("layerTimeOptions", "");
                map.Add("dynamicLayers", "");
                map.Add("gdbVersion", "");
                map.Add("mapScale", "");
                map.Add("rotation", "");
                map.Add("datumTransformations", "");
                map.Add("layerParameterValues", "");
                map.Add("mapRangeValues", "");
                map.Add("layerRangeValues", "");
                map.Add("clipping", "");
                map.Add("spatialFilter", "");

                var serviceBodyJson = new FormUrlEncodedContent(map);
                serviceBodyJson.Headers.Clear();
                serviceBodyJson.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
                
                var client = httpClientFactory.CreateClient();
                client.Timeout = Timeout.InfiniteTimeSpan;
                client.DefaultRequestHeaders.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9");
                client.DefaultRequestHeaders.Add("Accept-Language", "en-US,en;q=0.9");
                client.DefaultRequestHeaders.Add("Cache-Control", "max-age=0");
                client.DefaultRequestHeaders.Add("Connection", "keep-alive");
                client.DefaultRequestHeaders.Add("Origin", config.GetSection("MapTokenGenerateURL").Value);
                client.DefaultRequestHeaders.Add("Referer",config.GetSection("MapTokenGenerateURL").Value + "arcgis/rest/services/KSPU/MapServer/export");
                client.DefaultRequestHeaders.Add("Sec-Fetch-Dest", "document");
                client.DefaultRequestHeaders.Add("Sec-Fetch-Mode", "navigate");
                client.DefaultRequestHeaders.Add("Sec-Fetch-Site", "same-origin");
                client.DefaultRequestHeaders.Add("Sec-Fetch-User", "?1");
                client.DefaultRequestHeaders.Add("Upgrade-Insecure-Requests", "1");
                client.DefaultRequestHeaders.Add("sec-ch-ua", "\"Not_A Brand\";v=\"99\", \"Google Chrome\";v=\"109\", \"Chromium\";v=\"109\"");
                client.DefaultRequestHeaders.Add("sec-ch-ua-mobile", "?0");
                client.DefaultRequestHeaders.Add("sec-ch-ua-platform", "\"Windows\"");
                //using var httpResponseMessage = await client.PostAsync(config.GetSection("MapPrint_API_URL").Value + "Export%20Web%20Map%20Task/execute?token=" + token, serviceBodyJson);
                System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                ServicePointManager.MaxServicePointIdleTime = 15000;
                using var httpResponseMessage = await client.PostAsync(config.GetSection("MapQuery_API_URL").Value + "export?token=" + token, serviceBodyJson);

                if (httpResponseMessage.IsSuccessStatusCode)
                {

                    //Note : For testing use this code..
                    var ImageData = JsonConvert.DeserializeObject<MapImage>(httpResponseMessage.Content.ReadAsStringAsync().Result);


                    if (!string.IsNullOrWhiteSpace(ImageData?.href))
                    {
                        string ImageUrl = ImageData?.href;
                        ImageUrl = ImageUrl.Replace("http://", "https://");
                        var bytes = await client.GetByteArrayAsync(ImageUrl);
                        return Convert.ToBase64String(bytes);
                    }
                    else
                    {
                        _logger.LogDebug("Response from Map service" + httpResponseMessage.Content.ReadAsStringAsync().Result);

                    }

                    return httpResponseMessage.Content.ReadAsStringAsync().Result;

                }
                else
                {
                    throw new HttpRequestException("Error occured while pulling Map Image from Server " + httpResponseMessage.StatusCode + " " + httpResponseMessage.Content.ReadAsStringAsync().Result);
                }
            }
            catch (Exception e)
            {
                _logger.LogError("Error Occured" + e.Message);
                throw;
            }

        }


        public async Task<string> GetExtent(string token, string condition)
        {
            try
            {
                var client = httpClientFactory.CreateClient();
                client.Timeout = TimeSpan.FromMinutes(30);

                Dictionary<string, string> mapQuery = new Dictionary<string, string>();
                mapQuery.Add("Where", condition);
                mapQuery.Add("geometryType", "esriGeometryEnvelope");
                mapQuery.Add("spatialRel", "esriSpatialRelIntersects");
                mapQuery.Add("returnExtentOnly", "true");
                mapQuery.Add("returnGeometry", "false");
                mapQuery.Add("f", "pjson");

                client.DefaultRequestHeaders.Referrer = new Uri(config.GetSection("MapTokenGenerateURL").Value);

                var serviceBodyJsonQuery = new FormUrlEncodedContent(mapQuery);
                serviceBodyJsonQuery.Headers.Clear();
                serviceBodyJsonQuery.Headers.Add("Content-Type", "application/x-www-form-urlencoded");

                using var httpResponseMessageQ = await client.PostAsync(config.GetSection("MapQuery_API_URL").Value + "5/query?token=" + token, serviceBodyJsonQuery);

                string resultExtent = "";

                if (httpResponseMessageQ.IsSuccessStatusCode)
                {
                    var Obj = JsonConvert.DeserializeObject<MapExtent>(httpResponseMessageQ.Content.ReadAsStringAsync().Result);

                    resultExtent = JsonConvert.SerializeObject(Obj.extent);
                }
                else
                {
                    throw new HttpRequestException("Error Occured while calling Query URI " + httpResponseMessageQ.StatusCode + " " + httpResponseMessageQ.Content.ReadAsStringAsync().Result);
                }
                return resultExtent = resultExtent.Replace("\n", "");
            }
            catch (Exception ex)
            {
                _logger.LogError("Error Occured" + ex.Message);
                throw;
            }


        }

        public async Task<string> GenerateToken()
        {
            //AppSettings appsettings = new AppSettings();
            HttpRequestMessage msg = new HttpRequestMessage(HttpMethod.Post, config.GetSection("MapTokenGenerateURL").Value) { Content = TokenRequestContent() };
            using HttpClient httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Referrer = new Uri(config.GetSection("MapTokenGenerateURL").Value);

            var t = await httpClient.SendAsync(msg);
            var responseToken = await t.Content.ReadAsStringAsync();
            if (!string.IsNullOrWhiteSpace(responseToken))
            {
                GenerateToken generateToken = new GenerateToken();
                generateToken = JsonConvert.DeserializeObject<GenerateToken>(responseToken);
                return generateToken.token;
            }
            return string.Empty;

            FormUrlEncodedContent TokenRequestContent()
            {
                var parameters = new Dictionary<string, string>
                {
                    {"username", _appSettingRepository.GetAppSettingValue(AppSetting.ArcGisAdminUserName,true).Result},
                    {"password", _appSettingRepository.GetAppSettingValue(AppSetting.ArcGisAdminPassword,true).Result},
                    {"client", "referer" },
                    {"referer", config.GetSection("MapTokenGenerateURL").Value },
                    {"expiration", "1440" },
                    {"f", "pjson" },
                };

                var content = new FormUrlEncodedContent(parameters);
                return content;
            }
        }
    }
}
