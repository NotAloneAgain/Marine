using Exiled.API.Features;
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

        public bool IsUsedBy(Player player) => Uses.ContainsKey(player);

        public CommandUse GetLastUsing(Player player) => IsUsedBy(player) ? Uses[player].Last() : null;
    }
}
