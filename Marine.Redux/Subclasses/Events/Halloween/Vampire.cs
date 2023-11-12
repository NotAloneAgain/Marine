using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using Marine.Redux.API;
using Marine.Redux.API.Enums;
using Marine.Redux.API.Inventory;
using Marine.Redux.API.Subclasses;
using MEC;
using PlayerRoles;
using System;
using System.Collections.Generic;

namespace Marine.Redux.Subclasses.Events.Halloween
{
    public class Vampire : SingleSubclass
    {
        public override string Name { get; set; } = "Вампир";

        public override string Desc { get; set; } = "Ты подпитываешься кровью раненных тобой игроков";

        public override List<string> Abilities { get; set; } = new List<string>()
        {
            "Особая роль, доступная только в октябре в честь Хеллоуина.",
            "Повышенное количество здоровья.",
            "Восстановление вампиризмом.",
            "Бесконечная выносливость.",
            "Регенерация здоровья.",
        };

        public override bool ConsoleRemark { get; } = true;

        public override SpawnInfo SpawnInfo { get; set; } = new()
        {
            ShowInfo = true,
            Message = new("###", 15, true, "#480607"),
            Health = 125,
            Inventory = new()
            {
                IsRandomable = false,
                Slots = new List<Slot>(8)
                {
                    new Slot(new ItemChances()
                    {
                        { ItemType.KeycardScientist, 100 },
                    }, false),
                    new Slot(new ItemChances()
                    {
                        { ItemType.Radio, 100 },
                    }, false)
                }
            }
        };

        public override RoleTypeId Role { get; set; } = RoleTypeId.Scientist;

        public override int Chance { get; set; } = 20;

        public float Multiplayer => DateTime.Now.Day == 31 ? 0.4f : 0.25f;

        public override void Subscribe()
        {
            base.Subscribe();

            Exiled.Events.Handlers.Player.UsingItem += OnUsingItem;
        }

        public override void Unsubscribe()
        {
            Exiled.Events.Handlers.Player.UsingItem -= OnUsingItem;

            base.Unsubscribe();
        }

        public override bool Can(in Player player) => base.Can(player) && DateTime.Now.Month == 10;

        protected override void OnAssigned(Player player)
        {
            player.IsUsingStamina = false;

            Timing.RunCoroutine(_Regeneration());
        }

        protected override void OnRevoked(Player player, in RevokeReason reason)
        {
            player.IsUsingStamina = true;
        }

        protected internal override void OnDamage(HurtingEventArgs ev)
        {
            ev.Attacker.Heal(ev.Amount * Multiplayer);
        }

        private void OnUsingItem(UsingItemEventArgs ev)
        {
            if (ev.Item.Type is ItemType.Medkit or ItemType.Painkillers or ItemType.SCP500 && Has(ev.Player))
            {
                ev.IsAllowed = false;
            }
        }

        private IEnumerator<float> _Regeneration()
        {
            while (Player != null)
            {
                Player.Heal(0.5f);

                yield return Timing.WaitForSeconds(DateTime.Now.Day == 31 ? 0.95f : 1.5f);
            }
        }
    }
}
