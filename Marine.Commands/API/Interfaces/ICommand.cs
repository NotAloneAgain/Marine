using Exiled.API.Features;
using Marine.Commands.API.Enums;
using System.Collections.Generic;

namespace Marine.Commands.API.Interfaces
{
    public interface ICommand : CommandSystem.ICommand
    {
        List<int> Counts { get; set; }

        Dictionary<int, string> Syntax { get; set; }

        List<CommandType> Types { get; set; }

        Dictionary<CommandResultType, string> Messages { get; set; }

        CommandPermission Permission { get; set; }

        CommandHistory History { get; set; }

        public void Subscribe();

        public void Unsubscribe();

        bool ParseSyntax(List<string> input, int count, out List<object> output);

        CommandResultType Handle(List<object> arguments, Player player, out string response);
    }
}
