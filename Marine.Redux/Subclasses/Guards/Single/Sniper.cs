using Exiled.API.Enums;
using Exiled.API.Features;
using Marine.Redux.API;
using Marine.Redux.API.Inventory;
using Marine.Redux.API.Subclasses;
using PlayerRoles;
using System.Collections.Generic;

namespace Marine.Redux.Subclasses.Guards.Single
{
    public class Sniper : SingleSubclass
    {
        public Sniper() : base() { }

        public override string Name { get; set; } = "Снайпер";

        public override SpawnInfo SpawnInfo { get; set; } = new()
        {
            Message = new("Ты - Снайпер!\nТы очень точно стреляешь и тебе доверяет Фонд, за что ты получил винтовку.", 12, true),
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
                        { ItemType.GunE11SR, 100 },
                    }, false),
                    new Slot(new ItemChances()
                    {
                        { ItemType.GrenadeFlash, 100 },
                    }, false),
                    new Slot(new ItemChances()
                    {
                        { ItemType.Medkit, 100 },
                    }, false),
                    new Slot(new ItemChances()
                    {
                        { ItemType.Adrenaline, 100 },
                    }, false),
                    new Slot(new ItemChances()
                    {
                        { ItemType.Radio, 100 },
                    }, false),
                    new Slot(new ItemChances()
                    {
                        { ItemType.ArmorLight, 100 },
                    }, false),
                }
            }
        };

        public override RoleTypeId Role { get; set; } = RoleTypeId.FacilityGuard;

        public override int Chance { get; set; } = 15;
    }
}
