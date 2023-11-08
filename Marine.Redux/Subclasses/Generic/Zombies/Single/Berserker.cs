using CustomPlayerEffects;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using Marine.Redux.API;
using Marine.Redux.API.Subclasses;
using MEC;
using PlayerRoles;
using System.Collections.Generic;
using UnityEngine;

namespace Marine.Redux.Subclasses.Generic.Zombies.Single
{
    public class Berserker : SingleSubclass
    {
        private MovementBoost _boost;

        public override string Name { get; set; } = "Берсерк";

        public override string Desc { get; set; } = "Ты яростный зомби, готовый сражаться до конца.";

        public override List<string> Abilities { get; set; } = new List<string>()
        {
            "Повышение количества здоровья от убийств.",
            "Повышение количества урона от убийств.",
            "Повышение скорости от убийств.",
            "Регенерация здоровья от атак.",
            "Сниженный до 88% урон по вам",
        };

        public override SpawnInfo SpawnInfo { get; set; } = new()
        {
            ShowInfo = true,
            Message = new Message(string.Empty, 12, true),
            Shield = new Shield(50, 300, -0.88f, 1, 6, false),
            Size = Vector3.one * 1.04f,
        };

        public override RoleTypeId Role { get; set; } = RoleTypeId.Scp0492;

        public override int Chance { get; set; } = 8;

        public override float HurtMultiplayer { get; set; } = 0.88f;

        public int Health { get; set; } = 350;

        public int Damage { get; set; } = 20;

        public byte Movement { get; set; } = 2;

        public override void Subscribe()
        {
            base.Subscribe();

            Exiled.Events.Handlers.Player.Died += OnDied;
        }

        public override void Unsubscribe()
        {
            Exiled.Events.Handlers.Player.Died -= OnDied;

            base.Unsubscribe();
        }

        protected override void OnAssigned(Player player)
        {
            player.Health = Health;
            player.MaxHealth = Health;

            _boost = Player.GetEffect(EffectType.MovementBoost) as MovementBoost;

            Timing.RunCoroutine(_UpdateValues());
        }

        protected internal override void OnDamage(HurtingEventArgs ev)
        {
            ev.Amount = Damage;

            ev.Attacker.Heal(ev.Amount * 0.7f);
        }

        private void OnDied(DiedEventArgs ev)
        {
            if (!Has(ev.Attacker))
            {
                return;
            }

            Damage = Mathf.Clamp(Damage + 10, 20, 150);
            Health = Mathf.Clamp(Health + 50, 350, 1400);
            Movement = (byte)Mathf.Clamp(Movement + 2, 2, 40);
        }

        private IEnumerator<float> _UpdateValues()
        {
            while (Player != null)
            {
                if (Player.MaxHealth < Health)
                {
                    var value = Player.MaxHealth - Health;

                    Player.MaxHealth = Health;
                    Player.Heal(value);
                }

                _boost.ServerSetState(Movement, 0, false);

                yield return Timing.WaitForSeconds(5);
            }
        }
    }
}
