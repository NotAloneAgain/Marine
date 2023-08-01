using Marine.Redux.API.Enums;
using Marine.Redux.API.Interfaces;
using PlayerRoles;
using Exiled.API.Features;
using System.Collections.Generic;

namespace Marine.Redux.API.Subclasses
{
    public abstract class GroupSubclass : Subclass, IGroupPlayer
    {
        public GroupSubclass() { }

        public GroupSubclass(string name, RoleTypeId role, SpawnInfo spawnInfo) : base(name, role, spawnInfo) { }

        public sealed override SubclassType Type { get; } = SubclassType.Group;

        public List<Player> Players { get; set; }

        public sealed override void Assign(Player player)
        {
            if (player == null || Has(player))
            {
                return;
            }

            base.Assign(player);

            Players.Add(player);
        }

        public sealed override void Revoke(Player player, in RevokeReason reason)
        {
            if (player == null || !Has(player) || reason == RevokeReason.Died && player.IsAlive)
            {
                return;
            }

            base.Revoke(player, reason);

            Players.Remove(player);
        }

        public sealed override bool Has(in Player player)
        {
            if (player == null || player.Role != Role)
            {
                return false;
            }

            return Players.Contains(player);
        }
    }
}
