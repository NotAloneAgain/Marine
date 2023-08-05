using Exiled.API.Features;
using Marine.Redux.API;
using Marine.Redux.API.Inventory;
using Marine.Redux.API.Subclasses;
using PlayerRoles;
using System.Collections.Generic;

namespace Marine.Redux.Subclasses.ClassD.Single
{
    public class Letting : SingleSubclass
    {
        public Letting() : base() { }

        public override string Name { get; set; } = "Попущенный";

        public override SpawnInfo SpawnInfo { get; set; } = new()
        {
            ShowInfo = true,
            Message = new("Ты - попущенный!\nУ тебя слабое тело, из-за чего тебя попустил блатной.", 12, true),
            Health = 75,
            Inventory = new()
            {
                IsRandomable = false,
                Slots = new List<Slot>(8)
            }
        };

        public override RoleTypeId Role { get; set; } = RoleTypeId.ClassD;

        public override int Chance { get; set; } = 15;

        public override bool Can(in Player player) => base.Can(player) && AnyHas<Gang>();
    }
}
