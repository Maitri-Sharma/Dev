using Puma.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Puma.DataLayer.BusinessEntity.UtvalgList
{
    /// <summary>
    /// Business Entity for change parent of list
    /// </summary>
    public class ChangeOfParentList
    {
        /// <summary>
        /// List id  to be update
        /// </summary>
        public int ListId { get; set; }

        /// <summary>
        /// Parent list id
        /// </summary>
        public int ParentListId { get; set; }

        /// <summary>
        /// Order type of list
        /// </summary>

        public PumaEnum.OrdreType OrderType { get; set; }

        /// <summary>
        /// Order type of parent list
        /// </summary>

        public PumaEnum.OrdreType NewOrderType { get; set; }

        /// <summary>
        /// Order reference of parent list
        /// </summary>
        public string NewOrderReference { get; set; }

        /// <summary>
        /// Order status of parent list
        /// </summary>

        public PumaEnum.OrdreStatus NewOrderStattus { get; set; }

        /// <summary>
        ///  /// <summary>
        /// antall of list
        /// </summary>
        /// </summary>
        public long Antall { get; set; }
    }
}
