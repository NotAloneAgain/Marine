using Exiled.API.Features;
using Marine.Redux.API.Enums;
using Marine.Redux.API.Interfaces;
using System.Collections.Generic;
using YamlDotNet.Serialization;

namespace Marine.Redux.API.Subclasses
{
    public abstract class GroupSubclass : Subclass, IGroupPlayer
    {
        public GroupSubclass() : base() => Players = new(Max);

        public sealed override SubclassType Type { get; } = SubclassType.Group;

        [YamlIgnore]
        public HashSet<Player> Players { get; set; }

        public abstract int Max { get; set; }

        public sealed override void Assign(Player player)
        {
            if (HasAny(player))
            {
                return;
            }

            _ = Players.Add(player);

            base.Assign(player);
        }

        public sealed override void Revoke(Player player, in RevokeReason reason)
        {
            if (player == null || !Has(player) || reason == RevokeReason.Admin && player.Role == GameRole)
            {
                return;
            }

            _ = Players.Remove(player);

            base.Revoke(player, reason);
        }

        public sealed override bool Has(in Player player)
        {
            return base.Has(player) && Players.Contains(player);
        }

        public override bool Can(in Player player)
        {
            return base.Can(player) && Players.Count + 1 <= Max;
        }
    }
}
