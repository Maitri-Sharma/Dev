using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Puma.Shared
{
    public class KapasitetDato
    {
        private bool p_isEarlyWeekFirstDay;
        private bool p_isEarlyWeekSecondDay;
        private bool p_isMidWeekFirstDay;
        private bool p_isMidWeekSecondDay;
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
        public bool IsEarlyWeekFirstDay
        {
            get
            {
                return this.p_isEarlyWeekFirstDay;
            }
            set
            {
                this.p_isEarlyWeekFirstDay = value;
            }
        }

        public bool IsEarlyWeekSecondDay
        {
            get
            {
                return this.p_isEarlyWeekSecondDay;
            }
            set
            {
                this.p_isEarlyWeekSecondDay = value;
            }
        }

        public bool IsMidWeekFirstDay
        {
            get
            {
                return this.p_isMidWeekFirstDay;
            }
            set
            {
                this.p_isMidWeekFirstDay = value;
            }
        }

        public bool IsMidWeekSecondDay
        {
            get
            {
                return this.p_isMidWeekSecondDay;
            }
            set
            {
                this.p_isMidWeekSecondDay = value;
            }
        }
    }
}
