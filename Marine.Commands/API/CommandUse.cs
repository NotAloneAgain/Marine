using Marine.Commands.API.Enums;
using System;

namespace Marine.Commands.API
{
    public class CommandUse
    {
        public CommandUse(DateTime time, CommandResultType result)
        {
            Time = time;
            Result = result;
        }

        public DateTime Time { get; set; }

        public CommandResultType Result { get; set; }
    }
}
