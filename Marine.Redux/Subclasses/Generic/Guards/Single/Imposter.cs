using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Pickups;
using Exiled.Events.EventArgs.Player;
using Marine.Redux.API;
using Marine.Redux.API.Enums;
using Marine.Redux.API.Inventory;
using Marine.Redux.API.Subclasses;
using PlayerRoles;
using System.Collections.Generic;

namespace Marine.Redux.Subclasses.Guards.Single
{
    public class Imposter : SingleSubclass
    {
        public override string Name { get; set; } = "Предатель";

        public override string Desc { get; set; } = "Агент повстанцев хаоса, носящий форму и оружие под формой Фонда.";

        public override List<string> Abilities { get; set; } = new List<string>()
        {
            "Отсутствие выносливости до переодевания.",
            "Нельзя наносить урон дешкам и хаосу.",
            "Возможность переодеться [.sus]."
        };

        public override bool ConsoleRemark { get; } = true;

        public override SpawnInfo SpawnInfo { get; set; } = new()
        {
            ShowInfo = false,
            Message = new("Ты - Предатель!\nТы тайный агент повстанцев хаоса, для того чтобы перевоплотиться используй .sus.", 12, true),
            Inventory = new()
            {
                IsRandomable = false,
                Slots = new List<Slot>(8)
            }
        };

        public override RoleTypeId Role { get; set; } = RoleTypeId.FacilityGuard;

        public override int Chance { get; set; } = 6;

        protected override void OnAssigned(Player player)
        {
            player.GetEffect(EffectType.Asphyxiated).ServerSetState(1, 3600, false);
        }

        internal protected override void OnHurt(HurtingEventArgs ev)
        {
            if (Player != null && ev.Player.Role.Type == RoleTypeId.FacilityGuard && ev.Attacker.LeadingTeam == LeadingTeam.ChaosInsurgency)
            {
                ev.IsAllowed = false;

                Revoke(Player, RevokeReason.None);

                if (Player.IsInventoryFull)
                {
                    Pickup.CreateAndSpawn(ItemType.GunA7, Player.Position, Player.Rotation, Player);
                }
                else
                {
                    Player.AddItem(ItemType.GunA7);
                }
            }
        }

        internal protected override void OnDamage(HurtingEventArgs ev)
        {
            if (Player != null && Player.Role.Type == RoleTypeId.FacilityGuard && ev.Player.LeadingTeam == LeadingTeam.ChaosInsurgency)
            {
                ev.IsAllowed = false;
            }
        }
    }
}
