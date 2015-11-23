namespace AlgoLifter.Infrastructure
{
    public enum TMCLReturnStatus
    {
        WrongChecksum = 1,
        InvalidCommand = 2,
        WrongType = 3,
        InvalidValue = 4,
        EEPROMlocked = 5,
        CommandNotAvailable = 6,
        Success = 100,
        LoadedToEEPROM = 101
    }
}