using Exiled.API.Features;
using Marine.Commands.API;
using Marine.Commands.API.Abstract;
using Marine.Commands.API.Enums;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Marine.Commands.Commands
{
    public class Size : CooldownCommand
    {
        public override string Command { get; set; } = "size";

        public override string Description { get; set; } = "Команда для изменения размера.";

        public override List<CommandType> Types { get; set; } = new List<CommandType>(2) { CommandType.RemoteAdmin, CommandType.ServerConsole };

        public override string[] Aliases { get; set; } = new string[1] { "scale" };

        public override List<int> Counts { get; set; } = new List<int>() { 1, 2, 3, 4 };

        public override Dictionary<int, string> Syntax { get; set; } = new Dictionary<int, string>()
        {
            { 1, "[РАЗМЕР]" },
            { 2, "[ИГРОК] [РАЗМЕР]" },
            { 3, "[3D ВЕКТОР]" },
            { 4, "[ИГРОК] [3D ВЕКТОР]" }
        };

        public override CommandPermission Permission { get; set; } = new()
        {
            IsLimited = true,
            Groups = new HashSet<string>()
            {
                "adm",
                "mog",
                "soviet",
                "modt",
                "mod5"
            }
        };

        public override int Cooldown { get; set; } = 30;

        public override CommandResultType Handle(List<object> arguments, Player player, out string response)
        {
            if (base.Handle(arguments, player, out response) == CommandResultType.Fail)
            {
                return CommandResultType.Fail;
            }

            if (arguments.Count == 1)
            {
                player.Scale = (Vector3)arguments[0];

                return CommandResultType.Success;
            }
            else if (arguments.Count == 2)
            {
                var list = (List<Player>)arguments[0];

                if (!list.Any())
                {
                    list.Add(player);
                }

                foreach (var ply in list)
                {
                    ply.Scale = (Vector3)arguments[1];
                }

                return CommandResultType.Success;
            }
            else
            {
                return CommandResultType.Fail;
            }
        }

        public override bool ParseSyntax(List<string> input, int count, out List<object> output)
        {
            output = new();

            if (count == 1)
            {
                if (!float.TryParse(input[0], out var value))
                {
                    return false;
                }

                output.Add(Vector3.one * value);

                return true;
            }
            else if (count == 2)
            {
                if (!TryParsePlayers(input[0], out var players) || !float.TryParse(input[1], out var value))
                {
                    return false;
                }

                output.Add(players);
                output.Add(Vector3.one * value);

                return true;
            }
            else if (count == 3)
            {
                if (!float.TryParse(input[0], out var x) || !float.TryParse(input[1], out var y) || !float.TryParse(input[2], out var z))
                {
                    return false;
                }

                output.Add(new Vector3(x, y, z));

                return true;
            }
            else if (count == 4)
            {
                if (!TryParsePlayers(input[0], out var players) || !float.TryParse(input[1], out var x) || !float.TryParse(input[2], out var y) || !float.TryParse(input[3], out var z))
                {
                    return false;
                }

                output.Add(players);
                output.Add(new Vector3(x, y, z));

                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
