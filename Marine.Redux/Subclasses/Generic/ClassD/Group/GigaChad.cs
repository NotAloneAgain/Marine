using Marine.Redux.API;
using Marine.Redux.API.Inventory;
using Marine.Redux.API.Subclasses;
using PlayerRoles;
using System.Collections.Generic;
using UnityEngine;

namespace Marine.Redux.Subclasses.ClassD.Group
{
    public class GigaChad : GroupSubclass
    {
        public GigaChad() : base() { }

        public override string Name { get; set; } = "Гигант";

        public override string Desc { get; set; } = "У тебя крепкое тело и большой рост";

        public override List<string> Abilities { get; set; } = new List<string>()
        {
            "Повышенное количество здоровья.",
            "Высокий рост."
        };

        public override int Max { get; set; } = 3;

        public override SpawnInfo SpawnInfo { get; set; } = new()
        {
            ShowInfo = true,
            Message = new("Ты - гигант!\nУ тебя крепкое тело и большой рост.", 12, true, "#D6AE01"),
            Size = Vector3.one * 1.12f,
            Health = 150,
            Inventory = new()
            {
                IsRandomable = false,
                Slots = new List<Slot>(8)
            }
        };

        public override RoleTypeId Role { get; set; } = RoleTypeId.ClassD;

        public override int Chance { get; set; } = 15;
    }
}
