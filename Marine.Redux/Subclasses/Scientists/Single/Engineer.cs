using Exiled.API.Enums;
using Exiled.API.Features;
using Marine.Redux.API;
using Marine.Redux.API.Inventory;
using Marine.Redux.API.Subclasses;
using PlayerRoles;
using System.Collections.Generic;

namespace Marine.Redux.Subclasses.Scientists.Single
{
    public class Engineer : SingleSubclass
    {
        private const string ConsoleMessage = "\n\t+ У тебя есть:" +
                "\n\t\t- Возможность улучшать дверь, чтобы её было невозможно сломать гранатой или SCP-096 [.upgrade].";

        public Engineer() : base() { }

        public override string Name { get; set; } = "Инженер";

        public override SpawnInfo SpawnInfo { get; set; } = new()
        {
            ShowInfo = true,
            Message = new("Ты - Инженер!\nТы ремонтировал камеру содержания SCP-173 чтобы он мог туда вернуться (проверь консоль).", 12, true),
            Inventory = new()
            {
                IsRandomable = false,
                Slots = new List<Slot>(8)
                {
                    new Slot(new ItemChances()
                    {
                        { ItemType.KeycardContainmentEngineer, 100 },
                    }, false),
                    new Slot(new ItemChances()
                    {
                        { ItemType.Medkit, 100 },
                    }, false),
                    new Slot(new ItemChances()
                    {
                        { ItemType.Radio, 100 },
                    }, false),
                    new Slot(new ItemChances()
                    {
                        { ItemType.Flashlight, 100 },
                    }, false)
                }
            }
        };

        public override RoleTypeId Role { get; set; } = RoleTypeId.Scientist;

        public override int Chance { get; set; } = 10;

        protected override void OnAssigned(Player player)
        {
            player.Teleport(DoorType.Scp173Gate);

            player.SendConsoleMessage(ConsoleMessage, "yellow");
        }
    }
}
