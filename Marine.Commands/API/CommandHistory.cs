using Exiled.API.Features;
using Marine.Commands.API.Enums;
using System.Collections.Generic;
using System.Linq;

namespace Marine.Commands.API
{
    public class CommandHistory
    {
        public Dictionary<Player, List<CommandUse>> Uses { get; set; } = new();

        public void Add(Player player, CommandUse use)
        {
            if (!IsUsedBy(player))
            {
                Uses.Add(player, new List<CommandUse>() { use });

                return;
            }

            Uses[player].Add(use);
        }

        public bool IsUsedBy(Player player)
        {
            return Uses != null && Uses.ContainsKey(player);
        }

        public bool HasSuccessfulUse(Player player)
        {
            return GetLastSuccessfulUse(player) != null;
        }

        public CommandUse GetLastUse(Player player)
        {
            return IsUsedBy(player) ? Uses[player].Last() : null;
        }

        public CommandUse GetLastSuccessfulUse(Player player)
        {
            return IsUsedBy(player) && Uses[player].Any(use => use.Result == CommandResultType.Success) ? Uses[player].Last(use => use.Result == CommandResultType.Success) : null;
        }
    }
}
