using System.Collections.Generic;

namespace ShipmentReports.Common
{
    /// <summary>
    /// Contains information for a courier, including his name and his shipments.
    /// </summary>
    public class CourierInfo
    {
        private List<Dictionary<ShipmentElement, string>> shipments = new List<Dictionary<ShipmentElement, string>>();

        #region Properties
        /// <summary>
        /// Name of the courier.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Collection of shipments.
        /// All the information linked with a shipment are represented via a Dictionary, 
        /// that maps the descriptor of a particular piece of information with its value.
        /// </summary>
        public List<Dictionary<ShipmentElement, string>> Shipments
        {
            get { return shipments; }
            set { shipments = value; }
        }

        public int ID { get; set; }
        #endregion

        public override string ToString()
        {
            return string.Format("{0}: {1}", Name, Shipments.Count);
        }
    }
}
