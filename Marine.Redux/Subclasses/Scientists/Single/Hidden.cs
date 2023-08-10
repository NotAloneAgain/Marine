using Marine.Redux.API;
using Marine.Redux.API.Inventory;
using Marine.Redux.API.Subclasses;
using PlayerRoles;
using System.Collections.Generic;

namespace Marine.Redux.Subclasses.Scientists.Single
{
    public class Hidden : SingleSubclass
    {
        public Hidden() : base() { }

        public override string Name { get; set; } = "Скрытный";

        public override SpawnInfo SpawnInfo { get; set; } = new()
        {
            Message = new("Ты - Скрытный!\nТы можешь скрыться с помощью команды .hide.", 12, true),
            Inventory = new()
            {
                IsRandomable = false,
                Slots = new List<Slot>(8)
            }
        };

        public override RoleTypeId Role { get; set; } = RoleTypeId.Scientist;

        public override int Chance { get; set; } = 15;
    }
}
