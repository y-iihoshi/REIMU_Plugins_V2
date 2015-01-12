namespace ReimuPlugins.Common
{
    public enum Revision
    {
        Rev1 = 1,
        Rev2
    }

    public enum ErrorCode
    {
        NoFunction = -1,
        AllRight,
        NotSupport,
        FileReadError,
        FileWriteError,
        NoMemory,
        UnknownError,
        DialogCanceled
    }

    public enum TextAlign
    {
        Left = 0,
        Right,
        Center
    }

    public enum SortType
    {
        String = 0,
        Number,
        Float,
        Hex
    }

    public enum SystemInfoType
    {
        String = 0,
        Path,
        Directory,
        Title,
        Extension,
        CreateTime,
        LastAccessTime,
        LastWriteTime,
        FileSize
    }
}
