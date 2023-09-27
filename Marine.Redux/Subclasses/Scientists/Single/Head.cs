using Exiled.API.Enums;
using Exiled.API.Features;
using Marine.Redux.API;
using Marine.Redux.API.Inventory;
using Marine.Redux.API.Subclasses;
using PlayerRoles;
using System.Collections.Generic;

namespace Marine.Redux.Subclasses.Scientists.Single
{
    public class Head : SingleSubclass
    {
        public Head() : base() { }

        public override string Name { get; set; } = "Научный Руководитель";

        public override string Desc { get; set; } = "Хороший ученый и управленец, главенствующий над всем научным отделом";

        public override SpawnInfo SpawnInfo { get; set; } = new()
        {
            ShowInfo = true,
            Message = new("Ты - Научный Руководитель!\nТы хороший ученый и управленец, главенствующий над всем научным отделом.", 12, true),
            Inventory = new()
            {
                IsRandomable = false,
                Slots = new List<Slot>(8)
                {
                    new Slot(new ItemChances()
                    {
                        { ItemType.KeycardResearchCoordinator, 100 },
                    }, false),
                    new Slot(new ItemChances()
                    {
                        { ItemType.Medkit, 100 },
                    }, false),
                    new Slot(new ItemChances()
                    {
                        { ItemType.Painkillers, 100 },
                    }, false),
                    new Slot(new ItemChances()
                    {
                        { ItemType.Radio, 100 },
                    }, false)
                }
            }
        };

        public override RoleTypeId Role { get; set; } = RoleTypeId.Scientist;

        public override int Chance { get; set; } = 10;

        public override bool Can(in Player player)
        {
            return base.Can(player) && !AnyHas<Manager>();
        }

        protected override void OnAssigned(Player player)
        {
            player.Teleport(DoorType.Scp330);
        }
    }
}
