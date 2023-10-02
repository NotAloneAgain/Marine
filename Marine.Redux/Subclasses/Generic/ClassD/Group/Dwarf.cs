using Exiled.API.Features;
using Marine.Redux.API;
using Marine.Redux.API.Enums;
using Marine.Redux.API.Inventory;
using Marine.Redux.API.Subclasses;
using PlayerRoles;
using System.Collections.Generic;
using UnityEngine;

namespace Marine.Redux.Subclasses.ClassD.Group
{
    public class Dwarf : GroupSubclass
    {
        public Dwarf() : base() { }

        public override string Name { get; set; } = "Карлик";

        public override string Desc { get; set; } = "Несмотря на твое слабое тело и маленький рост ты смог выжить тут";

        public override List<string> Abilities { get; set; } = new List<string>()
        {
            "Пониженное количество здоровья.",
            "Бесконечная выносливость.",
            "Маленький рост."
        };

        public override int Max { get; set; } = 3;

        public override SpawnInfo SpawnInfo { get; set; } = new()
        {
            ShowInfo = true,
            Message = new("Ты - карлик!\nНесмотря на твое слабое тело и маленький рост ты смог выжить тут.", 12, true, "#D6AE01"),
            Size = Vector3.one * 0.64f,
            Health = 50,
            Inventory = new()
            {
                IsRandomable = true,
                Slots = new List<Slot>(8)
                {
                    new Slot(new ItemChances()
                    {
                        { ItemType.Medkit, 100 },
                    }, false),
                    new Slot(new ItemChances()
                    {
                        { ItemType.Medkit, 50 },
                        { ItemType.Painkillers, 100 },
                    }, true)
                }
            }
        };

        public override RoleTypeId Role { get; set; } = RoleTypeId.ClassD;

        public override int Chance { get; set; } = 15;

        protected override void OnAssigned(Player player)
        {
            player.IsUsingStamina = false;
        }

        protected override void OnRevoked(Player player, in RevokeReason reason)
        {
            player.IsUsingStamina = true;
        }
    }
}
