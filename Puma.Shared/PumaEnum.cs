using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Puma.Shared
{
    public class PumaEnum
    {
        public enum ListType
        {
            Utvalg = 1,
            UtvalgList = 2,
            Unsettled = 3
        }
        [Serializable]
        public enum ReceiverType
        {
            Households = 1,
            Farmers = 2,
            Houses = 3,
            Businesses = 4,
            HouseholdsReserved = 5,
            FarmersReserved = 6,
            HousesReserved = 7
        }

        [Serializable]
        public enum OrdreStatus
        {
            Null = 0,
            R = 1,
            G = 2,
            I = 3,
            K = 4,
        }

        [Serializable]
        public enum OrdreType
        {
            Null = 0,
            O = 1,
            T = 2,
        }

        [Serializable]
        public enum Site
        {
            NotSet,
            InternWeb,
            KundeWebLoggedIn,
            KundeWebNotLoggedIn
        }

        public enum NodeType
        {
            Fylke = 1,
            Kommune = 2,
            Team = 3,
            Rute = 4,
            Root = 5,
            Unsettled = 6
        }

        public enum NodeTypeNewReolTree
        {
            SuperRoot = 1,
            Fylke = 2,
            Kommune = 3,
            Team = 4,
            Budrute = 5,
            Unsettled = 6
        }

        public enum NodeCheckStatus
        {
            NodeIsChecked = 1,
            NodeAndChildNodesIsNotChecked = 2,
            SomeChildNodesAreChecked = 3,
            Unsettled = 4
        }

        [Serializable()]
        public enum CriteriaType
        {
            SelectedInMap = 1,
            Segment = 2,
            Fylke = 3,
            Kommune = 4,
            Team = 5,
            PostalZone = 6,
            Postboks = 7,
            Drivetime = 8,
            Drivedistance = 9,
            Drivethreshold = 10,
            SimpleThreshold = 11,
            Demography = 12,
            FromAddressPoints = 13,
            FullDistrKommune = 14,
            FullDistrPostalZone = 15,
            FullDistrDistrict = 16,
            BAAnalysis = 17,
            DataConversion = 18,
            GeografiPlukkliste = 19,
            GeografiReol = 20,
            Dummy1 = 21 // lagt til i tilfelle videre utvidelse her, uten å måtte endre versjon av Entities for Integrasjonen
 ,
            Dummy2 = 22,
            Dummy3 = 23
        }

        [Serializable]
        public enum UtvalgType
        {
            Utvalg = 1,
            UtvalgList = 2,
            Unknown = 3
        }
        [Serializable]
        public enum DistributionType
        {
            Null=0,
            S=1,
            B=2
        }

        public enum SearchMethod
        {
            StartsWithIgnoreCase = 0,
            ContainsIgnoreCase = 1,
            EqualsIgnoreCase = 2
        }

        public enum NotFoundAction
        {
            ThrowException = 0,
            ReturnNothing = 1
        }

        /// <summary>
        /// Beware of KSPU entity har its own value-order. For KSPU-entities mapping, use function
        /// </summary>
        public enum UtvalgsTypeKode
        {
            Liste,
            Utvalg
        }

        public enum FordelingsTypeKode
        {
            Null,
            Normal,
            KommuneBydel,
            Informasjon
        }

        /// <summary>
        /// Beware of KSPU entity har its own value-order. For KSPU-entities mapping, use EntityMapping
        /// </summary>
        public enum MottakerGruppeKode
        {
            Boligblokker,
            EneboligerOgRekkehus,
            Gardbrukere,
            Husholdninger,
            Virksomheter
        }

        public enum FeilKode
        {
            UtvalgEksistererIkke = 100,
            UtvalgKnyttetTilAnnetTilbudOrdre = 200,
            UtvalgIkkeKnyttetTilOpgittKunde = 300
        }

        /// <summary>
        /// Beware of KSPU entity har its own value-order. For KSPU-entities mapping, use EntityMapping
        /// </summary>
        public enum OEBSTypeKode
        {
            Tilbud,
            Ordre
        }

        /// <summary>
        /// Beware of KSPU entity har its own value-order. For KSPU-entities mapping, use EntityMapping
        /// </summary>
        public enum OrdreTilbudStatusKode
        {
            Registrert,
            Godkjent,
            Innlevert,
            Kansellert
        }

        /// <summary>
        /// Same as KSPU-entities, but use EntityMapping
        /// </summary>
        public enum Distribusjonstype
        {
            Null,
            S,
            B
        }

        /// <summary>
        /// Exposes different types of logfiles
        /// </summary>
        public enum TypeOfObject
        {
            RequestFromClient,
            ResponseToClient,
            ResponseFromGD,
            ResponseToQueue
        }

        [Serializable]
        public enum SelectionCriteria
        {
            Date = 0,
            DateTime = 1,
            CustomxText = 2,
        }

        public enum MessageType
        {
            InfoMessage,
            ErrorMessage,
            ApplicationExceptionMessage,
            ExceptionMessage,
            TimeDurationMessage,
            MessageFromQueue,
            FailedToQueueMessageToSB
        }

        public enum ReturnCodes : int
        {
            ErrorApplicationKilled = 1,
            NoError = 0,
            WarningTimeOut = -2,
            Warning = -1,
            TimeOut = -100,
            ErrorLicenseInUse = 101,
            ErrorAllReadyRectrated = 102,
            ErrorFailedSearchingUtvalg = 103,
            ErrorConnectingToDB = 104,
            ErrorFailedSaveUtvalg = 105,
            ErrorFailedCreateOldGeometry = 106,
            ErrorFailedToRecrateByGeography = 107,
            ErrorOverlayOldGeometryFailed = 108,
            ErrorOldReolMapMissing = 109,
            ErrorOldUtvalgGeometryMissing = 110,
            ErrorUtvalgLockedOrRecreated = 111,
            ErrorProcessToLongInQue = 112,
            ErrorUnknownError = 200
        }

        public enum RestCapacityMessageType
        {
            InfoMessage,
            ErrorMessage,
            ApplicationExceptionMessage,
            ExceptionMessage,
            TimeDurationMessage
        }
    }
}
