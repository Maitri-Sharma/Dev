using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Puma.DataLayer.BusinessEntity.Report
{
    public class MapExtent
    {
        public Extent extent { get; set; }
    }


    public class Extent
    {
        public double xmin { get; set; }
        public double ymin { get; set; }
        public double xmax { get; set; }
        public double ymax { get; set; }
    }



    public class MapImage
    {
        public string href { get; set; }
    }

}
