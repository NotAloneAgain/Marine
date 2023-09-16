using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using Marine.Redux.API;
using Marine.Redux.API.Subclasses;
using PlayerRoles;
using System.Linq;

namespace Marine.Redux.Subclasses.ClassD.Single
{
    public class Scp073 : SingleSubclass
    {
        private const string ConsoleMessage = "\n\t+ Помимо этого:" +
                "\n\t\t- Возможность выбивать двери командой .knock (наведись на дверь)." +
                "\n\t\t- Треть получаемого вами урона отражается на противника." +
                "\n\t\t- Вы получаете на 50% меньше урона.";

        public Scp073() : base() { }

        public override string Name { get; set; } = "SCP-073";

        public override SpawnInfo SpawnInfo { get; set; } = new()
        {
            ShowInfo = true,
            Message = new("Ты - SCP-073!\nУ тебя очень крепкое тело и неплохая регенерация (проверь консоль).", 15, true, "#009A63"),
            Health = 120,
            Shield = new Shield(100, 100, -0.75f, 1, 5, true)
        };

        public override RoleTypeId Role { get; set; } = RoleTypeId.ClassD;

        public override int Chance { get; set; } = 5;

        public override void Subscribe()
        {
            base.Subscribe();

            Exiled.Events.Handlers.Player.Hurting += OnHurting;
        }

        public override void Unsubscribe()
        {
            Exiled.Events.Handlers.Player.Hurting -= OnHurting;

            base.Unsubscribe();
        }

        public override bool Can(in Player player) => base.Can(player) && !AnyHas<Scp343>() && !AnyHas<Scp181>() && Player.List.Count() >= 5;

        protected override void OnAssigned(Player player)
        {
            player.SendConsoleMessage(ConsoleMessage, "yellow");
        }

        private void OnHurting(HurtingEventArgs ev)
        {
            if (ev.Player == null || ev.Player.IsHost || ev.Player.IsNPC || !ev.IsAllowed || ev.Attacker == null || ev.Attacker.IsHost || ev.Attacker.IsNPC || !Has(ev.Player))
            {
                return;
            }

            bool isScp = ev.Attacker.IsScp;

            var amount = isScp switch
            {
                true => 100,
                _ => ev.Amount / 3
            };

            ev.Amount = isScp switch
            {
                true => 40,
                _ => ev.Amount / 2
            };

            if (ev.Player.UserId == ev.Attacker.UserId || ev.Attacker.IsGodModeEnabled || ev.DamageHandler.Type is DamageType.PocketDimension)
            {
                return;
            }

            if (ev.Attacker.Health - amount > 0)
            {
                ev.Attacker.Hurt(ev.Player, amount, ev.DamageHandler.Type, default, "Отражение SCP-073");
            }
            else
            {
                ev.Attacker.Kill("Отражение SCP-073");
            }
        }
    }
}
