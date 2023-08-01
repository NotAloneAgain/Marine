using Exiled.API.Features;
using Marine.Redux.API.Enums;
using Marine.Redux.API.Interfaces;
using PlayerRoles;
using YamlDotNet.Serialization;

namespace Marine.Redux.API.Subclasses
{
    public abstract class SingleSubclass : Subclass, ISinglePlayer
    {
        public SingleSubclass() { }

        public SingleSubclass(string name, RoleTypeId role, SpawnInfo spawnInfo) : base(name, role, spawnInfo) { }

        public sealed override SubclassType Type { get; } = SubclassType.Single;

        [YamlIgnore]
        public Player Player { get; set; }

        public override void Assign(Player player)
        {
            if (Player != null)
            {
                return;
            }

            base.Assign(player);

            Player = player;
        }

        public override void Revoke(Player player, in RevokeReason reason)
        {
            if (player == null || !Has(player) || reason == RevokeReason.Died && player.IsAlive)
            {
                return;
            }

            base.Revoke(player, reason);

            Player = null;
        }

        public sealed override bool Has(in Player player)
        {
            if (player == null || player.Role != Role)
            {
                return false;
            }

            return Player == player;
        }
    }
}
