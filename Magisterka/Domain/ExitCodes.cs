using System;
using System.Collections.Generic;

public static class ExitCodes
{
    public enum ExitCode
    {
        SUCCESS = 0x0,
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
        {ExitCode.SUCCESS, "Załdowano poprawnie." },
        {ExitCode.ERROR_FILE_NOT_FOUND, "Nie znaleziono pliku." },
        {ExitCode.ERROR_PATH_NOT_FOUND, "Błąd ścieżki." },
        {ExitCode.ERROR_NOT_ENOUGH_MEMORY, "Brak pamięci." },
        {ExitCode.ERROR_CANCELLED, "Anulowano." },
        {ExitCode.ERROR_DEVICE_UNREACHABLE, "Urządzenie niedostępne"}
    };
}
