using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Puma.DataLayer.BusinessEntity.Report
{


    // Root myDeserializedClass = JsonConvert.DeserializeObject<List<Root>>(myJsonResponse);
    public class Attributes
    {
        public string Match_addr { get; set; }
        public string display { get; set; }
    }

    public class Geometry
    {
        public SpatialReference spatialReference { get; set; }
        public double x { get; set; }
        public double y { get; set; }
    }

    public class Outline
    {
        public string type { get; set; }
        public List<int> color { get; set; }
        public double width { get; set; }
        public string style { get; set; }
    }

    public class SelectedAddress
    {
        public Geometry geometry { get; set; }
        public Symbol symbol { get; set; }
        public Attributes attributes { get; set; }
        public object popupTemplate { get; set; }

        public Location location { get; set; }
    }

    public class Location
    {
        public SpatialReference spatialReference { get; set; }
        public double x { get; set; }
        public double y { get; set; }
    }

    public class SpatialReference
    {
        public int latestWkid { get; set; }
        public int wkid { get; set; }
    }

    public class Symbol
    {
        public string type { get; set; }
        public List<int> color { get; set; }
        public int angle { get; set; }
        public int xoffset { get; set; }
        public int yoffset { get; set; }
        public int size { get; set; }
        public string style { get; set; }
        public Outline outline { get; set; }
        public string verticalAlignment { get; set; }
        public string horizontalAlignment { get; set; }
        public string text { get; set; }
        public Font font { get; set; }
    }

    public class Font {
        public string family { get; set; }
        public int size { get; set; }
        public string style { get; set; }
        public string weight { get; set; }

        public string decoration { get; set; }

    }

    public class FeatureText
    {
        public Geometry geometry { get; set; }

        public Symbol symbol { get; set; }

    }

    public class FeaturePoint
    {
        public Geometry geometry { get; set; }

    }
}
