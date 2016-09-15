using ShipmentReports.Common;

namespace ShipmentReports.Parser.Interface
{
    public interface IParser
    {
        ShipmentsData Parse(string[] lines);
    }
}
