using System;

public class ExitCodes
{
    public enum ExitCode
    {
        ERROR_SUCCESS = 0x0,
        ERROR_FILE_NOT_FOUND = 0x2,
        ERROR_PATH_NOT_FOUND = 0x3,
        ERROR_NOT_ENOUGH_MEMORY = 0x8,
        ERROR_DEVICE_UNREACHABLE = 0x141   
    }
}
