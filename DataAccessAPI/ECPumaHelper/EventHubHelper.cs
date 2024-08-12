using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Int = Puma.Shared;
using Ext = DataAccessAPI.Models;
using static Puma.Shared.PumaEnum;
//using Microsoft.ServiceBus.Messaging;
using System.Collections.Concurrent;
using Microsoft.Azure.EventHubs;
using System.Text;
using System.Threading;
using Microsoft.Extensions.Configuration;
//using Microsoft.AspNetCore.Mvc.Diagnostics;
//using Microsoft.ServiceBus.Messaging;

namespace DataAccessAPI.ECPumaHelper
{
    public interface IEventHubHelper
    {
        Task<bool> SendMessagetoEventHubAsync(string messageId, string message, string eventHubName);
    }

    public class EventHubHelper: IEventHubHelper
    {
        #region Variables
        private readonly ILogger<EventHubHelper> _logger;
        #endregion

        #region Constructors
        /// <summary>
        /// Paramaterized Constructor
        /// </summary>
        /// <param name="logger">Instance of Microsoft.Extensions.Logging</param>
        public EventHubHelper(ILogger<EventHubHelper> logger)
        {
            _logger = logger;
        }
        #endregion

        #region Public Methods

        public async Task<bool> SendMessagetoEventHubAsync(string messageId, string message, string eventHubName)
        {
            var isRetryAttempt = false;
            int retryCount = 1;
            while (retryCount < 6)
            {
                try
                {
                    var eventHubClient = EventHubClientFactory.GetEventHubClient(Startup.StaticConfiguration.GetConnectionString(""), eventHubName, isRetryAttempt);

                    await eventHubClient.SendAsync(new EventData(Encoding.UTF8.GetBytes(message)));
                    return true;
                }
                catch (Exception e)
                {
                    _logger.LogError(e,
                            "ErrorCode: ", Int.PumaEnum.MessageType.FailedToQueueMessageToSB,
                            "MessageId: ", messageId,
                            "EventHubErrorMessage: ", e.ToString(),
                            "Event Hub Name: ", eventHubName,
                            "Attempt: ", retryCount,
                            "MessageLength: ", message.Length);
                    try
                    {
                        Thread.Sleep(new Random().Next(50, 200));
                    }
                    catch { }
                    isRetryAttempt = true;
                    retryCount++;
                }
            }
            return false;
        }

        public void SendDataOverEventHub(string application, Int.Utvalgsfordeling fordeling, string reason, string correlationID)
        {
            //string sendData = System.IO.File.ReadAllText(file);
            //var dataSize = System.Text.ASCIIEncoding.Unicode.GetByteCount(sendData);
            //return sendData;
            if (fordeling == null)
            {
                throw new ArgumentNullException("fordeling");
            }

            if (string.IsNullOrEmpty(reason))
            {
                throw new ArgumentNullException("reason");
            }

            if (string.IsNullOrEmpty(correlationID))
            {
                correlationID = string.Empty;
            }

            Ext.Utvalgsfordeling utvalgsfordeling = TranslateInternalToExternal.Utvalgsfordeling(fordeling);
            utvalgsfordeling.Reason = reason;

            _logger.LogInformation(application, TypeOfObject.ResponseToQueue, utvalgsfordeling);

            string message = Serialize.ToUTF8String(utvalgsfordeling);

            //mq.QueueMessagePut(message);

            //if (!mq.IsMessagePutSuccessful)
            //{
            //    message = string.Format("Exception i OppdaterOrdreStatus:SendOverMQTransport. Melding kan ikke lagres. Antall forsøk utført: {0}", mq.NumberOfRetriesWrite.ToString());
            //    throw new ApplicationException(message);
            //};
        }


        #endregion
    }

    public sealed class EventHubClientFactory
    {
        #region Variables
        private static readonly ConcurrentDictionary<string, EventHubClient> EventHubClientPool = new ConcurrentDictionary<string, EventHubClient>();
        #endregion

        #region public methods
        public static EventHubClient GetEventHubClient(string connString, string eventHubName, bool createFreshInstance = false)
        {
            if (createFreshInstance)
            {
                try
                {
                    EventHubClientPool.TryRemove(eventHubName, out var client);
                    if (client != null)
                    {
                        client.Close();
                    }
                }
                catch { }
            }
            return EventHubClientPool.GetOrAdd(eventHubName, ehName =>
            {
                var connectionStringBuilder = new EventHubsConnectionStringBuilder(connString)
                {
                    EntityPath = ehName
                };
                return EventHubClient.CreateFromConnectionString(connectionStringBuilder.ToString());
            });
        }

        //public EventHubClient GetEventHubClient1(string connString, string eventHubName)
        //{
        //    while (true)
        //    {


        //        if (eventHubName == eHubHeartBeat)
        //        {
        //            eventHubClient = HeartBeatEHubClient.GetEventHubClient(connString);
        //            break;
        //        }

        //        if (eventHubName == eHubOperationListAck)
        //        {
        //            eventHubClient = OpListAckEHubClient.GetEventHubClient(connString);
        //            break;
        //        }

        //        if (eventHubName == eHubWorkStaus)
        //        {
        //            eventHubClient = WorkStatusEHubClient.GetEventHubClient(connString);
        //            break;
        //        }

        //        if (eventHubName == eHubDriverAvailability)
        //        {
        //            eventHubClient = DriverAvailabilityEHubClient.GetEventHubClient(connString);
        //            break;
        //        }

        //        if (eventHubName == eHubOperationListResponse)
        //        {
        //            eventHubClient = OpListResponseEHubClient.GetEventHubClient(connString);
        //            break;
        //        }

        //        if (eventHubName == eHubOperationListEvent)
        //        {
        //            eventHubClient = OperationListEHubClient.GetEventHubClient(connString);
        //            break;
        //        }

        //        if (eventHubName == eHubMessageFromDriver)
        //        {
        //            eventHubClient = MessageFromDriverEHubClient.GetEventHubClient(connString);
        //            break;
        //        }
        //        if (eventHubName == eHubTripEvents)
        //        {
        //            eventHubClient = TripEventsEHubClient.GetEventHubClient(connString);
        //            break;
        //        }
        //        if (eventHubName == eHubUserProf)
        //        {
        //            eventHubClient = UserProfSyncEHubClient.GetEventHubClient(connString);
        //            break;
        //        }
        //        if (eventHubName == eHubAvailableCapacity)
        //        {
        //            eventHubClient = AvailableCapacityEHubClient.GetEventHubClient(connString);
        //            break;
        //        }
        //        if (eventHubName == eHubDistributionActuals)
        //        {
        //            eventHubClient = DistributionActualsEHubClient.GetEventHubClient(connString);
        //            break;
        //        }
        //        if (eventHubName == eHubPH)
        //        {
        //            eventHubClient = PHEHubClient.GetEventHubClient(connString, eventHubName);
        //            break;
        //        }

        //        break;

        //    }
        //    return eventHubClient;
        //}

        #endregion
    }
}
