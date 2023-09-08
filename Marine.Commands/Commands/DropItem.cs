using Exiled.API.Features;
using Exiled.API.Features.Pickups;
using Marine.Commands.API;
using Marine.Commands.API.Abstract;
using Marine.Commands.API.Enums;
using MEC;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Marine.Commands.Commands
{
    public class DropItem : CooldownCommand
    {
        public override string Command { get; set; } = "dropitem";

        public override string Description { get; set; } = "Команда для сбрасывания предметов.";

        public override List<CommandType> Types { get; set; } = new List<CommandType>(2) { CommandType.RemoteAdmin, CommandType.ServerConsole };

        public override string[] Aliases { get; set; } = new string[1] { "dropi" };

        public override List<int> Counts { get; set; } = new List<int>() { 2, 3 };

        public override Dictionary<int, string> Syntax { get; set; } = new Dictionary<int, string>()
        {
            { 2, "[ПРЕДМЕТ] [КОЛИЧЕСТВО]" },
            { 3, "[ИГРОК] [ПРЕДМЕТ] [КОЛИЧЕСТВО]" },
        };

        public override CommandPermission Permission { get; set; } = new()
        {
            IsLimited = true,
            Groups = new HashSet<string>()
            {
                "adm",
                "mog",
                "soviet",
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

            if (arguments.Count == 2)
            {
                Timing.RunCoroutine(_SpawnItems(player, (ItemType)arguments[0], (int)arguments[1]));

                return CommandResultType.Success;
            }
            else if (arguments.Count == 3)
            {
                var list = (List<Player>)arguments[0];

                if (!list.Any())
                {
                    list.Add(player);
                }

                foreach (var ply in list)
                {
                    Timing.RunCoroutine(_SpawnItems(ply, (ItemType)arguments[1], (int)arguments[2]));
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

            if (count == 2)
            {
                if (!int.TryParse(input[0], out var item) || !int.TryParse(input[1], out var itemCount) || item > 51 || item < 0)
                {
                    return false;
                }

                output.Add((ItemType)item);
                output.Add(itemCount);

                return true;
            }
            else if (count == 3)
            {
                if (!TryParsePlayers(input[0], out var players) || !int.TryParse(input[1], out var item) || !int.TryParse(input[2], out var itemCount) || item > 51 || item < 0)
                {
                    return false;
                }

                output.Add(players);
                output.Add((ItemType)item);
                output.Add(itemCount);

                return true;
            }
            else
            {
                return false;
            }
        }

        private IEnumerator<float> _SpawnItems(Player player, ItemType item, int count)
        {
            for (int index = 0; index < count - 1; index++)
            {
                Pickup.CreateAndSpawn(item, player.Position, player.Rotation, player);

                yield return Timing.WaitForSeconds(0.1f);
            }
        }
    }
}
