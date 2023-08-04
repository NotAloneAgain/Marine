using Exiled.API.Features;
using Marine.Redux.API.Enums;
using Marine.Redux.API.Interfaces;
using System.Collections.Generic;
using YamlDotNet.Serialization;

namespace Marine.Redux.API.Subclasses
{
    public abstract class GroupSubclass : Subclass, IGroupPlayer
    {
        public GroupSubclass() : base()
        {
            Players = new(Max);
        }

        public sealed override SubclassType Type { get; } = SubclassType.Group;

        [YamlIgnore]
        public HashSet<Player> Players { get; set; }

        public abstract int Max { get; set; }

        public sealed override void Assign(Player player)
        {
            if (player == null || Has(player))
            {
                return;
            }

            Players.Add(player);

            base.Assign(player);
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

        public override bool Can(in Player player) => base.Can(player) && Players.Count + 1 <= Max;
    }
}
