using System;
using ShipmentReports.Parser.Interface;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using ShipmentReports.Common;
using ShipmentReports.Logging.Interface;
using System.Diagnostics.Contracts;

namespace ShipmentReports.Parser.Textual
{
    public class TextualParser : IParser
    {
        List<ShipmentElement> tableInfo;
        ILogger logger;
        char[] linesToIgnore = new char[] { ' ', '-'};
        char[] characterToSkip = new char[] { '\f', (char)0xFF };
        const char SEPARATOR_CHAR = '-';
        Regex courier = new Regex(@"Autista:\s*([0-9]+)\s*(.*)");
        string date;
        
        public TextualParser(ILogger logger)
        {
            this.logger = logger;
        }

        /// <summary>
        /// Used for parsing textual data
        /// </summary>
        /// <param name="lines">Lines in the textual input file</param>
        /// <returns>Information regarding the input file</returns>
        public ShipmentsData Parse(string[] lines)
        {
            List<CourierInfo> couriers = new List<CourierInfo>();
            CourierInfo currentCourier = new CourierInfo();

            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].Length == 0)
                {
                    logger.Debug("Empty line found.", i, 0);
                    continue;
                }

                if (characterToSkip.Contains(lines[i][0]))
                {
                    lines[i] = lines[i].TrimStart(characterToSkip);
                    if (lines[i].Length == 0)
                        continue;
                }

                if (linesToIgnore.Contains(lines[i][0]))
                {
                    logger.Debug("Empty line found.", i, 0);
                    continue;
                }

                //Manage courier name
                if (courier.IsMatch(lines[i]))
                {
                    if (currentCourier.Name != null)
                    {
                        string msg = string.Format("Adding info for {0}. {1} shipments.", currentCourier.Name, currentCourier.Shipments.Count);
                        logger.Debug(msg, i, 0);
                        couriers.Add(currentCourier);
                        currentCourier = new CourierInfo();
                    }

                    Match regExMatch = courier.Match(lines[i]);
                    currentCourier.ID = int.Parse(regExMatch.Groups[1].Value.Trim());
                    currentCourier.Name = regExMatch.Groups[2].Value.Trim();
                    logger.Debug(string.Format("Found courier with name: {0}", currentCourier.Name), i, 0);
                }
                // Manage header information
                else if (i + 1 < lines.Length && lines[i + 1].Length > 0 && lines[i + 1][0] == '-')
                {
                    if (tableInfo == null)
                    {
                        logger.Debug("Meta-info: found.", i, 0);
                        tableInfo = HandleTableInfo(lines[i], lines[i + 1]);
                        logger.Debug("Meta-info: added successfully.", i, 0);
                    }
                }
                // Manage shipment line
                else
                {
                    if (currentCourier.Name != null && tableInfo != null)
                        currentCourier.Shipments.Add(HandleShipmentRecord(lines[i], tableInfo));
                }
            }

            // Managing last courier
            if (currentCourier.Name != null)
            {
                string msg = string.Format("Adding info for {0}. {1} shipments.", currentCourier.Name, currentCourier.Shipments.Count);
                logger.Debug(msg, lines.Length, 0);
                couriers.Add(currentCourier);
            }

            if (tableInfo == null)
            {
                throw new ParsingException("Formato del file incorretto. Header non trovato. ", null);
            }

            if (couriers.Count == 0)
            {
                throw new ParsingException("Formato del file incorretto. Nessun corriere trovato. ", null);
            }

            return new ShipmentsData() { Shipments = couriers, MetaInformation = tableInfo, Date = date };
        }

        /// <summary>
        /// Used for extract information about the header.
        /// </summary>
        /// <param name="tableHeader">Lines containing the header</param>
        /// <param name="separators">Lines following the header, delimitating the end of the header.</param>
        /// <returns>Meta-information about the input file</returns>
        /// <remarks>
        /// The software uses the line following the header in order to understand how long the field can be.
        /// Nor the line containing the header, neither the single field are valid indicators about the maximum lenght
        /// of the fields.
        /// </remarks>
        private List<ShipmentElement> HandleTableInfo(string tableHeader, string separators)
        {
            List<ShipmentElement> tableInfo = new List<ShipmentElement>();
            
            int index = 0;

            while (index < separators.Length)
            {
                int start, end, lenght;

                //Try to find the lenght of one header element
                start = index;
                while (index + 1 < separators.Length && separators[++index] == SEPARATOR_CHAR);
                end = index;

                //Ignore white characters
                while (index + 1 < separators.Length && separators[++index] == ' ');

                lenght = Math.Min(end, tableHeader.Length) - start;

                if (lenght <= 0)
                    break;

                ShipmentElement current = new ShipmentElement()
                {
                    Name = tableHeader.Substring(start, lenght).Trim(),
                    CharacterEnd = end,
                    CharacterStart = start,
                    Position = tableInfo.Count
                };
                tableInfo.Add(current);
            }

            return tableInfo;
        }

        /// <summary>
        /// Used to extract information about a shipment line
        /// </summary>
        /// <param name="line">Shipment line</param>
        /// <param name="tableInfo">Header information</param>
        /// <returns>A dictionary that map header information with shipment information</returns>
        private Dictionary<ShipmentElement, string> HandleShipmentRecord(string line, List<ShipmentElement> tableInfo)
        {
            Dictionary<ShipmentElement, string> shipmentRecord = new Dictionary<ShipmentElement, string>();

            foreach (var v in tableInfo)
            {
                if (v.CharacterStart < line.Length)
                {
                    int endShipmentRow = Math.Min(v.CharacterEnd, line.Length);
                    string value = line.Substring(v.CharacterStart, endShipmentRow - v.CharacterStart).Trim();
                    shipmentRecord.Add(v, value);
                }
            }

            if (date == null)
            {
                try
                {
                    date = shipmentRecord.Single(x => x.Key.Name == "Data").Value;
                }
                catch (InvalidOperationException)
                {
                    logger.Warning("Impossibile recuperare le informazioni dal campo \"Data\". Verrà tentato con il prossimo record.",0,0);
                }
            }

            return shipmentRecord;
        }
    }
}
