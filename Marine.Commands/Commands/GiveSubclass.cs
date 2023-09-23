using Exiled.API.Features;
using Marine.Commands.API;
using Marine.Commands.API.Abstract;
using Marine.Commands.API.Enums;
using Marine.Redux.API.Subclasses;
using System.Collections.Generic;
using System.Linq;

namespace Marine.Commands.Commands
{
    public class GiveSubclass : CommandBase
    {
        public override string Command { get; set; } = "subclass";

        public override string Description { get; set; } = "Команда для выдачи подкласса.";

        public override List<CommandType> Types { get; set; } = new List<CommandType>(2) { CommandType.RemoteAdmin, CommandType.ServerConsole };

        public override List<int> Counts { get; set; } = new List<int>() { 2 };

        public override Dictionary<int, string> Syntax { get; set; } = new Dictionary<int, string>()
        {
            { 2, "[ИГРОК] [ПОДКЛАСС]" },
        };

        public override CommandPermission Permission { get; set; } = new()
        {
            IsLimited = true,
            Groups = new HashSet<string>()
            {
                "adm",
                "modt",
            }
        };

        public override CommandResultType Handle(List<object> arguments, Player player, out string response)
        {
            var list = (List<Player>)arguments[0];

            if (!list.Any())
            {
                list.Add(player);
            }

            response = string.Empty;

            Subclass subclass = Subclass.ReadOnlyCollection.First(sub => arguments[1] is ushort ? sub.Id == (ushort)arguments[1] : sub.Name.Equals((string)arguments[1], System.StringComparison.OrdinalIgnoreCase));

            if (subclass == null)
            {
                response = "Не найден такой подкласс!";

                return CommandResultType.Fail;
            }

            if (subclass is SingleSubclass single && (list.Count > 1 || single.Player != null))
            {
                response = "Невозможно выдать подкласс!";

                return CommandResultType.Fail;
            }

            if (subclass is GroupSubclass group && (list.Count + group.Players.Count > group.Max || group.Players.Count == group.Max))
            {
                response = "Невозможно выдать подкласс!";

                return CommandResultType.Fail;
            }

            foreach (Player ply in list)
            {
                subclass.Assign(ply);
            }

            return CommandResultType.Success;
        }

        public override bool ParseSyntax(List<string> input, int count, out List<object> output)
        {
            output = new();

            if (!TryParsePlayers(input[0], out List<Player> players))
            {
                return false;
            }

            output.Add(players);

            if (count == 2)
            {
                if (!ushort.TryParse(input[1], out var value))
                {
                    output.Add(input[1]);

                    return true;
                }

                output.Add(value);

                return true;
            }

            return true;
        }
    }
}
