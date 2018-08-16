using System.Collections.Generic;

public static class ExitCodes
{
    public enum ExitCode
    {
        SUCCESS = 0x0,
        ERROR_UNEXPECTED = 0x1,
        ERROR_FILE_NOT_FOUND = 0x2,
        ERROR_PATH_NOT_FOUND = 0x3,
        ERROR_NOT_ENOUGH_MEMORY = 0x8,
        ERROR_CANCELLED = 0x3F,
        ERROR_DEVICE_UNREACHABLE = 0x141        
    }

    public static string ExitMessage(ExitCode exitCode)
    {
        return _exitCodeMessages[exitCode];
    }

    private static Dictionary<ExitCode, string> _exitCodeMessages = new Dictionary<ExitCode, string>()
    {
        {ExitCode.SUCCESS, "Load corectly." },
        {ExitCode.ERROR_UNEXPECTED, "Unexpected exception" },
        {ExitCode.ERROR_FILE_NOT_FOUND, "File not found." },
        {ExitCode.ERROR_PATH_NOT_FOUND, "Path error." },
        {ExitCode.ERROR_NOT_ENOUGH_MEMORY, "Memory exception" },
        {ExitCode.ERROR_CANCELLED, "Canceled" },
        {ExitCode.ERROR_DEVICE_UNREACHABLE, "Device not aviable."}
    };
}
