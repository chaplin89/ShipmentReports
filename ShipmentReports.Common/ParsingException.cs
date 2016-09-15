using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShipmentReports.Common
{
    public class ParsingException : Exception
    {
        public ParsingException(string message, Exception innerException) : base(message, innerException)
        {

        }
    }
}
