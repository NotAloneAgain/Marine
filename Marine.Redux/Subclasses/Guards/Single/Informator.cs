using Exiled.API.Enums;
using Exiled.API.Features;
using Marine.Redux.API;
using Marine.Redux.API.Inventory;
using Marine.Redux.API.Subclasses;
using PlayerRoles;
using System.Collections.Generic;

namespace Marine.Redux.Subclasses.Guards.Single
{
    public class Informator : SingleSubclass
    {
        public Informator() : base() { }

        public override string Name { get; set; } = "Информатор";

        public override SpawnInfo SpawnInfo { get; set; } = new()
        {
            Message = new("Ты - Информатор!\nТы владеешь информацией о том, какие SCP сбежали (проверь консоль).", 12, true),
            Inventory = new()
            {
                IsRandomable = false,
                Slots = new List<Slot>(8)
            }
        };

        public override RoleTypeId Role { get; set; } = RoleTypeId.FacilityGuard;

        public override int Chance { get; set; } = 10;

        protected override void OnAssigned(Player player)
        {
            string scps = "Сбежали: ";

            bool first = true;

            foreach (var ply in Player.List)
            {
                if (ply.IsScp)
                {
                    continue;
                }

                string suffix = first switch
                {
                    true => string.Empty,
                    false => ", "
                };

                scps += $"{suffix}{ply.Role.Type.ToString().ToUpper().Replace("SCP", "SCP-")}";
            }

            player.SendConsoleMessage(scps, "red");
        }
    }
}
