using System.Collections.Generic;

namespace DataAccessAPI.HandleRequest.Response.Calendar
{
    public class ResponseGetRuteinfo
    {
        private string m_ErrorMessage;

        public string ErrorMessage
        {
            get
            {
                return m_ErrorMessage;
            }

            set
            {
                m_ErrorMessage = value;
            }
        }

        private List<RuteInfo> m_RuteInfo;

        public List<RuteInfo> RuteInfo
        {
            get
            {
                return m_RuteInfo;
            }

            set
            {
                m_RuteInfo = value;
            }
        }

        private long m_TotaltAntallBudruter;

        public long TotaltAntallBudruter
        {
            get
            {
                return m_TotaltAntallBudruter;
            }

            set
            {
                m_TotaltAntallBudruter = value;
            }
        }

        private long m_TotaltAntallMottakere;

        public long TotaltAntallMottakere
        {
            get
            {
                return m_TotaltAntallMottakere;
            }

            set
            {
                m_TotaltAntallMottakere = value;
            }
        }

        private List<ListInfo> m_RuterIListe;

        public List<ListInfo> RuterIListe
        {
            get
            {
                return m_RuterIListe;
            }

            set
            {
                m_RuterIListe = value;
            }
        }
    }

    public class ListInfo
    {
        private long m_ListeId;

        public long ListeId
        {
            get
            {
                return m_ListeId;
            }

            set
            {
                m_ListeId = value;
            }
        }

        private string m_ListeNavn;

        public string ListeNavn
        {
            get
            {
                return m_ListeNavn;
            }

            set
            {
                m_ListeNavn = value;
            }
        }

        private long m_UtvalgId;

        public long UtvalgId
        {
            get
            {
                return m_UtvalgId;
            }

            set
            {
                m_UtvalgId = value;
            }
        }

        private string m_utvalgNavn;

        public string UtvalgNavn
        {
            get
            {
                return m_utvalgNavn;
            }

            set
            {
                m_utvalgNavn = value;
            }
        }

        private List<long> m_RuteId;

        public List<long> RuteId
        {
            get
            {
                return m_RuteId;
            }

            set
            {
                m_RuteId = value;
            }
        }
    }

    public class RuteInfo
    {
        private long m_RuteId;

        public long RuteId
        {
            get
            {
                return m_RuteId;
            }

            set
            {
                m_RuteId = value;
            }
        }

        private string m_RuteNavn;

        public string RuteNavn
        {
            get
            {
                return m_RuteNavn;
            }

            set
            {
                m_RuteNavn = value;
            }
        }

        private long m_RuteAntallMotakere;

        public long RuteAntallMotakere
        {
            get
            {
                return m_RuteAntallMotakere;
            }

            set
            {
                m_RuteAntallMotakere = value;
            }
        }

        private string m_Fylke;

        public string Fylke
        {
            get
            {
                return m_Fylke;
            }

            set
            {
                m_Fylke = value;
            }
        }

        private string m_Kommune;

        public string Kommune
        {
            get
            {
                return m_Kommune;
            }

            set
            {
                m_Kommune = value;
            }
        }

        private string m_Team;

        public string Team
        {
            get
            {
                return m_Team;
            }

            set
            {
                m_Team = value;
            }
        }

        public int? RestVekt { get; set; }

        public int? RestAntall { get; set; }
       

        
        public double? RestThickness { get; set; }
    }
}