﻿using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using Marine.Redux.API;
using Marine.Redux.API.Inventory;
using Marine.Redux.API.Subclasses;
using PlayerRoles;
using System.Collections.Generic;

namespace Marine.Redux.Subclasses.ClassD.Group
{
    public sealed class Thief : GroupSubclass
    {
        public override string Name { get; set; } = "Вор";

        public override string Desc { get; set; } = "Ты крайне давно увлекаешься воровством, когда-то ты ограбил банк";

        public override List<string> Abilities { get; set; } = new List<string>()
        {
            "Украсть случайный предмет командой [.steal].",
            "Подбирание предметов на 10% быстрее.",
            "Перемещение на 6% быстрее остальных.",
            "Набор полезных предметов.",
        };

        public override bool ConsoleRemark { get; } = true;

        public override int Max { get; set; } = 4;

        public override SpawnInfo SpawnInfo { get; set; } = new()
        {
            Message = new("Ты - Вор!\nТы имеешь некоторые предметы, +5% к скорости и можешь воровать командой .steal.", 12, true),
            Inventory = new()
            {
                IsRandomable = true,
                Slots = new List<Slot>(8)
                {
                    new Slot(new ItemChances()
                    {
                        { ItemType.KeycardZoneManager, 15 },
                        { ItemType.KeycardScientist, 15 },
                        { ItemType.KeycardJanitor, 100 },
                    }, true),
                    new Slot(new ItemChances()
                    {
                        { ItemType.SCP500, 5 },
                        { ItemType.Medkit, 100 },
                    }, true),
                    new Slot(new ItemChances()
                    {
                        { ItemType.Flashlight, 30 },
                        { ItemType.ArmorLight, 15 },
                        { ItemType.Radio, 100 },
                    }, true)
                }
            }
        };

        public override RoleTypeId Role { get; set; } = RoleTypeId.ClassD;

        public override int Chance { get; set; } = 20;

        public override void Subscribe()
        {
            base.Subscribe();

            Exiled.Events.Handlers.Player.SearchingPickup += OnSearchingPickup;
        }

        public override void Unsubscribe()
        {
            Exiled.Events.Handlers.Player.SearchingPickup -= OnSearchingPickup;

            base.Unsubscribe();
        }

        protected override void OnAssigned(Player player)
        {
            player.GetEffect(EffectType.MovementBoost)?.ServerSetState(6, 0, false);

            base.OnAssigned(player);
        }

        private void OnSearchingPickup(SearchingPickupEventArgs ev)
        {
            if (!Has(ev.Player))
            {
                return;
            }

            ev.SearchTime *= 0.75f;
        }
    }
}
