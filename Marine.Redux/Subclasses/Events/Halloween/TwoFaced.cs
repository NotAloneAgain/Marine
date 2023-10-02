using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using Marine.Redux.API;
using Marine.Redux.API.Inventory;
using Marine.Redux.API.Subclasses;
using PlayerRoles;
using System;
using System.Collections.Generic;

namespace Marine.Redux.Subclasses.Events.Halloween
{
    public class TwoFaced : SingleSubclass
    {
        public override string Name { get; set; } = "Двуликий";

        public override string Desc { get; set; } = "Ты живешь сменой лиц и маскировкой.";

        public override List<string> Abilities { get; set; } = new List<string>()
        {
            "Особая роль, доступная только в октябре в честь Хеллоуина.",
            "Возможность переодеваться в трупы [.clothes].",
        };

        public override bool ConsoleRemark { get; } = true;

        public override SpawnInfo SpawnInfo { get; set; } = new()
        {
            ShowInfo = false,
            Message = new("###", 12, true, "#480607"),
            Inventory = new()
            {
                IsRandomable = false,
                Slots = new List<Slot>(8)
                {
                    new Slot(new ItemChances()
                    {
                        { ItemType.KeycardGuard, 100 },
                    }, false),
                }
            }
        };

        public override RoleTypeId Role { get; set; } = RoleTypeId.ClassD;

        public RoleTypeId Model { get; set; } = RoleTypeId.ClassD;

        public override int Chance { get; set; } = 15;

        public override void Subscribe()
        {
            base.Subscribe();

            Exiled.Events.Handlers.Player.Verified += OnVerified;
        }

        public override void Unsubscribe()
        {
            Exiled.Events.Handlers.Player.Verified -= OnVerified;

            base.Unsubscribe();
        }

        public override bool Can(in Player player) => base.Can(player) && DateTime.Now.Month == 10;

        private void OnVerified(VerifiedEventArgs ev)
        {
            if (Player == null && Model != Role)
            {
                return;
            }

            Player.ChangeAppearance(Model, new List<Player>(1) { ev.Player }, true);
        }
    }
}
