using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Roles;
using Exiled.Events.EventArgs.Player;
using Marine.Redux.API;
using Marine.Redux.API.Enums;
using Marine.Redux.API.Subclasses;
using MEC;
using PlayerRoles;
using System;
using System.Collections.Generic;

namespace Marine.Redux.Subclasses.Generic.Zombies.Single
{
    public class Parasite : SingleSubclass
    {
        public override string Name { get; set; } = "Паразит";

        public override string Desc { get; set; } = "Ты паразитическая сущность, которая может превращаться в своих жертв.";

        public override List<string> Abilities { get; set; } = new List<string>()
        {
            "Возможность контроллировать тела жертв и использовать человеческие вещи.",
            "Пассивная регенерация здоровья.",
        };

        public override bool ConsoleRemark { get; } = true;

        public override SpawnInfo SpawnInfo { get; set; } = new()
        {
            ShowInfo = true,
            Message = new Message(string.Empty, 12, true),
            Health = 350,
        };

        public override RoleTypeId Role { get; set; } = RoleTypeId.Scp0492;

        public override int Chance { get; set; } = -1;

        protected override void OnAssigned(Player player)
        {
            player.IsUsingStamina = false;

            SetFriendlyFire(player);

            Timing.RunCoroutine(_Regeneration());
        }

        protected override void OnRevoked(Player player, in RevokeReason reason)
        {
            player.IsUsingStamina = true;

            player.FriendlyFireMultiplier.Clear();

            player.DisplayNickname = player.Nickname;
        }

        private void OnDying(DyingEventArgs ev)
        {
            if (!Has(ev.Attacker) || !ev.IsAllowed)
            {
                return;
            }

            Ragdoll.CreateAndSpawn(ev.Attacker.Role, ev.Attacker.CustomName, ev.DamageHandler, ev.Attacker.Position, ev.Attacker.Rotation);

            ev.Attacker.DisplayNickname = ev.Player.Nickname;
            ev.Attacker.DropItems();
            ev.Attacker.Position = ev.Player.Position;
            ev.Attacker.Rotation = ev.Player.Rotation;
            ev.Attacker.Scale = ev.Player.Scale;
            ev.Attacker.Role.Set(ev.Player.Role, SpawnReason.Revived, RoleSpawnFlags.None);
            ev.Attacker.ResetInventory(ev.Player.Items);

            ev.IsAllowed = false;

            ev.Player.ClearInventory();
            ev.Player.Role.Set(RoleTypeId.Spectator, SpawnReason.Died, RoleSpawnFlags.All);

            ev.Attacker.Health = SpawnInfo.Health;

            SetFriendlyFire(ev.Attacker);

            Round.KillsByScp++;
        }

        private IEnumerator<float> _Regeneration()
        {
            while (Player != null)
            {
                Player.Heal(Player.Role.As<FpcRole>().IsAfk ? 1 : 0.25f);

                yield return Timing.WaitForSeconds(2);
            }
        }

        private void SetFriendlyFire(Player player)
        {
            foreach (var role in Enum.GetValues(typeof(RoleTypeId)).ToArray<RoleTypeId>())
            {
                if (role.GetTeam() != Team.SCPs)
                {
                    player.SetFriendlyFire(role, 1);
                }
            }
        }
    }
}
