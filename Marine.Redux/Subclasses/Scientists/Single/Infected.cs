using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using Marine.Misc.API;
using Marine.Redux.API;
using Marine.Redux.API.Inventory;
using Marine.Redux.API.Subclasses;
using PlayerRoles;
using System.Collections.Generic;
using System.Linq;

namespace Marine.Redux.Subclasses.Scientists.Single
{
    public class Infected : SingleSubclass
    {
        public Infected() : base() { }

        public override string Name { get; set; } = "Зараженный";

        public override SpawnInfo SpawnInfo { get; set; } = new()
        {
            Message = new("Ты - Зараженный!\nТы заражен зомби-вирусом и станешь зомби после смерти или использовав команду .sus.", 12, true),
            Health = 75,
            Inventory = new()
            {
                IsRandomable = false,
                Slots = new List<Slot>(8)
                {
                    new Slot(new ItemChances()
                    {
                        { ItemType.KeycardScientist, 100 },
                    }, false)
                }
            }
        };

        public override RoleTypeId Role { get; set; } = RoleTypeId.Scientist;

        public override int Chance { get; set; } = 15;

        public override void Subscribe()
        {
            base.Subscribe();

            Exiled.Events.Handlers.Player.Dying += OnDying;
        }

        public override void Unsubscribe()
        {
            Exiled.Events.Handlers.Player.Dying -= OnDying;

            base.Unsubscribe();
        }

        public override bool Can(in Player player) => base.Can(player) && Player.List.Count(x => x.Role.Type == RoleTypeId.Scp049) > 0;

        protected override void OnChangingRole(ChangingRoleEventArgs ev)
        {
            if (ev.Reason == SpawnReason.Revived)
            {
                return;
            }

            base.OnChangingRole(ev);
        }

        private void OnDying(DyingEventArgs ev)
        {
            if (!Has(ev.Player) || ev.Player.Role.Type != Role)
            {
                return;
            }

            ev.IsAllowed = false;

            ev.Player.DropAllWithoutKeycard();
            ev.Player.Role.Set(RoleTypeId.Scp0492, SpawnReason.Revived);
        }
    }
}
