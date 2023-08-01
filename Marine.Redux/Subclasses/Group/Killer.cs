using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Pickups;
using Exiled.Events.EventArgs.Player;
using Marine.Redux.API;
using Marine.Redux.API.Inventory;
using Marine.Redux.API.Subclasses;
using PlayerRoles;
using System.Collections.Generic;

namespace Marine.Redux.Subclasses.Group
{
    public sealed class Killer : GroupSubclass
    {
        public override string Name { get; set; } = "Убийца";

        public override int Max { get; set; } = 3;

        public override SpawnInfo SpawnInfo { get; set; } = new()
        {
            ShowInfo = false,
            Message = new("Вы - Убийца!\nУ вас есть информацию о пистолете (проверь консоль) и +5% к наносимому вами урону.", 12, true),
            Inventory = new()
            {
                IsRandomable = true,
                Slots = new List<Slot>(8)
                {
                    new Slot(new ItemChances()
                    {
                        { ItemType.GunCOM15, 2 },
                        { ItemType.Adrenaline, 30 },
                        { ItemType.Medkit, 100 }
                    }, true),
                    new Slot(new ItemChances()
                    {
                        { ItemType.Painkillers, 100 },
                    }, false),
                    new Slot(new ItemChances()
                    {
                        { ItemType.Painkillers, 100 },
                    }, false)
                }
            }
        };

        public override RoleTypeId Role { get; set; } = RoleTypeId.ClassD;

        public override int Chance { get; set; } = 10;

        public override void Subscribe()
        {
            base.Subscribe();

            Exiled.Events.Handlers.Player.Hurting += OnHurting;
        }

        public override void Unsubscribe()
        {
            Exiled.Events.Handlers.Player.Hurting -= OnHurting;

            base.Unsubscribe();
        }

        protected override void OnAssigned(Player player)
        {
            foreach (var pickup in Pickup.List)
            {
                if (pickup == null || pickup.Room == null || pickup.Room.Zone != ZoneType.LightContainment || pickup.Type != ItemType.GunCOM15)
                {
                    continue;
                }

                player.SendConsoleMessage($"Пистолет находится в {Translate(pickup.Room.Type)}", "red");
            }
        }

        private void OnHurting(HurtingEventArgs ev)
        {
            if (!Has(ev.Attacker))
            {
                return;
            }

            ev.Amount *= 1.05f;
        }

        private string Translate(RoomType type) => type switch
        {
            RoomType.LczCafe => "комнате с компьютерами",
            RoomType.Lcz173 => "оружейке К.С. SCP-173.",
            RoomType.LczToilets => "туалетах",
            RoomType.LczGlassBox => "К.С. SCP-372",
            _ => "неизвестно..."
        };
    }
}
