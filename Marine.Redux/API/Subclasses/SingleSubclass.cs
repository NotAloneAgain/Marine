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
            if (Player != null)
            {
                return;
            }

            Player = player;

            base.Assign(player);
        }

        public sealed override void Revoke(Player player, in RevokeReason reason)
        {
            if (player == null || !Has(player) || reason == RevokeReason.Died && player.IsAlive)
            {
                return;
            }

            Player = null;

            base.Revoke(player, reason);
        }

        public sealed override bool Has(in Player player)
        {
            if (player == null || player.Role != Role)
            {
                return false;
            }

            return Player == player;
        }

        public override bool Can(in Player player) => base.Can(player) && Player == null;
    }
}
