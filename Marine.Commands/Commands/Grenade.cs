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
                "mod5"
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

                grenade.SpawnActive(ply.Position, ply);
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
