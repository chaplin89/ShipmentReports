using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Configuration;
using System.Configuration;
using ShipmentReports.Maker.Interface;

namespace ShipmentReports
{
    internal class Program
    {
        private const string DEFAULT_FILE_NAME = @"Stampe testuali.txt";
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
