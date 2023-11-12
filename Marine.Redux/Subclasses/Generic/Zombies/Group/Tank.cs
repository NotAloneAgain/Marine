using Exiled.API.Enums;
using Exiled.API.Features;
using Marine.Redux.API;
using Marine.Redux.API.Subclasses;
using PlayerRoles;
using System.Collections.Generic;
using UnityEngine;

namespace Marine.Redux.Subclasses.Generic.Zombies.Single
{
    public class Tank : GroupSubclass
    {
        public override string Name { get; set; } = "Танк";

        public override string Desc { get; set; } = "Ты живучий зомби, способный защитить своего создателя";

        public override List<string> Abilities { get; set; } = new List<string>()
        {
            "Повышенное количество здоровья.",
        };

        public override SpawnInfo SpawnInfo { get; set; } = new()
        {
            ShowInfo = true,
            Message = new Message(string.Empty, 12, true),
            Health = 1500,
            Size = Vector3.one * 1.11f
        };

        public override RoleTypeId Role { get; set; } = RoleTypeId.Scp0492;

        public override int Chance { get; set; } = 16;

        public override int Max { get; set; } = 4;

        protected override void OnAssigned(Player player)
        {
            player.GetEffect(EffectType.SinkHole)?.ServerSetState(1, 0, false);
        }
    }
}
