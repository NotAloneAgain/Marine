namespace Marine.Commands.API.Enums
{
    public enum CommandResultType : sbyte
    {
        PlayerError = -3,
        PermissionError = -2,
        Error = -1,
        Syntax,
        Fail,
        Success
    }
}
