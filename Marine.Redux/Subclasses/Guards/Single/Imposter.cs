using Exiled.API.Enums;
using Exiled.API.Features;
using Marine.Redux.API;
using Marine.Redux.API.Inventory;
using Marine.Redux.API.Subclasses;
using PlayerRoles;
using System.Collections.Generic;

namespace Marine.Redux.Subclasses.Guards.Single
{
    public class Imposter : SingleSubclass
    {
        public Imposter() : base() { }

        public override string Name { get; set; } = "Предатель";

        public override SpawnInfo SpawnInfo { get; set; } = new()
        {
            Message = new("Ты - Предатель!\nТы тайный агент повстанцев хаоса, для того чтобы перевоплотиться используй .sus.", 12, true),
            Inventory = new()
            {
                IsRandomable = false,
                Slots = new List<Slot>(8)
            }
        };

        public override RoleTypeId Role { get; set; } = RoleTypeId.FacilityGuard;

        public override int Chance { get; set; } = 3;

        protected override void OnAssigned(Player player)
        {
            player.GetEffect(EffectType.Asphyxiated).ServerSetState(1, 3600, false);
        }
    }
}
