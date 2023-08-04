using Marine.Redux.API;
using Marine.Redux.API.Inventory;
using Marine.Redux.API.Subclasses;
using PlayerRoles;
using System.Collections.Generic;

namespace Marine.Redux.Subclasses.Scientists.Single
{
    public class Hidden : SingleSubclass
    {
        public override string Name { get; set; } = "Скрытный";

        public override SpawnInfo SpawnInfo { get; set; } = new()
        {
            Message = new("Ты - Скрытный!\nТы имеете хорошие медицинские познания и можете лечить других (проверь консоль).", 12, true),
            Inventory = new()
            {
                IsRandomable = false,
                Slots = new List<Slot>(8)
                {
                    new Slot(new ItemChances()
                    {
                        { ItemType.KeycardScientist, 100 },
                    }, false),
                    new Slot(new ItemChances()
                    {
                        { ItemType.Medkit, 100 },
                    }, false),
                    new Slot(new ItemChances()
                    {
                        { ItemType.Medkit, 100 },
                    }, false),
                    new Slot(new ItemChances()
                    {
                        { ItemType.Painkillers, 100 },
                    }, false)
                }
            }
        };

        public override RoleTypeId Role { get; set; } = RoleTypeId.Scientist;

        public override int Chance { get; set; } = 15;
    }
}
