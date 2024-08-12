using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Puma.DataLayer.BusinessEntity.Utvalg
{
    public class BasicDetail
    {
        /// <summary>
        /// The name
        /// </summary>
        public string name;
        /// <summary>
        /// The cat
        /// </summary>
        public string cat;
        /// <summary>
        /// The pris zone
        /// </summary>
        public int prisZone;
        /// <summary>
        /// The key
        /// </summary>
        public string key;
        /// <summary>
        /// The house
        /// </summary>
        public int house;
        /// <summary>
        /// The households reserved
        /// </summary>
        public int householdsReserved;
        /// <summary>
        /// The businesses
        /// </summary>
        public int businesses;
        /// <summary>
        /// The hh d1
        /// </summary>
        public int HHD1;
        /// <summary>
        /// The hh d2
        /// </summary>
        public int HHD2;
        /// <summary>
        /// The vh d1
        /// </summary>
        public int VHD1;
        /// <summary>
        /// The vh d2
        /// </summary>
        public int VHD2;
        /// <summary>
        /// The zone0
        /// </summary>
        public int zone0;
        /// <summary>
        /// The zone1
        /// </summary>
        public int zone1;
        /// <summary>
        /// The zone2
        /// </summary>
        public int zone2;
        /// <summary>
        /// The pkey
        /// </summary>
        public string pkey;
        /// <summary>
        /// The h0
        /// </summary>
        public int H0;
        /// <summary>
        /// The h1
        /// </summary>
        public int H1;
        /// <summary>
        /// The h2
        /// </summary>
        public int H2;
        /// <summary>
        /// The v0
        /// </summary>
        public int V0;
        /// <summary>
        /// The v1
        /// </summary>
        public int V1;
        /// <summary>
        /// The v2
        /// </summary>
        public int V2;
        /// <summary>
        /// The total
        /// </summary>
        public int total;

        /// <summary>
        /// Class to display in report
        /// </summary>
        public string CssClass { get; set; }
        /// <summary>
        /// The children
        /// </summary>
        public List<BasicDetail> children;
    }
}
