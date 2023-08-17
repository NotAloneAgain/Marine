using Exiled.API.Features;
using Exiled.API.Features.Items;
using Marine.Commands.API;
using Marine.Commands.API.Abstract;
using Marine.Commands.API.Enums;
using System.Collections.Generic;
using System.Linq;

namespace Marine.Commands.Commands
{
    public class Grenade : CooldownCommand
    {
        public override string Command { get; set; } = "grenade";

        public override string Description { get; set; } = "Команда для взрыва.";

        public override List<CommandType> Types { get; set; } = new List<CommandType>(2) { CommandType.RemoteAdmin, CommandType.ServerConsole };

        public override List<int> Counts { get; set; } = new List<int>() { 1, 2 };

        public override Dictionary<int, string> Syntax { get; set; } = new Dictionary<int, string>()
        {
            { 1, "[ИГРОК]" },
            { 2, "[ИГРОК] [ВРЕМЯ АКТИВАЦИИ]" }
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
                "ceo",
                "mod4"
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
                var grenade = ExplosiveGrenade.Create(ItemType.GrenadeHE, ply) as ExplosiveGrenade;

                if (arguments.Count == 2)
                {
                    grenade.FuseTime = (float)arguments[1];
                }

                grenade.SpawnActive(ply.Position, ply);
            }

            return CommandResultType.Success;
        }

        public override bool ParseSyntax(List<string> input, int count, out List<object> output)
        {
            output = new();

            if (!TryParsePlayers(input[0], out var players))
            {
                return false;
            }

            output.Add(players);

            if (count == 2)
            {
                if (!float.TryParse(input[1], out var value))
                {
                    return false;
                }

                output.Add(value);

                return true;
            }

            return true;
        }
    }
}
