using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using Marine.Redux.API;
using Marine.Redux.API.Subclasses;
using MEC;
using PlayerRoles;
using System.Collections.Generic;
using UnityEngine;

namespace Marine.Redux.Subclasses.Generic.Zombies.Single
{
    public class Poisoned : GroupSubclass
    {
        public override string Name { get; set; } = "Ядовитый";

        public override string Desc { get; set; } = "Ты ядовитый зомби, твои атаки отравляют и заставляют гнить цели";

        public override List<string> Abilities { get; set; } = new List<string>()
        {
            "Выдача замедления, травмы и яда при атаке.",
        };

        public override SpawnInfo SpawnInfo { get; set; } = new()
        {
            ShowInfo = true,
            Message = new Message(string.Empty, 12, true),
            Size = Vector3.one * 0.96f,
            Health = 0,
        };

        public override RoleTypeId Role { get; set; } = RoleTypeId.Scp0492;

        public override int Chance { get; set; } = 16;

        public override int Max { get; set; } = 4;

        protected internal override void OnDamage(HurtingEventArgs ev)
        {
            ev.Player.EnableEffect(EffectType.TraumatizedByEvil, 0, false);
            ev.Player.EnableEffect(EffectType.SinkHole, 4, true);
            ev.Player.EnableEffect(EffectType.Poisoned, 5, true);
            ev.Player.Stamina -= 0.02f;
        }
    }
}
