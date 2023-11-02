using Exiled.API.Features;
using Marine.Redux.API.Enums;
using Marine.Redux.API.Interfaces;
using YamlDotNet.Serialization;

namespace Marine.Redux.API.Subclasses
{
    public abstract class SingleSubclass : Subclass, ISinglePlayer
    {
        public SingleSubclass() : base() { }

        public sealed override SubclassType Type { get; } = SubclassType.Single;

        [YamlIgnore]
        public Player Player { get; set; }

        public sealed override void Assign(Player player)
        {
            if (HasAny(player) || Player != null)
            {
                return;
            }

            Player = player;

            base.Assign(player);
        }

        public sealed override void Revoke(Player player, in RevokeReason reason)
        {
            if (player == null || !Has(player) || reason == RevokeReason.Admin && player.Role == GameRole)
            {
                return;
            }

            Player = null;

            base.Revoke(player, reason);
        }

        public sealed override bool Has(in Player player)
        {
            return base.Has(player) && Player?.UserId == player.UserId;
        }

        public override bool Can(in Player player)
        {
            return base.Can(player) && Player == null;
        }
    }
}
