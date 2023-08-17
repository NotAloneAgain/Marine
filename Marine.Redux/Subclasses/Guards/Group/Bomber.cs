using Exiled.API.Enums;
using Exiled.API.Features;
using Marine.Redux.API;
using Marine.Redux.API.Inventory;
using Marine.Redux.API.Subclasses;
using PlayerRoles;
using System.Collections.Generic;

namespace Marine.Redux.Subclasses.Guards.Group
{
    public class Bomber : GroupSubclass
    {
        public Bomber() : base() { }

        public override string Name { get; set; } = "Подрывник";

        public override int Max { get; set; } = 2;

        public override SpawnInfo SpawnInfo { get; set; } = new()
        {
            Health = 150,
            Shield = new Shield(15, 15, -0.4f, 1, 5, true),
            Message = new("Ты - Подрывник!\nТы обучен пользоваться дробовиком и осколочной гранатой.", 12, true),
            Inventory = new()
            {
                IsRandomable = false,
                Slots = new List<Slot>(8)
                {
                    new Slot(new ItemChances()
                    {
                        { ItemType.KeycardGuard, 100 },
                    }, false),
                    new Slot(new ItemChances()
                    {
                        { ItemType.GunShotgun, 100 },
                    }, false),
                    new Slot(new ItemChances()
                    {
                        { ItemType.GrenadeHE, 100 },
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
                        { ItemType.ArmorCombat, 100 },
                    }, false),
                }
            }
        };

        public override RoleTypeId Role { get; set; } = RoleTypeId.FacilityGuard;

        public override int Chance { get; set; } = 15;
    }
}
