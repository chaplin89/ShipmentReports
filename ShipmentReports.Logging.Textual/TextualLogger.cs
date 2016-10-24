using ShipmentReports.Logging.Interface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShipmentReports.Logging.Textual
{
    public class TextualLogger : ILogger
    {
        private static object fileLock = new object();
        private FileStream logfile = File.Open(@".\log.txt", FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite);

        public TextualLogger()
        {
        }

        public void Debug(string message, int line, int pos)
        {
            lock (fileLock)
            {
                string msg = string.Format("[D]\t{0},{1}\t{2}", line, pos, message);
                byte[] encoded = Encoding.ASCII.GetBytes(msg);
                logfile.Write(encoded,0, encoded.Length);
            }
        }

        public void Error(string message, int line, int pos)
        {
            lock (fileLock)
            {
                string msg = string.Format("[E]\t{0},{1}\t{2}", line, pos, message);
                byte[] encoded = Encoding.ASCII.GetBytes(msg);
                logfile.Write(encoded, 0, encoded.Length);
            }
        }

        public void Info(string message, int line, int pos)
        {
            lock (fileLock)
            {
                string msg = string.Format("[I]\t{0},{1}\t{2}", line, pos, message);
                byte[] encoded = Encoding.ASCII.GetBytes(msg);
                logfile.Write(encoded, 0, encoded.Length);
            }
        }

        public void SetMask(LoggingLevels mask)
        {
        }

        public void Warning(string message, int line, int pos)
        {
            lock (fileLock)
            {
                string msg = string.Format("[W]\t{0},{1}\t{2}", line, pos, message);
                byte[] encoded = Encoding.ASCII.GetBytes(msg);
                logfile.Write(encoded, 0, encoded.Length);
            }
        }
    }
}
