using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Puma.Shared
{
    public class KapasitetData
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
        public int UkeNr
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
        public bool Distribusjonsdag
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
        public bool Virkedag
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
    }
}
