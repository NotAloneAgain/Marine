using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using Marine.ScpSwap.API;
using System.Collections.Generic;
using System.Linq;

namespace Marine.ScpSwap.Handlers
{
    internal sealed class PlayerHandlers
    {
        private readonly string _text;
        private readonly float _duration;

        internal PlayerHandlers(string text, float duration)
        {
            _text = text;
            _duration = duration;

            Swap.StartScps = new Dictionary<PlayerRoles.RoleTypeId, int>(6)
            {
                { PlayerRoles.RoleTypeId.Scp096, 0 },
                { PlayerRoles.RoleTypeId.Scp049, 0 },
                { PlayerRoles.RoleTypeId.Scp173, 0 },
                { PlayerRoles.RoleTypeId.Scp939, 0 },
                { PlayerRoles.RoleTypeId.Scp106, 0 },
                { PlayerRoles.RoleTypeId.Scp079, 0 },
                { PlayerRoles.RoleTypeId.Scp3114, 0 }
            };
        }

        public void OnChangingRole(ChangingRoleEventArgs ev)
        {
            if (ev.Reason is not SpawnReason.RoundStart and not SpawnReason.LateJoin and not SpawnReason.Respawn || !ev.IsAllowed)
            {
                return;
            }

            if (ev.NewRole.GetTeam() != PlayerRoles.Team.SCPs || ev.NewRole == PlayerRoles.RoleTypeId.Scp0492)
            {
                return;
            }

            if (Player.List.Count(ply => ply.Role.Team == PlayerRoles.Team.SCPs) == 6 && ev.NewRole is not PlayerRoles.RoleTypeId.Scp939 and not PlayerRoles.RoleTypeId.Scp3114)
            {
                ev.NewRole = PlayerRoles.RoleTypeId.Scp939;
            }

            if (Player.List.Count < 10 && ev.NewRole == PlayerRoles.RoleTypeId.Scp3114)
            {
                ev.NewRole = PlayerRoles.RoleTypeId.Scp939;
            }

            ev.Player.ShowHint(string.Format(_text, Swap.SwapDuration), _duration);

            Swap.StartScps[ev.NewRole]++;
        }

        public void OnDestroying(DestroyingEventArgs ev)
        {
            if (ev.Player.Role.Team != PlayerRoles.Team.SCPs || ev.Player.Role.Type == PlayerRoles.RoleTypeId.Scp0492)
            {
                return;
            }

            Swap.StartScps[ev.Player.Role.Type]--;
        }

        public void Reset()
        {
            foreach (PlayerRoles.RoleTypeId role in Swap.StartScps.Keys.ToHashSet())
            {
                Swap.StartScps[role] = 0;
            }
        }
    }
}
