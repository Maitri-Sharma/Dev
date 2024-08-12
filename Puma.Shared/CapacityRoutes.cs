
using System;

namespace Puma.Shared
{
    /// <summary>
    /// 
    /// </summary>
    public class CapacityRoute
    {
        #region Private variables

        private System.DateTime dateField;
        private string reolNumberStringField;
        private long reolNumberIntField;
        private int restWeightField;
        private int restCountField;
        private string recipientTypeField;
        private double restThicknessField;

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

        public string ReolNumberString
        {
            get
            {
                return reolNumberStringField;
            }

            set
            {
                reolNumberStringField = value;
            }
        }

        public long ReolNumberInt
        {
            get
            {
                return reolNumberIntField;
            }

            set
            {
                reolNumberIntField = value;
            }
        }

        public int RestWeight
        {
            get
            {
                return restWeightField;
            }

            set
            {
                restWeightField = value;
            }
        }

        public int RestCount
        {
            get
            {
                return restCountField;
            }

            set
            {
                restCountField = value;
            }
        }

        public string RecipientType
        {
            get
            {
                return recipientTypeField;
            }

            set
            {
                recipientTypeField = value;
            }
        }

        public double RestThickness
        {
            get
            {
                return restThicknessField;
            }

            set
            {
                restThicknessField = value;
            }
        }

        #endregion
    }
}