namespace ShipmentReports.Logging.Interface
{
    public enum LoggingLevels : int
    {
        Debug = 1,
        Info = 2,
        Warning = 4,
        Erorr = 8
    }

    public interface ILogger
    {
        void SetMask(LoggingLevels mask);
        void Debug(string message, int line, int pos);
        void Info(string message, int line, int pos);
        void Warning(string message, int line, int pos);
        void Error(string message, int line, int pos);
    }
}
