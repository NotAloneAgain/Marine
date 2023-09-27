using Exiled.API.Enums;
using Exiled.API.Features;
using Marine.Redux.API;
using Marine.Redux.API.Inventory;
using Marine.Redux.API.Subclasses;
using PlayerRoles;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Marine.Redux.Subclasses.ClassD.Group
{
    public class Janitor : GroupSubclass
    {
        public Janitor() : base() { }

        public override string Name { get; set; } = "Уборщик";

        public override string Desc { get; set; } = "Ты убирался в комплексе, но теперь похоже не можешь продолжить свое дело";

        public override int Max { get; set; } = 3;

        public override SpawnInfo SpawnInfo { get; set; } = new()
        {
            Message = new("Ты - уборщик!\nТы убирался в комплексе, но теперь похоже не можешь продолжить свое дело.", 12, true, "#D6AE01"),
            Inventory = new()
            {
                IsRandomable = false,
                Slots = new List<Slot>(8)
                {
                    new Slot(new ItemChances()
                    {
                        { ItemType.KeycardJanitor, 100 }
                    }, false),
                }
            }
        };

        public override RoleTypeId Role { get; set; } = RoleTypeId.ClassD;

        public override int Chance { get; set; } = 15;

        protected override void OnAssigned(Player player)
        {
            IEnumerable<Room> rooms = Room.List.Where(room => room.Type == RoomType.LczTCross);

            player.Position = rooms.ElementAt(Random.Range(0, rooms.Count())).Position + Vector3.up;
        }
    }
}
