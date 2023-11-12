using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using Marine.Redux.API;
using Marine.Redux.API.Subclasses;
using MEC;
using PlayerRoles;
using System.Collections.Generic;

namespace Marine.Redux.Subclasses.Generic.Zombies.Single
{
    public class Cursed : SingleSubclass
    {
        public override string Name { get; set; } = "Проклятый";

        public override string Desc { get; set; } = "Ты проклятый зомби, носящий в себе скрытый вирус, игнорирующий все щиты и иммунитет";

        public override List<string> Abilities { get; set; } = new List<string>()
        {
            "После твоей смерти ты накладываешь проклятье на убийцу.",
            "Атаки наносят случайный негативныйэффект.",
        };

        public override bool ConsoleRemark { get; } = true;

        public override SpawnInfo SpawnInfo { get; set; } = new()
        {
            ShowInfo = true,
            Message = new Message(string.Empty, 12, true),
            Health = 0,
        };

        public override RoleTypeId Role { get; set; } = RoleTypeId.Scp0492;

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

        protected internal override void OnDamage(HurtingEventArgs ev)
        {
            ev.Player.EnableEffect(EffectCategory.Negative.GetRandomByCategory(), 5);
        }

        private void OnDying(DyingEventArgs ev)
        {
            if (!Has(ev.Player) || !ev.IsAllowed)
            {
                return;
            }

            Timing.RunCoroutine(_CursedDamage(ev.Attacker));
        }

        private IEnumerator<float> _CursedDamage(Player player)
        {
            while (player?.IsAlive ?? false)
            {
                player.Health -= 1.44f;

                yield return Timing.WaitForSeconds(1);
            }
        }
    }
}
