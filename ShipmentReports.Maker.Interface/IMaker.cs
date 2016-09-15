using System.Collections.Generic;
using ShipmentReports.Common;

namespace ShipmentReports.Maker.Interface
{
    public interface IMaker
    {        
        byte[] MakeFinalReport(ShipmentsData shipments);
    }
}
