using iTextSharp.text;
using iTextSharp.text.pdf;
using ShipmentReports.Common;
using ShipmentReports.Logging.Interface;
using ShipmentReports.Maker.Interface;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;

namespace ShipmentReports.Maker.PDF
{
    public class PDFMaker : IMaker
    {
        ILogger logger;

        public PDFMaker(ILogger logger)
        {
            this.logger = logger;
        }

        /// <summary>
        /// Used for creating the PDF output file
        /// </summary>
        /// <returns>Byte array containing the output PDF file</returns>
        public byte[] MakeFinalReport(ShipmentsData shipments)
        {
            Rectangle page = new Rectangle(PageSize.A4);
            PDFMakerSection section = ConfigurationManager.GetSection("PDFMaker") as PDFMakerSection;

            Dictionary<ShipmentElement, FinalReportSettingsElement> elementConfigurations = 
                new Dictionary<ShipmentElement, FinalReportSettingsElement>();

            // Merge the configuration with the info retrieved
            foreach (FinalReportSettingsElement field in section.FinalReportSettings)
            {
                ShipmentElement element = shipments.MetaInformation.Single(x => x.Name == field.SourceFieldName);
                elementConfigurations[element] = field;
            }

            using (var stream = new MemoryStream())
            using (var document = new Document(page))
            using (var writer = PdfWriter.GetInstance(document, stream))
            {
                // Initialization of PDF file
                document.Open();
                document.AddAuthor("Marco");
                document.AddCreationDate();
                document.AddLanguage("Italian");
                document.AddTitle(string.Format("Report di spedizioni per il {0}", DateTime.Now.ToString("dd/M/yyyy")));

                //Every iteration print information for a single courier
                foreach (var currentCourier in shipments.Shipments)
                {
                    Chunk idPhrase = new Chunk(currentCourier.ID.ToString() + "  ")
                    {
                        Font = FontFactory.GetFont(section.TitleFontName, section.IDFontSize, Font.BOLD, BaseColor.BLACK)
                    };

                    Chunk namePhrase = new Chunk(currentCourier.Name)
                    {
                        Font = FontFactory.GetFont(section.TitleFontName, section.NameFontSize, Font.BOLD, BaseColor.BLACK)
                    };

                    Chunk dateChunk = new Chunk(shipments.Date)
                    {
                        Font = FontFactory.GetFont(section.TitleFontName, section.DateFontSize, Font.BOLD, BaseColor.BLACK)
                    };

                    Paragraph title = new Paragraph()
                    {
                        Alignment = 1
                    };

                    title.Add(idPhrase);
                    title.Add(namePhrase);
                    title.Add(Chunk.NEWLINE);
                    title.Add(dateChunk);
                    title.Add(Chunk.NEWLINE);
                    title.Add(Chunk.NEWLINE);

                    PdfPTable table = new PdfPTable(elementConfigurations.Count)
                    {
                        TotalWidth = 523,
                        LockedWidth = true
                    };

                    table.SetWidths(elementConfigurations.Select(x => x.Key.CharacterEnd - x.Key.CharacterStart).ToArray());

                    // Set the header
                    foreach (var item in elementConfigurations)
                    {
                        table.AddCell(new PdfPCell(new Phrase(new Chunk(item.Value.DestinationFieldName) { Font = new Font(Font.FontFamily.COURIER, 9,Font.BOLD) })));
                    }

                    // Every iteration add a line to the table
                    foreach (var currentShipment in currentCourier.Shipments)
                    {
                        // Handling shipments with empty "mitt" field
                        if (currentShipment.Single(x => x.Key.Name == "Mitt").Value.Length == 0)
                            continue;

                        // Every iteration add a cell to a line
                        foreach (var currentShipmentElement in currentShipment)
                        {
                            if (elementConfigurations.ContainsKey(currentShipmentElement.Key))
                                table.AddCell(new PdfPCell( new Phrase(new Chunk(currentShipmentElement.Value) { Font = new Font(Font.FontFamily.COURIER,8)})));
                        }

                        // Handle records that doesn't contain all the columns
                        table.CompleteRow();
                    }

                    // Used to effectively create the page(s) associated to a given courier
                    document.NewPage();
                    document.Add(title);

                    if (table.Rows.Count > 1)
                        document.Add(table);
                    else
                    {
                        Paragraph noShipment = new Paragraph()
                        {
                            Alignment = 1
                        };

                        Chunk noShipmentMessage = new Chunk("Nessun ritiro.")
                        {
                            Font = new Font(Font.FontFamily.COURIER, 12, Font.BOLD)
                        };

                        noShipment.Add(noShipmentMessage);
                        document.Add(noShipment);
                    }
                }

                document.Close();
                return stream.ToArray();
            }
        }
    }
}
