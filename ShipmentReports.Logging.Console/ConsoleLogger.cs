using ShipmentReports.Logging.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShipmentReports.Logging.Console
{
    public class ConsoleLogger : ILogger
    {
        LoggingLevels mask = LoggingLevels.Erorr|LoggingLevels.Info| LoggingLevels.Warning | LoggingLevels.Debug;

        object consoleLock = new object();
        public ConsoleLogger()
        {
            lock (consoleLock)
            {
                System.Console.ForegroundColor = ConsoleColor.White;
                System.Console.BackgroundColor = ConsoleColor.Black;
            }
        }

        public void Debug(string message, int line, int pos)
        {
            if ((mask & LoggingLevels.Debug) == 0)
                return;

            lock (consoleLock)
            {
                System.ConsoleColor originalColor = System.Console.ForegroundColor;
                System.Console.ForegroundColor = ConsoleColor.White;
                System.Console.WriteLine("[D] {0},{1}: {2}", line, pos, message);
                System.Console.ForegroundColor = originalColor;
            }
        }

        public void Error(string message, int line, int pos)
        {
            if ((mask & LoggingLevels.Erorr) == 0)
                return;

            lock (consoleLock)
            {
                System.ConsoleColor originalColor = System.Console.ForegroundColor;
                System.Console.ForegroundColor = ConsoleColor.Red;
                System.Console.WriteLine("[E] {0},{1}: {2}", line, pos, message);
                System.Console.ForegroundColor = originalColor;
            }
        }

        public void Info(string message, int line, int pos)
        {
            if ((mask & LoggingLevels.Info) == 0)
                return;

            lock (consoleLock)
            {
                System.ConsoleColor originalColor = System.Console.ForegroundColor;
                System.Console.ForegroundColor = ConsoleColor.Green;
                System.Console.WriteLine("[I] {0},{1}: {2}", line, pos, message);
                System.Console.ForegroundColor = originalColor;
            }
        }  

        public void Warning(string message, int line, int pos)
        {
            if ((mask & LoggingLevels.Warning) == 0)
                return;

            lock (consoleLock)
            {
                System.ConsoleColor originalColor = System.Console.ForegroundColor;
                System.Console.ForegroundColor = ConsoleColor.Yellow;
                System.Console.WriteLine("[W] {0},{1}: {2}", line, pos, message);
                System.Console.ForegroundColor = originalColor;
            }
        }

        public void SetMask(LoggingLevels mask)
        {
            this.mask = mask;
        }
    }
}
