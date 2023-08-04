using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using InventorySystem.Items.Usables;
using Marine.Redux.API;
using Marine.Redux.API.Inventory;
using Marine.Redux.API.Subclasses;
using PlayerRoles;
using System.Collections.Generic;
using System.Linq;

namespace Marine.Redux.Subclasses.Scientists.Single
{
    public class Manager : SingleSubclass
    {
        public override string Name { get; set; } = "Менеджер Комплекса";

        public override SpawnInfo SpawnInfo { get; set; } = new()
        {
            ShowInfo = true,
            Message = new("Ты - Менеджер Комплекса!\nТы элитный управленец и важная шишка.", 12, true),
            Inventory = new()
            {
                IsRandomable = false,
                Slots = new List<Slot>(8)
                {
                    new Slot(new ItemChances()
                    {
                        { ItemType.KeycardFacilityManager, 100 },
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
                        { ItemType.ArmorLight, 100 },
                    }, false)
                }
            }
        };

        public override RoleTypeId Role { get; set; } = RoleTypeId.Scientist;

        public override int Chance { get; set; } = 10;

        public override bool Can(in Player player) => base.Can(player) && !AnyHas<Head>();

        protected override void OnAssigned(Player player) => player.Teleport(DoorType.NukeArmory);
    }
}
