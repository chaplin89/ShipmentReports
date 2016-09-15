using ShipmentReports.Common;
using ShipmentReports.Logging.Console;
using ShipmentReports.Logging.Interface;
using ShipmentReports.Maker.Interface;
using ShipmentReports.Maker.PDF;
using ShipmentReports.Parser.Interface;
using ShipmentReports.Parser.Textual;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace ShipmentReports
{
    class Program
    {
        public static int Main(string[] args)
        {
            string defaultFile = string.Concat(Directory.GetCurrentDirectory(), @"\Stampe testuali.txt");

            if (File.Exists(defaultFile))
            {
                DoTheMagic(new string[] { defaultFile });
            }
            else if (args.Length > 0)
            {
                DoTheMagic(args);
            }
            else
            {
                ManageTestaDiCazzoUser();
            }

            Console.WriteLine("Premi un tasto per uscire.");
            Console.ReadKey();
            return 0;
        }

        public static void ManageTestaDiCazzoUser()
        {
            Console.WriteLine(@"Gentilissimo e illustre utente, ");
            Console.WriteLine();
            Console.WriteLine(@"per favore assicurati che nella directory di questo programma");
            Console.WriteLine(@"ci sia un file chiamato 'Stampe testuali.txt'");
            Console.WriteLine(@"In alternativa trascina una o più stampe testuali sull'icona del programma.");
            Console.WriteLine();
            Console.WriteLine(@"Un abbraccio,");
            Console.WriteLine(@"ShipmentReports");
        }

        public static void DoTheMagic(string[] filesToProcess)
        {
            byte[] outputFile = null;

            ILogger logger = new ConsoleLogger();
            IParser parser = new TextualParser(logger);
            IMaker maker = new PDFMaker(logger);

            int correctCounter = 0;
            int failedCounter = 0;

            FiltersSection section = ConfigurationManager.GetSection("Filters") as FiltersSection;

            foreach (var file in filesToProcess)
            {
                try
                {
                    logger.Info(string.Format("Inizio a processare il file {0}", Path.GetFileName(file)), 0, 0);

                    string path = Directory.GetParent(file).FullName;
                    string name = Path.GetFileNameWithoutExtension(file);

                    // Step #1: Parsing 
                    ShipmentsData info = parser.Parse(File.ReadAllLines(file));
                    logger.Info(string.Format("===> Parsing completato correttamente. {0} corrieri trovati.", info.Shipments.Count), 0, 0);

                    List<ShipmentsData> filteredData = new List<ShipmentsData>();

                    foreach (FiltersSettingsElement v in section.FiltersSettings)
                    {
                        string outputFileName = string.Format("{0}{1}.pdf", name, v.Suffix);

                        // Step #2: Filtering
                        Regex currentRegex = new Regex(v.Pattern);
                        ShipmentsData currentData = new ShipmentsData()
                        {
                            MetaInformation = info.MetaInformation,
                            Shipments = info.Shipments.Where(x => currentRegex.IsMatch(x.Name)).ToList(),
                            Date = info.Date
                        };

                        info.Shipments.RemoveAll(x => currentRegex.IsMatch(x.Name));

                        // Step #3: Making the final report
                        try
                        {
                            outputFile = maker.MakeFinalReport(currentData);
                            logger.Info(string.Format("===> Report creato correttamente. {0} kB totali.", outputFile.Length / 1024), 0, 0);

                            // Step #4: Writing the report
                            File.WriteAllBytes(string.Format("{0}\\{1}", path, outputFileName), outputFile);
                            logger.Info(string.Format("===> Report scritto correttamente: {0}. {1} Corrieri.", outputFileName, currentData.Shipments.Count), 0, 0);
                        }
                        catch (Exception ex)
                        {
                            logger.Error(string.Format("Errore durante la creazione/scrittura del report {0}: {1}", outputFileName, ex.Message), 0, 0);
                        }
                    }

                    if (info.Shipments.Count > 0)
                    {
                        logger.Warning("**WARNING** Trovati corrieri spuri!", 0, 0);

                        outputFile = maker.MakeFinalReport(info);
                        logger.Info(string.Format("===> Report creato correttamente. {0} kB totali.", outputFile.Length / 1024), 0, 0);

                        string outputFileName = string.Format("{0}_Spurious.pdf", name);
                        File.WriteAllBytes(string.Format("{0}\\{1}", path, outputFileName), outputFile);
                        logger.Info(string.Format("===> Report corrieri spuri scritto correttamente: {0}.", outputFileName), 0, 0);
                    }

                    correctCounter++;
                }
                catch (Exception ex)
                {
                    logger.Error(ex.Message, 0, 0);
                    failedCounter++;
                }
            }

            Console.WriteLine();
            Console.WriteLine(string.Format("Finito! {0} file totali, {1} processati correttamente, {2} file falliti!", filesToProcess.Length, correctCounter, failedCounter));
        }
    }
}
