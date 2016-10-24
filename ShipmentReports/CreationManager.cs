using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Configuration;
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
    internal class CreationManager
    {
        /// <summary>
        /// Manage the steps of parsing and creation of the various reports.
        /// </summary>
        /// <param name="filesToProcess">Files to parse</param>
        public static void DoTheMagic(string[] filesToProcess)
        {
            var unitySection = (UnityConfigurationSection)ConfigurationManager.GetSection("Unity");
            IUnityContainer container = new UnityContainer();
            unitySection.Configure(container);

            IMaker maker = container.Resolve<IMaker>();
            IParser parser = container.Resolve<IParser>();
            ILogger logger = container.Resolve<ILogger>();

            byte[] outputFile = null;

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

                    int subReport = 1;

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
                            logger.Info(string.Format("===> [{1}/{2}] Report creato correttamente. {0} kB totali.", outputFile.Length / 1024, subReport, section.FiltersSettings.Count), 0, 0);

                            // Step #4: Writing the report
                            File.WriteAllBytes(Path.Combine(path, outputFileName), outputFile);
                            logger.Info(string.Format("===> [{2}/{3}] Report scritto correttamente: {0}. {1} Corrieri.", outputFileName, currentData.Shipments.Count, subReport,section.FiltersSettings.Count), 0, 0);
                        }
                        catch (Exception ex)
                        {
                            logger.Error(string.Format("Errore durante la creazione/scrittura del report {0}: {1}", outputFileName, ex.Message), 0, 0);
                        }
                        subReport++;
                    }

                    if (info.Shipments.Count > 0)
                    {
                        logger.Warning("**WARNING** Trovati corrieri spuri!", 0, 0);

                        outputFile = maker.MakeFinalReport(info);
                        logger.Info(string.Format("===> Report creato correttamente. {0} kB totali.", outputFile.Length / 1024), 0, 0);

                        string outputFileName = string.Format("{0}_Spurious.pdf", name);

                        File.WriteAllBytes(Path.Combine(path, outputFileName), outputFile);
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
