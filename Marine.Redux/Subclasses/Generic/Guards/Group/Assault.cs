﻿using Marine.Redux.API;
using Marine.Redux.API.Inventory;
using Marine.Redux.API.Subclasses;
using PlayerRoles;
using System.Collections.Generic;

namespace Marine.Redux.Subclasses.Guards.Group
{
    public class Assault : GroupSubclass
    {
        public override string Name { get; set; } = "Штурмовик";

        public override string Desc { get; set; } = "Ты проверенный в боях сотрудник, готовый ко всем ситуациям";

        public override List<string> Abilities { get; set; } = new List<string>()
        {
            "Выбить некоторые двери с помощью [.knock].",
            "Повышенное количество здоровья."
        };

        public override bool ConsoleRemark { get; } = true;

        public override int Max { get; set; } = 2;

        public override SpawnInfo SpawnInfo { get; set; } = new()
        {
            Health = 125,
            Message = new("Ты - Штурмовик!\nТы проверенный в боях сотрудник, готовый ко всем ситуациям.", 12, true),
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
                        { ItemType.GunFRMG0, 40 },
                        { ItemType.GunFSP9, 100 },
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

        public override int Chance { get; set; } = 18;
    }
}
