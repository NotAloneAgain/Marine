using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using Marine.Redux.API;
using Marine.Redux.API.Inventory;
using Marine.Redux.API.Subclasses;
using PlayerRoles;
using System.Collections.Generic;
using System.Linq;

namespace Marine.Redux.Subclasses.ClassD.Single
{
    public class Scp073 : SingleSubclass
    {
        private const string ConsoleMessage = "\n\t+ Помимо этого:" +
                "\n\t\t- 25% получаемого вами урона отражается на противника." +
                "\n\t\t- Вы получаете на ещё 25% меньше урона.";

        public Scp073() : base() { }

        public override string Name { get; set; } = "SCP-073";

        public override SpawnInfo SpawnInfo { get; set; } = new()
        {
            ShowInfo = true,
            Message = new("Ты - SCP-073!\nУ тебя очень крепкое тело и неплохая регенерация (проверь консоль).", 15, true, "#009A63"),
            Health = 150,
            Shield = new Shield(100, 100, -0.36f, 1, 10, true),
            Inventory = new()
            {
                IsRandomable = false,
                Slots = new List<Slot>(8)
                {
                    new Slot(new ItemChances()
                    {
                        { ItemType.KeycardJanitor, 100 },
                    }, false),
                    new Slot(new ItemChances()
                    {
                        { ItemType.ArmorLight, 100 },
                    }, false)
                }
            }
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

        public override bool Can(in Player player) => base.Can(player) && !AnyHas<Scp343>() && !AnyHas<Scp181>() && Player.List.Count() >= 8;

        protected override void OnAssigned(Player player)
        {
            base.OnAssigned(player);

            player.SendConsoleMessage(ConsoleMessage, "yellow");
        }

        private void OnHurting(HurtingEventArgs ev)
        {
            if (ev.Player == null || !ev.IsAllowed || !Has(ev.Player))
            {
                return;
            }

            if (ev.Attacker != null && ev.Player.UserId != ev.Attacker.UserId && !ev.Attacker.IsGodModeEnabled)
            {
                var amount = ev.Amount / 4;

                if (ev.Attacker.Health - amount > 0)
                {
                    ev.Attacker.Health -= amount;
                }
                else
                {
                    ev.Attacker.Kill("Урон, нанесенный SCP-073 был отражен на тебя...");
                }
            }

            ev.Amount /= 2;
        }
    }
}
