using PlayerRoles;
using PluginAPI.Core;
using PluginAPI.Core.Attributes;
using PluginAPI.Enums;
using PluginAPI.Events;
using System.Collections.Generic;

namespace Marine.Based.Handlers
{
    internal sealed class EventHandlers
    {
        public Dictionary<Player, Dictionary<string, int>> _used;

        [PluginEvent(ServerEventType.RemoteAdminCommand)]
        public void OnRemoteAdminCommand(RemoteAdminCommandEvent ev)
        {
            var player = Player.Get(ev.Sender);

            if (player == null || player.IsServer || player.ReferenceHub.serverRoles.Group.KickPower > 0)
            {
                return;
            }

            if (!_used.ContainsKey(player))
            {
                _used.Add(player, new(2));
            }

            var used = _used[player];

            var key = ev.Command;

            switch (key)
            {
                case "forceclass" or "forcerole" or "fc" or "fr":
                    {
                        if (!used.ContainsKey("forceclass"))
                        {
                            used.Add("forceclass", 0);
                        }

                        RoleTypeId role;

                        break;
                    }
                case "give":
                    {
                        if (!used.ContainsKey("give"))
                        {
                            used.Add("give", 0);
                        }

                        break;
                    }
            }
        }
    }
}
