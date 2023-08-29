using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Scp049;
using Exiled.Events.EventArgs.Scp079;
using Marine.LevelSystem.API;
using PlayerRoles;
using UnityEngine;

namespace Marine.LevelSystem.Handlers
{
    internal sealed class PlayerHandlers
    {
        public void OnVerified(VerifiedEventArgs ev) => ev.Player.Track();

        public void OnDestroying(DestroyingEventArgs ev) => ev.Player.Remove();

        public void OnDied(DiedEventArgs ev)
        {
            if (Round.IsEnded || ev.Attacker == null || ev.Player == null || ev.Player == ev.Attacker || ev.TargetOldRole == RoleTypeId.Tutorial || ev.Attacker.Role.Type == RoleTypeId.Tutorial)
            {
                return;
            }

            var amount = ev.TargetOldRole.GetTeam() switch
            {
                Team.FoundationForces => 50,
                Team.ChaosInsurgency => 50,
                Team.ClassD => 25,
                Team.Scientists => 25,
                Team.SCPs => 100,
                _ => 0
            };

            if (ev.Attacker.Role.Team == Team.SCPs)
            {
                amount = Mathf.Max(amount / 2, 25);
            }

            ev.Attacker.Reward(amount, "убийство");
        }

        public void OnEscaping(EscapingEventArgs ev)
        {
            if (!ev.IsAllowed || ev.Player == null)
            {
                return;
            }

            ev.Player.Reward(50, "побег");
            ev.Player.Cuffer?.Reward(50, "вывод связанного противника");
        }

        public void OnGainingExperience(GainingExperienceEventArgs ev) => ev.Player.Reward(ev.Amount, "полезное действие SCP-079");

        public void OnFinishingRecall(FinishingRecallEventArgs ev) => ev.Player.Reward(25, "поднятие зомби");
    }
}
