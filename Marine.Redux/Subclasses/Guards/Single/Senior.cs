using Exiled.API.Enums;
using Exiled.API.Features;
using Marine.Redux.API;
using Marine.Redux.API.Inventory;
using Marine.Redux.API.Subclasses;
using PlayerRoles;
using System.Collections.Generic;

namespace Marine.Redux.Subclasses.Guards.Single
{
    public class Senior : SingleSubclass
    {
        public Senior() : base() { }

        public override string Name { get; set; } = "Глава охраны";

        public override SpawnInfo SpawnInfo { get; set; } = new()
        {
            ShowInfo = true,
            Message = new("Ты - Глава охраны!\nТы руководишь всей охраной комплекса и имеешь повышенный уровень доступа.", 12, true),
            Inventory = new()
            {
                IsRandomable = false,
                Slots = new List<Slot>(8)
                {
                    new Slot(new ItemChances()
                    {
                        { ItemType.KeycardNTFOfficer, 100 },
                    }, false),
                    new Slot(new ItemChances()
                    {
                        { ItemType.GunCrossvec, 100 },
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
                        { ItemType.ArmorCombat, 100 },
                    }, false),
                }
            }
        };

        public override RoleTypeId Role { get; set; } = RoleTypeId.FacilityGuard;

        public override int Chance { get; set; } = 15;
    }
}
