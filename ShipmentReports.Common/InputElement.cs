namespace ShipmentReports.Common
{
    /// <summary>
    /// Represent a piece of information linked with a single shipment.
    /// </summary>
    public class ShipmentElement
    {
        /// <summary>
        /// Order into the input file.
        /// </summary>
        public int Position { get; set; }

        /// <summary>
        /// Start character for this column in the input file.
        /// </summary>
        public int CharacterStart { get; set; }

        /// <summary>
        /// End character for this column in the input file.
        /// </summary>
        public int CharacterEnd { get; set; }

        /// <summary>
        /// Name of the column in the input file.
        /// </summary>
        public string Name { get; set; }

        public override string ToString()
        {
            return Name;
        }

    }
}
