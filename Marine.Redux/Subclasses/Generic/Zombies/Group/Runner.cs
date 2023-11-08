﻿using Exiled.API.Enums;
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
    public class Runner : GroupSubclass
    {
        public override string Name { get; set; } = "Бегун";

        public override string Desc { get; set; } = "Ты быстрый зомби, готовый не дать цели сбежать.";

        public override List<string> Abilities { get; set; } = new List<string>()
        {
            "Повышенная скорость.",
        };

        public override SpawnInfo SpawnInfo { get; set; } = new()
        {
            ShowInfo = true,
            Message = new Message(string.Empty, 12, true),
            Size = Vector3.one * 0.98f,
        };

        public override RoleTypeId Role { get; set; } = RoleTypeId.Scp0492;

        public override int Chance { get; set; } = 15;

        public override int Max { get; set; } = 4;

        protected override void OnAssigned(Player player)
        {
            player.GetEffect(EffectType.MovementBoost)?.ServerSetState(10, 0, false);
        }
    }
}