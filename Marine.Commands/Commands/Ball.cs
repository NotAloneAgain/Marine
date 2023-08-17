using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.API.Features.Pickups;
using Marine.Commands.API;
using Marine.Commands.API.Abstract;
using Marine.Commands.API.Enums;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Marine.Commands.Commands
{
    public class Ball : CooldownCommand
    {
        public override string Command { get; set; } = "ball";

        public override string Description { get; set; } = "Команда для спавна мячика под человеком.";

        public override List<CommandType> Types { get; set; } = new List<CommandType>(2) { CommandType.RemoteAdmin, CommandType.ServerConsole };

        public override List<int> Counts { get; set; } = new List<int>() { 1 };

        public override Dictionary<int, string> Syntax { get; set; } = new Dictionary<int, string>()
        {
            { 1, "[ИГРОК]" },
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
                "mod5",
                "ceo"
            }
        };

        public override int Cooldown { get; set; } = 3;

        public override CommandResultType Handle(List<object> arguments, Player player, out string response)
        {
            if (base.Handle(arguments, player, out response) == CommandResultType.Fail)
            {
                return CommandResultType.Fail;
            }

            var list = (List<Player>)arguments[0];

            if (!list.Any())
            {
                list.Add(player);
            }

            foreach (var ply in list)
            {
                Pickup.CreateAndSpawn(ItemType.SCP018, ply.Position, Quaternion.Euler(ply.Rotation), ply);
            }

            return CommandResultType.Success;
        }

        public override bool ParseSyntax(List<string> input, int count, out List<object> output)
        {
            output = new();

            if (count == 1)
            {
                if (!TryParsePlayers(input[0], out var players))
                {
                    return false;
                }

                output.Add(players);

                return true;
            }

            return false;
        }
    }
}
