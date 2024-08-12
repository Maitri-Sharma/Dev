using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataAccessAPI.HandleRequest.Response.Calendar
{
    public class ResponseGetRestcapacity
    {
        private List<RestCapacity> m_Kapasitet;
        public List<RestCapacity> Kapasitet
        {
            get
            {
                return m_Kapasitet;
            }
            set
            {
                m_Kapasitet = value;
            }
        }

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
    }

    public class RestCapacity
    {
        private DateTime m_Dato;
        public DateTime Dato
        {
            get
            {
                return m_Dato;
            }
            set
            {
                m_Dato = value;
            }
        }

        private int m_UkeNr;
        public int U
        {
            get
            {
                return m_UkeNr;
            }
            set
            {
                m_UkeNr = value;
            }
        }

        private bool m_Distribusjonsdag;
        public bool DD
        {
            get
            {
                return m_Distribusjonsdag;
            }
            set
            {
                m_Distribusjonsdag = value;
            }
        }

        private bool m_Virkedag;
        public bool VD
        {
            get
            {
                return m_Virkedag;
            }
            set
            {
                m_Virkedag = value;
            }
        }

        private bool m_Kapasitet;
        public bool K
        {
            get
            {
                return m_Kapasitet;
            }
            set
            {
                m_Kapasitet = value;
            }
        }

        private List<List<long>> m_MK;
        public List<List<long>> MK
        {
            get
            {
                return m_MK;
            }
            set
            {
                m_MK = value;
            }
        }
        private bool m_selectable;
        public bool IsSelectable
        {
            get
            {
                return m_selectable;
            }
            set
            {
                m_selectable = value;
            }
        }

        public bool IsFullyBokked = false;
    }

    public class RuterLackingCapacityForPeriod
    {
        private long m_reolId;
        public long ReolId
        {
            get
            {
                return m_reolId;
            }
            set
            {
                m_reolId = value;
            }
        }

        private double m_thickness;
        public double Thickness
        {
            get
            {
                return m_thickness;
            }
            set
            {
                m_thickness = value;
            }
        }
        private int m_Vekt;
        public int Vekt
        {
            get
            {
                return m_Vekt;
            }
            set
            {
                m_Vekt = value;
            }
        }

    }
}
