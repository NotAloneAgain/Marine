using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using Marine.Redux.API;
using Marine.Redux.API.Subclasses;
using PlayerRoles;
using System.Collections.Generic;
using System.Linq;

namespace Marine.Redux.Subclasses.ClassD.Single
{
    public class Scp073 : SingleSubclass
    {
        public override string Name { get; set; } = "SCP-073";

        public override string Desc { get; set; } = "У тебя очень крепкое тело и неплохая регенерация";

        public override List<string> Abilities { get; set; } = new List<string>()
        {
            "Отражение трети получаемого урона в противника.",
            "Выбить некоторые двери с помощью [.knock].",
            "Пониженный до 60% получаемый урон.",
        };

        public override bool ConsoleRemark { get; } = true;

        public override SpawnInfo SpawnInfo { get; set; } = new()
        {
            ShowInfo = true,
            Message = new("Ты - SCP-073!\nУ тебя очень крепкое тело и неплохая регенерация (проверь консоль).", 15, true, "#009A63"),
            Health = 120,
            Shield = new Shield(100, 100, -0.75f, 1, 5, true)
        };

        public override RoleTypeId Role { get; set; } = RoleTypeId.ClassD;

        public override float HurtMultiplayer { get; set; } = 0.6f;

        public override int Chance { get; set; } = 5;

        public override bool Can(in Player player) => base.Can(player) && !AnyHas<Scp343>() && !AnyHas<Scp181>() && Player.List.Count() >= 5;

        internal protected override void OnHurt(HurtingEventArgs ev)
        {
            if (ev.Attacker == null)
            {
                return;
            }

            var isScp = ev.Attacker.IsScp;

            var amount = isScp switch
            {
                true => 100,
                _ => ev.Amount / 3
            };

            if (isScp)
            {
                ev.Amount = 40;
            }

            if (ev.Attacker.IsGodModeEnabled || ev.DamageHandler.Type is DamageType.PocketDimension)
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
