
using System;

namespace Puma.Shared
{
    /// <summary>
    /// 
    /// </summary>
    public class CapacityDate
    {
        #region Private variables

        private System.DateTime dateField;
        private int weekNumberField;
        private string distributionDayField;
        private string businessDayField;

        #endregion

        #region Public Properties

        public DateTime Date
        {
            get
            {
                return dateField;
            }

            set
            {
                dateField = value;
            }
        }

        public int WeekNumber
        {
            get
            {
                return weekNumberField;
            }

            set
            {
                weekNumberField = value;
            }
        }

        public string DistributionDay
        {
            get
            {
                return distributionDayField;
            }

            set
            {
                distributionDayField = value;
            }
        }

        public string BusinessDay
        {
            get
            {
                return businessDayField;
            }

            set
            {
                businessDayField = value;
            }
        }

        #endregion
    }

    public class PRSAdminCapacity
    {
        #region Private variables

        private System.DateTime dateField;
        private string isHolidayField;
        private string isEarlyWeekFirstDayField;
        private string isEarlyWeekSecondDayField;
        private string isMidWeekFirstDayField;
        private string isMidWeekSecondDayField;
        private string frequencyTypeField;
        private System.DateTime lastModifiedDateField;
        private long weekNoField;

        #endregion

        #region Public Properties

        public DateTime Date
        {
            get
            {
                return dateField;
            }

            set
            {
                dateField = value;
            }
        }

        public string IsHoliday
        {
            get
            {
                return isHolidayField;
            }

            set
            {
                isHolidayField = value;
            }
        }

        public string IsEarlyWeekFirstDay
        {
            get
            {
                return isEarlyWeekFirstDayField;
            }

            set
            {
                isEarlyWeekFirstDayField = value;
            }
        }

        public string IsEarlyWeekSecondDay
        {
            get
            {
                return isEarlyWeekSecondDayField;
            }

            set
            {
                isEarlyWeekSecondDayField = value;
            }
        }

        public string IsMidWeekFirstDay
        {
            get
            {
                return isMidWeekFirstDayField;
            }

            set
            {
                isMidWeekFirstDayField = value;
            }
        }

        public string IsMidWeekSecondDay
        {
            get
            {
                return isMidWeekSecondDayField;
            }

            set
            {
                isMidWeekSecondDayField = value;
            }
        }

        public string FrequencyType
        {
            get
            {
                return frequencyTypeField;
            }

            set
            {
                frequencyTypeField = value;
            }
        }

        public DateTime LastModifiedDate
        {
            get
            {
                return lastModifiedDateField;
            }

            set
            {
                lastModifiedDateField = value;
            }
        }

        public long WeekNo
        {
            get
            {
                return weekNoField;
            }

            set
            {
                weekNoField = value;
            }
        }



        #endregion
    }
}