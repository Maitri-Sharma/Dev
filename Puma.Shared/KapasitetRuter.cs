using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Puma.Shared
{
    public class KapasitetRuter
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

        private long m_RuteNr;
        public long RuteNr
        {
            get
            {
                return m_RuteNr;
            }
            set
            {
                m_RuteNr = value;
            }
        }

        private int m_RestVekt;
        public int RestVekt
        {
            get
            {
                return m_RestVekt;
            }
            set
            {
                m_RestVekt = value;
            }
        }

        private int m_RestAntall;
        public int RestAntall
        {
            get
            {
                return m_RestAntall;
            }
            set
            {
                m_RestAntall = value;
            }
        }

        private string m_MottakerType;
        public string MottakerType
        {
            get
            {
                return m_MottakerType;
            }
            set
            {
                m_MottakerType = value;
            }
        }
        private double m_RestThickness;
        public double RestThickness
        {
            get
            {
                return m_RestThickness;
            }
            set
            {
                m_RestThickness = value;
            }
        }
    }

    public class LackingCapacity
    {
        private DateTime _Date;
        public DateTime Dato
        {
            get
            {
                return _Date;
            }
            set
            {
                _Date = value;
            }
        }

        public long HouseholdsLackingCapacity
        {
            get
            {
                return _HouseholdsLackingCapacity;
            }
            set
            {
                _HouseholdsLackingCapacity = value;
            }
        }
        private long _HouseholdsLackingCapacity;
    }
}
