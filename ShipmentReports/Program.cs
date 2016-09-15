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
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace ShipmentReports
{
    class Program
    {
        const string DEFAULT_FILE_NAME = @"Stampe testuali.txt";
        public static int Main(string[] args)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            FileVersionInfo fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
            string version = fileVersionInfo.ProductVersion;
            
            string defaultFile = Path.Combine(Directory.GetCurrentDirectory(), DEFAULT_FILE_NAME);

            Console.WriteLine("Bevenuto!");
            Console.WriteLine("ShipmentReports v. {0}", version);
            Console.WriteLine();

            if (File.Exists(defaultFile))
            {
                CreationManager.DoTheMagic(new string[] { defaultFile });
            }
            else if (args.Length > 0)
            {
                CreationManager.DoTheMagic(args);
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
    }
}
