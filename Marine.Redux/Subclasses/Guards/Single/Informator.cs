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
            ShowInfo = true,
            Message = new("Ты - Информатор!\nТы владеешь информацией о том, какие SCP сбежали (проверь консоль).", 12, true),
            Inventory = new()
            {
                IsRandomable = false,
                Slots = new List<Slot>(8)
            }
        };

        public override RoleTypeId Role { get; set; } = RoleTypeId.FacilityGuard;

        public override int Chance { get; set; } = 12;

        protected override void OnAssigned(Player player)
        {
            var scps = "Сбежали: ";

            var first = true;

            foreach (Player ply in Player.List)
            {
                if (!ply.IsScp)
                {
                    continue;
                }

                var suffix = first switch
                {
                    true => string.Empty,
                    false => ", "
                };

                first = false;

                scps += $"{suffix}{ply.Role.Type.ToString().ToUpper().Replace("SCP", "SCP-")}";
            }

            player.SendConsoleMessage($"{scps}.", "red");
        }
    }
}
