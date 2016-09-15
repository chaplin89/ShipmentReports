using System.Collections.Generic;

namespace ShipmentReports.Common
{
    /// <summary>
    /// High level representation of an input file.
    /// </summary>
    public class ShipmentsData
    {
        public string Date { get; set; }

        /// <summary>
        /// Contains information about the table itself.
        /// </summary>
        public List<ShipmentElement> MetaInformation { get; set; }

        /// <summary>
        /// Contains information about the courier and the shipments.
        /// </summary>
        public List<CourierInfo> Shipments { get; set; }
    }
}
