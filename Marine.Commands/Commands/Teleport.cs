using Exiled.API.Features;
using Exiled.API.Features.Doors;
using Exiled.API.Features.Pickups;
using Marine.Commands.API;
using Marine.Commands.API.Abstract;
using Marine.Commands.API.Enums;
using Marine.Redux.API.Subclasses;
using Marine.Redux.Subclasses.ClassD.Single;
using System;
using System.Collections.Generic;

namespace Marine.Commands.Commands
{
    public class Teleport : CommandBase
    {
        private static readonly Type[] _teleportObjects;

        static Teleport() => _teleportObjects = new Type[6]
        {
            typeof(Door),
            typeof(Room),
            typeof(Player),
            typeof(Pickup),
            typeof(TeslaGate),
            typeof(Window),
        };

        public override string Command { get; set; } = "tp";

        public override string Description { get; set; } = "Команда для выдачи предмета.";

        public override List<CommandType> Types { get; set; } = new List<CommandType>(1) { CommandType.PlayerConsole };

        public override string[] Aliases { get; set; } = new string[1] { "teleport" };

        public override List<int> Counts { get; set; } = new List<int>(1) { 0, 1 };

        public override Dictionary<int, string> Syntax { get; set; } = new Dictionary<int, string>()
        {
            { 0, string.Empty },
            { 1, "[ТИП ОБЪЕКТА]" }
        };

        public override CommandPermission Permission { get; set; } = new()
        {
            IsLimited = true,
        };

        public override CommandResultType Handle(List<object> arguments, Player player, out string response)
        {
            response = string.Empty;

            if (arguments.Count == 0)
            {
                player.RandomTeleport(_teleportObjects);
            }
            else
            {
                player.RandomTeleport(arguments[0] as Type);
            }

            return CommandResultType.Success;
        }

        public override bool ParseSyntax(List<string> input, int count, out List<object> output)
        {
            output = new();

            if (count == 1)
            {
                switch (input[0].ToLower())
                {
                    case "игрок" or "player":
                        {
                            output.Add(typeof(Player));

                            break;
                        }
                    case "дверь" or "door":
                        {
                            output.Add(typeof(Door));

                            break;
                        }
                    case "комната" or "room":
                        {
                            output.Add(typeof(Room));

                            break;
                        }
                }
            }

            return true;
        }

        public override bool CheckPermissions(Player player) => base.CheckPermissions(player) || Subclass.Has<Scp343>(player);
    }
}
