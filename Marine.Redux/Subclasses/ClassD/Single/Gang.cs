using Marine.Redux.API;
using Marine.Redux.API.Inventory;
using Marine.Redux.API.Subclasses;
using PlayerRoles;
using System.Collections.Generic;

namespace Marine.Redux.Subclasses.ClassD.Single
{
    public class Gang : SingleSubclass
    {
        public Gang() : base() { }

        public override string Name { get; set; } = "Блатной";

        public override SpawnInfo SpawnInfo { get; set; } = new()
        {
            ShowInfo = true,
            Message = new("Ты - блатной!\nТебя уважают все в тюрьме, это уважение завоевано силой. Используй .knock чтобы выбивать двери.", 12, true),
            Health = 125,
            Inventory = new()
            {
                IsRandomable = false,
                Slots = new List<Slot>(8)
                {
                    new Slot(new ItemChances()
                    {
                        { ItemType.Adrenaline, 100 },
                    }, false),
                    new Slot(new ItemChances()
                    {
                        { ItemType.Painkillers, 100 },
                    }, false)
                }
            }
        };

        public override RoleTypeId Role { get; set; } = RoleTypeId.ClassD;

        public override int Chance { get; set; } = 15;
    }
}
