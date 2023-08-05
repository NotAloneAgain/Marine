using Marine.Redux.API;
using Marine.Redux.API.Inventory;
using Marine.Redux.API.Subclasses;
using PlayerRoles;
using System.Collections.Generic;
using UnityEngine;

namespace Marine.Redux.Subclasses.ClassD.Group
{
    public class Dwarf : GroupSubclass
    {
        public Dwarf() : base() { }

        public override string Name { get; set; } = "Карлик";

        public override int Max { get; set; } = 3;

        public override SpawnInfo SpawnInfo { get; set; } = new()
        {
            ShowInfo = true,
            Message = new("Ты - карлик!\nНесмотря на твое слабое тело и маленький рост ты смог выжить тут.", 12, true, "#D6AE01"),
            Size = Vector3.one * 0.59f,
            Health = 50,
            Inventory = new()
            {
                IsRandomable = true,
                Slots = new List<Slot>(8)
                {
                    new Slot(new ItemChances()
                    {
                        { ItemType.Medkit, 100 },
                    }, false),
                    new Slot(new ItemChances()
                    {
                        { ItemType.Medkit, 50 },
                        { ItemType.Painkillers, 100 },
                    }, true)
                }
            }
        };

        public override RoleTypeId Role { get; set; } = RoleTypeId.ClassD;

        public override int Chance { get; set; } = 15;
    }
}
