using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Puma.Shared
{
    public class DemographyValue
    {
        private string _Demography;
        public string Demography
        {
            get
            {
                return _Demography;
            }
            set
            {
                _Demography = value;
            }
        }

        private string _DemographyCode;
        public string DemographyCode
        {
            get
            {
                return _DemographyCode;
            }
            set
            {
                _DemographyCode = value;
            }
        }

        private int _Min;
        public int Min
        {
            get
            {
                return _Min;
            }
            set
            {
                _Min = value;
            }
        }

        private int _Max;
        public int Max
        {
            get
            {
                return _Max;
            }
            set
            {
                _Max = value;
            }
        }

        public DemographyValue()
        {
        }

        public DemographyValue(string demovar, int minVal, int maxVal, string democode)
        {
            Demography = demovar;
            Min = minVal;
            Max = maxVal;
            DemographyCode = democode;
        }
    }

    public class DemographyValuesCollection : System.Collections.Generic.List<DemographyValue>
    {
    }
}
