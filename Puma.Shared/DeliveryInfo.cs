using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Puma.Shared
{
    public class DeliveryInfo
    {
        public int Weight { get; set; }
        public PumaEnum.DistributionType DistributionType { get; set; }
        public DateTime DistributionDate { get; set; }
        public List<long> RemoveRuter { get; set; }
        public double Thickness { get; set; }

        public DeliveryInfo()
        {
            RemoveRuter = new List<long>();
        }
    }

}
