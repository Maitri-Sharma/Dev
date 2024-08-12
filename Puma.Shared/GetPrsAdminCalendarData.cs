using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Puma.Shared
{
    public class GetPrsAdminCalendarData
    {
        private DateTime p_date;
        private bool p_isHoliday;
        private bool p_isEarlyWeekFirstDay;
        private bool p_isEarlyWeekSecondDay;
        private bool p_isMidWeekFirstDay;
        private bool p_isMidWeekSecondDay;
        private string p_frequencyType;
        private DateTime p_lastModifiedDate;
        private int p_weekNo;

        public DateTime Dato
        {
            get
            {
                return this.p_date;
            }
            set
            {
                this.p_date = value;
            }
        }

        public bool IsHoliday
        {
            get
            {
                return this.p_isHoliday;
            }
            set
            {
                this.p_isHoliday = value;
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

        public string FrequencyType
        {
            get
            {
                return this.p_frequencyType;
            }
            set
            {
                this.p_frequencyType = value;
            }
        }

        public DateTime LastModifiedDate
        {
            get
            {
                return this.p_lastModifiedDate;
            }
            set
            {
                this.p_lastModifiedDate = value;
            }
        }

        public int WeekNo
        {
            get
            {
                return this.p_weekNo;
            }
            set
            {
                this.p_weekNo = value;
            }
        }

        
    }
}
